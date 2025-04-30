using System.Reflection;
using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    public class Translator
    {
        private readonly int _colNo;
        private readonly object _data;
        private readonly int _rowNo;
        private readonly Toplevel _top;

        private readonly PropertyInfo[] _topProperties;

        private readonly Dictionary<string, ValueBinding> ALL_BINDINGS = new Dictionary<string, ValueBinding>();

        private readonly Dictionary<string, FrameView> views = [];
        private View _mainView = new FrameView("Main View")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            Text = "SPACER"
        };

        private MenuBar _menuBar = new();
        private MenuBarItem[] _menuBarItems;

        public Translator(Toplevel top, object data, int rowNo = 5, int colNo = 5)
        {
            _top = top;
            _data = data;
            _topProperties = data.GetType().GetProperties();

            _rowNo = rowNo;
            _colNo = colNo;
        }

        public void Translate()
        {
            GenerateMenuBar();

            _menuBar = new MenuBar(_menuBarItems);
            _menuBar.X = 0;

            GenerateViews();

            _top.Add(_menuBar);
            _top.Add(_mainView);

            Application.Init();
        }

        private void GenerateMenuBar()
        {
            _menuBarItems = [.. _topProperties.Select(property =>
            {
                views[property.Name] = new FrameView(property.Name)
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };

                return new MenuBarItem
                {
                    Title = property.Name,
                    Action = () =>
                    {
                        if (!views.ContainsKey(property.Name))
                        {
                            FrameView newFrame = new FrameView(property.Name)
                            {
                                X = 0,
                                Y = 1,
                                Width = Dim.Fill(),
                                Height = Dim.Fill(),
                            };

                            views[property.Name] = newFrame;
                            _mainView = newFrame;
                        }
                        else
                        {
                            _mainView = views[property.Name];
                        }

                        Redraw();
                        Application.Refresh();
                    }
                };
            })];
        }

        private void GenerateViews()
        {
            if (_topProperties.Any())
            {
                Traverse(_data);
            }
        }

        private void Redraw()
        {
            Application.Top.RemoveAll();
            Application.Top.Add(_menuBar);
            Application.Top.Add(_mainView);
        }

        private void Traverse(object data, int depth = 0)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            PropertyInfo[] properties = data.GetType().GetProperties();

            foreach (var property in properties)
            {
                TraverseMenuItem((property, property.GetValue(data)), depth + 1, property.Name, views[property.Name]);
            }
        }

        private void TraverseMenuItem((PropertyInfo info, object value) binding, int depth, string route, View? parent, int xIndex = 0)
        {
            string baseRoute = route.Split(".").FirstOrDefault();
            switch (binding.value)
            {
                case (not string and not int):
                    if (binding.info.PropertyType.IsClass &&
                        binding.info.PropertyType != typeof(string) &&
                        binding.info.PropertyType.GetProperties().Length > 0)
                    {
                        foreach (PropertyInfo property in binding.info.PropertyType.GetProperties())
                        {
                            string newRoute = route + $".{property.Name}";
                            TraverseMenuItem((info: property, value: property.GetValue(binding.value)), depth + 1, newRoute, parent, xIndex++);
                        }
                    }
                    break;
                case string @string:
                case int @int:
                    {
                        int row = xIndex / _colNo;
                        int col = xIndex % _colNo;

                        FrameView frameView = new FrameView
                        {
                            X = col * 25,
                            Y = row * 5,
                            Width = Dim.Percent(20f),
                            Height = Dim.Percent(20f),
                            Title = $"{route}",
                        };

                        Label label = new Label
                        {
                            X = 0,
                            Y = 0,
                            Text = "Value:",
                        };

                        TextField valueField = new TextField
                        {
                            X = Pos.Right(label) + 1,
                            Y = Pos.Top(label),
                            Width = Dim.Fill(),
                            Text = binding.value.ToString(),
                        };

                        valueField.TextChanged += (s) =>
                        {
                            if (binding.value is int && int.TryParse(valueField.Text.ToString(), out var newValue))
                            {
                                ALL_BINDINGS[route].Value = newValue;

                                if (newValue is 2137)
                                {
                                    ALL_BINDINGS[route].View.Text = "You found the secret!";

                                    MessageBox.Query("Value Changed", $"You found the secret!", "OK");
                                }
                            }
                            else
                            {
                                ALL_BINDINGS[route].Value = valueField.Text.ToString();
                            }
                        };

                        frameView.Add(label, valueField);

                        if (views.TryGetValue(baseRoute, out var view))
                        {
                            parent.Add(frameView);
                        }

                        ALL_BINDINGS.Add(route, new ValueBinding(valueField, binding.value));
                    }
                    break;
            }
        }
    }
}
