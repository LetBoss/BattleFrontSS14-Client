// Decompiled with JetBrains decompiler
// Type: Content.Client.SurveillanceCamera.SurveillanceCameraMonitorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.SurveillanceCamera;

public sealed class SurveillanceCameraMonitorSystem : EntitySystem
{
  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<ActiveSurveillanceCameraMonitorVisualsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveSurveillanceCameraMonitorVisualsComponent>();
    EntityUid entityUid;
    ActiveSurveillanceCameraMonitorVisualsComponent visualsComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent))
    {
      visualsComponent.TimeLeft -= frameTime;
      if ((double) visualsComponent.TimeLeft <= 0.0)
      {
        Action onFinish = visualsComponent.OnFinish;
        if (onFinish != null)
          onFinish();
        this.RemCompDeferred<ActiveSurveillanceCameraMonitorVisualsComponent>(entityUid);
      }
    }
  }

  public void AddTimer(EntityUid uid, Action onFinish)
  {
    this.EnsureComp<ActiveSurveillanceCameraMonitorVisualsComponent>(uid).OnFinish = onFinish;
  }

  public void RemoveTimer(EntityUid uid)
  {
    this.RemCompDeferred<ActiveSurveillanceCameraMonitorVisualsComponent>(uid);
  }
}
