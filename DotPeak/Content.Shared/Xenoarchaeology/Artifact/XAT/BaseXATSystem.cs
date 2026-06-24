// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.BaseXATSystem`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public abstract class BaseXATSystem<T> : EntitySystem where T : Component
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  protected SharedXenoArtifactSystem XenoArtifact;
  private Robust.Shared.GameObjects.EntityQuery<XenoArtifactUnlockingComponent> _unlockingQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._unlockingQuery = this.GetEntityQuery<XenoArtifactUnlockingComponent>();
  }

  protected void XATSubscribeDirectEvent<TEvent>(
    BaseXATSystem<
    #nullable disable
    T>.XATEventHandler<
    #nullable enable
    TEvent> eventHandler)
    where TEvent : notnull
  {
    this.SubscribeLocalEvent<T, XenoArchNodeRelayedEvent<TEvent>>((ComponentEventHandler<T, XenoArchNodeRelayedEvent<TEvent>>) ((uid, component, args) =>
    {
      XenoArtifactNodeComponent comp2 = this.Comp<XenoArtifactNodeComponent>(uid);
      if (!this.CanTrigger(args.Artifact, (Entity<XenoArtifactNodeComponent>) (uid, comp2)))
        return;
      Entity<T, XenoArtifactNodeComponent> node = new Entity<T, XenoArtifactNodeComponent>(uid, component, comp2);
      eventHandler(args.Artifact, node, ref args.Args);
    }));
  }

  protected bool CanTrigger(
    Entity<XenoArtifactComponent> artifact,
    Entity<XenoArtifactNodeComponent> node)
  {
    XenoArtifactUnlockingComponent component;
    return !(this.Timing.CurTime < artifact.Comp.NextUnlockTime) && (!this._unlockingQuery.TryComp((EntityUid) artifact, out component) || !component.TriggeredNodeIndexes.Contains(this.XenoArtifact.GetIndex(artifact, (EntityUid) node))) && this.XenoArtifact.CanUnlockNode((Entity<XenoArtifactNodeComponent>) ((EntityUid) node, (XenoArtifactNodeComponent) node));
  }

  protected void Trigger(
    Entity<XenoArtifactComponent> artifact,
    Entity<T, XenoArtifactNodeComponent> node)
  {
    if (!this.Timing.IsFirstTimePredicted)
      return;
    this.Log.Debug($"Activated trigger {typeof (T).Name} on node {this.ToPrettyString(new EntityUid?((EntityUid) node))} for {this.ToPrettyString(new EntityUid?((EntityUid) artifact))}");
    this.XenoArtifact.TriggerXenoArtifact(artifact, new Entity<XenoArtifactNodeComponent>?((Entity<XenoArtifactNodeComponent>) (node.Owner, node.Comp2)));
  }

  protected delegate void XATEventHandler<TEvent>(
    Entity<XenoArtifactComponent> artifact,
    Entity<T, XenoArtifactNodeComponent> node,
    ref TEvent args)
    where T : 
    #nullable disable
    Component
    where TEvent : 
    #nullable enable
    notnull;
}
