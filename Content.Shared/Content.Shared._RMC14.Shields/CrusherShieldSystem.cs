using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Shields;

public sealed class CrusherShieldSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private XenoShieldSystem _shield;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CrusherShieldComponent, DamageModifyAfterResistEvent>((EntityEventRefHandler<CrusherShieldComponent, DamageModifyAfterResistEvent>)OnDamage, new Type[1] { typeof(XenoShieldSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CrusherShieldComponent, GetExplosionResistanceEvent>((EntityEventRefHandler<CrusherShieldComponent, GetExplosionResistanceEvent>)OnGetExplosionResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CrusherShieldComponent, RemovedShieldEvent>((EntityEventRefHandler<CrusherShieldComponent, RemovedShieldEvent>)OnShieldRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CrusherShieldComponent, XenoDefensiveShieldActionEvent>((EntityEventRefHandler<CrusherShieldComponent, XenoDefensiveShieldActionEvent>)OnXenoDefensiveShieldAction, (Type[])null, (Type[])null);
	}

	private void OnXenoDefensiveShieldAction(Entity<CrusherShieldComponent> xeno, ref XenoDefensiveShieldActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		((EntitySystem)this).EnsureComp<XenoShieldComponent>(Entity<CrusherShieldComponent>.op_Implicit(xeno));
		_shield.ApplyShield(Entity<CrusherShieldComponent>.op_Implicit(xeno), XenoShieldSystem.ShieldType.Crusher, xeno.Comp.Amount);
		ApplyEffects(xeno);
		if (_net.IsClient)
		{
			return;
		}
		_popup.PopupEntity(base.Loc.GetString("rmc-xeno-defensive-shield-activate", (ValueTuple<string, object>)("user", xeno)), Entity<CrusherShieldComponent>.op_Implicit(xeno), Filter.PvsExcept(Entity<CrusherShieldComponent>.op_Implicit(xeno), 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
		_popup.PopupEntity(base.Loc.GetString("rmc-xeno-defensive-shield-activate-self", (ValueTuple<string, object>)("user", xeno)), Entity<CrusherShieldComponent>.op_Implicit(xeno), Entity<CrusherShieldComponent>.op_Implicit(xeno), PopupType.Medium);
		((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoDefensiveShieldActionEvent>(Entity<CrusherShieldComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), toggled: true);
		}
	}

	public void ApplyEffects(Entity<CrusherShieldComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		CMArmorComponent armor = default(CMArmorComponent);
		if (((EntitySystem)this).TryComp<CMArmorComponent>(Entity<CrusherShieldComponent>.op_Implicit(ent), ref armor))
		{
			ent.Comp.ExplosionOffAt = _timing.CurTime + ent.Comp.ExplosionResistanceDuration;
			ent.Comp.ShieldOffAt = _timing.CurTime + ent.Comp.ShieldDuration;
			ent.Comp.ExplosionResistApplying = true;
		}
	}

	public void OnShieldRemove(Entity<CrusherShieldComponent> ent, ref RemovedShieldEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Type != XenoShieldSystem.ShieldType.Crusher)
		{
			return;
		}
		_popup.PopupEntity(base.Loc.GetString("rmc-xeno-defensive-shield-end"), Entity<CrusherShieldComponent>.op_Implicit(ent), Entity<CrusherShieldComponent>.op_Implicit(ent), PopupType.MediumCaution);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoDefensiveShieldActionEvent>(Entity<CrusherShieldComponent>.op_Implicit(ent)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit(action.Owner), toggled: false);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<CrusherShieldComponent, XenoShieldComponent> crusherQuery = ((EntitySystem)this).EntityQueryEnumerator<CrusherShieldComponent, XenoShieldComponent>();
		EntityUid uid = default(EntityUid);
		CrusherShieldComponent crushShield = default(CrusherShieldComponent);
		XenoShieldComponent shield = default(XenoShieldComponent);
		while (crusherQuery.MoveNext(ref uid, ref crushShield, ref shield))
		{
			if (crushShield.ExplosionResistApplying && crushShield.ExplosionOffAt <= time)
			{
				crushShield.ExplosionResistApplying = false;
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-defensive-shield-resist-end"), uid, uid, PopupType.SmallCaution);
			}
			if (shield.Active && shield.Shield == XenoShieldSystem.ShieldType.Crusher && crushShield.ShieldOffAt <= time)
			{
				_shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Crusher);
			}
		}
	}

	public void OnDamage(Entity<CrusherShieldComponent> ent, ref DamageModifyAfterResistEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		XenoShieldComponent shield = default(XenoShieldComponent);
		if (!((EntitySystem)this).TryComp<XenoShieldComponent>(Entity<CrusherShieldComponent>.op_Implicit(ent), ref shield) || !shield.Active || shield.Shield != XenoShieldSystem.ShieldType.Crusher)
		{
			return;
		}
		foreach (KeyValuePair<string, FixedPoint2> type in args.Damage.DamageDict)
		{
			if (!(args.Damage.DamageDict[type.Key] <= 0))
			{
				args.Damage.DamageDict[type.Key] -= (FixedPoint2)ent.Comp.DamageReduction;
				if (args.Damage.DamageDict[type.Key] < 0)
				{
					args.Damage.DamageDict[type.Key] = 0;
				}
			}
		}
	}

	public void OnGetExplosionResistance(Entity<CrusherShieldComponent> ent, ref GetExplosionResistanceEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ExplosionResistApplying)
		{
			int explosionResist = ent.Comp.ExplosionResistance;
			float resist = (float)Math.Pow(1.1, (double)explosionResist / 5.0);
			args.DamageCoefficient /= resist;
		}
	}
}
