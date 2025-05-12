using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal class VoidType : Type
    {
        private const string UnexpectedMessage = "Nobody expected you here.";

        public override Assembly Assembly => throw new NotImplementedException(UnexpectedMessage);

        public override string? AssemblyQualifiedName => throw new NotImplementedException(UnexpectedMessage);

        public override Type? BaseType => throw new NotImplementedException(UnexpectedMessage);

        public override string? FullName => throw new NotImplementedException(UnexpectedMessage);

        public override Guid GUID => throw new NotImplementedException(UnexpectedMessage);

        public override Module Module => throw new NotImplementedException(UnexpectedMessage);

        public override string? Namespace => throw new NotImplementedException(UnexpectedMessage);

        public override Type UnderlyingSystemType => throw new NotImplementedException(UnexpectedMessage);

        public override string Name => throw new NotImplementedException(UnexpectedMessage);

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override Type? GetElementType()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        public override Type? GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException(UnexpectedMessage);
        }
    }
}
