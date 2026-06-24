// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.SunShadowSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.GameTicking.Managers;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class SunShadowSystem : SharedSunShadowSystem
{
  [Dependency]
  private ClientGameTicker _ticker;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private MetaDataSystem _metadata;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    AllEntityQueryEnumerator<SunShadowCycleComponent, SunShadowComponent> entityQueryEnumerator = this.AllEntityQuery<SunShadowCycleComponent, SunShadowComponent>();
    EntityUid entityUid;
    SunShadowCycleComponent shadowCycleComponent;
    SunShadowComponent sunShadowComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref shadowCycleComponent, ref sunShadowComponent))
    {
      if (shadowCycleComponent.Running && shadowCycleComponent.Directions.Count != 0)
      {
        TimeSpan pauseTime = this._metadata.GetPauseTime(entityUid, (MetaDataComponent) null);
        TimeSpan timeSpan = this._timing.CurTime;
        timeSpan = timeSpan.Add(shadowCycleComponent.Offset);
        timeSpan = timeSpan.Subtract(this._ticker.RoundStartTimeSpan);
        timeSpan = timeSpan.Subtract(pauseTime);
        float time = (float) (timeSpan.TotalSeconds % shadowCycleComponent.Duration.TotalSeconds);
        (Vector2 Direction, float Alpha) = this.GetShadow(Entity<SunShadowCycleComponent>.op_Implicit((entityUid, shadowCycleComponent)), time);
        sunShadowComponent.Direction = Direction;
        sunShadowComponent.Alpha = Alpha;
      }
    }
  }

  public (Vector2 Direction, float Alpha) GetShadow(
    Entity<SunShadowCycleComponent> entity,
    float time)
  {
    float num1 = time / (float) entity.Comp.Duration.TotalSeconds;
    for (int index = entity.Comp.Directions.Count - 1; index >= 0; --index)
    {
      SunShadowCycleDirection direction1 = entity.Comp.Directions[index];
      if ((double) num1 > (double) direction1.Ratio)
      {
        SunShadowCycleDirection direction2 = entity.Comp.Directions[(index + 1) % entity.Comp.Directions.Count];
        float num2 = (index != entity.Comp.Directions.Count - 1 ? direction2.Ratio : direction2.Ratio + 1f) - direction1.Ratio;
        float num3 = (num1 - direction1.Ratio) / num2;
        Angle angle1 = DirectionExtensions.ToAngle(direction1.Direction);
        Angle angle2 = DirectionExtensions.ToAngle(direction2.Direction);
        Angle angle3 = Angle.Lerp(ref angle1, ref angle2, num3);
        float amount = MathF.Pow(num3, 0.5f);
        float num4 = float.Lerp(direction1.Direction.Length(), direction2.Direction.Length(), amount);
        return (((Angle) ref angle3).ToVec() * num4, float.Lerp(direction1.Alpha, direction2.Alpha, num3));
      }
    }
    throw new InvalidOperationException();
  }
}
