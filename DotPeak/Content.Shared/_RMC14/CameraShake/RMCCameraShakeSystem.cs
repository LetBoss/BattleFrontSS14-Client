// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.CameraShake.RMCCameraShakeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Camera;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.CameraShake;

public sealed class RMCCameraShakeSystem : EntitySystem
{
  [Dependency]
  private SharedCameraRecoilSystem _cameraRecoil;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IGameTiming _timing;

  public void ShakeCamera(EntityUid user, int shakes, int strength, TimeSpan? spacing = null)
  {
    spacing.GetValueOrDefault();
    if (!spacing.HasValue)
      spacing = new TimeSpan?(TimeSpan.FromSeconds(0.1));
    RMCCameraShakingComponent shakingComponent = this.EnsureComp<RMCCameraShakingComponent>(user);
    shakingComponent.Spacing = spacing.Value;
    shakingComponent.Shakes = shakes;
    shakingComponent.Strength = strength;
    this.Dirty(user, (IComponent) shakingComponent);
  }

  public void ShakeCamera(Filter filter, int shakes, int strength, TimeSpan? spacing = null)
  {
    foreach (ICommonSession recipient in filter.Recipients)
    {
      EntityUid? attachedEntity = recipient.AttachedEntity;
      if (attachedEntity.HasValue)
        this.ShakeCamera(attachedEntity.GetValueOrDefault(), shakes, strength, spacing);
    }
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCCameraShakingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCCameraShakingComponent>();
    EntityUid uid;
    RMCCameraShakingComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Shakes <= 0)
        this.RemCompDeferred<RMCCameraShakingComponent>(uid);
      else if (!(curTime < comp1.NextShake))
      {
        --comp1.Shakes;
        comp1.NextShake = curTime + comp1.Spacing;
        Vector2 kickback = this._random.NextVector2Box((float) -comp1.Strength, (float) comp1.Strength);
        this._cameraRecoil.KickCamera(uid, kickback);
      }
    }
  }
}
