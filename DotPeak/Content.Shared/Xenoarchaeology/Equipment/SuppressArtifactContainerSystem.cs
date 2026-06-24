// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Equipment.SuppressArtifactContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Equipment;

public sealed class SuppressArtifactContainerSystem : EntitySystem
{
  [Dependency]
  private SharedXenoArtifactSystem _xenoArtifact;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SuppressArtifactContainerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<SuppressArtifactContainerComponent, EntInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<SuppressArtifactContainerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<SuppressArtifactContainerComponent, EntRemovedFromContainerMessage>(this.OnRemoved));
  }

  private void OnInserted(
    EntityUid uid,
    SuppressArtifactContainerComponent component,
    EntInsertedIntoContainerMessage args)
  {
    XenoArtifactComponent comp;
    if (!this.TryComp<XenoArtifactComponent>(args.Entity, out comp))
      return;
    this._xenoArtifact.SetSuppressed((Entity<XenoArtifactComponent>) (args.Entity, comp), true);
  }

  private void OnRemoved(
    EntityUid uid,
    SuppressArtifactContainerComponent component,
    EntRemovedFromContainerMessage args)
  {
    XenoArtifactComponent comp;
    if (!this.TryComp<XenoArtifactComponent>(args.Entity, out comp))
      return;
    this._xenoArtifact.SetSuppressed((Entity<XenoArtifactComponent>) (args.Entity, comp), false);
  }
}
