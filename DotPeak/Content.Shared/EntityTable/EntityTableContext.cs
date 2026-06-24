// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntityTableContext
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.EntityTable;

public sealed class EntityTableContext
{
  private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

  public EntityTableContext()
  {
  }

  public EntityTableContext(Dictionary<string, object> data) => this._data = data;

  public bool TryGetData<T>([ForbidLiteral] string key, [NotNullWhen(true)] out T? value)
  {
    value = default (T);
    object obj1;
    if (!this._data.TryGetValue(key, out obj1) || !(obj1 is T obj2))
      return false;
    value = obj2;
    return true;
  }
}
