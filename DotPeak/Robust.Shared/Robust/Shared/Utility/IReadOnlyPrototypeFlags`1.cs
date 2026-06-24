// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.IReadOnlyPrototypeFlags`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Utility;

public interface IReadOnlyPrototypeFlags<T> : IEnumerable<string>, IEnumerable where T : class, IPrototype
{
  int Count { get; }

  bool Contains(string flag);

  bool ContainsAll(IEnumerable<string> flags);

  bool ContainsAll(params string[] flags);

  bool ContainsAny(IEnumerable<string> flags);

  bool ContainsAny(params string[] flags);

  IEnumerable<T> GetPrototypes(IPrototypeManager prototypeManager);
}
