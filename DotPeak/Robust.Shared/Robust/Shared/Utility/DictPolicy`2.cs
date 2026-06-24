// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.DictPolicy`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class DictPolicy<T1, T2> : PooledObjectPolicy<Dictionary<T1, T2>> where T1 : notnull
{
  public override Dictionary<T1, T2> Create() => new Dictionary<T1, T2>();

  public override bool Return(Dictionary<T1, T2> obj)
  {
    obj.Clear();
    return true;
  }
}
