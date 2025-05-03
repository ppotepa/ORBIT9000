using EngineTerminal.Bindings;
using NStack;
using ORBIT9000.Core.Attributes.UI;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Terminal.Gui;

namespace EngineTerminal.Builders.Pipeline
{
    public class ActionPipelineBuilder
    {
        private readonly List<Action<ustring>> _pipeline = new();
        private TextField? _valueField;
        private ValueBinding? _targetBinding;
        private object? _parent;
        private PropertyInfo? _propertyInfo;

        public ActionPipelineBuilder Create(TextField valueField, ValueBinding targetBinding, object parent, PropertyInfo propertyInfo)
        {
            _valueField = valueField;
            _targetBinding = targetBinding;
            _parent = parent;
            _propertyInfo = propertyInfo;
            _pipeline.Clear();
            _pipeline.Add(_ => ProcessInputValue());
            return this;
        }

        public ActionPipelineBuilder AddIf(Func<bool> condition, Func<ValueBinding, int> action)
        {
            _pipeline.Add(_ =>
            {
                if (condition())
                    action(_targetBinding!);
            });
            return this;
        }

        public ActionPipelineBuilder AddPre(Action<ustring> action)
        {
            _pipeline.Insert(0, action);
            return this;
        }

        public ActionPipelineBuilder AddPost(Action<ustring> action)
        {
            _pipeline.Add(action);
            return this;
        }

        public Action<ustring> Build() => input =>
        {
            foreach (var step in _pipeline)
                step(input);
        };

        private void ProcessInputValue()
        {
            if (_valueField == null || _targetBinding == null || _parent == null || _propertyInfo == null)
                return;

            var text = _valueField.Text.ToString();
            switch (Type.GetTypeCode(_propertyInfo.PropertyType))
            {
                case TypeCode.String:
                    ApplyValue(text, text); break;
                case TypeCode.Int32:
                    if (int.TryParse(text, out var i)) ApplyValue(i, text);
                    break;

                case TypeCode.Boolean:
                    if (bool.TryParse(text, out var b)) ApplyValue(b, text);
                    break;
            }
        }

        private void ApplyValue<T>(T value, string original)
        {
            if (_propertyInfo == null || _parent == null || _targetBinding == null) return;
            if (Validate(_propertyInfo, _parent, value))
            {
                _targetBinding.Value = value!;
                _propertyInfo.SetValue(_parent, value);
            }
        }

        private bool Validate(PropertyInfo prop, object parent, object value)
        {
            var attributes = prop.GetCustomAttributes<ValidationAttribute>();

            if (!attributes.Any())
            {
                return true;
            }

            var context = new ValidationContext(parent) { MemberName = prop.Name };

            foreach (var attr in attributes)
            {
                ValidationResult? res = attr.GetValidationResult(value, context);
                if (res != ValidationResult.Success)
                {
                    HandleFailure(attr, prop, parent, ref value);
                    return false;
                }
            }

            return true;
        }

        private void HandleFailure(ValidationAttribute attr, PropertyInfo prop, object parent, ref object value)
        {
            if (attr is MaxValueAttribute max)
            {
                value = max.MaxValue;
                prop.SetValue(parent, value);
                MessageBox.ErrorQuery("Validation Error", $"Reset to max: {max.MaxValue}", "OK");
                _valueField!.Text = value.ToString();
            }
            else
            {
                MessageBox.ErrorQuery("Validation Error", attr.ErrorMessage ?? "Invalid", "OK");
            }
        }
    }
}