// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedContentEyeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Camera;
using Content.Shared.Ghost;
using Content.Shared.Input;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Player;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Movement.Systems;

public abstract class SharedContentEyeSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminManager _admin;
  public const AdminFlags EyeFlag = AdminFlags.Debug;
  public const float ZoomMod = 1.5f;
  public static readonly Vector2 DefaultZoom = Vector2.One;
  public static readonly Vector2 MinZoom = SharedContentEyeSystem.DefaultZoom * (float) Math.Pow(1.5, -3.0);
  [Dependency]
  private SharedEyeSystem _eye;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ContentEyeComponent, ComponentStartup>(new ComponentEventHandler<ContentEyeComponent, ComponentStartup>(this.OnContentEyeStartup));
    this.SubscribeAllEvent<SharedContentEyeSystem.RequestTargetZoomEvent>(new EntitySessionEventHandler<SharedContentEyeSystem.RequestTargetZoomEvent>(this.OnContentZoomRequest));
    this.SubscribeAllEvent<SharedContentEyeSystem.RequestPvsScaleEvent>(new EntitySessionEventHandler<SharedContentEyeSystem.RequestPvsScaleEvent>(this.OnPvsScale));
    this.SubscribeAllEvent<SharedContentEyeSystem.RequestEyeEvent>(new EntitySessionEventHandler<SharedContentEyeSystem.RequestEyeEvent>(this.OnRequestEye));
    CommandBinds.Builder.Bind(ContentKeyFunctions.ZoomIn, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.ZoomIn), handle: false)).Bind(ContentKeyFunctions.ZoomOut, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.ZoomOut), handle: false)).Bind(ContentKeyFunctions.ResetZoom, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.ResetZoom), handle: false)).Register<SharedContentEyeSystem>();
    this.Log.Level = new LogLevel?(LogLevel.Info);
    this.UpdatesOutsidePrediction = true;
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<SharedContentEyeSystem>();
  }

  private void ResetZoom(ICommonSession? session)
  {
    ContentEyeComponent comp;
    if (!this.TryComp<ContentEyeComponent>((EntityUid?) session?.AttachedEntity, out comp))
      return;
    this.ResetZoom(session.AttachedEntity.Value, comp);
  }

  private void ZoomOut(ICommonSession? session)
  {
    ContentEyeComponent comp;
    if (!this.TryComp<ContentEyeComponent>((EntityUid?) session?.AttachedEntity, out comp))
      return;
    this.SetZoom(session.AttachedEntity.Value, comp.TargetZoom * 1.5f, eye: comp);
  }

  private void ZoomIn(ICommonSession? session)
  {
    ContentEyeComponent comp;
    if (!this.TryComp<ContentEyeComponent>((EntityUid?) session?.AttachedEntity, out comp))
      return;
    this.SetZoom(session.AttachedEntity.Value, comp.TargetZoom / 1.5f, eye: comp);
  }

  private Vector2 Clamp(Vector2 zoom, ContentEyeComponent component)
  {
    return Vector2.Clamp(zoom, SharedContentEyeSystem.MinZoom, component.MaxZoom);
  }

  public void SetZoom(EntityUid uid, Vector2 zoom, bool ignoreLimits = false, ContentEyeComponent? eye = null)
  {
    if (!this.Resolve<ContentEyeComponent>(uid, ref eye, false))
      return;
    eye.TargetZoom = ignoreLimits ? zoom : this.Clamp(zoom, eye);
    this.Dirty(uid, (IComponent) eye);
  }

  private void OnContentZoomRequest(
    SharedContentEyeSystem.RequestTargetZoomEvent msg,
    EntitySessionEventArgs args)
  {
    bool ignoreLimits = msg.IgnoreLimit && this._admin.HasAdminFlag(args.SenderSession, AdminFlags.Debug);
    ContentEyeComponent comp;
    if (!this.TryComp<ContentEyeComponent>(args.SenderSession.AttachedEntity, out comp))
      return;
    this.SetZoom(args.SenderSession.AttachedEntity.Value, msg.TargetZoom, ignoreLimits, comp);
  }

  private void OnPvsScale(
    SharedContentEyeSystem.RequestPvsScaleEvent ev,
    EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    if (!this._admin.HasAdminFlag(args.SenderSession, AdminFlags.Debug))
      return;
    this._eye.SetPvsScale((Entity<EyeComponent>) valueOrDefault, ev.Scale);
  }

  private void OnRequestEye(SharedContentEyeSystem.RequestEyeEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EyeComponent comp;
    if (!this.HasComp<GhostComponent>(valueOrDefault) && !this._admin.IsAdmin(valueOrDefault) || !this.TryComp<EyeComponent>(valueOrDefault, out comp))
      return;
    this._eye.SetDrawFov(valueOrDefault, msg.DrawFov, comp);
    this._eye.SetDrawLight((Entity<EyeComponent>) (valueOrDefault, comp), msg.DrawLight);
  }

  private void OnContentEyeStartup(
    EntityUid uid,
    ContentEyeComponent component,
    ComponentStartup args)
  {
    EyeComponent comp;
    if (!this.TryComp<EyeComponent>(uid, out comp))
      return;
    this._eye.SetZoom(uid, component.TargetZoom, comp);
    this.Dirty(uid, (IComponent) component);
  }

  public void ResetZoom(EntityUid uid, ContentEyeComponent? component = null)
  {
    this._eye.SetPvsScale((Entity<EyeComponent>) uid, 1f);
    this.SetZoom(uid, SharedContentEyeSystem.DefaultZoom, eye: component);
  }

  public void SetMaxZoom(EntityUid uid, Vector2 value, ContentEyeComponent? component = null)
  {
    if (!this.Resolve<ContentEyeComponent>(uid, ref component))
      return;
    component.MaxZoom = value;
    component.TargetZoom = this.Clamp(component.TargetZoom, component);
    this.Dirty(uid, (IComponent) component);
  }

  public void UpdateEyeOffset(Entity<EyeComponent> eye)
  {
    GetEyeOffsetAttemptEvent args1 = new GetEyeOffsetAttemptEvent();
    this.RaiseLocalEvent<GetEyeOffsetAttemptEvent>((EntityUid) eye, ref args1);
    if (args1.Cancelled)
    {
      this._eye.SetOffset((EntityUid) eye, Vector2.Zero, (EyeComponent) eye);
    }
    else
    {
      GetEyeOffsetEvent args2 = new GetEyeOffsetEvent();
      this.RaiseLocalEvent<GetEyeOffsetEvent>((EntityUid) eye, ref args2);
      GetEyeOffsetRelayedEvent args3 = new GetEyeOffsetRelayedEvent();
      this.RaiseLocalEvent<GetEyeOffsetRelayedEvent>((EntityUid) eye, ref args3);
      this._eye.SetOffset((EntityUid) eye, args2.Offset + args3.Offset, (EyeComponent) eye);
    }
  }

  public void UpdatePvsScale(EntityUid uid, ContentEyeComponent? contentEye = null, EyeComponent? eye = null)
  {
    if (!this.Resolve<ContentEyeComponent>(uid, ref contentEye) || !this.Resolve<EyeComponent>(uid, ref eye))
      return;
    GetEyePvsScaleAttemptEvent args1 = new GetEyePvsScaleAttemptEvent();
    this.RaiseLocalEvent<GetEyePvsScaleAttemptEvent>(uid, ref args1);
    if (args1.Cancelled)
    {
      this._eye.SetPvsScale((Entity<EyeComponent>) (uid, eye), 1f);
    }
    else
    {
      GetEyePvsScaleEvent args2 = new GetEyePvsScaleEvent();
      this.RaiseLocalEvent<GetEyePvsScaleEvent>(uid, ref args2);
      GetEyePvsScaleRelayedEvent args3 = new GetEyePvsScaleRelayedEvent();
      this.RaiseLocalEvent<GetEyePvsScaleRelayedEvent>(uid, ref args3);
      this._eye.SetPvsScale((Entity<EyeComponent>) (uid, eye), 1f + args2.Scale + args3.Scale);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RequestTargetZoomEvent : EntityEventArgs
  {
    public Vector2 TargetZoom;
    public bool IgnoreLimit;
  }

  [NetSerializable]
  [Serializable]
  public sealed class RequestPvsScaleEvent(float scale) : EntityEventArgs
  {
    public float Scale = scale;
  }

  [NetSerializable]
  [Serializable]
  public sealed class RequestEyeEvent : EntityEventArgs
  {
    public readonly bool DrawFov;
    public readonly bool DrawLight;

    public RequestEyeEvent(bool drawFov, bool drawLight)
    {
      this.DrawFov = drawFov;
      this.DrawLight = drawLight;
    }
  }
}
