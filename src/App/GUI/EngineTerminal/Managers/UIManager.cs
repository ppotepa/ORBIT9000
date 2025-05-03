using EngineTerminal.Bindings;
using EngineTerminal.Processors;
using ORBIT9000.Core.Models.Pipe;
using System.ComponentModel;
using System.Reflection;
using Terminal.Gui;

/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.  
/// It is designed with minimal dependencies and libraries to focus on core functionality.  
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace EngineTerminal.Managers
{
    public class UIManager
    {
        private Dictionary<string, ValueBinding> _bindings;
        public Dictionary<string, ValueBinding> Bindings
        {
            get => _bindings;
            private set => _bindings = value;
        }

        private StatusItem _statusItem;
        public StatusItem StatusItem
        {
            get => _statusItem;
            private set => _statusItem = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize(ExampleData data)
        {
            Application.Init();
            var topLayer = new View
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1,
                CanFocus = true
            };

            SetupColorScheme();

            var translator = new Translator(topLayer, data);

            Bindings = translator.Translate();
            StatusItem = new StatusItem(Key.Null, "Starting...", null);

            var statusBar = new StatusBar(new StatusItem[]
            {
                new StatusItem(Key.F1, "~F1~ Help", () => ShowHelp()),
                StatusItem
            })
            {
                CanFocus = true
            };

            Application.Top.Add(topLayer);
            Application.Top.Add(statusBar);

            if (data.Frame1 != null) data.Frame1.PropertyChanged += ForwardPropertyChanged;
            if (data.Frame2 != null) data.Frame2.PropertyChanged += ForwardPropertyChanged;
        }

        public void Run()
        {
            Application.Run();
        }

        public void UpdateStatusMessage(string message)
        {
            Application.MainLoop.Invoke(() => StatusItem.Title = message);
        }

        public void UpdateUIFromData(List<Action<Dictionary<string, ValueBinding>>> updateActions)
        {
            Application.MainLoop.Invoke(() =>
            {
                foreach(var action in updateActions)
                {
                    action(Bindings);
                }   

                Application.Refresh();
            });
        }

        public void UpdateBindingFromPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            string propertyPath = GetPropertyPath(sender, e.PropertyName);

            if (Bindings.TryGetValue(propertyPath, out var binding))
            {
                object propertyValue = sender.GetType().GetProperty(e.PropertyName)?.GetValue(sender);

                Application.MainLoop.Invoke(() =>
                {
                    binding.Value = propertyValue;
                    binding.View.SetNeedsDisplay();
                    Application.Refresh();
                });
            }
        }

        private void ForwardPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private string GetPropertyPath(object sender, string propertyName)
        {
            var type = sender.GetType();
            return $"{type.Name}.{propertyName}";
        }

        private void SetupColorScheme()
        {
            Application.Current.ColorScheme = new ColorScheme
            {
                Normal = Application.Driver.MakeAttribute(Color.White, Color.Blue),
                Focus = Application.Driver.MakeAttribute(Color.Gray, Color.DarkGray),
                HotNormal = Application.Driver.MakeAttribute(Color.BrightBlue, Color.Blue),
                HotFocus = Application.Driver.MakeAttribute(Color.BrightYellow, Color.DarkGray)
            };
        }

        private void ShowHelp()
        {
            MessageBox.Query("Help", "Engine Terminal\n\nUse menus to navigate between views.\nValues update automatically.", "OK");
        }
    }
}
