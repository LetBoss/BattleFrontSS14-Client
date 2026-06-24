// Decompiled with JetBrains decompiler
// Type: Content.Client.Eye.EyeLerpingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Camera;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Eye;

public sealed class EyeLerpingSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private SharedMoverController _mover;
  [Dependency]
  private SharedTransformSystem _transform;

  [Robust.Shared.ViewVariables.ViewVariables]
  private IEnumerable<LerpingEyeComponent> ActiveEyes
  {
    get => this.EntityQuery<LerpingEyeComponent>(false);
  }

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EyeComponent, ComponentStartup>(new ComponentEventHandler<EyeComponent, ComponentStartup>((object) this, __methodptr(OnEyeStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EyeComponent, ComponentShutdown>(new ComponentEventHandler<EyeComponent, ComponentShutdown>((object) this, __methodptr(OnEyeShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EyeAttachedEvent>(new EntityEventRefHandler<EyeAttachedEvent>((object) this, __methodptr(OnAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LerpingEyeComponent, EntParentChangedMessage>(new ComponentEventRefHandler<LerpingEyeComponent, EntParentChangedMessage>((object) this, __methodptr(HandleMapChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LerpingEyeComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<LerpingEyeComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnDetached)), (Type[]) null, (Type[]) null);
    this.UpdatesAfter.Add(typeof (TransformSystem));
    this.UpdatesAfter.Add(typeof (PhysicsSystem));
    this.UpdatesBefore.Add(typeof (SharedEyeSystem));
    this.UpdatesOutsidePrediction = true;
  }

  private void OnEyeStartup(EntityUid uid, EyeComponent component, ComponentStartup args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.AddEye(uid, component, true);
  }

  private void OnEyeShutdown(EntityUid uid, EyeComponent component, ComponentShutdown args)
  {
    this.RemCompDeferred<LerpingEyeComponent>(uid);
  }

  public void AddEye(EntityUid uid, EyeComponent? component = null, bool automatic = false)
  {
    if (!this.Resolve<EyeComponent>(uid, ref component, true))
      return;
    LerpingEyeComponent lerpingEyeComponent = this.EnsureComp<LerpingEyeComponent>(uid);
    lerpingEyeComponent.TargetRotation = this.GetRotation(uid);
    lerpingEyeComponent.LastRotation = lerpingEyeComponent.TargetRotation;
    lerpingEyeComponent.ManuallyAdded |= !automatic;
    lerpingEyeComponent.TargetZoom = component.Zoom;
    lerpingEyeComponent.LastZoom = lerpingEyeComponent.TargetZoom;
    if (component.Eye == null)
      return;
    this._eye.SetRotation(uid, lerpingEyeComponent.TargetRotation, component);
    this._eye.SetZoom(uid, lerpingEyeComponent.TargetZoom, component);
  }

  public void RemoveEye(EntityUid uid)
  {
    LerpingEyeComponent lerpingEyeComponent;
    if (!this.TryComp<LerpingEyeComponent>(uid, ref lerpingEyeComponent))
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
      lerpingEyeComponent.ManuallyAdded = false;
    else
      this.RemComp(uid, (IComponent) lerpingEyeComponent);
  }

  private void HandleMapChange(
    EntityUid uid,
    LerpingEyeComponent component,
    ref EntParentChangedMessage args)
  {
    EntityUid? oldMapId = args.OldMapId;
    EntityUid? mapUid = ((EntParentChangedMessage) ref args).Transform.MapUid;
    if ((oldMapId.HasValue == mapUid.HasValue ? (oldMapId.HasValue ? (EntityUid.op_Inequality(oldMapId.GetValueOrDefault(), mapUid.GetValueOrDefault()) ? 1 : 0) : 0) : 1) == 0)
      return;
    component.LastRotation = this.GetRotation(uid, ((EntParentChangedMessage) ref args).Transform);
  }

  private void OnAttached(ref EyeAttachedEvent ev)
  {
    this.AddEye(((EyeAttachedEvent) ref ev).Entity, ((EyeAttachedEvent) ref ev).Component, true);
  }

  private void OnDetached(
    EntityUid uid,
    LerpingEyeComponent component,
    LocalPlayerDetachedEvent args)
  {
    if (component.ManuallyAdded)
      return;
    this.RemCompDeferred(uid, (IComponent) component);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._gameTiming.IsFirstTimePredicted)
      return;
    AllEntityQueryEnumerator<LerpingEyeComponent, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<LerpingEyeComponent, TransformComponent>();
    EntityUid uid;
    LerpingEyeComponent lerpingEyeComponent;
    TransformComponent xform;
    while (entityQueryEnumerator.MoveNext(ref uid, ref lerpingEyeComponent, ref xform))
    {
      lerpingEyeComponent.LastRotation = lerpingEyeComponent.TargetRotation;
      lerpingEyeComponent.TargetRotation = this.GetRotation(uid, xform);
      lerpingEyeComponent.LastZoom = lerpingEyeComponent.TargetZoom;
      lerpingEyeComponent.TargetZoom = this.UpdateZoom(uid, frameTime);
    }
  }

  private Vector2 UpdateZoom(
    EntityUid uid,
    float frameTime,
    EyeComponent? eye = null,
    ContentEyeComponent? content = null)
  {
    if (!this.Resolve<ContentEyeComponent, EyeComponent>(uid, ref content, ref eye, false))
      return Vector2.One;
    Vector2 vector2_1 = content.TargetZoom - eye.Zoom;
    if ((double) vector2_1.LengthSquared() < 9.9999997473787516E-06)
      return content.TargetZoom;
    Vector2 vector2_2 = vector2_1 * Math.Min(8f * frameTime, 1f);
    return eye.Zoom + vector2_2;
  }

  private bool NeedsLerp(InputMoverComponent? mover)
  {
    return mover != null && !((Angle) ref mover.RelativeRotation).Equals(mover.TargetRelativeRotation);
  }

  private Angle GetRotation(EntityUid uid, TransformComponent? xform = null, InputMoverComponent? mover = null)
  {
    if (!this.Resolve(uid, ref xform, true))
      return Angle.Zero;
    if (this.Resolve<InputMoverComponent>(uid, ref mover, false))
      return Angle.op_UnaryNegation(this._mover.GetParentGridAngle(mover));
    EntityUid? nullable = xform.GridUid ?? xform.MapUid;
    return nullable.HasValue ? Angle.op_UnaryNegation(this._transform.GetWorldRotation(nullable.Value)) : Angle.Zero;
  }

  public virtual void FrameUpdate(float frameTime)
  {
    float amount = (float) this._gameTiming.TickFraction / (float) ushort.MaxValue;
    AllEntityQueryEnumerator<LerpingEyeComponent, EyeComponent, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<LerpingEyeComponent, EyeComponent, TransformComponent>();
    EntityUid uid;
    LerpingEyeComponent lerpingEyeComponent;
    EyeComponent eyeComponent;
    TransformComponent xform;
    while (entityQueryEnumerator.MoveNext(ref uid, ref lerpingEyeComponent, ref eyeComponent, ref xform))
    {
      Vector2 vector2 = Vector2.Lerp(lerpingEyeComponent.LastZoom, lerpingEyeComponent.TargetZoom, amount);
      if (!this.HasComp<RMCStaticZoomLevelComponent>(uid))
      {
        if ((double) (vector2 - lerpingEyeComponent.TargetZoom).Length() < 1E-05)
          this._eye.SetZoom(uid, lerpingEyeComponent.TargetZoom, eyeComponent);
        else
          this._eye.SetZoom(uid, vector2, eyeComponent);
      }
      InputMoverComponent mover;
      this.TryComp<InputMoverComponent>(uid, ref mover);
      lerpingEyeComponent.TargetRotation = this.GetRotation(uid, xform, mover);
      if (!this.NeedsLerp(mover))
      {
        this._eye.SetRotation(uid, lerpingEyeComponent.TargetRotation, eyeComponent);
      }
      else
      {
        Angle angle = Angle.ShortestDistance(ref lerpingEyeComponent.LastRotation, ref lerpingEyeComponent.TargetRotation);
        if (Math.Abs(angle.Theta) < 1E-05)
          this._eye.SetRotation(uid, lerpingEyeComponent.TargetRotation, eyeComponent);
        else
          this._eye.SetRotation(uid, Angle.op_Addition(Angle.op_Implicit(Angle.op_Implicit(angle) * (double) amount), lerpingEyeComponent.LastRotation), eyeComponent);
      }
    }
  }
}
