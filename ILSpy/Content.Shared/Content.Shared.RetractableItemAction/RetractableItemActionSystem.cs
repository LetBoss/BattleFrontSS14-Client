using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Cuffs;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.RetractableItemAction;

public sealed class RetractableItemActionSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedPopupSystem _popups;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RetractableItemActionComponent, MapInitEvent>((EntityEventRefHandler<RetractableItemActionComponent, MapInitEvent>)OnActionInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RetractableItemActionComponent, OnRetractableItemActionEvent>((EntityEventRefHandler<RetractableItemActionComponent, OnRetractableItemActionEvent>)OnRetractableItemAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionRetractableItemComponent, ComponentShutdown>((EntityEventRefHandler<ActionRetractableItemComponent, ComponentShutdown>)OnActionSummonedShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.SubscribeWithRelay<ActionRetractableItemComponent, HeldRelayedEvent<TargetHandcuffedEvent>>((EntityEventRefHandler<ActionRetractableItemComponent, HeldRelayedEvent<TargetHandcuffedEvent>>)OnItemHandcuffed, true, false, true);
	}

	private void OnActionInit(Entity<RetractableItemActionComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_containers.EnsureContainer<Container>(Entity<RetractableItemActionComponent>.op_Implicit(ent), "item-action-item-container", (ContainerManagerComponent)null);
		PopulateActionItem(Entity<RetractableItemActionComponent>.op_Implicit(ent.Owner));
	}

	private void OnRetractableItemAction(Entity<RetractableItemActionComponent> ent, ref OnRetractableItemActionEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		string activeHand = _hands.GetActiveHand(Entity<HandsComponent>.op_Implicit(args.Performer));
		if (activeHand == null)
		{
			return;
		}
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(ent.Owner));
		if (!action.HasValue || !action.GetValueOrDefault().Comp.AttachedEntity.HasValue || !ent.Comp.ActionItemUid.HasValue)
		{
			return;
		}
		if (_hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(ent.Owner)).HasValue && !_hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.Performer), ent.Comp.ActionItemUid) && !_hands.CanDropHeld(args.Performer, activeHand, checkActionBlocker: false))
		{
			_popups.PopupClient(base.Loc.GetString("retractable-item-hand-cannot-drop"), args.Performer, args.Performer);
			return;
		}
		if (_hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.Performer), ent.Comp.ActionItemUid))
		{
			RetractRetractableItem(args.Performer, ent.Comp.ActionItemUid.Value, Entity<RetractableItemActionComponent>.op_Implicit(ent.Owner));
		}
		else
		{
			SummonRetractableItem(args.Performer, ent.Comp.ActionItemUid.Value, activeHand, Entity<RetractableItemActionComponent>.op_Implicit(ent.Owner));
		}
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnActionSummonedShutdown(Entity<ActionRetractableItemComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		EntityUid? summoningAction = ent.Comp.SummoningAction;
		Entity<ActionComponent>? action = actions.GetAction(summoningAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(summoningAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		if (!action.HasValue)
		{
			return;
		}
		Entity<ActionComponent> action2 = action.GetValueOrDefault();
		RetractableItemActionComponent retract = default(RetractableItemActionComponent);
		if (((EntitySystem)this).TryComp<RetractableItemActionComponent>(Entity<ActionComponent>.op_Implicit(action2), ref retract))
		{
			summoningAction = retract.ActionItemUid;
			EntityUid owner = ent.Owner;
			if (summoningAction.HasValue && !(summoningAction.GetValueOrDefault() != owner))
			{
				PopulateActionItem(Entity<RetractableItemActionComponent>.op_Implicit(action2.Owner));
			}
		}
	}

	private void OnItemHandcuffed(Entity<ActionRetractableItemComponent> ent, ref HeldRelayedEvent<TargetHandcuffedEvent> args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		EntityUid? summoningAction = ent.Comp.SummoningAction;
		Entity<ActionComponent>? action = actions.GetAction(summoningAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(summoningAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		if (action.HasValue)
		{
			Entity<ActionComponent> action2 = action.GetValueOrDefault();
			if (action2.Comp.AttachedEntity.HasValue && _hands.GetActiveHand(Entity<HandsComponent>.op_Implicit(action2.Comp.AttachedEntity.Value)) != null)
			{
				RetractRetractableItem(action2.Comp.AttachedEntity.Value, Entity<ActionRetractableItemComponent>.op_Implicit(ent), Entity<RetractableItemActionComponent>.op_Implicit(action2.Owner));
			}
		}
	}

	private void PopulateActionItem(Entity<RetractableItemActionComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? summoned = default(EntityUid?);
		if (((EntitySystem)this).Resolve<RetractableItemActionComponent>(ent.Owner, ref ent.Comp, false) && !((EntitySystem)this).TerminatingOrDeleted(Entity<RetractableItemActionComponent>.op_Implicit(ent), (MetaDataComponent)null) && ((EntitySystem)this).PredictedTrySpawnInContainer(EntProtoId.op_Implicit(ent.Comp.SpawnedPrototype), ent.Owner, "item-action-item-container", ref summoned, (ContainerManagerComponent)null, (ComponentRegistry)null))
		{
			ent.Comp.ActionItemUid = summoned.Value;
			ActionRetractableItemComponent summonedComp = ((EntitySystem)this).AddComp<ActionRetractableItemComponent>(summoned.Value);
			summonedComp.SummoningAction = ent.Owner;
			((EntitySystem)this).Dirty(summoned.Value, (IComponent)(object)summonedComp, (MetaDataComponent)null);
			((EntitySystem)this).Dirty<RetractableItemActionComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void RetractRetractableItem(EntityUid holder, EntityUid item, Entity<RetractableItemActionComponent?> action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RetractableItemActionComponent>(Entity<RetractableItemActionComponent>.op_Implicit(action), ref action.Comp, false))
		{
			((EntitySystem)this).RemComp<UnremoveableComponent>(item);
			BaseContainer container = _containers.GetContainer(Entity<RetractableItemActionComponent>.op_Implicit(action), "item-action-item-container", (ContainerManagerComponent)null);
			_containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(item), container, (TransformComponent)null, false);
			_audio.PlayPredicted((SoundSpecifier)(object)action.Comp.RetractSounds, holder, (EntityUid?)holder, (AudioParams?)null);
		}
	}

	private void SummonRetractableItem(EntityUid holder, EntityUid item, string hand, Entity<RetractableItemActionComponent?> action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RetractableItemActionComponent>(Entity<RetractableItemActionComponent>.op_Implicit(action), ref action.Comp, false))
		{
			_hands.TryForcePickup(Entity<HandsComponent>.op_Implicit(holder), item, hand, checkActionBlocker: false);
			_audio.PlayPredicted((SoundSpecifier)(object)action.Comp.SummonSounds, holder, (EntityUid?)holder, (AudioParams?)null);
			((EntitySystem)this).EnsureComp<UnremoveableComponent>(item);
		}
	}
}
