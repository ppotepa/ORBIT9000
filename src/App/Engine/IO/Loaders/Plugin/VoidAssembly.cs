using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal class VoidAssembly : Type
    {
        public override Assembly Assembly => throw new NotImplementedException("Nobody expected you here.");

        public override string? AssemblyQualifiedName => throw new NotImplementedException("Nobody expected you here.");

        public override Type? BaseType => throw new NotImplementedException("Nobody expected you here.");

        public override string? FullName => throw new NotImplementedException("Nobody expected you here.");

        public override Guid GUID => throw new NotImplementedException("Nobody expected you here.");

        public override Module Module => throw new NotImplementedException("Nobody expected you here.");

        public override string? Namespace => throw new NotImplementedException("Nobody expected you here.");

        public override Type UnderlyingSystemType => throw new NotImplementedException("Nobody expected you here.");

        public override string Name => throw new NotImplementedException("Nobody expected you here.");

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override Type? GetElementType()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        public override Type? GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException("Nobody expected you here.");
        }
    }
}
