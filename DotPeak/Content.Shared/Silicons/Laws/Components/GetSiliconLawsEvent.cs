// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Laws.Components.GetSiliconLawsEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Laws.Components;

[ByRefEvent]
public record struct GetSiliconLawsEvent(EntityUid Entity)
{
  public EntityUid Entity = Entity;
  public SiliconLawset Laws = new SiliconLawset();
  public bool Handled = false;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<EntityUid>.Default.GetHashCode(this.Entity) * -1521134295 + EqualityComparer<SiliconLawset>.Default.GetHashCode(this.Laws)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetSiliconLawsEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<SiliconLawset>.Default.Equals(this.Laws, other.Laws) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid Entity) => Entity = this.Entity;
}
