// Decompiled with JetBrains decompiler
// Type: Content.Client.HotPotato.HotPotatoSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.HotPotato;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.HotPotato;

public sealed class HotPotatoSystem : SharedHotPotatoSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedTransformSystem _transform;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    AllEntityQueryEnumerator<ActiveHotPotatoComponent> entityQueryEnumerator = this.AllEntityQuery<ActiveHotPotatoComponent>();
    EntityUid entityUid;
    ActiveHotPotatoComponent hotPotatoComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref hotPotatoComponent))
    {
      if (!(this._timing.CurTime < hotPotatoComponent.TargetTime))
      {
        hotPotatoComponent.TargetTime = this._timing.CurTime + TimeSpan.FromSeconds((double) hotPotatoComponent.EffectCooldown);
        MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(entityUid, (TransformComponent) null);
        this.Spawn("HotPotatoEffect", ((MapCoordinates) ref mapCoordinates).Offset(this._random.NextVector2(0.25f)), (ComponentRegistry) null, new Angle());
      }
    }
  }
}
