using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.CombatMode.Pacification;

public sealed class PacificationSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alertsSystem;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedCombatModeSystem _combatSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PacifiedComponent, ComponentStartup>((ComponentEventHandler<PacifiedComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PacifiedComponent, ComponentShutdown>((ComponentEventHandler<PacifiedComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PacifiedComponent, BeforeThrowEvent>((EntityEventRefHandler<PacifiedComponent, BeforeThrowEvent>)OnBeforeThrow, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PacifiedComponent, AttackAttemptEvent>((ComponentEventHandler<PacifiedComponent, AttackAttemptEvent>)OnAttackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PacifiedComponent, ShotAttemptedEvent>((EntityEventRefHandler<PacifiedComponent, ShotAttemptedEvent>)OnShootAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PacifismDangerousAttackComponent, AttemptPacifiedAttackEvent>((EntityEventRefHandler<PacifismDangerousAttackComponent, AttemptPacifiedAttackEvent>)OnPacifiedDangerousAttack, (Type[])null, (Type[])null);
	}

	private bool PacifiedCanAttack(EntityUid user, EntityUid target, [NotNullWhen(false)] out string? reason)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		AttemptPacifiedAttackEvent ev = new AttemptPacifiedAttackEvent(user);
		((EntitySystem)this).RaiseLocalEvent<AttemptPacifiedAttackEvent>(target, ref ev, false);
		if (ev.Cancelled)
		{
			reason = ev.Reason;
			return false;
		}
		reason = null;
		return true;
	}

	private void ShowPopup(Entity<PacifiedComponent> user, EntityUid target, string reason)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? lastAttackedEntity = user.Comp.LastAttackedEntity;
		if (lastAttackedEntity.HasValue && target == lastAttackedEntity.GetValueOrDefault())
		{
			TimeSpan curTime = _timing.CurTime;
			TimeSpan? nextPopupTime = user.Comp.NextPopupTime;
			if (!(curTime > nextPopupTime))
			{
				return;
			}
		}
		EntityUid targetName = Identity.Entity(target, (IEntityManager)(object)base.EntityManager);
		_popup.PopupClient(base.Loc.GetString(reason, (ValueTuple<string, object>)("entity", targetName)), Entity<PacifiedComponent>.op_Implicit(user), Entity<PacifiedComponent>.op_Implicit(user));
		user.Comp.NextPopupTime = _timing.CurTime + user.Comp.PopupCooldown;
		user.Comp.LastAttackedEntity = target;
	}

	private void OnShootAttempt(Entity<PacifiedComponent> ent, ref ShotAttemptedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<PacifismAllowedGunComponent>(Entity<GunComponent>.op_Implicit(args.Used)))
		{
			ShowPopup(ent, Entity<GunComponent>.op_Implicit(args.Used), "pacified-cannot-fire-gun");
			args.Cancel();
		}
	}

	private void OnAttackAttempt(EntityUid uid, PacifiedComponent component, AttackAttemptEvent args)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		string reason;
		if (component.DisallowAllCombat || (args.Disarm && component.DisallowDisarm))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (!args.Disarm && args.Target.HasValue && (!args.Weapon.HasValue || !(args.Weapon.Value.Comp.Damage.GetTotal() == FixedPoint2.Zero)) && !PacifiedCanAttack(uid, args.Target.Value, out reason))
		{
			ShowPopup(Entity<PacifiedComponent>.op_Implicit((uid, component)), args.Target.Value, reason);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnStartup(EntityUid uid, PacifiedComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		CombatModeComponent combatMode = default(CombatModeComponent);
		if (((EntitySystem)this).TryComp<CombatModeComponent>(uid, ref combatMode))
		{
			if (component.DisallowDisarm && combatMode.CanDisarm.HasValue)
			{
				_combatSystem.SetCanDisarm(uid, canDisarm: false, combatMode);
			}
			if (component.DisallowAllCombat)
			{
				_combatSystem.SetInCombatMode(uid, value: false, combatMode);
				SharedActionsSystem actionsSystem = _actionsSystem;
				EntityUid? combatToggleActionEntity = combatMode.CombatToggleActionEntity;
				actionsSystem.SetEnabled(combatToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(combatToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), enabled: false);
			}
			_alertsSystem.ShowAlert(uid, component.PacifiedAlert);
		}
	}

	private void OnShutdown(EntityUid uid, PacifiedComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		CombatModeComponent combatMode = default(CombatModeComponent);
		if (((EntitySystem)this).TryComp<CombatModeComponent>(uid, ref combatMode))
		{
			if (combatMode.CanDisarm.HasValue)
			{
				_combatSystem.SetCanDisarm(uid, canDisarm: true, combatMode);
			}
			SharedActionsSystem actionsSystem = _actionsSystem;
			EntityUid? combatToggleActionEntity = combatMode.CombatToggleActionEntity;
			actionsSystem.SetEnabled(combatToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(combatToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), enabled: true);
			_alertsSystem.ClearAlert(uid, component.PacifiedAlert);
		}
	}

	private void OnBeforeThrow(Entity<PacifiedComponent> ent, ref BeforeThrowEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		EntityUid thrownItem = args.ItemUid;
		EntityUid itemName = Identity.Entity(thrownItem, (IEntityManager)(object)base.EntityManager);
		AttemptPacifiedThrowEvent ev = new AttemptPacifiedThrowEvent(thrownItem, Entity<PacifiedComponent>.op_Implicit(ent));
		((EntitySystem)this).RaiseLocalEvent<AttemptPacifiedThrowEvent>(thrownItem, ref ev, false);
		if (ev.Cancelled)
		{
			args.Cancelled = true;
			string cannotThrowMessage = ev.CancelReasonMessageId ?? "pacified-cannot-throw";
			_popup.PopupEntity(base.Loc.GetString(cannotThrowMessage, (ValueTuple<string, object>)("projectile", itemName)), Entity<PacifiedComponent>.op_Implicit(ent), Entity<PacifiedComponent>.op_Implicit(ent));
		}
	}

	private void OnPacifiedDangerousAttack(Entity<PacifismDangerousAttackComponent> ent, ref AttemptPacifiedAttackEvent args)
	{
		args.Cancelled = true;
		args.Reason = "pacified-cannot-harm-indirect";
	}
}
