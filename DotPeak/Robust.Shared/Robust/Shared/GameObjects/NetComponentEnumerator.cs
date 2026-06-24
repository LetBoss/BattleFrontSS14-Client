// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.NetComponentEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

public struct NetComponentEnumerator
{
  private Dictionary<ushort, IComponent>.Enumerator _dictEnum;

  public NetComponentEnumerator(Dictionary<ushort, IComponent> dictionary)
  {
    this._dictEnum = dictionary.GetEnumerator();
  }

  public bool MoveNext() => this._dictEnum.MoveNext();

  public (ushort netId, IComponent component) Current
  {
    get
    {
      KeyValuePair<ushort, IComponent> current = this._dictEnum.Current;
      return (current.Key, current.Value);
    }
  }
}
