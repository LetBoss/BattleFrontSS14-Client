// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Effect.SharedRMCEffectSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Effect;

public abstract class SharedRMCEffectSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<EffectAlphaAnimationComponent, MapInitEvent>(new EntityEventRefHandler<EffectAlphaAnimationComponent, MapInitEvent>(this.OnAlphaAnimationMapInit));
  }

  private void OnAlphaAnimationMapInit(
    Entity<EffectAlphaAnimationComponent> ent,
    ref MapInitEvent args)
  {
    ent.Comp.SpawnedAt = new TimeSpan?(this._timing.CurTime);
    this.Dirty<EffectAlphaAnimationComponent>(ent);
  }
}
