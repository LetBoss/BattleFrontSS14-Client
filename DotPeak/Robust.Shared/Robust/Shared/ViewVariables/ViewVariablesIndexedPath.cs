// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.ViewVariablesIndexedPath
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Reflection;
using System;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal sealed class ViewVariablesIndexedPath : ViewVariablesPath
{
  private readonly object? _object;
  private readonly PropertyInfo _indexer;
  private readonly object?[] _index;
  private readonly VVAccess? _access;

  internal ViewVariablesIndexedPath(
    object? obj,
    PropertyInfo indexer,
    object?[] index,
    VVAccess? parentAccess)
  {
    if (indexer.GetIndexParameters().Length == 0)
      throw new ArgumentException("PropertyInfo is not an indexer!", nameof (indexer));
    this._object = obj;
    this._indexer = indexer;
    this._index = index;
    this._access = parentAccess;
  }

  public override Type Type => this._indexer.GetUnderlyingType();

  public override object? Get()
  {
    if (!this._access.HasValue)
      return (object) null;
    try
    {
      return this._object != null ? this._indexer.GetValue(this._object, this._index) : (object) null;
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
    if (!(access.GetValueOrDefault() == vvAccess & access.HasValue) || this._object == null)
      return;
    this._indexer.SetValue(this._object, value, this._index);
  }

  public override object? Invoke(object?[]? parameters) => (object) null;
}
