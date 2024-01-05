using System;
using System.Reflection;

namespace LethalMenu.Util
{
    public class ReflectionUtil
    {
        private const BindingFlags privateInst = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags privateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        private const BindingFlags privateField = ReflectionUtil.privateInst | BindingFlags.GetField;
        private const BindingFlags privateProp = ReflectionUtil.privateInst | BindingFlags.GetProperty;
        private const BindingFlags privateMethod = ReflectionUtil.privateInst | BindingFlags.InvokeMethod;
        private const BindingFlags staticField = ReflectionUtil.privateStatic | BindingFlags.GetField;
        private const BindingFlags staticProp = ReflectionUtil.privateStatic | BindingFlags.GetProperty;
        private const BindingFlags staticMethod = ReflectionUtil.privateStatic | BindingFlags.InvokeMethod;

        private object @object { get; }
        private Type type { get; }

        ReflectionUtil(object obj)
        {
            this.@object = obj;
            this.type = obj.GetType();
        }

        private T? GetValue<T>(string variableName, BindingFlags flags) 
        { 
            try { return (T)this.type.GetField(variableName, flags).GetValue(this.@object); } catch (InvalidCastException) { return default; }
        }
        private T? GetProperty<T>(string propertyName, BindingFlags flags) 
        { 
            try { return (T)this.type.GetProperty(propertyName, flags).GetValue(this.@object); } catch (InvalidCastException) { return default; } 
        }

        private ReflectionUtil? SetValue(string variableName, object value, BindingFlags flags) 
        { 
            try { this.type.GetField(variableName, flags).SetValue(this.@object, value); return this; } catch (Exception) { return null; } 
        }
        private ReflectionUtil? SetProperty(string propertyName, object value, BindingFlags flags)
        {
            try { this.type.GetProperty(propertyName, flags).SetValue(this.@object, value); return this; } catch (Exception) { return null; }
        }

        private T? Invoke<T>(string methodName, BindingFlags flags, params object[] args)
        {
            try { return (T)this.type.GetMethod(methodName, flags).Invoke(this.@object, args); } catch (InvalidCastException) { return default; }
        }


        private T? GetValue<T>(string fieldName, bool isStatic = false, bool isProperty = false)
        {
            BindingFlags flags = isProperty ? isStatic ? staticProp : privateProp : isStatic ? staticField : privateField;
            return isProperty ? this.GetProperty<T>(fieldName, flags) : this.GetValue<T>(fieldName, flags);
        }
        public ReflectionUtil? SetValue(string fieldName, object value, bool isStatic = false, bool isProperty = false)
        {
            BindingFlags flags = isProperty ? isStatic ? staticProp : privateProp : isStatic ? staticField : privateField;
            return isProperty ? this.SetProperty(fieldName, value, flags) : this.SetValue(fieldName, value, flags);
        }
        private T? Invoke<T>(string methodName, bool isStatic = false, params object[] args) => this.Invoke<T>(methodName, isStatic ? ReflectionUtil.staticMethod : ReflectionUtil.privateMethod, args);

        public object? GetValue(string fieldName, bool isStatic = false, bool isProperty = false) => this.GetValue<object>(fieldName, isStatic, isProperty);
        public ReflectionUtil? Invoke(string methodName, bool isStatic = false, params object[] args) => this.Invoke<object>(methodName, isStatic, args)?.Reflect();
        public ReflectionUtil? Invoke(string methodName, params object[] args) => this.Invoke<object>(methodName, args: args)?.Reflect();


        public static ReflectionUtil GetReflection(object obj) => new(obj);
    }

    public static class ReflectorExtensions
    {
        public static ReflectionUtil Reflect(this object obj) => ReflectionUtil.GetReflection(obj);
    }


}
