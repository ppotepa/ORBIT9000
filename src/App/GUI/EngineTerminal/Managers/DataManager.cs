using EngineTerminal.Bindings;
using ORBIT9000.Core.Models.Pipe;
using System.ComponentModel;
using System.Reflection;

/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.  
/// It is designed with minimal dependencies and libraries to focus on core functionality.  
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace EngineTerminal.Managers
{
    public class DataManager
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ExampleData ExampleData { get; private set; }

        /// <summary>
        /// NOTE: future update : WE WANT ONLY UPDATE PROPERTIES THAT HAVE CHANGED
        /// </summary>
        /// <param name="newData"></param>
        public List<Action<Dictionary<string, ValueBinding>>> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings)
        {
            if (newData == null) return [];

            if (newData.Frame1 != null && newData.Frame1 != ExampleData.Frame1)
            {
                return GetUpdateActions(newData.Frame1, ExampleData.Frame1, nameof(newData.Frame1));
            }

            if (newData.Frame2 != null && newData.Frame2 != ExampleData.Frame2)
            {
                return GetUpdateActions(newData.Frame2, ExampleData.Frame2, nameof(newData.Frame2));
            }

            return [];
        }

        public void Initialize()
        {
            ExampleData = CreateInitialData();
        }
        private ExampleData CreateInitialData()
        {
            var data = new ExampleData
            {
                Frame1 = new SettingsData
                {
                    Setting1 = 1,
                    Setting2 = "Text2"
                },
                Frame2 = new EngineData
                {
                    Setting1 = 100,
                    Setting2 = 200,
                    IsValid = false
                }
            };
            
            data.Frame1.PropertyChanged += OnSubPropertyChanged;
            data.Frame2.PropertyChanged += OnSubPropertyChanged;

            return data;
        }

        private List<Action<Dictionary<string, ValueBinding>>> GetUpdateActions(object source, object target, string parentPropertyName)
        {
            var actions = new List<Action<Dictionary<string, ValueBinding>>>();

            if (source == null || target == null) return actions;

            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var sourceValue = property.GetValue(source);
                    var targetValue = property.GetValue(target);

                    if (!Equals(sourceValue, targetValue))
                    {
                        actions.Add((bindings) =>
                        {
                            property.SetValue(target, sourceValue);
                            bindings[$"{parentPropertyName}.{property.Name}"].Value = sourceValue;
                        });
                    }
                }
            }

            return actions;
        }

        private void OnSubPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
