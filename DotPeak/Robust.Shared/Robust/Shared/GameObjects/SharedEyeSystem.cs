// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedEyeSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedEyeSystem : EntitySystem
{
  [Dependency]
  private readonly SharedViewSubscriberSystem _views;
  [Dependency]
  protected readonly SharedTransformSystem TransformSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EyeComponent, PlayerAttachedEvent>(new EntityEventRefHandler<EyeComponent, PlayerAttachedEvent>(this.OnEyePlayerAttached));
    this.SubscribeLocalEvent<EyeComponent, PlayerDetachedEvent>(new EntityEventRefHandler<EyeComponent, PlayerDetachedEvent>(this.OnEyePlayerDetached));
  }

  private void OnEyePlayerAttached(Entity<EyeComponent> ent, ref PlayerAttachedEvent args)
  {
    EntityUid? target = ent.Comp.Target;
    ActorComponent comp;
    if (!target.HasValue || !this.TryComp<ActorComponent>(ent.Owner, out comp))
      return;
    this._views.AddViewSubscriber(target.Value, comp.PlayerSession);
  }

  private void OnEyePlayerDetached(Entity<EyeComponent> ent, ref PlayerDetachedEvent args)
  {
    EntityUid? target = ent.Comp.Target;
    ActorComponent comp;
    if (!target.HasValue || !this.TryComp<ActorComponent>(ent.Owner, out comp))
      return;
    this._views.RemoveViewSubscriber(target.Value, comp.PlayerSession);
  }

  public void UpdateEye(Entity<EyeComponent?> entity)
  {
    EyeComponent comp = entity.Comp;
    if (!this.Resolve<EyeComponent>((EntityUid) entity, ref comp))
      return;
    comp.Eye.Offset = comp.Offset;
    comp.Eye.DrawFov = comp.DrawFov;
    comp.Eye.DrawLight = comp.DrawLight;
    comp.Eye.Rotation = comp.Rotation;
    comp.Eye.Zoom = comp.Zoom;
  }

  public void SetOffset(EntityUid uid, Vector2 value, EyeComponent? eyeComponent = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eyeComponent) || eyeComponent.Offset.Equals(value))
      return;
    eyeComponent.Offset = value;
    eyeComponent.Eye.Offset = value;
    this.DirtyField<EyeComponent>(uid, eyeComponent, "Offset");
  }

  public void SetDrawFov(EntityUid uid, bool value, EyeComponent? eyeComponent = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eyeComponent) || eyeComponent.DrawFov.Equals(value))
      return;
    eyeComponent.DrawFov = value;
    eyeComponent.Eye.DrawFov = value;
    this.DirtyField<EyeComponent>(uid, eyeComponent, "DrawFov");
  }

  public void SetDrawLight(Entity<EyeComponent?> entity, bool value)
  {
    if (!this.Resolve<EyeComponent>((EntityUid) entity, ref entity.Comp) || entity.Comp.DrawLight == value)
      return;
    entity.Comp.DrawLight = value;
    entity.Comp.Eye.DrawLight = value;
    this.DirtyField<EyeComponent>(entity, "DrawLight");
  }

  public void SetRotation(EntityUid uid, Angle rotation, EyeComponent? eyeComponent = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eyeComponent) || ((Angle) ref eyeComponent.Rotation).Equals(rotation))
      return;
    eyeComponent.Rotation = rotation;
    eyeComponent.Eye.Rotation = rotation;
  }

  public void SetTarget(EntityUid uid, EntityUid? value, EyeComponent? eyeComponent = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eyeComponent) || eyeComponent.Target.Equals((object) value))
      return;
    ActorComponent comp;
    if (this.TryComp<ActorComponent>(uid, out comp))
    {
      if (value.HasValue)
        this._views.AddViewSubscriber(value.Value, comp.PlayerSession);
      EntityUid? target = eyeComponent.Target;
      if (target.HasValue)
        this._views.RemoveViewSubscriber(target.GetValueOrDefault(), comp.PlayerSession);
    }
    eyeComponent.Target = value;
    this.DirtyField<EyeComponent>(uid, eyeComponent, "Target");
  }

  public void SetZoom(EntityUid uid, Vector2 value, EyeComponent? eyeComponent = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eyeComponent) || eyeComponent.Zoom.Equals(value))
      return;
    eyeComponent.Zoom = value;
    eyeComponent.Eye.Zoom = value;
  }

  public void SetPvsScale(Entity<EyeComponent?> eye, float scale)
  {
    if (!this.Resolve<EyeComponent>(eye.Owner, ref eye.Comp, false))
      return;
    if (!float.IsFinite(scale))
    {
      this.Log.Error($"Attempted to set pvs scale to invalid value: {scale}. Eye: {this.ToPrettyString(new EntityUid?((EntityUid) eye))}");
      this.SetPvsScale(eye, 1f);
    }
    else
      eye.Comp.PvsScale = Math.Clamp(scale, 0.1f, 100f);
  }

  public void SetVisibilityMask(EntityUid uid, int value, EyeComponent? eyeComponent = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eyeComponent) || eyeComponent.VisibilityMask.Equals(value))
      return;
    eyeComponent.VisibilityMask = value;
    this.DirtyField<EyeComponent>(uid, eyeComponent, "VisibilityMask");
  }

  public void RefreshVisibilityMask(Entity<EyeComponent?> entity)
  {
    if (!this.Resolve<EyeComponent>(entity.Owner, ref entity.Comp, false))
      return;
    GetVisMaskEvent args = new GetVisMaskEvent()
    {
      Entity = entity.Owner
    };
    this.RaiseLocalEvent<GetVisMaskEvent>(entity.Owner, ref args, true);
    this.SetVisibilityMask(entity.Owner, args.VisibilityMask, entity.Comp);
  }
}
