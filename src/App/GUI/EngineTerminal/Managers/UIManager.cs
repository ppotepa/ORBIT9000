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
        private StatusItem StatusItem = new StatusItem(Key.Null, "Starting...", null);

        public StatusBar StatusBar { get; private set; }
        public Dictionary<string, ValueBinding> Bindings { get; private set; }
        public View MainView { get; private set; }
        public PropertyGridView Grid { get; private set; }
        public MenuBar MenuBar { get; private set; }

        public void Initialize(ExampleData initialData)
        {
            Application.Init();

            MainView = new View { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() - 1 };
            Grid = new PropertyGridView(MainView, initialData);
            MenuBar = new MenuBar();

            StatusBar = new StatusBar([new StatusItem(Key.F1, "~F1~ Help", ShowHelp), StatusItem])
            {
                CanFocus = true
            };

            Application.Top.Add(MenuBar, MainView, StatusBar);
        }

        public void Run() => Application.Run();

        public void UpdateStatusMessage(string message)
        {
            Application.MainLoop.Invoke(() =>
            {
                StatusItem.Title = $"{DateTime.Now.TimeOfDay} {message}";
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