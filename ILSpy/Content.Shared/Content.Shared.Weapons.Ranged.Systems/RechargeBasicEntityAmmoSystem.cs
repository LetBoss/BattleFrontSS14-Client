using System;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class RechargeBasicEntityAmmoSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private MetaDataSystem _metadata;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, MapInitEvent>((ComponentEventHandler<RechargeBasicEntityAmmoComponent, MapInitEvent>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, ExaminedEvent>((ComponentEventHandler<RechargeBasicEntityAmmoComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<RechargeBasicEntityAmmoComponent, BasicEntityAmmoProviderComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RechargeBasicEntityAmmoComponent, BasicEntityAmmoProviderComponent>();
		EntityUid uid = default(EntityUid);
		RechargeBasicEntityAmmoComponent recharge = default(RechargeBasicEntityAmmoComponent);
		BasicEntityAmmoProviderComponent ammo = default(BasicEntityAmmoProviderComponent);
		while (query.MoveNext(ref uid, ref recharge, ref ammo))
		{
			int? count = ammo.Count;
			if (count.HasValue && ammo.Count != ammo.Capacity && recharge.NextCharge.HasValue && !(recharge.NextCharge > _timing.CurTime))
			{
				if (_gun.UpdateBasicEntityAmmoCount(uid, ammo.Count.Value + 1, ammo) && _netManager.IsServer)
				{
					_audio.PlayPvs(recharge.RechargeSound, uid, (AudioParams?)null);
				}
				if (ammo.Count == ammo.Capacity)
				{
					recharge.NextCharge = null;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)recharge, (MetaDataComponent)null);
				}
				else
				{
					recharge.NextCharge = recharge.NextCharge.Value + TimeSpan.FromSeconds(recharge.RechargeCooldown);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)recharge, (MetaDataComponent)null);
				}
			}
		}
	}

	private void OnInit(EntityUid uid, RechargeBasicEntityAmmoComponent component, MapInitEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		component.NextCharge = _timing.CurTime;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnExamined(EntityUid uid, RechargeBasicEntityAmmoComponent component, ExaminedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (component.ShowExamineText)
		{
			BasicEntityAmmoProviderComponent ammo = default(BasicEntityAmmoProviderComponent);
			if (!((EntitySystem)this).TryComp<BasicEntityAmmoProviderComponent>(uid, ref ammo) || ammo.Count == ammo.Capacity || !component.NextCharge.HasValue)
			{
				args.PushMarkup(base.Loc.GetString("recharge-basic-entity-ammo-full"));
				return;
			}
			TimeSpan? timeLeft = component.NextCharge + _metadata.GetPauseTime(uid, (MetaDataComponent)null) - _timing.CurTime;
			args.PushMarkup(base.Loc.GetString("recharge-basic-entity-ammo-can-recharge", (ValueTuple<string, object>)("seconds", Math.Round(timeLeft.Value.TotalSeconds, 1))));
		}
	}

	public void Reset(EntityUid uid, RechargeBasicEntityAmmoComponent? recharge = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RechargeBasicEntityAmmoComponent>(uid, ref recharge, false) && (!recharge.NextCharge.HasValue || recharge.NextCharge < _timing.CurTime))
		{
			recharge.NextCharge = _timing.CurTime + TimeSpan.FromSeconds(recharge.RechargeCooldown);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)recharge, (MetaDataComponent)null);
		}
	}
}
