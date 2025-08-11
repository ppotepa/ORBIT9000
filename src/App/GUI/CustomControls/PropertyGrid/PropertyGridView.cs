using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
<<<<<<< HEAD
=======
using Terminal.Gui;
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
using Terminal.Gui.CustomViews.Misc;

namespace Terminal.Gui.CustomViews
{
<<<<<<< HEAD
    /// <summary>
    /// A specialized view that displays and allows editing of object properties in a grid layout.
    /// It supports property navigation through a menu bar and provides binding capabilities.
    /// </summary>
    public class PropertyGridView : View
    {
        /// <summary>
        /// Event triggered when a property binding value changes.
        /// </summary>
        public event EventHandler<BindingChangedEventArgs>? BindingChanged;

        /// <summary>
        /// Raises the <see cref="BindingChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
=======
    public class PropertyGridView : View
    {
        public event EventHandler<BindingChangedEventArgs>? BindingChanged;

>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
        protected virtual void OnBindingChanged(string propertyName, object? oldValue, object? newValue)
        {
            BindingChanged?.Invoke(this, new BindingChangedEventArgs(propertyName, oldValue, newValue));
        }

<<<<<<< HEAD
        /// <summary>
        /// Event arguments for the <see cref="BindingChanged"/> event.
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the <see cref="BindingChangedEventArgs"/> class.
        /// </remarks>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public class BindingChangedEventArgs(string propertyName, object? oldValue, object? newValue) : EventArgs
        {
            /// <summary>
            /// Gets the name of the property that changed.
            /// </summary>
            public string? PropertyName { get; } = propertyName;

            /// <summary>
            /// Gets the previous value of the property.
            /// </summary>
            public object? OldValue { get; } = oldValue;

            /// <summary>
            /// Gets the new value of the property.
            /// </summary>
            public object? NewValue { get; } = newValue;
        }

=======
        public class BindingChangedEventArgs : EventArgs
        {
            public string PropertyName { get; }
            public object? OldValue { get; }
            public object? NewValue { get; }

            public BindingChangedEventArgs(string propertyName, object? oldValue, object? newValue)
            {
                PropertyName = propertyName;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
        private const BindingFlags NOT_INHERITED = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly int _cols, _rows;
        private readonly object _data;
        private readonly View _top;

<<<<<<< HEAD
        private readonly ustring SECRET = "👴🏼";

        /// <summary>
        /// A dictionary containing all value bindings managed by this property grid.
        /// Keys are property paths in format "PropertyName.SubPropertyName".
        /// </summary>
        public Dictionary<string, ValueBinding> Bindings { get; }

        private readonly View _main;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridView"/> class.
        /// </summary>
        /// <param name="top">The parent view that will contain this property grid.</param>
        /// <param name="data">The object whose properties will be displayed and edited.</param>
        /// <param name="rows">The number of rows in the property grid layout.</param>
        /// <param name="cols">The number of columns in the property grid layout.</param>
=======

        private readonly ustring SECRET =
@"
┌──────────────────────────────────────────────┐
│ WHEN YOU’RE POLISH, LOVE KARATE,            │
│ AND VISIT 104 COUNTRIES AS POPE            │
└──────────────────────────────────────────────┘

             .-""""-.
           .'        '.
          /   _  _     \
         |   (o)(o)     |
         |     __       |
         |   .'  '.     |
         |  |      |    |
         |   '.  .'     |
          \    '--'    /
           '.        .'
             '-.__.-'

             /|     |\
            /_|_____|_\
              |     |
              |     |
              |     |
             /|     |\
            /_|_____|_\

┌──────────────────────────────────────────────┐
│ “I TRAVELLED MORE THAN YOUR AVERAGE PILGRIM”│
└──────────────────────────────────────────────┘";
        public readonly Dictionary<string, ValueBinding> Bindings = new Dictionary<string, ValueBinding>();


        private View _main;

>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
        public PropertyGridView(View top, object data, int rows = 5, int cols = 5)
        {
            _top = top;
            _data = data;
            _rows = rows;
            _cols = cols;

            _main = new FrameView("Main")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

<<<<<<< HEAD
            Bindings = TranslateView();
        }

        /// <summary>
        /// Redraws the property grid, refreshing the property bindings and layout.
        /// </summary>
        /// <param name="bounds">The bounding rectangle for the view.</param>
=======
            this.Bindings = TranslateView();
        }

>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
        public override void Redraw(Rect bounds)
        {
            TranslateView();
            base.Redraw(bounds);
        }
<<<<<<< HEAD

        private readonly Dictionary<Type, PropertyInfo[]> _propertyInfoCache = [];

        /// <summary>
        /// Creates the property grid view structure based on the provided data object.
        /// </summary>
        /// <returns>A dictionary of property bindings created during the translation.</returns>
        public Dictionary<string, ValueBinding> TranslateView()
        {
            Type dataType = _data.GetType();
            if (!_propertyInfoCache.TryGetValue(dataType, out PropertyInfo[]? properties))
=======
        private Dictionary<Type, PropertyInfo[]> _propertyInfoCache = new();

        public Dictionary<string, ValueBinding> TranslateView()
        {
            Type dataType = _data.GetType();
            if (!_propertyInfoCache.TryGetValue(dataType, out PropertyInfo[] properties))
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
            {
                properties = dataType.GetProperties(NOT_INHERITED);
                _propertyInfoCache[dataType] = properties;
            }

            MenuBarItem[] items = new MenuBarItem[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo info = properties[i];
<<<<<<< HEAD
                FrameView frame = new(info.Name) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };
=======
                FrameView frame = new FrameView(info.Name) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)

                items[i] = new MenuBarItem(info.Name, "", () =>
                {
                    _main.RemoveAll();
                    _main.Add(frame);

                    Application.Refresh();
                });

                GeneratePropertyGrid(frame, info);
            }

<<<<<<< HEAD
            MenuBar? menuBar = Application.Top.Subviews.OfType<MenuBar>()
=======
            var menuBar = Application.Top.Subviews.OfType<MenuBar>()
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
              .FirstOrDefault();

            if (menuBar is not null)
            {
                menuBar.Data = items;
                _top.Add(_main);
            }
            else
            {
                menuBar = new MenuBar(items);
                _top.Add(menuBar, _main);
            }

<<<<<<< HEAD
            return Bindings;
        }

        /// <summary>
        /// Generates UI elements for editing the properties of a given object.
        /// </summary>
        /// <param name="container">The frame view that will contain the property editors.</param>
        /// <param name="info">The property info representing the object whose properties will be displayed.</param>
        private void GeneratePropertyGrid(FrameView container, PropertyInfo info)
        {
            object? val = info.GetValue(_data);
=======
            return this.Bindings;
        }

        private void GeneratePropertyGrid(FrameView container, PropertyInfo info)
        {
            var val = info.GetValue(_data);
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)

            if (info.PropertyType.IsClass && info.PropertyType.GetProperties(NOT_INHERITED).Length > 0 && val != null)
            {
                int index = 0;
<<<<<<< HEAD
                foreach (PropertyInfo subProperty in info.PropertyType.GetProperties(NOT_INHERITED))
                {
                    object? subValue = subProperty.GetValue(val);
                    string route = $"{info.Name}.{subProperty.Name}";

                    View frameItem = new()
                    {
                        X = Pos.Percent((index % _cols) * (100f / _cols)),
                        Y = Pos.Percent((index / _cols) * (100f / _rows)),

                        Width = Dim.Percent(100f / _cols),
                        Height = Dim.Percent(100f / _rows),
                    };

                    Label label = new(0, 0, subProperty.Name, true);
                    TextField text = new(15, 0, 20, subValue?.ToString() ?? "")
                    {
                        Id = route
                    };

                    ValueBinding binding = new(text, ref subValue!);

                    Bindings[route] = binding;

                    text.TextChanged += PipelineFactory.Builder
=======
                foreach (var subProperty in info.PropertyType.GetProperties(NOT_INHERITED))
                {
                    var subValue = subProperty.GetValue(val);
                    var route = $"{info.Name}.{subProperty.Name}";

                    var frameItem = new View()
                    {
                        X = Pos.Percent((index % _cols) * (100 / _cols)),
                        Y = Pos.Percent((index / _cols) * (100 / _rows)),

                        Width = Dim.Percent(100 / _cols),
                        Height = Dim.Percent(100 / _rows),
                    };

                    var label = new Label(0, 0, subProperty.Name, true);
                    var text = new TextField(15, 0, 20, subValue?.ToString() ?? "");

                    text.Id = route;

                    ValueBinding binding = new ValueBinding(text, ref subValue!);

                    Bindings[route] = binding;

                    text.TextChanged += PipelineFactory.Instance.Builder
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
                        .Create(text, binding, val!, subProperty)
                        .AddIf(() => text.Text == "2137", _ => MessageBox.Query("Secret", SECRET, "OK"))
                        .Build();

                    frameItem.Add(label, text);
                    container.Add(frameItem);
<<<<<<< HEAD
=======

>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
                    index++;
                }
            }
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
