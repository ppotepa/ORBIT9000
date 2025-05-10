using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Abstractions.Providers;
using ORBIT9000.Core.Abstractions.Runtime;
using ORBIT9000.Core.Events;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using System.Runtime.CompilerServices;

namespace ORBIT9000.Engine.Providers
{
    internal class PluginProvider : Core.Environment.Disposable, IPluginProvider
    {
        private readonly Dictionary<Type, IOrbitPlugin> _activePlugins = [];
        private readonly GlobalMessageChannel<PluginEvent> _channel;
        private readonly RuntimeSettings _config;
        private readonly ILogger<PluginProvider> _logger;
        private readonly IPluginLoader _pluginLoader;
        private readonly ILifetimeScope _rootScope;
        private readonly List<PluginInfo> _validPlugins;
        private Dictionary<Type, ILifetimeScope>? _individualPluginScopes;
        private ILifetimeScope? _pluginScope;

        public PluginProvider
        (
            ILogger<PluginProvider> logger,
            RuntimeSettings config,
            IPluginLoader pluginLoader,
            ILifetimeScope rootScope,
            GlobalMessageChannel<PluginEvent> channel)
        {
            this._logger = logger;
            this._config = config;
            this._pluginLoader = pluginLoader;
            this._rootScope = rootScope;
            this._validPlugins = [.. this._config.Plugins.Where(x => x.ContainsPlugins)];
            this._channel = channel;
        }

        private Dictionary<Type, ILifetimeScope> IndividualPluginScopes
        {
            get
            {
                if (this._individualPluginScopes == null)
                {
                    this._individualPluginScopes = [];
                    foreach (PluginInfo info in this._validPlugins)
                    {
                        this._individualPluginScopes[info.PluginType] = this._rootScope.BeginLifetimeScope(builder =>
                        {
                            ServiceCollection services = new();
                            RegisterPlugin(builder, services, info);
                            builder.Populate(services);
                        });
                    }
                }

                return this._individualPluginScopes;
            }
        }

        private ILifetimeScope PluginScope => this._pluginScope ??= this.CreateSharedScope();

        public IEnumerable<Type> Plugins => this._validPlugins.Select(plugin => plugin.PluginType);

        public async Task<IOrbitPlugin> Activate(object plugin, bool executeOnLoad = false)
        {
            if (plugin is string pluginName)
            {
                PluginInfo? target = this._validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));

                if (target != null)
                {
                    return await this.ActivatePrivate(target, executeOnLoad);
                }
            }

            this._logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public Task<IOrbitPlugin> Activate(Type plugin, bool executeOnLoad = false)
        {
            //NOTE: fix this temporary solution
            return this.Activate(plugin.Name, executeOnLoad);
        }

        public void Unload(object plugin)
        {
            this._pluginLoader.Unload(plugin);
        }

        protected override void DisposeManagedObjects()
        {
            if (this._pluginScope != null)
            {
                this._pluginScope.Dispose();
                this._pluginScope = null;
            }

            if (this._individualPluginScopes != null)
            {
                foreach (ILifetimeScope scope in this._individualPluginScopes.Values)
                {
                    scope.Dispose();
                }

                this._individualPluginScopes.Clear();
                this._individualPluginScopes = null;
            }
        }

        private static void RegisterPlugin(ContainerBuilder builder, ServiceCollection services, PluginInfo info)
        {
            if (info.IsSingleton)
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().SingleInstance();
            }
            else
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().InstancePerDependency();
            }

            IOrbitPlugin dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(info.PluginType);
            dummy.RegisterServices(services);
        }

        private async Task<IOrbitPlugin> ActivatePrivate(PluginInfo target, bool exectueOnLoad = false)
        {
            if (target.IsSingleton && this._activePlugins.TryGetValue(target.PluginType, out IOrbitPlugin? existingInstance))
            {
                this._logger.LogInformation("Plugin already active (singleton): {Plugin}", target.PluginType.Name);
                return existingInstance;
            }

            IOrbitPlugin? instance = this.CreateInstanceFromScope(target.PluginType);

            if (instance != null && target.IsSingleton)
            {
                this._activePlugins[target.PluginType] = instance;

                PluginEvent @event = new()
                {
                    Type = PluginEventType.Activated,
                    PluginType = target.PluginType
                };

                await this._channel.PublishAsync(@event);
            }

            this._logger.LogInformation("Plugin activated: {Plugin}", target.PluginType.Name);

            if (exectueOnLoad)
            {
                await instance!.OnLoad();
            }

            return instance!;
        }

        private IOrbitPlugin? CreateInstanceFromScope(Type type)
        {
            try
            {
                IServiceProvider serviceProvider = this._config.SharePluginScopes
                    ? this.PluginScope.Resolve<IServiceProvider>()
                    : this.IndividualPluginScopes[type].Resolve<IServiceProvider>();

                return (IOrbitPlugin?)ActivatorUtilities.CreateInstance(serviceProvider, type);
            }
            catch (Exception ex)
            {
                this._logger.LogInformation(ex, "Failed to create instance from scope for type {Type}", type);
                return null;
            }
        }

        private ILifetimeScope CreateSharedScope()
        {
            return this._rootScope.BeginLifetimeScope(builder =>
            {
                ServiceCollection services = new();

                foreach (PluginInfo info in this._validPlugins)
                {
                    RegisterPlugin(builder, services, info);
                }

                builder.Populate(services);
            });
        }
    }
}
