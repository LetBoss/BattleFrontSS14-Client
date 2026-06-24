// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityUnpausedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct EntityUnpausedEvent(TimeSpan PausedTime)
{
  public readonly TimeSpan PausedTime = PausedTime;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return EqualityComparer<TimeSpan>.Default.GetHashCode(this.PausedTime);
  }

  [CompilerGenerated]
  public bool Equals(EntityUnpausedEvent other)
  {
    return EqualityComparer<TimeSpan>.Default.Equals(this.PausedTime, other.PausedTime);
  }

  [CompilerGenerated]
  public void Deconstruct(out TimeSpan PausedTime) => PausedTime = this.PausedTime;
}
