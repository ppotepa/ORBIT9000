using EngineTerminal.Contracts;
using Terminal.Gui;
using Terminal.Gui.CustomViews;
using Terminal.Gui.CustomViews.Misc;

namespace EngineTerminal.Managers
{
    public class UIManager : IUIManager
    {
        public delegate void BindingAction(Dictionary<string, ValueBinding> bindings);

        private readonly StatusItem StatusItem = new(Key.Null, "Starting...", null);
        private readonly StatusItem AdditionalStatusItem = new(Key.Null, "...", null);
        private readonly StatusItem CurrentMethod = new(Key.Null, "[No Invocations just yet]", null);

        public StatusBar? StatusBar { get; private set; }
        public Dictionary<string, ValueBinding> GridBindings
        {
            get
            {
                if (Grid == null)
                {
                    throw new InvalidOperationException("Grid is not initialized.");
                }

                return Grid.Bindings ?? [];
            }
        }

        public View? MainView { get; private set; }
        public PropertyGridView? Grid { get; private set; }
        public MenuBar? MenuBar { get; private set; }

        public void Initialize(object data)
        {
            Application.Init();

            MainView = new View { X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill() - 1 };
            Grid = new PropertyGridView(MainView, data);
            MenuBar = new MenuBar();

            StatusBar = new StatusBar([new StatusItem(Key.F1, "~F1~ Help", ShowHelp), StatusItem, AdditionalStatusItem, CurrentMethod])
            {
                CanFocus = true
            };

            Application.Top.Add(MenuBar, MainView, StatusBar);
        }

        public void Run() => Application.Run();

        public void UpdateStatusMessage(string? message, string? additionalInfo = default)
        {
            Application.MainLoop.Invoke(() =>
            {
                if (additionalInfo != null)
                {
                    AdditionalStatusItem.Title = additionalInfo;
                }
                else
                {
                    StatusItem.Title = $"{DateTime.Now.TimeOfDay} {message}";
                }

                Application.Refresh();
            });
        }

        public void UpdateCurrentMethod(string? message)
        {
            Application.MainLoop.Invoke(() =>
            {
                CurrentMethod.Title = message;
                Application.Refresh();
            });
        }

        public void UpdateUIFromData(object? sender, IReadOnlyList<BindingAction> updates)
        {
            Application.MainLoop.Invoke(() =>
            {
                foreach (BindingAction update in updates)
                {
                    update(Grid!.Bindings);
                }

                Application.Refresh();
            });
        }

        private static void ShowHelp()
        {
            MessageBox.Query("Help", "Engine Terminal\nUse menus to navigate.\nValues update automatically.", "OK");
        }
    }
}