using EngineTerminal;
using System.Reflection;
using System.Runtime.InteropServices;
using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    public class Translator
    {
        private readonly int _colNo;

        private readonly int _rowNo;
        private readonly string _secret =
@"
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
        private readonly FieldInfo[] _topFields;
        private readonly Dictionary<string, ValueBinding> ALL_BINDINGS = new Dictionary<string, ValueBinding>();
        private readonly Dictionary<string, FrameView> views = [];
        private ExampleData _data;
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

        public Translator(Toplevel top, ref ExampleData data, int rowNo = 5, int colNo = 5)
        {
            _top = top;
            _data = data;
            _topFields = data.GetType().GetFields();

            _rowNo = rowNo;
            _colNo = colNo;
        }

        public Dictionary<string, ValueBinding> Translate()
        {
            GenerateMenuBar();

            _menuBar = new MenuBar(_menuBarItems);
            _menuBar.X = 0;

            GenerateViews();

            _top.Add(_menuBar);
            _top.Add(_mainView);

            return this.ALL_BINDINGS;
        }

        private void GenerateMenuBar()
        {
            _menuBarItems = [.. _topFields.Select(field =>
            {
                views[field.Name] = new FrameView(field.Name)
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                };

                return new MenuBarItem
                {
                    Title = field.Name,
                    Action = () =>
                    {
                        if (!views.ContainsKey(field.Name))
                        {
                            FrameView newFrame = new FrameView(field.Name)
                            {
                                X = 0,
                                Y = 1,
                                Width = Dim.Fill(),
                                Height = Dim.Fill(),
                            };

                            views[field.Name] = newFrame;
                            _mainView = newFrame;
                        }
                        else
                        {
                            _mainView = views[field.Name];
                        }

                        Redraw();
                        Application.Refresh();
                    }
                };
            })];
        }

        private void GenerateViews()
        {
            if (_topFields.Any())
            {
                Traverse(ref _data);
            }
        }

        private void Redraw()
        {
            Application.Top.RemoveAll();
            Application.Top.Add(_menuBar);
            Application.Top.Add(_mainView);
        }

        private void Traverse(ref ExampleData data, int depth = 0)
        {
            FieldInfo[] fields = data.GetType().GetFields();

            foreach (var field in fields)
            {
                Console.Title = "";
                Console.Title = data.Frame1.GetHashCode().ToString();
                TraverseMenuItem((field, field.GetValue(data)), data, depth + 1, field.Name, views[field.Name]);
            }
        }

        private void TraverseMenuItem((FieldInfo info, object value) binding, object parent, int depth, string route, View? root, int xIndex = 0)
        {
            string baseRoute = route.Split(".").FirstOrDefault();

            switch (binding.value)
            {
                case (not string and not int):
                    if (binding.info.FieldType.IsValueType &&                        
                        binding.info.FieldType.GetFields().Length > 0)
                    {
                        foreach (FieldInfo field in binding.info.FieldType.GetFields())
                        {
                            string newRoute = route + $".{field.Name}";
                            TraverseMenuItem((info: field, value: field.GetValue(binding.value)), binding.value, depth + 1, newRoute, root, xIndex++);
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

                        ValueBinding bindingValue = new ValueBinding(valueField, ref binding.value);
                        
                        ALL_BINDINGS.Add(route, bindingValue);
             
                        valueField.TextChanged += PipelineFactory.Instance.Builder
                            .Create(valueField, bindingValue, parent, binding.info)
                            .AddIf(() => valueField.Text == "2137", (bindingValue) => MessageBox.Query("Secret", _secret, "OK"))                            
                            .Build(); 

                        frameView.Add(label, valueField);

                        if (views.TryGetValue(baseRoute, out var view))
                        {
                            root?.Add(frameView);
                        }
                       
                }
                break;
            }
        }
    }
}
