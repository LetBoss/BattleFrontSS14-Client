// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XenoArchNodeRelayedEvent`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact;

[ByRefEvent]
public record struct XenoArchNodeRelayedEvent<TEvent>(
  Entity<XenoArtifactComponent> Artifact,
  TEvent Args)
{
  public TEvent Args = Args;
  public Entity<XenoArtifactComponent> Artifact = Artifact;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<TEvent>.Default.GetHashCode(this.Args) * -1521134295 + EqualityComparer<Entity<XenoArtifactComponent>>.Default.GetHashCode(this.Artifact);
  }

  [CompilerGenerated]
  public readonly bool Equals(XenoArchNodeRelayedEvent<
  #nullable disable
  TEvent> other)
  {
    return EqualityComparer<TEvent>.Default.Equals(this.Args, other.Args) && EqualityComparer<Entity<XenoArtifactComponent>>.Default.Equals(this.Artifact, other.Artifact);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out Entity<
  #nullable enable
  XenoArtifactComponent> Artifact, out TEvent Args)
  {
    Artifact = this.Artifact;
    Args = this.Args;
  }
}
