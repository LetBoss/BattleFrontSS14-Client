using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Particles;
using Content.Shared.Explosion.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._CIV14merka.Particles;

public sealed class CivExplosionParticlesSystem : EntitySystem
{
	[Dependency]
	private readonly CivLocalParticleSystem _particles;

	private static readonly ProtoId<CivEmitterPresetPrototype> Dust = ProtoId<CivEmitterPresetPrototype>.op_Implicit("CivEmitterExplosionDust");

	private static readonly ProtoId<CivEmitterPresetPrototype> Smoke = ProtoId<CivEmitterPresetPrototype>.op_Implicit("CivEmitterExplosionSmoke");

	private HashSet<EntityUid> _seen = new HashSet<EntityUid>();

	private HashSet<EntityUid> _current = new HashSet<EntityUid>();

	public override void FrameUpdate(float frameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		_current.Clear();
		EntityQueryEnumerator<ExplosionVisualsComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ExplosionVisualsComponent>();
		EntityUid item = default(EntityUid);
		ExplosionVisualsComponent explosionVisualsComponent = default(ExplosionVisualsComponent);
		while (val.MoveNext(ref item, ref explosionVisualsComponent))
		{
			_current.Add(item);
			if (!_seen.Contains(item))
			{
				List<float> intensity = explosionVisualsComponent.Intensity;
				if (intensity != null && intensity.Count > 0)
				{
					float scale = Math.Clamp((float)explosionVisualsComponent.Intensity.Count / 4f, 0.6f, 2.5f);
					_particles.EmitBurst(Dust, explosionVisualsComponent.Epicenter, scale);
					_particles.EmitBurst(Smoke, explosionVisualsComponent.Epicenter, scale);
				}
			}
		}
		HashSet<EntityUid> current = _current;
		HashSet<EntityUid> seen = _seen;
		_seen = current;
		_current = seen;
	}
}
