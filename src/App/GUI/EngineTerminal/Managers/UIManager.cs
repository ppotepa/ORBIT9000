<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
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
=======
﻿using EngineTerminal.Bindings;
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
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
            {
                CanFocus = true
            };

<<<<<<< HEAD
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
=======
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
<<<<<<< HEAD
                UpdateControlsFromObject(data.Frame1, "SettingsData");
                UpdateControlsFromObject(data.Frame2, "EngineData");
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
=======
                foreach(var action in updateActions)
                {
                    action(Bindings);
                }   
>>>>>>> b9809e5 (Remove redunant portion of the code.)

                Application.Refresh();
            });
        }

<<<<<<< HEAD
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
=======
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
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
