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
        public void Initialize()
        {
            ExampleData = CreateInitialData();
        }

        /// <summary>
        /// NOTE: future update : WE WANT ONLY UPDATE PROPERTIES THAT HAVE CHANGED
        /// </summary>
        /// <param name="newData"></param>
        public void UpdateData(ExampleData newData, Dictionary<string, ValueBinding> bindings)
        {
            if (newData == null) return;
            
            if (newData.Frame1 != null)
            {
                UpdateProperties(newData.Frame1, ExampleData.Frame1, bindings, nameof(newData.Frame1));
            }
            
            if (newData.Frame2 != null)
            {
                UpdateProperties(newData.Frame2, ExampleData.Frame2, bindings, nameof(newData.Frame2));
            }
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

        private void OnSubPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private void UpdateProperties(object source, object target, Dictionary<string, ValueBinding> bindings, string parentPropertyName)
        {
            if (source == null || target == null) return;

            var properties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var sourceValue = property.GetValue(source);
                    var targetValue = property.GetValue(target);

                    if (!Equals(sourceValue, targetValue))
                    {
                        property.SetValue(target, sourceValue);
                        bindings[$"{parentPropertyName}.{property.Name}"].Value = sourceValue;
                    }
                }
            }
        }
    }
}
