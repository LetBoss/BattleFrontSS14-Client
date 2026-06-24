// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Equipment.NodeScannerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Equipment;

public sealed class NodeScannerSystem : EntitySystem
{
  [Dependency]
  private UseDelaySystem _useDelay;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<NodeScannerComponent, BeforeRangedInteractEvent>(new ComponentEventHandler<NodeScannerComponent, BeforeRangedInteractEvent>(this.OnBeforeRangedInteract));
    this.SubscribeLocalEvent<NodeScannerComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<NodeScannerComponent, GetVerbsEvent<UtilityVerb>>(this.AddScanVerb));
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<NodeScannerConnectedComponent, NodeScannerComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<NodeScannerConnectedComponent, NodeScannerComponent, TransformComponent>();
    EntityUid uid;
    NodeScannerConnectedComponent comp1;
    NodeScannerComponent comp2;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      if (!(comp1.NextUpdate > this._timing.CurTime))
      {
        comp1.NextUpdate = this._timing.CurTime + comp1.LinkUpdateInterval;
        if (!this._transform.InRange(this.Transform(comp1.AttachedTo).Coordinates, comp3.Coordinates, (float) comp2.MaxLinkedRange))
          this.RemCompDeferred(uid, (IComponent) comp1);
      }
    }
  }

  private void OnBeforeRangedInteract(
    EntityUid uid,
    NodeScannerComponent component,
    BeforeRangedInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!this.HasComp<XenoArtifactComponent>(valueOrDefault))
      return;
    XenoArtifactUnlockingComponent comp;
    Entity<XenoArtifactUnlockingComponent> unlockingEnt = (Entity<XenoArtifactUnlockingComponent>) (this.TryComp<XenoArtifactUnlockingComponent>(valueOrDefault, out comp) ? (valueOrDefault, comp) : (valueOrDefault, (XenoArtifactUnlockingComponent) null));
    this.Attach((Entity<NodeScannerComponent>) (uid, component), unlockingEnt, args.User);
    args.Handled = true;
  }

  private void AddScanVerb(
    EntityUid uid,
    NodeScannerComponent component,
    GetVerbsEvent<UtilityVerb> args)
  {
    XenoArtifactUnlockingComponent unlockingComponent;
    if (!args.CanAccess || !this.TryComp<XenoArtifactUnlockingComponent>(args.Target, out unlockingComponent))
      return;
    UtilityVerb utilityVerb1 = new UtilityVerb();
    utilityVerb1.Act = (Action) (() => this.Attach((Entity<NodeScannerComponent>) (uid, component), (Entity<XenoArtifactUnlockingComponent>) (args.Target, unlockingComponent), args.User));
    utilityVerb1.Text = this.Loc.GetString("node-scan-tooltip");
    UtilityVerb utilityVerb2 = utilityVerb1;
    args.Verbs.Add(utilityVerb2);
  }

  private void Attach(
    Entity<NodeScannerComponent> device,
    Entity<XenoArtifactUnlockingComponent?> unlockingEnt,
    EntityUid actor)
  {
    UseDelayComponent comp;
    if (!this._timing.IsFirstTimePredicted || this.TryComp<UseDelayComponent>((EntityUid) device, out comp) && !this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) device, comp), true))
      return;
    NodeScannerConnectedComponent connectedComponent = this.EnsureComp<NodeScannerConnectedComponent>((EntityUid) device);
    EntityUid entityUid = (EntityUid) unlockingEnt;
    if (connectedComponent.AttachedTo != entityUid)
    {
      connectedComponent.AttachedTo = entityUid;
      this.Dirty((EntityUid) device, (IComponent) connectedComponent);
    }
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) ((EntityUid) device, (UserInterfaceComponent) null), (Enum) NodeScannerUiKey.Key, actor, true);
  }
}
