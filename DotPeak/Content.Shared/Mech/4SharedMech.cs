// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mech.MechEquipmentRemovedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Mech;

[ByRefEvent]
public readonly record struct MechEquipmentRemovedEvent(EntityUid Mech)
{
  public readonly EntityUid Mech = Mech;

  [CompilerGenerated]
  public override int GetHashCode() => EqualityComparer<EntityUid>.Default.GetHashCode(this.Mech);

  [CompilerGenerated]
  public bool Equals(MechEquipmentRemovedEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Mech, other.Mech);
  }

  [CompilerGenerated]
  public void Deconstruct(out EntityUid Mech) => Mech = this.Mech;
}
