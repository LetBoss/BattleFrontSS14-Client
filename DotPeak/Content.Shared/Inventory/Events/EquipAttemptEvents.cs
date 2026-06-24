// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.Events.EquipAttemptBase
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Inventory.Events;

public abstract class EquipAttemptBase : CancellableEntityEventArgs, IInventoryRelayEvent
{
  public readonly EntityUid Equipee;
  public readonly EntityUid EquipTarget;
  public readonly EntityUid Equipment;
  public readonly SlotFlags SlotFlags;
  public readonly string Slot;
  public string? Reason;

  protected EquipAttemptBase(
    EntityUid equipee,
    EntityUid equipTarget,
    EntityUid equipment,
    SlotDefinition slotDefinition)
  {
    this.Equipee = equipee;
    this.EquipTarget = equipTarget;
    this.Equipment = equipment;
    this.SlotFlags = slotDefinition.SlotFlags;
    this.Slot = slotDefinition.Name;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public SlotFlags TargetSlots { get; }
}
