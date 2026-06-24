// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesFieldOrPropertyPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Reflection;
using System;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal sealed class ViewVariablesFieldOrPropertyPath : ViewVariablesPath
{
  private readonly IEntityManager _entMan;
  private readonly object? _object;
  private readonly MemberInfo _member;
  private readonly VVAccess? _access;

  internal ViewVariablesFieldOrPropertyPath(object? obj, MemberInfo member, IEntityManager entMan)
  {
    if ((object) (member as FieldInfo) == null && (object) (member as PropertyInfo) == null)
      throw new ArgumentException("Member must be either a field or a property!", nameof (member));
    this._object = obj;
    this._member = member;
    this._entMan = entMan;
    ViewVariablesUtility.TryGetViewVariablesAccess(member, out this._access);
  }

  public override Type Type => this._member.GetUnderlyingType();

  public override object? Get()
  {
    if (!this._access.HasValue)
      return (object) null;
    try
    {
      return this._object != null ? this._member.GetValue(this._object) : (object) null;
    }
    catch (Exception ex)
    {
      return (object) null;
    }
  }

  public override void Set(object? value)
  {
    VVAccess? access = this._access;
    VVAccess vvAccess = VVAccess.ReadWrite;
    if (!(access.GetValueOrDefault() == vvAccess & access.HasValue))
      return;
    if (this._object != null)
      this._member.SetValue(this._object, value);
    if (this.ParentComponent == null)
      return;
    this._entMan.Dirty(this.ParentComponent.Owner, this.ParentComponent.Component);
  }

  public override object? Invoke(object?[]? parameters) => (object) null;
}
