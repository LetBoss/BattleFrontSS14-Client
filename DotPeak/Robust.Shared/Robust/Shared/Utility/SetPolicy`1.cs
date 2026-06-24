// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SetPolicy`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class SetPolicy<T> : PooledObjectPolicy<HashSet<T>>
{
  public override HashSet<T> Create() => new HashSet<T>();

  public override bool Return(HashSet<T> obj)
  {
    obj.Clear();
    return true;
  }
}
