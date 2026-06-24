// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.GetUsedEntityEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Interaction;

[ByRefEvent]
public record struct GetUsedEntityEvent(EntityUid User)
{
  public EntityUid User = User;
  public EntityUid? Used = new EntityUid?();

  public bool Handled => this.Used.HasValue;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.User) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.Used);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetUsedEntityEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.User, other.User) && EqualityComparer<EntityUid?>.Default.Equals(this.Used, other.Used);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid User) => User = this.User;
}
