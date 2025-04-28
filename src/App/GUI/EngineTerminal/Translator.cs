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

        public Translator(Toplevel top, object data)
        {
            _top = top;
            _data = data;
            _topProperties = data.GetType().GetProperties();
        }

        public void Translate()
        {
            var topProperties = _data.GetType().GetProperties();
            IEnumerable<MenuBarItem> topItems = GenerateMenuBar(topProperties);

            _menuBar = new MenuBar(topItems.ToArray());

            _top.Add(_menuBar);
            _top.Add(_mainView);

            Application.Init();
        }

        private IEnumerable<MenuBarItem> GenerateMenuBar(System.Reflection.PropertyInfo[] topProperties)
        {
            return topProperties.Select(property => new MenuBarItem()
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
            });
        }

        private void Redraw()
        {
            Application.Top.RemoveAll();
            Application.Top.Add(_menuBar);
            Application.Top.Add(_mainView);
        }
    }
}
