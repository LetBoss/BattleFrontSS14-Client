// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.RefreshMovementSpeedModifiersEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Movement.Systems;

public sealed class RefreshMovementSpeedModifiersEvent : EntityEventArgs, IInventoryRelayEvent
{
  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public float WalkSpeedModifier { get; private set; } = 1f;

  public float SprintSpeedModifier { get; private set; } = 1f;

  public void ModifySpeed(float walk, float sprint)
  {
    this.WalkSpeedModifier *= walk;
    this.SprintSpeedModifier *= sprint;
  }

  public void ModifySpeed(float mod) => this.ModifySpeed(mod, mod);
}
