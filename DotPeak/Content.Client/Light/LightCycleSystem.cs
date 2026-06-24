// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.LightCycleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.GameTicking.Managers;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Light;

public sealed class LightCycleSystem : SharedLightCycleSystem
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
    AllEntityQueryEnumerator<LightCycleComponent, MapLightComponent> entityQueryEnumerator = this.AllEntityQuery<LightCycleComponent, MapLightComponent>();
    EntityUid entityUid;
    LightCycleComponent lightCycleComponent;
    MapLightComponent mapLightComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref lightCycleComponent, ref mapLightComponent))
    {
      if (lightCycleComponent.Running)
      {
        TimeSpan pauseTime = this._metadata.GetPauseTime(entityUid, (MetaDataComponent) null);
        TimeSpan timeSpan = this._timing.CurTime;
        timeSpan = timeSpan.Add(lightCycleComponent.Offset);
        timeSpan = timeSpan.Subtract(this._ticker.RoundStartTimeSpan);
        timeSpan = timeSpan.Subtract(pauseTime);
        float totalSeconds = (float) timeSpan.TotalSeconds;
        Color color = SharedLightCycleSystem.GetColor(Entity<LightCycleComponent>.op_Implicit((entityUid, lightCycleComponent)), lightCycleComponent.OriginalColor, totalSeconds);
        mapLightComponent.AmbientLightColor = color;
      }
    }
  }
}
