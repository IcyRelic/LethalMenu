using System;
using System.Reflection;

namespace LethalMenu.Util;

public class ReflectionUtil<TR>
{
    private const BindingFlags PrivateInst = BindingFlags.NonPublic | BindingFlags.Instance;
    private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

    private const BindingFlags PrivateField = PrivateInst | BindingFlags.GetField;
    private const BindingFlags PrivateProp = PrivateInst | BindingFlags.GetProperty;
    private const BindingFlags PrivateMethod = PrivateInst | BindingFlags.InvokeMethod;
    private const BindingFlags StaticField = PrivateStatic | BindingFlags.GetField;
    private const BindingFlags StaticProp = PrivateStatic | BindingFlags.GetProperty;
    private const BindingFlags StaticMethod = PrivateStatic | BindingFlags.InvokeMethod;

    internal ReflectionUtil(TR obj)
    {
        Object = obj;
        Type = typeof(TR);
    }

    private TR Object { get; }
    private Type Type { get; }

    private T? GetValue<T>(string variableName, BindingFlags flags)
    {
        try
        {
            return (T)Type.GetField(variableName, flags)?.GetValue(Object);
        }
        catch (InvalidCastException)
        {
            return default;
        }
    }

    private T? GetProperty<T>(string propertyName, BindingFlags flags)
    {
        try
        {
            return (T)Type.GetProperty(propertyName, flags)?.GetValue(Object);
        }
        catch (InvalidCastException)
        {
            return default;
        }
    }

    private ReflectionUtil<TR>? SetValue(string variableName, object value, BindingFlags flags)
    {
        try
        {
            Type.GetField(variableName, flags)?.SetValue(Object, value);
            return this;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private ReflectionUtil<TR>? SetProperty(string propertyName, object value, BindingFlags flags)
    {
        try
        {
            Type.GetProperty(propertyName, flags)?.SetValue(Object, value);
            return this;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private T? Invoke<T>(string methodName, BindingFlags flags, params object[] args)
    {
        try
        {
            return (T)Type.GetMethod(methodName, flags)?.Invoke(Object, args);
        }
        catch (InvalidCastException)
        {
            return default;
        }
    }


    public T? GetValue<T>(string fieldName, bool isStatic = false, bool isProperty = false)
    {
        var flags = isProperty ? isStatic ? StaticProp : PrivateProp : isStatic ? StaticField : PrivateField;
        return isProperty ? GetProperty<T>(fieldName, flags) : GetValue<T>(fieldName, flags);
    }

    public ReflectionUtil<TR>? SetValue(string fieldName, object value, bool isStatic = false, bool isProperty = false)
    {
        var flags = isProperty ? isStatic ? StaticProp : PrivateProp : isStatic ? StaticField : PrivateField;
        return isProperty ? SetProperty(fieldName, value, flags) : SetValue(fieldName, value, flags);
    }

    private T? Invoke<T>(string methodName, bool isStatic = false, params object[] args)
    {
        return Invoke<T>(methodName, isStatic ? StaticMethod : PrivateMethod, args);
    }

    public object? GetValue(string fieldName, bool isStatic = false, bool isProperty = false)
    {
        return GetValue<TR>(fieldName, isStatic, isProperty);
    }

    public ReflectionUtil<TR>? Invoke(string methodName, bool isStatic = false, params object[] args)
    {
        return Invoke<TR>(methodName, isStatic, args)?.Reflect();
    }

    public ReflectionUtil<TR>? Invoke(string methodName, params object[] args)
    {
        return Invoke<TR>(methodName, args: args)?.Reflect();
    }
}

public static class ReflectorExtensions
{
    public static ReflectionUtil<TR> Reflect<TR>(this TR obj)
    {
        return new ReflectionUtil<TR>(obj);
    }
}