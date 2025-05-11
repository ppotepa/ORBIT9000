using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal class VoidAssembly : Type
    {
        public override Assembly Assembly => throw new NotImplementedException("VoidAssembly does not implement Assembly.");

        public override string? AssemblyQualifiedName => throw new NotImplementedException("VoidAssembly does not implement AssemblyQualifiedName.");

        public override Type? BaseType => throw new NotImplementedException("VoidAssembly does not implement BaseType.");

        public override string? FullName => throw new NotImplementedException("VoidAssembly does not implement FullName.");

        public override Guid GUID => throw new NotImplementedException("VoidAssembly does not implement GUID.");

        public override Module Module => throw new NotImplementedException("VoidAssembly does not implement Module.");

        public override string? Namespace => throw new NotImplementedException("VoidAssembly does not implement Namespace.");

        public override Type UnderlyingSystemType => throw new NotImplementedException("VoidAssembly does not implement UnderlyingSystemType.");

        public override string Name => throw new NotImplementedException("VoidAssembly does not implement Name.");

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetConstructors.");
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetCustomAttributes.");
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetCustomAttributes with attributeType.");
        }

        public override Type? GetElementType()
        {
            throw new NotImplementedException("VoidAssembly does not implement GetElementType.");
        }

        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException($"VoidAssembly does not implement GetEvent for event name '{name}'.");
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetEvents.");
        }

        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException($"VoidAssembly does not implement GetField for field name '{name}'.");
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetFields.");
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        public override Type? GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException($"VoidAssembly does not implement GetInterface for interface name '{name}'.");
        }

        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException("VoidAssembly does not implement GetInterfaces.");
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetMembers.");
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetMethods.");
        }

        public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException($"VoidAssembly does not implement GetNestedType for nested type name '{name}'.");
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetNestedTypes.");
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetProperties.");
        }

        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        {
            throw new NotImplementedException($"VoidAssembly does not implement InvokeMember for member name '{name}'.");
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException($"VoidAssembly does not implement IsDefined for attribute type '{attributeType}'.");
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement GetAttributeFlagsImpl.");
        }

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException("VoidAssembly does not implement GetConstructorImpl.");
        }

        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException($"VoidAssembly does not implement GetMethodImpl for method name '{name}'.");
        }

        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException($"VoidAssembly does not implement GetPropertyImpl for property name '{name}'.");
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement HasElementTypeImpl.");
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement IsArrayImpl.");
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement IsByRefImpl.");
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement IsCOMObjectImpl.");
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement IsPointerImpl.");
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException("VoidAssembly does not implement IsPrimitiveImpl.");
        }
    }
}
