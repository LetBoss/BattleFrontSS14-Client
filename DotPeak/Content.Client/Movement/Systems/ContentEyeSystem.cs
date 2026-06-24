// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Systems.ContentEyeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Movement.Systems;

public sealed class ContentEyeSystem : SharedContentEyeSystem
{
  [Dependency]
  private IPlayerManager _player;

  public void RequestZoom(
    EntityUid uid,
    Vector2 zoom,
    bool ignoreLimit,
    bool scalePvs,
    ContentEyeComponent? content = null)
  {
    if (!this.Resolve<ContentEyeComponent>(uid, ref content, false))
      return;
    this.RaisePredictiveEvent<SharedContentEyeSystem.RequestTargetZoomEvent>(new SharedContentEyeSystem.RequestTargetZoomEvent()
    {
      TargetZoom = zoom,
      IgnoreLimit = ignoreLimit
    });
    if (!scalePvs)
      return;
    this.RequestPvsScale(Math.Max(zoom.X, zoom.Y));
  }

  public void RequestPvsScale(float scale)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new SharedContentEyeSystem.RequestPvsScaleEvent(scale));
  }

  public void RequestToggleFov()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    this.RequestToggleFov(localEntity.GetValueOrDefault());
  }

  public void RequestToggleFov(EntityUid uid, EyeComponent? eye = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eye, false))
      return;
    this.RequestEye(!eye.DrawFov, eye.DrawLight);
  }

  public void RequestToggleLight(EntityUid uid, EyeComponent? eye = null)
  {
    if (!this.Resolve<EyeComponent>(uid, ref eye, false))
      return;
    this.RequestEye(eye.DrawFov, !eye.DrawLight);
  }

  public void RequestEye(bool drawFov, bool drawLight)
  {
    this.RaisePredictiveEvent<SharedContentEyeSystem.RequestEyeEvent>(new SharedContentEyeSystem.RequestEyeEvent(drawFov, drawLight));
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    AllEntityQueryEnumerator<ContentEyeComponent, EyeComponent> entityQueryEnumerator = this.AllEntityQuery<ContentEyeComponent, EyeComponent>();
    EntityUid entityUid;
    ContentEyeComponent contentEyeComponent;
    EyeComponent eyeComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref contentEyeComponent, ref eyeComponent))
      this.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((entityUid, eyeComponent)));
  }
}
