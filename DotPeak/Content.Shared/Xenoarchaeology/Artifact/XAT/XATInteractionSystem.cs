// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATInteractionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATInteractionSystem : BaseXATSystem<XATInteractionComponent>
{
  public override void Initialize()
  {
    base.Initialize();
    this.XATSubscribeDirectEvent<PullStartedMessage>(new BaseXATSystem<XATInteractionComponent>.XATEventHandler<PullStartedMessage>(this.OnPullStart));
    this.XATSubscribeDirectEvent<AttackedEvent>(new BaseXATSystem<XATInteractionComponent>.XATEventHandler<AttackedEvent>(this.OnAttacked));
    this.XATSubscribeDirectEvent<InteractHandEvent>(new BaseXATSystem<XATInteractionComponent>.XATEventHandler<InteractHandEvent>(this.OnInteractHand));
  }

  private void OnPullStart(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATInteractionComponent, XenoArtifactNodeComponent> node,
    ref PullStartedMessage args)
  {
    this.Trigger(artifact, node);
  }

  private void OnAttacked(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATInteractionComponent, XenoArtifactNodeComponent> node,
    ref AttackedEvent args)
  {
    this.Trigger(artifact, node);
  }

  private void OnInteractHand(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATInteractionComponent, XenoArtifactNodeComponent> node,
    ref InteractHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.Trigger(artifact, node);
  }
}
