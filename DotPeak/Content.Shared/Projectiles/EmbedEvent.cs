// Decompiled with JetBrains decompiler
// Type: Content.Shared.Projectiles.EmbedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Projectiles;

[ByRefEvent]
public readonly record struct EmbedEvent(EntityUid? Shooter, EntityUid Embedded)
{
  public readonly EntityUid? Shooter = Shooter;
  public readonly EntityUid Embedded = Embedded;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return EqualityComparer<EntityUid?>.Default.GetHashCode(this.Shooter) * -1521134295 + EqualityComparer<EntityUid>.Default.GetHashCode(this.Embedded);
  }

  [CompilerGenerated]
  public bool Equals(EmbedEvent other)
  {
    return EqualityComparer<EntityUid?>.Default.Equals(this.Shooter, other.Shooter) && EqualityComparer<EntityUid>.Default.Equals(this.Embedded, other.Embedded);
  }

  [CompilerGenerated]
  public void Deconstruct(out EntityUid? Shooter, out EntityUid Embedded)
  {
    Shooter = this.Shooter;
    Embedded = this.Embedded;
  }
}
