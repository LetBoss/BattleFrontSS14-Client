// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesFakePath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesFakePath : ViewVariablesPath
{
  private readonly Func<object?>? _getter;
  private readonly Action<object?>? _setter;
  private readonly Func<object?, object?>? _invoker;

  public ViewVariablesFakePath(
    Func<object?>? getter,
    Action<object?>? setter,
    Func<object?, object?>? invoker = null,
    Type? type = null,
    Type[]? invokeParameterTypes = null,
    uint invokeOptionalParameters = 0,
    Type? invokeReturnType = null)
  {
    this._getter = getter;
    this._setter = setter;
    this._invoker = invoker;
    Type type1 = type;
    if ((object) type1 == null)
      type1 = typeof (void);
    this.Type = type1;
    this.InvokeParameterTypes = invokeParameterTypes ?? Array.Empty<Type>();
    this.InvokeOptionalParameters = invokeOptionalParameters;
    Type type2 = invokeReturnType;
    if ((object) type2 == null)
      type2 = typeof (void);
    this.InvokeReturnType = type2;
  }

  public ViewVariablesFakePath(
    Func<object?>? getter,
    Action<object?>? setter,
    Action<object?> invoker,
    Type? type = null,
    Type[]? invokeParameterTypes = null,
    uint invokeOptionalParameters = 0,
    Type? invokeReturnType = null)
    : this(getter, setter, type: type, invokeParameterTypes: invokeParameterTypes, invokeOptionalParameters: invokeOptionalParameters, invokeReturnType: invokeReturnType)
  {
    this._invoker = (Func<object, object>) (p =>
    {
      invoker(p);
      return (object) null;
    });
  }

  public override Type Type { get; }

  public override Type[] InvokeParameterTypes { get; }

  public override uint InvokeOptionalParameters { get; }

  public override Type InvokeReturnType { get; }

  public override object? Get()
  {
    Func<object> getter = this._getter;
    return getter == null ? (object) null : getter();
  }

  public override void Set(object? value)
  {
    Action<object> setter = this._setter;
    if (setter == null)
      return;
    setter(value);
  }

  public override object? Invoke(object?[]? parameters)
  {
    Func<object, object> invoker = this._invoker;
    return invoker == null ? (object) null : invoker((object) parameters);
  }

  public ViewVariablesFakePath WithGetter(Func<object?> getter, Type? type = null)
  {
    Func<object> getter1 = getter;
    Action<object> setter = this._setter;
    Func<object, object> invoker = this._invoker;
    Type type1 = type;
    if ((object) type1 == null)
      type1 = this.Type;
    Type[] invokeParameterTypes = this.InvokeParameterTypes;
    int optionalParameters = (int) this.InvokeOptionalParameters;
    Type invokeReturnType = this.InvokeReturnType;
    return new ViewVariablesFakePath(getter1, setter, invoker, type1, invokeParameterTypes, (uint) optionalParameters, invokeReturnType);
  }

  public ViewVariablesFakePath WithSetter(Action<object?> setter, Type? type = null)
  {
    Func<object> getter = this._getter;
    Action<object> setter1 = setter;
    Func<object, object> invoker = this._invoker;
    Type type1 = type;
    if ((object) type1 == null)
      type1 = this.Type;
    Type[] invokeParameterTypes = this.InvokeParameterTypes;
    int optionalParameters = (int) this.InvokeOptionalParameters;
    Type invokeReturnType = this.InvokeReturnType;
    return new ViewVariablesFakePath(getter, setter1, invoker, type1, invokeParameterTypes, (uint) optionalParameters, invokeReturnType);
  }

  public ViewVariablesFakePath WithInvoker(
    Func<object?, object?> invoker,
    Type[]? invokeParameterTypes = null,
    uint invokeOptionalParameters = 0,
    Type? invokeReturnType = null)
  {
    return new ViewVariablesFakePath(this._getter, this._setter, invoker, this.Type, invokeParameterTypes, invokeOptionalParameters, invokeReturnType);
  }
}
