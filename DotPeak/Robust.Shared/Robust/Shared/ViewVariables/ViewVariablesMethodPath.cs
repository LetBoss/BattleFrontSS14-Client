// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesMethodPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal sealed class ViewVariablesMethodPath : ViewVariablesPath
{
  private readonly object? _object;
  private readonly MethodInfo _method;
  private readonly VVAccess? _access;

  internal ViewVariablesMethodPath(object? obj, MethodInfo method)
  {
    this._object = obj;
    this._method = method;
    ViewVariablesUtility.TryGetViewVariablesAccess((MemberInfo) method, out this._access);
  }

  public override Type Type => typeof (void);

  public override Type InvokeReturnType => this._method.ReturnType;

  public override object? Get() => (object) null;

  public override void Set(object? value)
  {
  }

  public override object? Invoke(object?[]? parameters)
  {
    VVAccess? access = this._access;
    VVAccess vvAccess = VVAccess.ReadWrite;
    if (!(access.GetValueOrDefault() == vvAccess & access.HasValue))
      return (object) null;
    return this._object == null ? (object) null : this._method.Invoke(this._object, parameters);
  }

  public override Type[] InvokeParameterTypes
  {
    get
    {
      VVAccess? access = this._access;
      VVAccess vvAccess = VVAccess.ReadWrite;
      return !(access.GetValueOrDefault() == vvAccess & access.HasValue) ? Array.Empty<Type>() : ((IEnumerable<ParameterInfo>) this._method.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (info => info.ParameterType)).ToArray<Type>();
    }
  }

  public override uint InvokeOptionalParameters
  {
    get
    {
      VVAccess? access = this._access;
      VVAccess vvAccess = VVAccess.ReadWrite;
      return !(access.GetValueOrDefault() == vvAccess & access.HasValue) ? 0U : (uint) ((IEnumerable<ParameterInfo>) this._method.GetParameters()).Count<ParameterInfo>((Func<ParameterInfo, bool>) (info => info.IsOptional));
    }
  }
}
