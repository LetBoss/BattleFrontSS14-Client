// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.ViewVariables;

[Virtual]
public abstract class ViewVariablesPath
{
  public ViewVariablesComponentPath? ParentComponent;

  public abstract Type Type { get; }

  public abstract object? Get();

  public abstract void Set(object? value);

  public abstract object? Invoke(object?[]? parameters);

  public virtual Type[] InvokeParameterTypes { get; } = Array.Empty<Type>();

  public virtual uint InvokeOptionalParameters { get; }

  public virtual Type InvokeReturnType { get; } = typeof (void);

  public static ViewVariablesFakePath FromObject(object obj)
  {
    return new ViewVariablesFakePath((Func<object>) (() => obj), (Action<object>) null, type: obj.GetType());
  }

  public static ViewVariablesFakePath FromGetter(Func<object?> getter, Type type)
  {
    return new ViewVariablesFakePath(getter, (Action<object>) null, type: type);
  }

  public static ViewVariablesFakePath FromSetter(Action<object?> setter, Type type)
  {
    return new ViewVariablesFakePath((Func<object>) null, setter, type: type);
  }

  public static ViewVariablesFakePath FromInvoker(
    Func<object?, object?> invoker,
    Type[]? invokeParameterTypes = null,
    uint invokeOptionalParameters = 0,
    Type? invokeReturnType = null)
  {
    return new ViewVariablesFakePath((Func<object>) null, (Action<object>) null, invoker, invokeParameterTypes: invokeParameterTypes, invokeOptionalParameters: invokeOptionalParameters, invokeReturnType: invokeReturnType);
  }

  public static ViewVariablesFakePath FromInvoker(
    Action<object?> invoker,
    Type[]? invokeParameterTypes = null,
    uint invokeOptionalParameters = 0,
    Type? invokeReturnType = null)
  {
    return new ViewVariablesFakePath((Func<object>) null, (Action<object>) null, invoker, invokeParameterTypes: invokeParameterTypes, invokeOptionalParameters: invokeOptionalParameters, invokeReturnType: invokeReturnType);
  }
}
