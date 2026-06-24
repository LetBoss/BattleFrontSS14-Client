// Decompiled with JetBrains decompiler
// Type: Content.Shared.Strip.Components.BaseBeforeStripEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;

#nullable disable
namespace Content.Shared.Strip.Components;

[ByRefEvent]
public abstract class BaseBeforeStripEvent(TimeSpan initialTime, bool stealth = false) : 
  EntityEventArgs,
  IInventoryRelayEvent
{
  public readonly TimeSpan InitialTime = initialTime;
  public float Multiplier = 1f;
  public TimeSpan Additive = TimeSpan.Zero;
  public bool Stealth = stealth;

  public TimeSpan Time
  {
    get
    {
      return TimeSpan.FromSeconds((double) MathF.Max((float) this.InitialTime.Seconds * this.Multiplier + (float) this.Additive.Seconds, 0.0f));
    }
  }

  public SlotFlags TargetSlots { get; } = SlotFlags.GLOVES;
}
