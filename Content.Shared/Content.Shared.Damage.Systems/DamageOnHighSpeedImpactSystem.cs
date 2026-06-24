using System;
using System.Collections.Generic;
using Content.Shared.Damage.Components;
using Content.Shared.Effects;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed class DamageOnHighSpeedImpactSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IRobustRandom _robustRandom;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private SharedStunSystem _stun;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageOnHighSpeedImpactComponent, StartCollideEvent>((ComponentEventRefHandler<DamageOnHighSpeedImpactComponent, StartCollideEvent>)HandleCollide, (Type[])null, (Type[])null);
	}

	private void HandleCollide(EntityUid uid, DamageOnHighSpeedImpactComponent component, ref StartCollideEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (!args.OurFixture.Hard || !args.OtherFixture.Hard || !((EntitySystem)this).HasComp<DamageableComponent>(uid))
		{
			return;
		}
		float speed = args.OurBody.LinearVelocity.Length();
		if (!(speed < component.MinimumSpeed) && (!component.LastHit.HasValue || !((_gameTiming.CurTime - component.LastHit.Value).TotalSeconds < (double)component.DamageCooldown)))
		{
			component.LastHit = _gameTiming.CurTime;
			if (RandomExtensions.Prob(_robustRandom, component.StunChance))
			{
				_stun.TryStun(uid, TimeSpan.FromSeconds(component.StunSeconds), refresh: true);
			}
			float damageScale = component.SpeedDamageFactor * speed / component.MinimumSpeed;
			_damageable.TryChangeDamage(uid, component.Damage * damageScale);
			if (_gameTiming.IsFirstTimePredicted)
			{
				SharedAudioSystem audio = _audio;
				SoundSpecifier soundHit = component.SoundHit;
				AudioParams val = ((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.125f);
				audio.PlayPvs(soundHit, uid, (AudioParams?)((AudioParams)(ref val)).WithVolume(-0.125f));
			}
			_color.RaiseEffect(Color.Red, new List<EntityUid> { uid }, Filter.Pvs(uid, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null));
		}
	}

	public void ChangeCollide(EntityUid uid, float minimumSpeed, float stunSeconds, float damageCooldown, float speedDamage, DamageOnHighSpeedImpactComponent? collide = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DamageOnHighSpeedImpactComponent>(uid, ref collide, false))
		{
			collide.MinimumSpeed = minimumSpeed;
			collide.StunSeconds = stunSeconds;
			collide.DamageCooldown = damageCooldown;
			collide.SpeedDamageFactor = speedDamage;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)collide, (MetaDataComponent)null);
		}
	}
}
