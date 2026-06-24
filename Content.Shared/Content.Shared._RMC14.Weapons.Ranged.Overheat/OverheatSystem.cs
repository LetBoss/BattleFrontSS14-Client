using System;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged.Overheat;

public sealed class OverheatSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IGameTiming _time;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<OverheatComponent, GunShotEvent>((EntityEventRefHandler<OverheatComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverheatComponent, AttemptShootEvent>((EntityEventRefHandler<OverheatComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverheatComponent, TryGainHeatEvent>((EntityEventRefHandler<OverheatComponent, TryGainHeatEvent>)OnTryGainHeat, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverheatComponent, OverheatedEvent>((EntityEventRefHandler<OverheatComponent, OverheatedEvent>)OnOverheated, (Type[])null, (Type[])null);
	}

	private void OnAttemptShoot(Entity<OverheatComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.OverHeated)
		{
			args.Cancelled = true;
		}
	}

	private void OnGunShot(Entity<OverheatComponent> ent, ref GunShotEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		TryGainHeatEvent ev = new TryGainHeatEvent(ent.Comp.HeatPerShot);
		((EntitySystem)this).RaiseLocalEvent<TryGainHeatEvent>(Entity<OverheatComponent>.op_Implicit(ent), ref ev, false);
	}

	private void OnTryGainHeat(Entity<OverheatComponent> ent, ref TryGainHeatEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Heat = MathF.Max(0f, ent.Comp.Heat + args.HeatGained);
		((EntitySystem)this).Dirty<OverheatComponent>(ent, (MetaDataComponent)null);
		HeatGainedEvent heatGainedEvent = new HeatGainedEvent(ent.Comp.Heat);
		((EntitySystem)this).RaiseLocalEvent<HeatGainedEvent>(Entity<OverheatComponent>.op_Implicit(ent), ref heatGainedEvent, false);
		if (!(ent.Comp.Heat < (float)ent.Comp.MaxHeat))
		{
			OverheatedEvent overheatEvent = new OverheatedEvent(OverHeated: true, ent.Comp.Damage);
			((EntitySystem)this).RaiseLocalEvent<OverheatedEvent>(Entity<OverheatComponent>.op_Implicit(ent), ref overheatEvent, false);
		}
	}

	private void OnOverheated(Entity<OverheatComponent> ent, ref OverheatedEvent args)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!args.OverHeated)
		{
			ent.Comp.OverHeated = false;
			float heatLost = ent.Comp.Heat * ent.Comp.EmergencyCooldownMultiplier - ent.Comp.Heat;
			TryGainHeatEvent ev = new TryGainHeatEvent(heatLost);
			((EntitySystem)this).RaiseLocalEvent<TryGainHeatEvent>(Entity<OverheatComponent>.op_Implicit(ent), ref ev, false);
		}
		else
		{
			ent.Comp.OverHeated = true;
			ent.Comp.OverHeatedAt = _time.CurTime;
			if (_net.IsServer)
			{
				_audio.PlayPvs(ent.Comp.OverheatSound, Entity<OverheatComponent>.op_Implicit(ent), (AudioParams?)null);
			}
		}
		((EntitySystem)this).Dirty<OverheatComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<OverheatComponent> query = ((EntitySystem)this).EntityQueryEnumerator<OverheatComponent>();
		EntityUid uid = default(EntityUid);
		OverheatComponent component = default(OverheatComponent);
		while (query.MoveNext(ref uid, ref component))
		{
			if (component.Heat != 0f)
			{
				if (!component.OverHeated)
				{
					TryGainHeatEvent ev = new TryGainHeatEvent(0f - component.CooldownRate * frameTime);
					((EntitySystem)this).RaiseLocalEvent<TryGainHeatEvent>(uid, ref ev, false);
				}
				else if (_time.CurTime > component.OverHeatedAt + component.EmergencyCooldownDelay)
				{
					OverheatedEvent ev2 = new OverheatedEvent(OverHeated: false);
					((EntitySystem)this).RaiseLocalEvent<OverheatedEvent>(uid, ref ev2, false);
				}
			}
		}
	}
}
