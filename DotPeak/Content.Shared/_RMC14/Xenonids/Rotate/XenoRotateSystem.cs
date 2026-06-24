// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rotate.XenoRotateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Rotate;

public sealed class XenoRotateSystem : EntitySystem
{
  [Dependency]
  private RotateToFaceSystem _rotateTo;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;

  public void RotateXeno(EntityUid uid, Direction direction, TimeSpan? delay = null)
  {
    XenoRotateComponent xenoRotateComponent = this.EnsureComp<XenoRotateComponent>(uid);
    xenoRotateComponent.TargetDirection = direction;
    xenoRotateComponent.Delay = delay ?? xenoRotateComponent.Delay;
    this.Dirty(uid, (IComponent) xenoRotateComponent);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoRotateComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoRotateComponent, TransformComponent>();
    EntityUid uid;
    XenoRotateComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (!(comp1.NextRotation > this._timing.CurTime))
      {
        if (comp1.FirstRotation)
        {
          XenoRotateComponent xenoRotateComponent = comp1;
          Angle worldRotation = this._transform.GetWorldRotation(comp2);
          Direction? nullable = new Direction?(((Angle) ref worldRotation).GetDir());
          xenoRotateComponent.OriginalDirection = nullable;
          comp1.NextRotation = this._timing.CurTime + comp1.Delay;
          comp1.FirstRotation = false;
          this.Dirty(uid, (IComponent) comp1);
          this._rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(comp1.TargetDirection), comp2);
        }
        else if (comp1.OriginalDirection.HasValue)
        {
          this._rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(comp1.OriginalDirection.Value), comp2);
          this.RemCompDeferred<XenoRotateComponent>(uid);
        }
      }
    }
  }

  public override void FrameUpdate(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoRotateComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoRotateComponent, TransformComponent>();
    EntityUid uid;
    XenoRotateComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
      this._rotateTo.TryFaceAngle(uid, DirectionExtensions.ToAngle(comp1.TargetDirection), comp2);
  }
}
