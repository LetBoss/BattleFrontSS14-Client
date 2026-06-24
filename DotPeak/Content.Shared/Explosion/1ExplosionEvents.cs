// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.BeforeExplodeEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion;

[ByRefEvent]
public record struct BeforeExplodeEvent(
  DamageSpecifier Damage,
  string Id,
  List<EntityUid> Contents)
{
  public DamageSpecifier Damage = Damage;
  public readonly string Id = Id;
  public float DamageCoefficient = 1f;
  public readonly List<EntityUid> Contents = Contents;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<DamageSpecifier>.Default.GetHashCode(this.Damage) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Id)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.DamageCoefficient)) * -1521134295 + EqualityComparer<List<EntityUid>>.Default.GetHashCode(this.Contents);
  }

  [CompilerGenerated]
  public readonly bool Equals(BeforeExplodeEvent other)
  {
    return EqualityComparer<DamageSpecifier>.Default.Equals(this.Damage, other.Damage) && EqualityComparer<string>.Default.Equals(this.Id, other.Id) && EqualityComparer<float>.Default.Equals(this.DamageCoefficient, other.DamageCoefficient) && EqualityComparer<List<EntityUid>>.Default.Equals(this.Contents, other.Contents);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(
    out DamageSpecifier Damage,
    out string Id,
    out List<EntityUid> Contents)
  {
    Damage = this.Damage;
    Id = this.Id;
    Contents = this.Contents;
  }
}
