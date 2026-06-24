// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.Events.RefreshEquipmentHudEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Inventory.Events;

[ByRefEvent]
public record struct RefreshEquipmentHudEvent<T>(SlotFlags TargetSlots) : IInventoryRelayEvent where T : IComponent
{
  public bool Active = false;
  public List<T> Components = new List<T>();

  public SlotFlags TargetSlots { get; } = TargetSlots;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    // ISSUE: reference to a compiler-generated field
    return (EqualityComparer<SlotFlags>.Default.GetHashCode(this.\u003CTargetSlots\u003Ek__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Active)) * -1521134295 + EqualityComparer<List<T>>.Default.GetHashCode(this.Components);
  }

  [CompilerGenerated]
  public readonly bool Equals(RefreshEquipmentHudEvent<
  #nullable disable
  T> other)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return EqualityComparer<SlotFlags>.Default.Equals(this.\u003CTargetSlots\u003Ek__BackingField, other.\u003CTargetSlots\u003Ek__BackingField) && EqualityComparer<bool>.Default.Equals(this.Active, other.Active) && EqualityComparer<List<T>>.Default.Equals(this.Components, other.Components);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out SlotFlags TargetSlots) => TargetSlots = this.TargetSlots;
}
