// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivImpactEffectSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Particles;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

public sealed class CivImpactEffectSystem : EntitySystem
{
  [Dependency]
  private readonly CivLocalParticleSystem _particles;
  [Dependency]
  private readonly EntityLookupSystem _lookup;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  private static readonly ProtoId<CivEmitterPresetPrototype> Dust = ProtoId<CivEmitterPresetPrototype>.op_Implicit("CivEmitterImpactDust");
  private static readonly ProtoId<CivEmitterPresetPrototype> Blood = ProtoId<CivEmitterPresetPrototype>.op_Implicit("CivEmitterImpactBlood");
  private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<ImpactEffectEvent>(new EntityEventHandler<ImpactEffectEvent>(this.OnImpact), (Type[]) null, (Type[]) null);
  }

  private void OnImpact(ImpactEffectEvent ev)
  {
    EntityCoordinates coordinates = this.GetCoordinates(ev.Coordinates);
    if (this.Deleted(coordinates.EntityId, (MetaDataComponent) null))
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(coordinates, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    this._mobs.Clear();
    this._lookup.GetEntitiesInRange<MobStateComponent>(coordinates, 0.6f, this._mobs, (LookupFlags) 110);
    this._particles.EmitBurst(this._mobs.Count > 0 ? CivImpactEffectSystem.Blood : CivImpactEffectSystem.Dust, mapCoordinates);
  }
}
