// Decompiled with JetBrains decompiler
// Type: Content.Shared.Slippery.SlipAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Slippery;

public sealed class SlipAttemptEvent : EntityEventArgs, IInventoryRelayEvent
{
  public bool NoSlip;
  public bool SlowOverSlippery;
  public EntityUid? SlipCausingEntity;

  public SlotFlags TargetSlots { get; } = SlotFlags.FEET;

  public SlipAttemptEvent(EntityUid? slipCausingEntity)
  {
    this.SlipCausingEntity = slipCausingEntity;
  }
}
