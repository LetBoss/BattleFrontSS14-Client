// Decompiled with JetBrains decompiler
// Type: Content.Shared.Camera.SharedCameraRecoilSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Camera;

public abstract class SharedCameraRecoilSystem : EntitySystem
{
  private const float RestoreRateMax = 30f;
  private const float RestoreRateMin = 0.1f;
  private const float RestoreRateRamp = 4f;
  protected const float KickMagnitudeMax = 1f;
  [Dependency]
  private SharedContentEyeSystem _eye;
  [Dependency]
  private INetManager _net;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CameraRecoilComponent, GetEyeOffsetEvent>(new EntityEventRefHandler<CameraRecoilComponent, GetEyeOffsetEvent>((object) this, __methodptr(OnCameraRecoilGetEyeOffset)), (Type[]) null, (Type[]) null);
  }

  private void OnCameraRecoilGetEyeOffset(
    Entity<CameraRecoilComponent> ent,
    ref GetEyeOffsetEvent args)
  {
    args.Offset += ent.Comp.BaseOffset + ent.Comp.CurrentKick;
  }

  public abstract void KickCamera(
    EntityUid euid,
    Vector2 kickback,
    CameraRecoilComponent? component = null);

  private void UpdateEyes(float frameTime)
  {
    AllEntityQueryEnumerator<CameraRecoilComponent, EyeComponent> entityQueryEnumerator = this.AllEntityQuery<CameraRecoilComponent, EyeComponent>();
    EntityUid entityUid;
    CameraRecoilComponent cameraRecoilComponent;
    EyeComponent eyeComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref cameraRecoilComponent, ref eyeComponent))
    {
      if ((double) cameraRecoilComponent.CurrentKick.Length() <= 0.004999999888241291)
      {
        cameraRecoilComponent.CurrentKick = Vector2.Zero;
      }
      else
      {
        Vector2 vector2_1 = Vector2Helpers.Normalized(cameraRecoilComponent.CurrentKick);
        cameraRecoilComponent.LastKickTime += frameTime;
        double num1 = (double) MathHelper.Lerp(0.1f, 30f, Math.Min(1f, cameraRecoilComponent.LastKickTime / 4f));
        Vector2 vector2_2 = vector2_1 * (float) num1 * frameTime;
        float num2;
        float num3;
        Vector2Helpers.Deconstruct(cameraRecoilComponent.CurrentKick - vector2_2, ref num2, ref num3);
        float x = num2;
        float y = num3;
        if (Math.Sign(x) != Math.Sign(cameraRecoilComponent.CurrentKick.X))
          x = 0.0f;
        if (Math.Sign(y) != Math.Sign(cameraRecoilComponent.CurrentKick.Y))
          y = 0.0f;
        cameraRecoilComponent.CurrentKick = new Vector2(x, y);
      }
      if (!(cameraRecoilComponent.CurrentKick == cameraRecoilComponent.LastKick))
      {
        cameraRecoilComponent.LastKick = cameraRecoilComponent.CurrentKick;
        this._eye.UpdateEyeOffset(Entity<EyeComponent>.op_Implicit((entityUid, eyeComponent)));
      }
    }
  }

  public virtual void Update(float frameTime)
  {
    if (!this._net.IsServer)
      return;
    this.UpdateEyes(frameTime);
  }

  public virtual void FrameUpdate(float frameTime) => this.UpdateEyes(frameTime);
}
