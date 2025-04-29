using System.Reflection;
using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    public class Translator
    {
        private readonly object _data;
        private readonly Toplevel _top;
        private readonly PropertyInfo[] _topProperties;
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

        public Translator(Toplevel top, object data)
        {
            _top = top;
            _data = data;
            _topProperties = data.GetType().GetProperties();
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
            _menuBarItems = _topProperties.Select(property =>
            {
                var frameView = new FrameView(property.Name)
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };

                views[property.Name] = frameView;

                return new MenuBarItem
                {
                    Title = property.Name,
                    Action = () =>
                    {
                        if (!views.ContainsKey(property.Name))
                        {
                            var newFrame = new FrameView(property.Name)
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
            }).ToArray();
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
                TraverseMenuItem(property.GetValue(data), views[property.Name], depth + 1, property.Name);
            }
        }

        private void TraverseMenuItem(object? prop, View? parent, int depth, string route)
        {
            if (prop is not string)
            {
                if (prop is not null && prop is not string)
                {
                    foreach (PropertyInfo property in prop.GetType().GetProperties())
                    {
                        TraverseMenuItem(property.GetValue(prop), views[route], depth + 1, route);
                    }
                }
            }
            else if (prop is string)
            {
                Label label = new Label
                {
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                    Text = prop.ToString() ?? string.Empty,
                };

                if (views.TryGetValue(route, out var view))
                {
                    view.Add(label);
                }
            }
        }
    }
}
