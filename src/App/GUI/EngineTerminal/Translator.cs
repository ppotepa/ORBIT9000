using EngineTerminal;
using System.Reflection;
using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    public class Translator
    {
        private readonly Dictionary<string, ValueBinding> _allBindings = new();
        private readonly int _cols;
        private readonly ExampleData _input;
        private readonly PropertyInfo[] _properties;
        private readonly int _rows;
        private readonly string _secret = @"
              ______
            /        \
           |  ( o  o ) |
           |    (_)    |   <- Pope John Paul II
            \  \___/  /
             |_______|
            /         \
           |  +----+  |  <- Papal garments (symbolic)
           |  | JP2 |  |
            \_______/
       ";

        private readonly Toplevel _top;
        private readonly Dictionary<string, FrameView> _views = new();

        private View _mainView;
        private MenuBar _menuBar;

        public Translator(Toplevel top, ExampleData input, int rows = 5, int cols = 5)
        {
            _top = top;

            _properties = input.GetType().GetProperties();

            _rows = rows;
            _cols = cols;
            _input = input;

            _mainView = CreateFrame("Main View");
        }

        public Dictionary<string, ValueBinding> Translate()
        {
            BuildMenuBar();
            BuildViews(_input);

            _top.Add(_menuBar, _mainView);
            return _allBindings;
        }

        private void AddBindingProperty(PropertyInfo info,
            ref object value, ref object parent,
            string route, View container,
            int index, int depth)
        {
            int row = index / _cols;
            int col = index % _cols;

            var frame = new FrameView($"{route} (Depth: {depth})")
            {
                X = col * 25,
                Y = row * 5,

                Width = Dim.Percent(20),
                Height = Dim.Percent(20)
            };

            Label label = new Label(0, 0, "Value:");

            TextField textField = new TextField(15, 0, 20, value?.ToString() ?? string.Empty);
            ValueBinding binding = new ValueBinding(textField, ref value);

            _allBindings[route] = binding;

            textField.TextChanged += PipelineFactory.Instance.Builder
                .Create(textField, binding, parent, info)
                .AddIf(() => textField.Text == "2137", _ => MessageBox.Query("Secret", _secret, "OK"))
                .Build();

            frame.Add(label, textField);
            container.Add(frame);
        }

        private void BuildMenuBar()
        {
            MenuBarItem[] items = _properties.Select(property => CreateMenuItem(property)).ToArray();
            _menuBar = new MenuBar(items) { X = 0 };
        }

        private void BuildViews(ExampleData data)
        {
            if (_properties.Length == 0) return;
            Traverse(ref data);
        }

        private FrameView CreateFrame(string title)
        {
            return new FrameView(title)
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
        }

        private MenuBarItem CreateMenuItem(PropertyInfo property)
        {
            _views[property.Name] = CreateFrame(property.Name);

            return new MenuBarItem(property.Name, string.Empty, () =>
            {
                _mainView = _views[property.Name];
                Redraw();
            });
        }

        private void Redraw()
        {
            Application.Top.RemoveAll();
            Application.Top.Add(_menuBar, _mainView);
            Application.Refresh();
        }

        private void Traverse(ref ExampleData input, int depth = 0)
        {
            foreach (PropertyInfo property in input.GetType().GetProperties())
            {
                object? current = property.GetValue(input);

                //TODO: Not sure if it's going to work without for objects with more nested properties
                //Currently aiming at maximal depth of 1

                // Pass 'input' as 'ref object' to TraverseMenuItem
                object parent = input; // Box the struct to pass as ref object
                TraverseMenuItem(property, ref current, ref parent, depth + 1, property.Name, _views[property.Name]);
                // Unbox and update the input after modifications
                input = (ExampleData)parent;
            }
        }

        private void TraverseMenuItem(PropertyInfo info, ref object current, ref object parent, int depth, string route, View container, int index = 0)
        {
            string baseKey = route.Split('.')[0];

            if (info.PropertyType.IsClass && info.PropertyType.GetProperties().Length > 0 && current is not string && current is not int)
            {
                foreach (var sub in info.PropertyType.GetProperties())
                {
                    object? subValue = sub.GetValue(current);
                    string subRoute = $"{route}.{sub.Name}";

                    TraverseMenuItem(sub, ref subValue, ref current, depth + 1, subRoute, container, index++);
                }

                return;
            }

            if (current is string || current is int)
            {
                AddBindingProperty(info, ref current, ref parent, route, container, index, depth);
            }
        }
    }
}
