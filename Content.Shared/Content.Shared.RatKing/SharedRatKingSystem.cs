using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.RatKing;

public abstract class SharedRatKingSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	protected IRobustRandom Random;

	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RatKingComponent, ComponentStartup>((ComponentEventHandler<RatKingComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RatKingComponent, ComponentShutdown>((ComponentEventHandler<RatKingComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RatKingComponent, RatKingOrderActionEvent>((ComponentEventHandler<RatKingComponent, RatKingOrderActionEvent>)OnOrderAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RatKingServantComponent, ComponentShutdown>((ComponentEventHandler<RatKingServantComponent, ComponentShutdown>)OnServantShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RatKingRummageableComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<RatKingRummageableComponent, GetVerbsEvent<AlternativeVerb>>)OnGetVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RatKingRummageableComponent, RatKingRummageDoAfterEvent>((ComponentEventHandler<RatKingRummageableComponent, RatKingRummageDoAfterEvent>)OnDoAfterComplete, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, RatKingComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		ActionsComponent comp = default(ActionsComponent);
		if (((EntitySystem)this).TryComp<ActionsComponent>(uid, ref comp))
		{
			SharedActionsSystem action = _action;
			ref EntityUid? actionRaiseArmyEntity = ref component.ActionRaiseArmyEntity;
			string actionRaiseArmy = component.ActionRaiseArmy;
			ActionsComponent component2 = comp;
			action.AddAction(uid, ref actionRaiseArmyEntity, actionRaiseArmy, default(EntityUid), component2);
			SharedActionsSystem action2 = _action;
			ref EntityUid? actionDomainEntity = ref component.ActionDomainEntity;
			string actionDomain = component.ActionDomain;
			component2 = comp;
			action2.AddAction(uid, ref actionDomainEntity, actionDomain, default(EntityUid), component2);
			SharedActionsSystem action3 = _action;
			ref EntityUid? actionOrderStayEntity = ref component.ActionOrderStayEntity;
			string actionOrderStay = component.ActionOrderStay;
			component2 = comp;
			action3.AddAction(uid, ref actionOrderStayEntity, actionOrderStay, default(EntityUid), component2);
			SharedActionsSystem action4 = _action;
			ref EntityUid? actionOrderFollowEntity = ref component.ActionOrderFollowEntity;
			string actionOrderFollow = component.ActionOrderFollow;
			component2 = comp;
			action4.AddAction(uid, ref actionOrderFollowEntity, actionOrderFollow, default(EntityUid), component2);
			SharedActionsSystem action5 = _action;
			ref EntityUid? actionOrderCheeseEmEntity = ref component.ActionOrderCheeseEmEntity;
			string actionOrderCheeseEm = component.ActionOrderCheeseEm;
			component2 = comp;
			action5.AddAction(uid, ref actionOrderCheeseEmEntity, actionOrderCheeseEm, default(EntityUid), component2);
			SharedActionsSystem action6 = _action;
			ref EntityUid? actionOrderLooseEntity = ref component.ActionOrderLooseEntity;
			string actionOrderLoose = component.ActionOrderLoose;
			component2 = comp;
			action6.AddAction(uid, ref actionOrderLooseEntity, actionOrderLoose, default(EntityUid), component2);
			UpdateActions(uid, component);
		}
	}

	private void OnShutdown(EntityUid uid, RatKingComponent component, ComponentShutdown args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		RatKingServantComponent servantComp = default(RatKingServantComponent);
		foreach (EntityUid servant in component.Servants)
		{
			if (((EntitySystem)this).TryComp<RatKingServantComponent>(servant, ref servantComp))
			{
				servantComp.King = null;
			}
		}
		ActionsComponent comp = default(ActionsComponent);
		if (((EntitySystem)this).TryComp<ActionsComponent>(uid, ref comp))
		{
			Entity<ActionsComponent> actions = default(Entity<ActionsComponent>);
			actions._002Ector(uid, comp);
			SharedActionsSystem action = _action;
			Entity<ActionsComponent> performer = actions;
			EntityUid? actionRaiseArmyEntity = component.ActionRaiseArmyEntity;
			action.RemoveAction(performer, actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionRaiseArmyEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action2 = _action;
			Entity<ActionsComponent> performer2 = actions;
			actionRaiseArmyEntity = component.ActionDomainEntity;
			action2.RemoveAction(performer2, actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionRaiseArmyEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action3 = _action;
			Entity<ActionsComponent> performer3 = actions;
			actionRaiseArmyEntity = component.ActionOrderStayEntity;
			action3.RemoveAction(performer3, actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionRaiseArmyEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action4 = _action;
			Entity<ActionsComponent> performer4 = actions;
			actionRaiseArmyEntity = component.ActionOrderFollowEntity;
			action4.RemoveAction(performer4, actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionRaiseArmyEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action5 = _action;
			Entity<ActionsComponent> performer5 = actions;
			actionRaiseArmyEntity = component.ActionOrderCheeseEmEntity;
			action5.RemoveAction(performer5, actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionRaiseArmyEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action6 = _action;
			Entity<ActionsComponent> performer6 = actions;
			actionRaiseArmyEntity = component.ActionOrderLooseEntity;
			action6.RemoveAction(performer6, actionRaiseArmyEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionRaiseArmyEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		}
	}

	private void OnOrderAction(EntityUid uid, RatKingComponent component, RatKingOrderActionEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (component.CurrentOrder != args.Type)
		{
			((HandledEntityEventArgs)args).Handled = true;
			component.CurrentOrder = args.Type;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			DoCommandCallout(uid, component);
			UpdateActions(uid, component);
			UpdateAllServants(uid, component);
		}
	}

	private void OnServantShutdown(EntityUid uid, RatKingServantComponent component, ComponentShutdown args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		RatKingComponent ratKingComponent = default(RatKingComponent);
		if (((EntitySystem)this).TryComp<RatKingComponent>(component.King, ref ratKingComponent))
		{
			ratKingComponent.Servants.Remove(uid);
		}
	}

	private void UpdateActions(EntityUid uid, RatKingComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RatKingComponent>(uid, ref component, true))
		{
			SharedActionsSystem action = _action;
			EntityUid? actionOrderStayEntity = component.ActionOrderStayEntity;
			action.SetToggled(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.CurrentOrder == RatKingOrderType.Stay);
			SharedActionsSystem action2 = _action;
			actionOrderStayEntity = component.ActionOrderFollowEntity;
			action2.SetToggled(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.CurrentOrder == RatKingOrderType.Follow);
			SharedActionsSystem action3 = _action;
			actionOrderStayEntity = component.ActionOrderCheeseEmEntity;
			action3.SetToggled(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.CurrentOrder == RatKingOrderType.CheeseEm);
			SharedActionsSystem action4 = _action;
			actionOrderStayEntity = component.ActionOrderLooseEntity;
			action4.SetToggled(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.CurrentOrder == RatKingOrderType.Loose);
			SharedActionsSystem action5 = _action;
			actionOrderStayEntity = component.ActionOrderStayEntity;
			action5.StartUseDelay(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action6 = _action;
			actionOrderStayEntity = component.ActionOrderFollowEntity;
			action6.StartUseDelay(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action7 = _action;
			actionOrderStayEntity = component.ActionOrderCheeseEmEntity;
			action7.StartUseDelay(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			SharedActionsSystem action8 = _action;
			actionOrderStayEntity = component.ActionOrderLooseEntity;
			action8.StartUseDelay(actionOrderStayEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionOrderStayEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		}
	}

	private void OnGetVerb(EntityUid uid, RatKingRummageableComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<RatKingComponent>(args.User) && !component.Looted)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("rat-king-rummage-text"),
				Priority = 0,
				Act = delegate
				{
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, component.RummageDuration, new RatKingRummageDoAfterEvent(), uid, uid)
					{
						BlockDuplicate = true,
						BreakOnDamage = true,
						BreakOnMove = true,
						DistanceThreshold = 2f
					});
				}
			});
		}
	}

	private void OnDoAfterComplete(EntityUid uid, RatKingRummageableComponent component, RatKingRummageDoAfterEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !component.Looted)
		{
			component.Looted = true;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			_audio.PlayPredicted(component.Sound, uid, (EntityUid?)args.User, (AudioParams?)null);
			string spawn = PrototypeManager.Index<WeightedRandomEntityPrototype>(component.RummageLoot).Pick(Random);
			if (_net.IsServer)
			{
				((EntitySystem)this).Spawn(spawn, ((EntitySystem)this).Transform(uid).Coordinates);
			}
		}
	}

	public void UpdateAllServants(EntityUid uid, RatKingComponent component)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid servant in component.Servants)
		{
			UpdateServantNpc(servant, component.CurrentOrder);
		}
	}

	public virtual void UpdateServantNpc(EntityUid uid, RatKingOrderType orderType)
	{
	}

	public virtual void DoCommandCallout(EntityUid uid, RatKingComponent component)
	{
	}
}
