using System;
using System.Reflection;

namespace LethalMenu.Util
{
    public class ReflectionUtil<R>
    {
        private const BindingFlags privateInst = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags privateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        private const BindingFlags privateField = ReflectionUtil<R>.privateInst | BindingFlags.GetField;
        private const BindingFlags privateProp = ReflectionUtil<R>.privateInst | BindingFlags.GetProperty;
        private const BindingFlags privateMethod = ReflectionUtil<R>.privateInst | BindingFlags.InvokeMethod;
        private const BindingFlags staticField = ReflectionUtil<R>.privateStatic | BindingFlags.GetField;
        private const BindingFlags staticProp = ReflectionUtil<R>.privateStatic | BindingFlags.GetProperty;
        private const BindingFlags staticMethod = ReflectionUtil<R>.privateStatic | BindingFlags.InvokeMethod;
        private R @object { get; }
        private Type type { get; }
        internal ReflectionUtil(R obj)
        {
            this.@object = obj;
            this.type = typeof(R);
        }
        private T? GetValue<T>(string variableName, BindingFlags flags) 
        { 
            try { return (T)this.type.GetField(variableName, flags).GetValue(this.@object); } catch (InvalidCastException) { return default; }
        }
        private T? GetProperty<T>(string propertyName, BindingFlags flags) 
        { 
            try { return (T)this.type.GetProperty(propertyName, flags).GetValue(this.@object); } catch (InvalidCastException) { return default; } 
        }
        private ReflectionUtil<R>? SetValue(string variableName, object value, BindingFlags flags) 
        { 
            try { this.type.GetField(variableName, flags).SetValue(this.@object, value); return this; } catch (Exception) { return null; } 
        }
        private ReflectionUtil<R>? SetProperty(string propertyName, object value, BindingFlags flags)
        {
            try { this.type.GetProperty(propertyName, flags).SetValue(this.@object, value); return this; } catch (Exception) { return null; }
        }
        private T? Invoke<T>(string methodName, BindingFlags flags, params object[] args)
        {
            try { return (T)this.type.GetMethod(methodName, flags).Invoke(this.@object, args); } catch (InvalidCastException) { return default; }
        }
        public T? GetValue<T>(string fieldName, bool isStatic = false, bool isProperty = false)
        {
            BindingFlags flags = isProperty ? isStatic ? staticProp : privateProp : isStatic ? staticField : privateField;
            return isProperty ? this.GetProperty<T>(fieldName, flags) : this.GetValue<T>(fieldName, flags);
        }
        public ReflectionUtil<R>? SetValue(string fieldName, object value, bool isStatic = false, bool isProperty = false)
        {
            BindingFlags flags = isProperty ? isStatic ? staticProp : privateProp : isStatic ? staticField : privateField;
            return isProperty ? this.SetProperty(fieldName, value, flags) : this.SetValue(fieldName, value, flags);
        }
        private T? Invoke<T>(string methodName, bool isStatic = false, params object[] args) => this.Invoke<T>(methodName, isStatic ? ReflectionUtil<R>.staticMethod : ReflectionUtil<R>.privateMethod, args);
        public object? GetValue(string fieldName, bool isStatic = false, bool isProperty = false) => this.GetValue<R>(fieldName, isStatic, isProperty);
        public ReflectionUtil<R>? Invoke(string methodName, bool isStatic = false, params object[] args) => this.Invoke<R>(methodName, isStatic, args)?.Reflect();
        public ReflectionUtil<R>? Invoke(string methodName, params object[] args) => this.Invoke<R>(methodName, args: args)?.Reflect();
    }

    public static class ReflectorExtensions
    {
        public static ReflectionUtil<R> Reflect<R>(this R obj) => new(obj);
    }
}
