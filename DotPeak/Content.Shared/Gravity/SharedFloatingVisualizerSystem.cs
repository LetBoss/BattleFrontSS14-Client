// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gravity.SharedFloatingVisualizerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System.Numerics;

#nullable enable
namespace Content.Shared.Gravity;

public abstract class SharedFloatingVisualizerSystem : EntitySystem
{
  [Dependency]
  private SharedGravitySystem GravitySystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FloatingVisualsComponent, ComponentStartup>(new ComponentEventHandler<FloatingVisualsComponent, ComponentStartup>(this.OnComponentStartup));
    this.SubscribeLocalEvent<GravityChangedEvent>(new EntityEventRefHandler<GravityChangedEvent>(this.OnGravityChanged));
    this.SubscribeLocalEvent<FloatingVisualsComponent, EntParentChangedMessage>(new ComponentEventRefHandler<FloatingVisualsComponent, EntParentChangedMessage>(this.OnEntParentChanged));
  }

  public virtual void FloatAnimation(
    EntityUid uid,
    Vector2 offset,
    string animationKey,
    float animationTime,
    bool stop = false)
  {
  }

  protected bool CanFloat(
    EntityUid uid,
    FloatingVisualsComponent component,
    TransformComponent? transform = null)
  {
    if (!this.Resolve(uid, ref transform) || transform.MapID == MapId.Nullspace)
      return false;
    component.CanFloat = this.GravitySystem.IsWeightless(uid, xform: transform);
    this.Dirty(uid, (IComponent) component);
    return component.CanFloat;
  }

  private void OnComponentStartup(
    EntityUid uid,
    FloatingVisualsComponent component,
    ComponentStartup args)
  {
    if (!this.CanFloat(uid, component))
      return;
    this.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime);
  }

  private void OnGravityChanged(ref GravityChangedEvent args)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<FloatingVisualsComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<FloatingVisualsComponent, TransformComponent>();
    EntityUid uid;
    FloatingVisualsComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp2.MapID == MapId.Nullspace))
      {
        EntityUid? gridUid = comp2.GridUid;
        EntityUid changedGridIndex = args.ChangedGridIndex;
        if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() != changedGridIndex ? 1 : 0) : 1) == 0)
        {
          comp1.CanFloat = !args.HasGravity;
          this.Dirty(uid, (IComponent) comp1);
          if (!args.HasGravity)
            this.FloatAnimation(uid, comp1.Offset, comp1.AnimationKey, comp1.AnimationTime);
        }
      }
    }
  }

  private void OnEntParentChanged(
    EntityUid uid,
    FloatingVisualsComponent component,
    ref EntParentChangedMessage args)
  {
    TransformComponent transform = args.Transform;
    if (!this.CanFloat(uid, component, transform))
      return;
    this.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime);
  }
}
