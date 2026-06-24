using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Particles;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeAllEvent<ImpactEffectEvent>((EntityEventHandler<ImpactEffectEvent>)OnImpact, (Type[])null, (Type[])null);
	}

	private void OnImpact(ImpactEffectEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(ev.Coordinates);
		if (!((EntitySystem)this).Deleted(coordinates.EntityId, (MetaDataComponent)null))
		{
			MapCoordinates val = _transform.ToMapCoordinates(coordinates, true);
			if (!(val.MapId == MapId.Nullspace))
			{
				_mobs.Clear();
				_lookup.GetEntitiesInRange<MobStateComponent>(coordinates, 0.6f, _mobs, (LookupFlags)110);
				ProtoId<CivEmitterPresetPrototype> presetId = ((_mobs.Count > 0) ? Blood : Dust);
				_particles.EmitBurst(presetId, val);
			}
		}
	}
}
