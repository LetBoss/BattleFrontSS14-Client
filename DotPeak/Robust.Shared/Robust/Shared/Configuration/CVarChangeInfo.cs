// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVarChangeInfo
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Timing;

#nullable enable
namespace Robust.Shared.Configuration;

public readonly struct CVarChangeInfo
{
  public readonly string Name;
  public readonly GameTick TickChanged;
  public readonly object NewValue;
  public readonly object OldValue;

  internal CVarChangeInfo(string name, GameTick tickChanged, object newValue, object oldValue)
  {
    this.Name = name;
    this.TickChanged = tickChanged;
    this.NewValue = newValue;
    this.OldValue = oldValue;
  }
}
