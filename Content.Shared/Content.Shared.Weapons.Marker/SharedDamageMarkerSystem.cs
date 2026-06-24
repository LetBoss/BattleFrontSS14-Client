using System;
using Content.Shared.Damage;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Weapons.Marker;

public abstract class SharedDamageMarkerSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageMarkerOnCollideComponent, StartCollideEvent>((ComponentEventRefHandler<DamageMarkerOnCollideComponent, StartCollideEvent>)OnMarkerCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageMarkerComponent, AttackedEvent>((ComponentEventHandler<DamageMarkerComponent, AttackedEvent>)OnMarkerAttacked, (Type[])null, (Type[])null);
	}

	private void OnMarkerAttacked(EntityUid uid, DamageMarkerComponent component, AttackedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!(component.Marker != args.Used))
		{
			args.BonusDamage += component.Damage;
			((EntitySystem)this).RemCompDeferred<DamageMarkerComponent>(uid);
			_audio.PlayPredicted(component.Sound, uid, (EntityUid?)args.User, (AudioParams?)null);
			LeechOnMarkerComponent leech = default(LeechOnMarkerComponent);
			if (((EntitySystem)this).TryComp<LeechOnMarkerComponent>(args.Used, ref leech))
			{
				_damageable.TryChangeDamage(args.User, leech.Leech, ignoreResistances: true, interruptsDoAfters: false, null, args.Used);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<DamageMarkerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DamageMarkerComponent>();
		EntityUid uid = default(EntityUid);
		DamageMarkerComponent comp = default(DamageMarkerComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (!(comp.EndTime > _timing.CurTime))
			{
				((EntitySystem)this).RemCompDeferred<DamageMarkerComponent>(uid);
			}
		}
	}

	private void OnMarkerCollide(EntityUid uid, DamageMarkerOnCollideComponent component, ref StartCollideEvent args)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectile = default(ProjectileComponent);
		if (!args.OtherFixture.Hard || args.OurFixtureId != "projectile" || component.Amount <= 0 || _whitelistSystem.IsWhitelistFail(component.Whitelist, args.OtherEntity) || !((EntitySystem)this).TryComp<ProjectileComponent>(uid, ref projectile) || !projectile.Weapon.HasValue)
		{
			return;
		}
		DamageMarkerComponent marker = ((EntitySystem)this).EnsureComp<DamageMarkerComponent>(args.OtherEntity);
		marker.Damage = new DamageSpecifier(component.Damage);
		marker.Marker = projectile.Weapon.Value;
		marker.EndTime = _timing.CurTime + component.Duration;
		component.Amount--;
		((EntitySystem)this).Dirty(args.OtherEntity, (IComponent)(object)marker, (MetaDataComponent)null);
		if (_netManager.IsServer)
		{
			if (component.Amount <= 0)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
			else
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
	}
}
