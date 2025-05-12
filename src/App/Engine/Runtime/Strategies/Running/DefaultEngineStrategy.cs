using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Abstractions.Data.Entities;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Core.Models;
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
            {
                throw new InvalidOperationException("Engine state is null.");
            }

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
                    List<Type> entities = pluginType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(t => typeof(IEntity).IsAssignableFrom(t))
                        .ToList();

                    if (entities.Any())
                    {
                        engine.LogInformation("Plugin {PluginType} contains IEntity entities.", pluginType.Name);

                        if (typeof(IEntity).IsAssignableFrom(pluginType))
                        {
                            engine.LogInformation("IEntity type: {EntityType}", pluginType.FullName);
                        }

                        foreach (Type? nested in pluginType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(t => typeof(IEntity).IsAssignableFrom(t)))
                        {
                            engine.LogInformation("IEntity nested type: {EntityType}", nested.FullName);
                        }

                        foreach (PropertyInfo? prop in pluginType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => typeof(IEntity).IsAssignableFrom(p.PropertyType)))
                        {
                            engine.LogInformation("IEntity property: {PropertyName} ({PropertyType})", prop.Name, prop.PropertyType.FullName);
                        }

                        foreach (FieldInfo? field in pluginType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                            .Where(f => typeof(IEntity).IsAssignableFrom(f.FieldType)))
                        {
                            engine.LogInformation("IEntity field: {FieldName} ({FieldType})", field.Name, field.FieldType.FullName);
                        }
                    }

                    List<IEngineAttribute> engineAttributes =
                        [.. pluginType.GetCustomAttributes().OfType<IEngineAttribute>()];

                    if (engineAttributes.Count != 0)
                    {
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

                engine.LogInformation("Plugin initialization completed.");
            }
            catch (Exception ex)
            {
                engine.LogError("An error occurred during plugin initialization: {Message}", ex.Message);
            }
        }
    }
}