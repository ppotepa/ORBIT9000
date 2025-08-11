<<<<<<< HEAD
﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Abstractions.Providers;
using ORBIT9000.Abstractions.Runtime;
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
            _logger = logger;
            _config = config;
            _pluginLoader = pluginLoader;
            _rootScope = rootScope;
            _validPlugins = [.. _config.Plugins.Where(x => x.ContainsPlugins)];
            _channel = channel;
        }

        private Dictionary<Type, ILifetimeScope> IndividualPluginScopes
        {
            get
            {
                if (_individualPluginScopes == null)
                {
                    _individualPluginScopes = [];
                    foreach (PluginInfo info in _validPlugins)
                    {
                        _individualPluginScopes[info.PluginType] = _rootScope.BeginLifetimeScope(builder =>
                        {
                            ServiceCollection services = new();
                            RegisterPlugin(builder, services, info);
                            builder.Populate(services);
                        });
                    }
                }

                return _individualPluginScopes;
            }
        }

        private ILifetimeScope PluginScope => _pluginScope ??= CreateSharedScope();

        public IEnumerable<Type> Plugins => _validPlugins.Select(plugin => plugin.PluginType);

        public async Task<IOrbitPlugin> Activate(object plugin, bool executeOnLoad = false)
        {
            if (plugin is string pluginName)
            {
                PluginInfo? target = _validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));

                if (target != null)
                {
                    return await ActivatePrivate(target, executeOnLoad);
                }
            }

            _logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public Task<IOrbitPlugin> Activate(Type plugin, bool executeOnLoad = false)
        {
            //NOTE: fix this temporary solution
            return Activate(plugin.Name, executeOnLoad);
        }

        public void Unload(object plugin)
        {
            _pluginLoader.Unload(plugin);
        }

        protected override void DisposeManagedObjects()
        {
            if (_pluginScope != null)
            {
                _pluginScope.Dispose();
                _pluginScope = null;
            }

            if (_individualPluginScopes != null)
            {
                foreach (ILifetimeScope scope in _individualPluginScopes.Values)
                {
                    scope.Dispose();
                }

                _individualPluginScopes.Clear();
                _individualPluginScopes = null;
            }
        }

        private void RegisterPlugin(ContainerBuilder builder, ServiceCollection services, PluginInfo info)
        {
            _logger.LogDebug("Registering plugin: {PluginType}, Singleton: {IsSingleton}", info.PluginType.FullName, info.IsSingleton);

            if (info.IsSingleton)
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().SingleInstance();
            }
            else
            {
                builder.RegisterType(info.PluginType).AsSelf().As<IOrbitPlugin>().InstancePerDependency();
            }

            try
            {
                IOrbitPlugin dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(info.PluginType);
                dummy.RegisterServices(services);
                _logger.LogDebug("Registered services for plugin: {PluginType}", info.PluginType.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register services for plugin: {PluginType}", info.PluginType.FullName);
            }
        }

        private async Task<IOrbitPlugin> ActivatePrivate(PluginInfo target, bool exectueOnLoad = false)
        {
            if (target.IsSingleton && _activePlugins.TryGetValue(target.PluginType, out IOrbitPlugin? existingInstance))
            {
                _logger.LogInformation("Plugin already active (singleton): {Plugin}", target.PluginType.Name);
                return existingInstance;
            }

            IOrbitPlugin? instance = CreateInstanceFromScope(target.PluginType);

            if (instance != null && target.IsSingleton)
            {
                _activePlugins[target.PluginType] = instance;

                PluginEvent @event = new()
                {
                    Type = PluginEventType.Activated,
                    PluginType = target.PluginType
                };

                await _channel.PublishAsync(@event);
            }

            _logger.LogInformation("Plugin activated: {Plugin}", target.PluginType.Name);

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
                IServiceProvider serviceProvider = _config.SharePluginScopes
                    ? PluginScope.Resolve<IServiceProvider>()
                    : IndividualPluginScopes[type].Resolve<IServiceProvider>();

                return (IOrbitPlugin?)ActivatorUtilities.CreateInstance(serviceProvider, type);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Failed to create instance from scope for type {Type}", type);
                return null;
            }
        }

        private ILifetimeScope CreateSharedScope()
        {
            _logger.LogInformation("Creating shared plugin scope with {PluginCount} plugins.",
                _validPlugins.Count);

            return _rootScope.BeginLifetimeScope(builder =>
            {
                ServiceCollection services = new();

                foreach (PluginInfo info in _validPlugins)
                {
                    RegisterPlugin(builder, services, info);
                }

                builder.Populate(services);
            });
=======
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using System.Runtime.CompilerServices;

namespace ORBIT9000.Engine.Providers
{
    internal class PluginProvider : IPluginProvider
    {   
        private readonly RuntimeConfiguration _config;
        private readonly ILogger<PluginProvider> _logger;
        private readonly IPluginLoader _pluginLoader;
        private readonly ServiceProvider _pluginServiceProvider;
        private readonly IServiceProvider _provider;
        private readonly List<PluginInfo> _validPlugins;

        public PluginProvider(ILogger<PluginProvider> logger, 
            IPluginLoader pluginLoader, 
            RuntimeConfiguration config, 
            IServiceProvider provider,
            IServiceCollection rootCollection
            )
        {
            _pluginLoader = pluginLoader;
            _logger = logger;
            _config = config;
            _provider = provider;
            _validPlugins = _config.Plugins.Where(x => x.ContainsPlugins).ToList();

            this._validPlugins.ForEach(plugin =>
            {
                var dummy = (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(plugin.PluginType);
                dummy.RegisterServices(rootCollection);
            });

            _pluginServiceProvider = rootCollection.BuildServiceProvider();
        }

        public IOrbitPlugin Activate(object plugin)
        {
            if (plugin is string pluginName)
            {
                var target = _validPlugins.FirstOrDefault(x => x.PluginType.Name.Contains(pluginName));
                if (target != null)
                {
                    var scope = _pluginServiceProvider.CreateScope();
                    IOrbitPlugin? instance = (IOrbitPlugin)ActivatorUtilities.CreateInstance(scope.ServiceProvider, target.PluginType);
                    return instance;
                }
            }

            _logger.LogError("Plugin activation failed. Invalid plugin identifier: {Plugin}", plugin);
            throw new ArgumentException("Invalid plugin identifier.", nameof(plugin));
        }

        public IOrbitPlugin Activate(Type plugin)
        {
<<<<<<< HEAD
            IServiceScope scope = _provider.CreateScope();
            return ActivatorUtilities.CreateInstance(scope.ServiceProvider, plugin, []) as IOrbitPlugin;
        }

        public Type[] GetPluginRegistrationInfo()
        {
            return _config.Plugins.Select(x => x.PluginType).ToArray();
        }

        public IOrbitPlugin Register(Type plugin)
        {
            IServiceScope scope = _provider.CreateScope();
            return (IOrbitPlugin)RuntimeHelpers.GetUninitializedObject(plugin);
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
=======
            throw new NotImplementedException();
>>>>>>> ed8e1ec (Remove PreBuild Helper)
        }
    }
}
