using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terminal.Gui.CustomViews.Misc;

namespace Terminal.Gui.CustomViews
{
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
        protected virtual void OnBindingChanged(string propertyName, object? oldValue, object? newValue)
        {
            BindingChanged?.Invoke(this, new BindingChangedEventArgs(propertyName, oldValue, newValue));
        }

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

        private const BindingFlags NOT_INHERITED = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

        private readonly int _cols, _rows;
        private readonly object _data;
        private readonly View _top;

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
        public PropertyGridView(View top, object data, int rows = 5, int cols = 5)
        {
            this._top = top;
            this._data = data;
            this._rows = rows;
            this._cols = cols;

            this._main = new FrameView("Main")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            this.Bindings = this.TranslateView();
        }

        /// <summary>
        /// Redraws the property grid, refreshing the property bindings and layout.
        /// </summary>
        /// <param name="bounds">The bounding rectangle for the view.</param>
        public override void Redraw(Rect bounds)
        {
            this.TranslateView();
            base.Redraw(bounds);
        }

        private readonly Dictionary<Type, PropertyInfo[]> _propertyInfoCache = [];

        /// <summary>
        /// Creates the property grid view structure based on the provided data object.
        /// </summary>
        /// <returns>A dictionary of property bindings created during the translation.</returns>
        public Dictionary<string, ValueBinding> TranslateView()
        {
            Type dataType = this._data.GetType();
            if (!this._propertyInfoCache.TryGetValue(dataType, out PropertyInfo[]? properties))
            {
                properties = dataType.GetProperties(NOT_INHERITED);
                this._propertyInfoCache[dataType] = properties;
            }

            MenuBarItem[] items = new MenuBarItem[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo info = properties[i];
                FrameView frame = new(info.Name) { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };

                items[i] = new MenuBarItem(info.Name, "", () =>
                {
                    this._main.RemoveAll();
                    this._main.Add(frame);

                    Application.Refresh();
                });

                this.GeneratePropertyGrid(frame, info);
            }

            MenuBar? menuBar = Application.Top.Subviews.OfType<MenuBar>()
              .FirstOrDefault();

            if (menuBar is not null)
            {
                menuBar.Data = items;
                this._top.Add(this._main);
            }
            else
            {
                menuBar = new MenuBar(items);
                this._top.Add(menuBar, this._main);
            }

            return this.Bindings;
        }

        /// <summary>
        /// Generates UI elements for editing the properties of a given object.
        /// </summary>
        /// <param name="container">The frame view that will contain the property editors.</param>
        /// <param name="info">The property info representing the object whose properties will be displayed.</param>
        private void GeneratePropertyGrid(FrameView container, PropertyInfo info)
        {
            object? val = info.GetValue(this._data);

            if (info.PropertyType.IsClass && info.PropertyType.GetProperties(NOT_INHERITED).Length > 0 && val != null)
            {
                int index = 0;
                foreach (PropertyInfo subProperty in info.PropertyType.GetProperties(NOT_INHERITED))
                {
                    object? subValue = subProperty.GetValue(val);
                    string route = $"{info.Name}.{subProperty.Name}";

                    View frameItem = new()
                    {
                        X = Pos.Percent((index % this._cols) * (100f / this._cols)),
                        Y = Pos.Percent((index / this._cols) * (100f / this._rows)),

                        Width = Dim.Percent(100f / this._cols),
                        Height = Dim.Percent(100f / this._rows),
                    };

                    Label label = new(0, 0, subProperty.Name, true);
                    TextField text = new(15, 0, 20, subValue?.ToString() ?? "")
                    {
                        Id = route
                    };

                    ValueBinding binding = new(text, ref subValue!);

                    this.Bindings[route] = binding;

                    text.TextChanged += PipelineFactory.Builder
                        .Create(text, binding, val!, subProperty)
                        .AddIf(() => text.Text == "2137", _ => MessageBox.Query("Secret", this.SECRET, "OK"))
                        .Build();

                    frameItem.Add(label, text);
                    container.Add(frameItem);
                    index++;
                }
            }
        }
    }
}