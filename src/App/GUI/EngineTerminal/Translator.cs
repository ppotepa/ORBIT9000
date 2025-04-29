using System.Reflection;
using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    public class Translator
    {
        private readonly object _data;
        private readonly Toplevel _top;
        private readonly PropertyInfo[] _topProperties;
        private readonly Dictionary<string, FrameView> views = new();

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
            _menuBarItems = [.. _topProperties.Select(property => new MenuBarItem()
            {
                Title = property.Name,
                Action = () =>
                {
                    if (views.ContainsKey(property.Name) is false)
                    {
                        var frame = new FrameView(property.Name)
                        {
                            X = 0,
                            Y = 1,
                            Width = Dim.Fill(),
                            Height = Dim.Fill(),
                        };

                        views[property.Name] = frame;
                        _mainView = frame;
                    }
                    else _mainView = views[property.Name];

                    Redraw();

                    Application.Refresh();
                }
            })];
        }

        private void GenerateViews()
        {
            if (_topProperties.Any())
            {
                foreach (var property in _topProperties) {
                    Traverse(property);
                }
            }
        }

        private void Redraw()
        {
            Application.Top.RemoveAll();
            Application.Top.Add(_menuBar);
            Application.Top.Add(_mainView);
        }

        private void Traverse(PropertyInfo property, View previous = null, int depth = 0)
        {
            Console.WriteLine($"Traversing property: {property.Name}, Depth: {depth}");

            if (views.ContainsKey(property.Name) is false)
            {
                Console.WriteLine($"Creating new FrameView for property: {property.Name}");

                var frame = new FrameView(property.Name)
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };

                views[property.Name] = frame;

                if (previous != null)
                {
                    Console.WriteLine($"Adding FrameView for property: {property.Name} to parent view: {previous.GetType().Name}");
                    previous.Add(frame);
                }

                if (property.PropertyType.GetType().GetProperties().Any())
                {
                    Console.WriteLine($"Property {property.Name} has sub-properties. Traversing...");
                    foreach (var subProperty in property.PropertyType.GetType().GetProperties())
                    {
                        Traverse(subProperty, frame, depth + 1);
                    }
                }
            }
            else
            {
                Console.WriteLine($"FrameView for property: {property.Name} already exists. Skipping creation.");
            }
        }
    }
}
