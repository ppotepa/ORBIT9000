using NStack;
using Orbit9000.EngineTerminal;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Terminal.Gui;

namespace EngineTerminal
{
    public class ActionPipelineBuilder
    {
        private List<Action<ustring>> _actions = new List<Action<ustring>>();
        private ValueBinding _targetBinding;
        private TextField _valueField;

        public ActionPipelineBuilder AddIf(Func<bool> condition, Func<ValueBinding, int> action)
        {
            _actions.Add((ustring input) =>
            {
                bool conditionMet = condition();
                if (conditionMet)
                {
                    action(_targetBinding);
                }
            });

            return this;
        }

        public ActionPipelineBuilder AddPost(Action<ustring> action)
        {
            _actions.Add(action);
            return this;
        }

        public ActionPipelineBuilder AddPre(Action<ustring> action)
        {
            List<Action<ustring>> updatedActions = _actions.Prepend(action).ToList();
            _actions = updatedActions;
            return this;
        }

        public Action<ustring> Build()
        {
            return (ustring input) =>
            {
                foreach (Action<ustring> action in _actions)
                {
                    action(input);
                }
            };
        }

        public ActionPipelineBuilder Create(TextField valueField, ValueBinding targetBinding, object parent, PropertyInfo info)
        {
            _valueField = valueField;
            _targetBinding = targetBinding;

            _actions.Add((ustring input) =>
            {
                string textValue = _valueField.Text.ToString();

                if (info.PropertyType == typeof(string))
                {
                    if (ValidateProperty(info, ref parent, textValue))
                    {
                        _targetBinding.Value = textValue;
                        info.SetValue(parent, textValue);
                    }
                }
                else if (info.PropertyType == typeof(int))
                {
                    bool success = int.TryParse(textValue, out int intValue);

                    if (success && ValidateProperty(info, ref parent, intValue))
                    {
                        _targetBinding.Value = intValue;
                        info.SetValue(parent, intValue);
                    }
                }
                else if (info.PropertyType == typeof(bool))
                {
                    bool success = bool.TryParse(textValue, out bool boolValue);

                    if (success && ValidateProperty(info, ref parent, boolValue))
                    {
                        _targetBinding.Value = boolValue;
                        info.SetValue(parent, boolValue);
                    }
                }
            });

            return this;
        }

        private bool ValidateProperty(PropertyInfo property, ref object parent,  object value)
        {
            var validationAttributes = property.GetCustomAttributes<ValidationAttribute>();
            var validationContext = new ValidationContext(parent)
            {
                MemberName = property.Name
            };

            foreach (var attribute in validationAttributes)
            {
                var result = attribute.GetValidationResult(value, validationContext);
                if (result != ValidationResult.Success)
                {
                    if (attribute is RangeAttribute rangeAttribute)
                    {
                        value = rangeAttribute.Maximum;
                        property.SetValue(parent, value);
                        MessageBox.ErrorQuery("Validation Error", $"Value reset to maximum: {rangeAttribute.Maximum}", "OK");
                    }
                    else
                    {
                        MessageBox.ErrorQuery("Validation Error", result.ErrorMessage, "OK");
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
