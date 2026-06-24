using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Soak;

public sealed class XenoSoakSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoSoakComponent, XenoSoakActionEvent>((EntityEventRefHandler<XenoSoakComponent, XenoSoakActionEvent>)OnXenoSoakAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSoakingDamageComponent, DamageChangedEvent>((EntityEventRefHandler<XenoSoakingDamageComponent, DamageChangedEvent>)OnXenoSoakingDamageChanged, (Type[])null, (Type[])null);
	}

	private void OnXenoSoakAction(Entity<XenoSoakComponent> xeno, ref XenoSoakActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoSoakingDamageComponent soak = ((EntitySystem)this).EnsureComp<XenoSoakingDamageComponent>(Entity<XenoSoakComponent>.op_Implicit(xeno));
			soak.EffectExpiresAt = _timing.CurTime + xeno.Comp.Duration;
			soak.DamageAccumulated = 0f;
			((EntitySystem)this).Dirty(xeno.Owner, (IComponent)(object)soak, (MetaDataComponent)null);
			_popup.PopupPredicted(base.Loc.GetString("rmc-xeno-soak-self"), base.Loc.GetString("rmc-xeno-soak-others", (ValueTuple<string, object>)("xeno", xeno)), Entity<XenoSoakComponent>.op_Implicit(xeno), Entity<XenoSoakComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			_aura.GiveAura(Entity<XenoSoakComponent>.op_Implicit(xeno), soak.SoakColor, xeno.Comp.Duration);
		}
	}

	private void OnXenoSoakingDamageChanged(Entity<XenoSoakingDamageComponent> xeno, ref DamageChangedEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (!args.DamageIncreased || args.DamageDelta == null || args.DamageDelta.GetTotal() < 0)
		{
			return;
		}
		xeno.Comp.DamageAccumulated += args.DamageDelta.GetTotal().Float();
		if (xeno.Comp.DamageAccumulated < (float)xeno.Comp.DamageGoal)
		{
			return;
		}
		DamageSpecifier amount = -_rmcDamageable.DistributeTypesTotal(Entity<DamageableComponent>.op_Implicit(xeno.Owner), xeno.Comp.Heal);
		_damage.TryChangeDamage(Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), amount, ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), Entity<XenoSoakingDamageComponent>.op_Implicit(xeno));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoTailStabEvent>(Entity<XenoSoakingDamageComponent>.op_Implicit(xeno)))
		{
			_action.ClearCooldown(action.AsNullable());
		}
		((EntitySystem)this).RemCompDeferred<XenoSoakingDamageComponent>(Entity<XenoSoakingDamageComponent>.op_Implicit(xeno));
		_aura.GiveAura(Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), xeno.Comp.RageColor, xeno.Comp.RageDuration);
		if (_net.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-soak-end-self"), Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-soak-end-others", (ValueTuple<string, object>)("xeno", xeno)), Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), Filter.PvsExcept(Entity<XenoSoakingDamageComponent>.op_Implicit(xeno), 2f, (IEntityManager)null), recordReplay: true, PopupType.MediumCaution);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoSoakingDamageComponent> soakingQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoSoakingDamageComponent>();
		EntityUid uid = default(EntityUid);
		XenoSoakingDamageComponent soak = default(XenoSoakingDamageComponent);
		while (soakingQuery.MoveNext(ref uid, ref soak))
		{
			if (!(soak.EffectExpiresAt > time))
			{
				((EntitySystem)this).RemCompDeferred<XenoSoakingDamageComponent>(uid);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-soak-end-fail"), uid, uid, PopupType.SmallCaution);
			}
		}
	}
}
