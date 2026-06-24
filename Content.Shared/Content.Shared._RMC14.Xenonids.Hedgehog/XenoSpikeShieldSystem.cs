using System;
using System.Numerics;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Hedgehog;

public sealed class XenoSpikeShieldSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoProjectileSystem _xenoProjectile;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private XenoShieldSystem _shield;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoSpikeShieldComponent, DamageModifyAfterResistEvent>((EntityEventRefHandler<XenoSpikeShieldComponent, DamageModifyAfterResistEvent>)OnHedgehogShieldDamage, new Type[1] { typeof(XenoShieldSystem) }, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoSpikeShieldComponent, XenoShieldComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoSpikeShieldComponent, XenoShieldComponent>();
		EntityUid uid = default(EntityUid);
		XenoSpikeShieldComponent spike = default(XenoSpikeShieldComponent);
		XenoShieldComponent shield = default(XenoShieldComponent);
		while (query.MoveNext(ref uid, ref spike, ref shield))
		{
			if (spike.ShieldExpireAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? shieldExpireAt = spike.ShieldExpireAt;
				if (value >= shieldExpireAt)
				{
					_shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Hedgehog);
				}
			}
		}
	}

	private void OnHedgehogShieldDamage(Entity<XenoSpikeShieldComponent> ent, ref DamageModifyAfterResistEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (((EntitySystem)this).TryComp<XenoShieldComponent>(Entity<XenoSpikeShieldComponent>.op_Implicit(ent), ref shield) && shield.Active && shield.Shield == XenoShieldSystem.ShieldType.Hedgehog)
		{
			if (((EntitySystem)this).HasComp<ProjectileComponent>(args.Tool))
			{
				XenoProjectileSystem xenoProjectile = _xenoProjectile;
				EntityUid owner = ent.Owner;
				EntityCoordinates targetCoords = new EntityCoordinates(Entity<XenoSpikeShieldComponent>.op_Implicit(ent), Vector2.UnitX * 2.5f);
				FixedPoint2 zero = FixedPoint2.Zero;
				EntProtoId projectile = ent.Comp.Projectile;
				int projectileCount = ent.Comp.ProjectileCount;
				Angle deviation = new Angle(Math.PI * 2.0);
				int? projectileHitLimit = ent.Comp.ProjectileHitLimit;
				xenoProjectile.TryShoot(owner, targetCoords, zero, projectile, null, projectileCount, deviation, 15f, null, null, predicted: false, projectileHitLimit);
				_popup.PopupPredicted(base.Loc.GetString("rmc-spike-shield-hit", (ValueTuple<string, object>)("user", ent)), Entity<XenoSpikeShieldComponent>.op_Implicit(ent), Entity<XenoSpikeShieldComponent>.op_Implicit(ent));
			}
			((EntitySystem)this).Dirty(Entity<XenoSpikeShieldComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}
}
