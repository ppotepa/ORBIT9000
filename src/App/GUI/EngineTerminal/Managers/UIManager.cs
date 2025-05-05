using EngineTerminal.Bindings;
using EngineTerminal.Contracts;
using EngineTerminal.Processing;
using Orbit9000.EngineTerminal;
using ORBIT9000.Core.Models.Pipe;
using Terminal.Gui;

namespace EngineTerminal.Managers
{
    public class UIManager : IUIManager
    {
        private StatusItem _statusItem;
        public Dictionary<string, ValueBinding> Bindings { get; private set; }
        public PropertyGridView Grid { get; private set; }

        public void Initialize(ExampleData initialData)
        {
            Application.Init();

            var top = new View { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() - 1 };
            Grid = new PropertyGridView(top, initialData);
            var menuBar = new MenuBar();

            _statusItem = new StatusItem(Key.Null, "Starting...", null);

            StatusBar statusBar = new StatusBar([new StatusItem(Key.F1, "~F1~ Help", ShowHelp), _statusItem])
            {
                CanFocus = true 
            };

            Application.Top.Add(menuBar, top, statusBar);
        }

        public void Run() => Application.Run();

        public void UpdateStatusMessage(string message)
        {
            Application.MainLoop.Invoke(() =>
            {
                _statusItem.Title = $"{DateTime.Now.TimeOfDay} {message}";
                Application.Refresh();
            });
            
        }

        public void UpdateUIFromData(object sender, IReadOnlyList<BindingAction> updates)
        {
            Application.MainLoop.Invoke(() =>
            {
                foreach (var update in updates)
                {
                    update(Grid.Bindings);
                }

                Application.Refresh();
            });
        }

        private void ShowHelp()
        {
            MessageBox.Query("Help", "Engine Terminal\nUse menus to navigate.\nValues update automatically.", "OK");
        }
    }
}