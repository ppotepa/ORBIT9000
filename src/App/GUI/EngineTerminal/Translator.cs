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
                FrameView frameView = new FrameView(property.Name)
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
                TraverseMenuItem((property, property.GetValue(data)), views[property.Name], depth + 1, property.Name);
            }
        }

        private void TraverseMenuItem((PropertyInfo info, object value) item, View? parent, int depth, string route, int xIndex = 0)
        {
            if (item.value is not string)
            {
                if (item.value is not null && item.value is not string)
                {
                    PropertyInfo[] subProperties = item.info.PropertyType.GetProperties();
                    foreach (PropertyInfo property in subProperties)
                    {
                        TraverseMenuItem((info: property, value: property.GetValue(item.value)), views[route], depth + 1, route, xIndex++);
                    }
                }
            }
            else if (item.value is string stringProp)
            {
                int row = xIndex / 5;
                int col = xIndex % 5;

                FrameView frameView = new FrameView
                {
                    Width = Dim.Percent(20f),
                    Height = Dim.Percent(20f),

                    X = col * 25,
                    Y = row * 5,

                    Title = $"{route}.{stringProp}",
                    Text = stringProp ?? string.Empty,
                };

                if (views.TryGetValue(route, out var view))
                {
                    view.Add(frameView);
                }
            }
        }
    }
}
