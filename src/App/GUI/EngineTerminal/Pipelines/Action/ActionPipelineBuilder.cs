using EngineTerminal.Bindings;
using NStack;
using ORBIT9000.Core.Attributes.UI;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Terminal.Gui;

namespace EngineTerminal.Pipelines.Action
{
    public class ActionPipelineBuilder
    {
        private readonly List<Action<ustring>> _pipeline = new();
        private ValueBinding? _targetBinding;
        private TextField? _valueField;
        private object? _parent;
        private PropertyInfo? _propertyInfo;

        public ActionPipelineBuilder Create(TextField valueField, ValueBinding targetBinding, object parent, PropertyInfo propertyInfo)
        {
            _valueField = valueField ?? throw new ArgumentNullException(nameof(valueField));
            _targetBinding = targetBinding ?? throw new ArgumentNullException(nameof(targetBinding));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            _pipeline.Add(_ => ProcessInputValue());

            return this;
        }

        public ActionPipelineBuilder AddIf(Func<bool> condition, Func<ValueBinding, int> action)
        {
            _pipeline.Add(_ =>
            {
                if (condition())
                {
                    action(_targetBinding!);
                }
            });

            return this;
        }

        public ActionPipelineBuilder AddPost(Action<ustring> action)
        {
            _pipeline.Add(action);
            return this;
        }

        public ActionPipelineBuilder AddPre(Action<ustring> action)
        {
            _pipeline.Insert(0, action);
            return this;
        }

        public Action<ustring> Build() =>
            input =>
            {
                foreach (var action in _pipeline)
                {
                    action(input);
                }
            };

        private void ProcessInputValue()
        {
            if (_valueField == null || _targetBinding == null || _parent == null || _propertyInfo == null)
                return;

            string textValue = _valueField.Text.ToString();

            switch (_propertyInfo.PropertyType)
            {
                case Type type when type == typeof(string):
                    ProcessValue(textValue, textValue);
                    break;

                case Type type when type == typeof(int):
                    if (int.TryParse(textValue, out int intValue))
                        ProcessValue(intValue, textValue);
                    break;

                case Type type when type == typeof(bool):
                    if (bool.TryParse(textValue, out bool boolValue))
                        ProcessValue(boolValue, textValue);
                    break;
            }
        }

        private void ProcessValue<T>(T value, string originalText)
        {
            if (_propertyInfo == null || _parent == null || _targetBinding == null)
                return;

            if (ValidateProperty(_propertyInfo, _parent, value))
            {
                _targetBinding.Value = value;
                _propertyInfo.SetValue(_parent, value);                
            }
        }

        private bool ValidateProperty(PropertyInfo property, object parent, object? value)
        {
            var validationAttributes = property.GetCustomAttributes<ValidationAttribute>();

            if (!validationAttributes.Any())
                return true;

            var validationContext = new ValidationContext(parent)
            {
                MemberName = property.Name
            };

            foreach (var attribute in validationAttributes)
            {
                var result = attribute.GetValidationResult(value, validationContext);
                if (result != ValidationResult.Success)
                {
                    HandleValidationFailure(attribute, property, parent, ref value);
                    return false;
                }
            }

            return true;
        }

        private void HandleValidationFailure(ValidationAttribute attribute, PropertyInfo property, object parent, ref object? value)
        {
            switch (attribute)
            {
                case MaxValueAttribute maxValueAttribute:
                    value = maxValueAttribute.MaxValue;
                    property.SetValue(parent, value);                    
                    MessageBox.ErrorQuery("Validation Error",
                        $"Value reset to maximum: {maxValueAttribute.MaxValue}", "OK");
                    _valueField!.Text = value.ToString();
                    break;

                default:
                    MessageBox.ErrorQuery("Validation Error",
                        attribute.ErrorMessage ?? "Invalid value", "OK");
                    break;
            }
        }
    }
}
