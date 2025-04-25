<<<<<<< HEAD
﻿using Microsoft.Extensions.DependencyInjection;
<<<<<<< HEAD
using ORBIT9000.Abstractions.Data.Entities;
using ORBIT9000.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Engine.Runtime.Pipe;
using ORBIT9000.Engine.Runtime.State;
using System.Reflection;

namespace ORBIT9000.Engine.Runtime.Strategies.Running
{
    internal static class Default
    {
        public static void EngineStartupStrategy(object? obj)
        {
            if (obj is not EngineState { Engine: { } engine })
=======
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Providers;
=======
﻿using ORBIT9000.Core.Abstractions.Loaders;
>>>>>>> ed8e1ec (Remove PreBuild Helper)
using ORBIT9000.Engine.Runtime.State;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal struct Default
    {
        public readonly static ParameterizedThreadStart EngineStartupStrategy = static (obj) =>
        {
            IServiceProvider pluginScope = default;

            if (obj is not EngineState state || state.Engine is null)
>>>>>>> e2b2b5a (Reworked Naming)
            {
                throw new InvalidOperationException("Engine state is null.");
            }

<<<<<<< HEAD
            EngineState? state = obj as EngineState;

            engine.LogInformation("Starting EngineStartupStrategy.");

            if (engine.Configuration.EnableTerminal)
            {
                Task.Run(() => new PipeThreadHandler(state!).StartAsync());
            }

            engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));

            Initialize(engine);

            while (engine.IsRunning)
            {
                engine.LogDebug("Engine loop iteration started.");

                Execute(engine);

                engine.LogDebug("Engine loop iteration completed.");
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }

            engine.LogInformation("EngineStartupStrategy has completed.");
        }

        private static void Execute(OrbitEngine engine)
        {
            try
            {
                foreach (Type plugin in engine.PluginProvider.Plugins)
                {
                    engine.PluginProvider.Activate(plugin);
                }
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred while loading plugins: {Message}", ex.Message);
            }
        }

        private static void Initialize(OrbitEngine engine)
        {
            ITextScheduleParser parser = engine.ServiceProvider.GetService<ITextScheduleParser>()
                ?? throw new InvalidOperationException("Job parser is not available.");

            try
            {
                engine.LogInformation("Initializing plugins with scheduled jobs.");

                foreach (Type pluginType in engine.PluginProvider.Plugins)
                {
                    LogIEntityDetails(engine, pluginType);
                    SchedulePluginJobs(engine, parser, pluginType);
                }

                engine.LogInformation("Plugin initialization completed.");
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred during plugin initialization: {Message}", ex.Message);
            }
        }

        private static void LogIEntityDetails(OrbitEngine engine, Type pluginType)
        {
            List<Type> entities = [.. pluginType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                .Where(t => typeof(IEntity).IsAssignableFrom(t))];

            if (entities.Count == 0)
                return;

            engine.LogInformation("Plugin {PluginType} contains IEntity entities.", pluginType.Name);

            if (typeof(IEntity).IsAssignableFrom(pluginType))
            {
                engine.LogInformation("IEntity type: {EntityType}", pluginType.FullName!);
            }

            foreach (Type? nested in pluginType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                .Where(t => typeof(IEntity).IsAssignableFrom(t)))
            {
                engine.LogInformation("IEntity nested type: {EntityType}", nested.FullName!);
            }

            foreach (PropertyInfo? prop in pluginType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)))
            {
                engine.LogInformation("IEntity property: {PropertyName} ({PropertyType})", prop.Name, prop.PropertyType.FullName!);
            }

            foreach (FieldInfo? field in pluginType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => typeof(IEntity).IsAssignableFrom(f.FieldType)))
            {
                engine.LogInformation("IEntity field: {FieldName} ({FieldType})", field.Name, field.FieldType.FullName!);
            }
        }

        private static void SchedulePluginJobs(OrbitEngine engine, ITextScheduleParser parser, Type pluginType)
        {
            List<IEngineAttribute> engineAttributes =
                [.. pluginType.GetCustomAttributes().OfType<IEngineAttribute>()];

            if (engineAttributes.Count == 0)
                return;

            engine.LogInformation("Found valid engine attributes in plugin: {PluginType}", pluginType.Name);

            foreach (IEngineAttribute? attribute in engineAttributes)
            {
                if (attribute is SchedulableServiceAttribute jobAttribute)
                {
                    IScheduleJob job = parser.Parse(jobAttribute.ScheduleExpression);
                    job.Name = pluginType.Name;
                    engine.LogInformation("Scheduled job in plugin: {PluginType}, Schedule: {Schedule}",
                        pluginType.Name, jobAttribute.ScheduleExpression);

                    engine.Scheduler.Schedule(job, () => engine.PluginProvider.Activate(pluginType, true));
                }
            }
        }
    }
}
=======
            state.Engine.LogInformation("Engine is running. Strategy {Strategy}", nameof(EngineStartupStrategy));
            

            while (state.Engine.IsRunning)
            {
                LoadPlugins!(state.Engine);
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }
        };

        private static readonly Action<OrbitEngine> LoadPlugins = async (engine) =>
        {
            var plugin = engine.PluginProvider.Activate("ExamplePlugin") as IOrbitPlugin;
            await plugin.OnLoad();
        };

        public Default()
        {
        }

        //private static readonly Action<OrbitEngine> LoadPlugins = async (engine) =>
        //{
        //    foreach (Loaders.PluginRegistrationInfo plugin in PluginProvider.GetPluginRegistrationInfo())
        //    {
        //        if (plugin.Tasks.Any() && !plugin.AllowMultiple)
        //        {
        //            engine.LogError("Plugin is already running. {Name}", plugin.Type.Name);
        //            continue;
        //        }

        //        engine.LogInformation("Processing Plugin: {Name}", plugin.Type.Name);

        //        using var scope = engine.ServiceProvider.CreateAsyncScope();
        //        Task task = Task.Run(async () =>
        //        {
        //            await using var scope = engine.ServiceProvider.CreateAsyncScope();
        //            IOrbitPlugin? instance = scope.ServiceProvider.GetService(plugin.Type) as IOrbitPlugin;
        //            await instance.OnLoad();
        //        });

        //        plugin.Tasks.Add(task);

        //        await task.ContinueWith(completed =>
        //        {
        //            engine.LogInformation("Plugin {Name} has finished running.", plugin.Type.Name);
        //            plugin.Tasks.Remove(completed);
        //        });
        //    }
        //};
    }
}
>>>>>>> e2b2b5a (Reworked Naming)
