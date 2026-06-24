using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Movement;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Mind;
using Content.Shared.Physics;
using Content.Shared.Rejuvenate;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Actions;

public abstract class SharedActionsSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming GameTiming;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private RotateToFaceSystem _rotateToFace;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCLagCompensationSystem _rmcLagCompensation;

	private EntityQuery<ActionComponent> _actionQuery;

	private EntityQuery<ActionsComponent> _actionsQuery;

	private EntityQuery<MindComponent> _mindQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_actionQuery = ((EntitySystem)this).GetEntityQuery<ActionComponent>();
		_actionsQuery = ((EntitySystem)this).GetEntityQuery<ActionsComponent>();
		_mindQuery = ((EntitySystem)this).GetEntityQuery<MindComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ActionComponent, MapInitEvent>((EntityEventRefHandler<ActionComponent, MapInitEvent>)OnActionMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionComponent, ComponentShutdown>((EntityEventRefHandler<ActionComponent, ComponentShutdown>)OnActionShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, ActionComponentChangeEvent>((EntityEventRefHandler<ActionsComponent, ActionComponentChangeEvent>)OnActionCompChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, RelayedActionComponentChangeEvent>((EntityEventRefHandler<ActionsComponent, RelayedActionComponentChangeEvent>)OnRelayActionCompChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, DidEquipEvent>((EntityEventRefHandler<ActionsComponent, DidEquipEvent>)OnDidEquip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, DidEquipHandEvent>((EntityEventRefHandler<ActionsComponent, DidEquipHandEvent>)OnHandEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, DidUnequipEvent>((ComponentEventHandler<ActionsComponent, DidUnequipEvent>)OnDidUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, DidUnequipHandEvent>((ComponentEventHandler<ActionsComponent, DidUnequipHandEvent>)OnHandUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, RejuvenateEvent>((EntityEventRefHandler<ActionsComponent, RejuvenateEvent>)OnRejuventate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, ComponentShutdown>((EntityEventRefHandler<ActionsComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, ComponentGetState>((EntityEventRefHandler<ActionsComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionComponent, ActionValidateEvent>((EntityEventRefHandler<ActionComponent, ActionValidateEvent>)OnValidate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InstantActionComponent, ActionValidateEvent>((EntityEventRefHandler<InstantActionComponent, ActionValidateEvent>)OnInstantValidate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTargetActionComponent, ActionValidateEvent>((EntityEventRefHandler<EntityTargetActionComponent, ActionValidateEvent>)OnEntityValidate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WorldTargetActionComponent, ActionValidateEvent>((EntityEventRefHandler<WorldTargetActionComponent, ActionValidateEvent>)OnWorldValidate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InstantActionComponent, ActionGetEventEvent>((EntityEventRefHandler<InstantActionComponent, ActionGetEventEvent>)OnInstantGetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTargetActionComponent, ActionGetEventEvent>((EntityEventRefHandler<EntityTargetActionComponent, ActionGetEventEvent>)OnEntityGetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WorldTargetActionComponent, ActionGetEventEvent>((EntityEventRefHandler<WorldTargetActionComponent, ActionGetEventEvent>)OnWorldGetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InstantActionComponent, ActionSetEventEvent>((EntityEventRefHandler<InstantActionComponent, ActionSetEventEvent>)OnInstantSetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTargetActionComponent, ActionSetEventEvent>((EntityEventRefHandler<EntityTargetActionComponent, ActionSetEventEvent>)OnEntitySetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WorldTargetActionComponent, ActionSetEventEvent>((EntityEventRefHandler<WorldTargetActionComponent, ActionSetEventEvent>)OnWorldSetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTargetActionComponent, ActionSetTargetEvent>((EntityEventRefHandler<EntityTargetActionComponent, ActionSetTargetEvent>)OnEntitySetTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WorldTargetActionComponent, ActionSetTargetEvent>((EntityEventRefHandler<WorldTargetActionComponent, ActionSetTargetEvent>)OnWorldSetTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestPerformActionEvent>((EntitySessionEventHandler<RequestPerformActionEvent>)OnActionRequest, (Type[])null, (Type[])null);
	}

	private void OnActionMapInit(Entity<ActionComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ActionComponent comp = ent.Comp;
		comp.OriginalIconColor = comp.IconColor;
		((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "OriginalIconColor", (MetaDataComponent)null);
	}

	private void OnActionShutdown(Entity<ActionComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ent.Comp.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			if (!((EntitySystem)this).TerminatingOrDeleted(user, (MetaDataComponent)null))
			{
				RemoveAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
			}
		}
	}

	private void OnShutdown(Entity<ActionsComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid actionId in ent.Comp.Actions)
		{
			RemoveAction(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(ent), Entity<ActionsComponent>.op_Implicit(ent))), Entity<ActionComponent>.op_Implicit(actionId));
		}
	}

	private void OnGetState(Entity<ActionsComponent> ent, ref ComponentGetState args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new ActionsComponentState(((EntitySystem)this).GetNetEntitySet(ent.Comp.Actions));
	}

	public Entity<ActionComponent>? GetAction(Entity<ActionComponent?>? action, bool logError = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (action.HasValue)
		{
			Entity<ActionComponent> ent = action.GetValueOrDefault();
			if (!((EntitySystem)this).Deleted(Entity<ActionComponent>.op_Implicit(ent), (MetaDataComponent)null))
			{
				if (!_actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, logError))
				{
					return null;
				}
				return Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), ent.Comp));
			}
		}
		return null;
	}

	public void SetCooldown(Entity<ActionComponent?>? action, TimeSpan start, TimeSpan end)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			ent.Comp.Cooldown = new ActionCooldown
			{
				Start = start,
				End = end
			};
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Cooldown", (MetaDataComponent)null);
		}
	}

	public void RemoveCooldown(Entity<ActionComponent?>? action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			ent.Comp.Cooldown = null;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Cooldown", (MetaDataComponent)null);
		}
	}

	public void SetCooldown(Entity<ActionComponent?>? action, TimeSpan cooldown)
	{
		TimeSpan start = GameTiming.CurTime;
		SetCooldown(action, start, start + cooldown);
	}

	public void ClearCooldown(Entity<ActionComponent?>? action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			ActionCooldown? cooldown = ent.Comp.Cooldown;
			if (cooldown.HasValue)
			{
				ActionCooldown cooldown2 = cooldown.GetValueOrDefault();
				ent.Comp.Cooldown = new ActionCooldown
				{
					Start = cooldown2.Start,
					End = GameTiming.CurTime
				};
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Cooldown", (MetaDataComponent)null);
			}
		}
	}

	public void SetIfBiggerCooldown(Entity<ActionComponent?>? action, TimeSpan cooldown)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (!action2.HasValue)
		{
			return;
		}
		Entity<ActionComponent> ent = action2.GetValueOrDefault();
		if (!(cooldown < TimeSpan.Zero))
		{
			TimeSpan start = GameTiming.CurTime;
			TimeSpan end = start + cooldown;
			ref ActionCooldown? cooldown2 = ref ent.Comp.Cooldown;
			if (!cooldown2.HasValue || !(cooldown2.GetValueOrDefault().End > end))
			{
				SetCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))), start, end);
			}
		}
	}

	public void StartUseDelay(Entity<ActionComponent?>? action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			TimeSpan? useDelay = ent.Comp.UseDelay;
			if (useDelay.HasValue)
			{
				TimeSpan delay = useDelay.GetValueOrDefault();
				SetCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))), delay);
			}
		}
	}

	public void SetUseDelay(Entity<ActionComponent?>? action, TimeSpan? delay)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			if (!(ent.Comp.UseDelay == delay))
			{
				ent.Comp.UseDelay = delay;
				UpdateAction(ent);
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "UseDelay", (MetaDataComponent)null);
			}
		}
	}

	public void ReduceUseDelay(Entity<ActionComponent?>? action, TimeSpan? lowerDelay)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			if (ent.Comp.UseDelay.HasValue && lowerDelay.HasValue)
			{
				ActionComponent comp = ent.Comp;
				comp.UseDelay -= lowerDelay;
			}
			if (ent.Comp.UseDelay < TimeSpan.Zero)
			{
				ent.Comp.UseDelay = null;
			}
			UpdateAction(ent);
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "UseDelay", (MetaDataComponent)null);
		}
	}

	private void OnRejuventate(Entity<ActionsComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid act in ent.Comp.Actions)
		{
			ClearCooldown(Entity<ActionComponent>.op_Implicit(act));
		}
	}

	public virtual void UpdateAction(Entity<ActionComponent> ent)
	{
	}

	public void SetToggled(Entity<ActionComponent?>? action, bool toggled)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			if (ent.Comp.Toggled != toggled)
			{
				ent.Comp.Toggled = toggled;
				UpdateAction(ent);
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Toggled", (MetaDataComponent)null);
			}
		}
	}

	public void SetEnabled(Entity<ActionComponent?>? action, bool enabled)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			if (ent.Comp.Enabled != enabled)
			{
				ent.Comp.Enabled = enabled;
				UpdateAction(ent);
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Enabled", (MetaDataComponent)null);
			}
		}
	}

	private void OnActionRequest(RequestPerformActionEvent ev, EntitySessionEventArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		_rmcLagCompensation.SetLastRealTick(((EntitySessionEventArgs)(ref args)).SenderSession.UserId, ev.LastRealTick);
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid user = attachedEntity.GetValueOrDefault();
		ActionsComponent component = default(ActionsComponent);
		if (!_actionsQuery.TryComp(user, ref component))
		{
			return;
		}
		EntityUid actionEnt = ((EntitySystem)this).GetEntity(ev.Action);
		MetaDataComponent metaData = default(MetaDataComponent);
		if (!((EntitySystem)this).TryComp(actionEnt, ref metaData))
		{
			return;
		}
		string name = ((EntitySystem)this).Name(actionEnt, metaData);
		if (!component.Actions.Contains(actionEnt))
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(56, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" attempted to perform an action that they do not have: ");
			handler.AppendFormatted(name);
			handler.AppendLiteral(".");
			adminLogger.Add(LogType.Action, ref handler);
			return;
		}
		Entity<ActionComponent>? action = GetAction(Entity<ActionComponent>.op_Implicit(actionEnt));
		if (!action.HasValue)
		{
			return;
		}
		Entity<ActionComponent> action2 = action.GetValueOrDefault();
		if (!action2.Comp.Enabled)
		{
			return;
		}
		TimeSpan curTime = GameTiming.CurTime;
		if (IsCooldownActive(Entity<ActionComponent>.op_Implicit(action2), curTime))
		{
			return;
		}
		ActionAttemptEvent attemptEv = new ActionAttemptEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ActionAttemptEvent>(Entity<ActionComponent>.op_Implicit(action2), ref attemptEv, false);
		if (!attemptEv.Cancelled)
		{
			EntityUid provider = action2.Comp.Container ?? user;
			ActionValidateEvent validateEv = new ActionValidateEvent
			{
				Input = ev,
				User = user,
				Provider = provider
			};
			((EntitySystem)this).RaiseLocalEvent<ActionValidateEvent>(Entity<ActionComponent>.op_Implicit(action2), ref validateEv, false);
			if (!validateEv.Invalid && _rmcActions.CanUseActionPopup(user, actionEnt, ((EntitySystem)this).GetEntity(ev.EntityTarget)))
			{
				PerformAction(Entity<ActionsComponent>.op_Implicit((user, component)), action2);
			}
		}
	}

	private void OnValidate(Entity<ActionComponent> ent, ref ActionValidateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if ((ent.Comp.CheckConsciousness && !_actionBlocker.CanConsciouslyPerformAction(args.User)) || (ent.Comp.CheckCanInteract && !_actionBlocker.CanInteract(args.User, null)))
		{
			args.Invalid = true;
		}
	}

	private void OnInstantValidate(Entity<InstantActionComponent> ent, ref ActionValidateEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(40, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
		handler.AppendLiteral(" is performing the ");
		handler.AppendFormatted(((EntitySystem)this).Name(Entity<InstantActionComponent>.op_Implicit(ent), (MetaDataComponent)null), 0, "action");
		handler.AppendLiteral(" action provided by ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Provider)), "provider", "ToPrettyString(args.Provider)");
		handler.AppendLiteral(".");
		adminLogger.Add(LogType.Action, ref handler);
	}

	private void OnEntityValidate(Entity<EntityTargetActionComponent> ent, ref ActionValidateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent ev = ent.Comp.Event;
		if (ev == null)
		{
			return;
		}
		NetEntity? entityTarget = args.Input.EntityTarget;
		if (entityTarget.HasValue)
		{
			NetEntity netTarget = entityTarget.GetValueOrDefault();
			EntityUid user = args.User;
			EntityUid target = ((EntitySystem)this).GetEntity(netTarget);
			Vector2 targetWorldPos = _transform.GetWorldPosition(target);
			if (ent.Comp.RotateOnUse)
			{
				_rotateToFace.TryFaceCoordinates(user, targetWorldPos);
			}
			if (!ValidateEntityTarget(user, target, ent))
			{
				args.Invalid = true;
				return;
			}
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(55, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" is performing the ");
			handler.AppendFormatted(((EntitySystem)this).Name(Entity<EntityTargetActionComponent>.op_Implicit(ent), (MetaDataComponent)null), 0, "action");
			handler.AppendLiteral(" action (provided by ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Provider)), "provider", "ToPrettyString(args.Provider)");
			handler.AppendLiteral(") targeted at ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(".");
			adminLogger.Add(LogType.Action, ref handler);
			ev.Target = target;
		}
		else
		{
			args.Invalid = true;
		}
	}

	private void OnWorldValidate(Entity<WorldTargetActionComponent> ent, ref ActionValidateEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		NetCoordinates? entityCoordinatesTarget = args.Input.EntityCoordinatesTarget;
		if (entityCoordinatesTarget.HasValue)
		{
			NetCoordinates netTarget = entityCoordinatesTarget.GetValueOrDefault();
			EntityUid user = args.User;
			EntityCoordinates target = ((EntitySystem)this).GetCoordinates(netTarget);
			if (ent.Comp.RotateOnUse)
			{
				_rotateToFace.TryFaceCoordinates(user, _transform.ToMapCoordinates(target, true).Position);
			}
			if (!ValidateWorldTarget(user, target, ent))
			{
				return;
			}
			EntityUid? targetEntity = ((EntitySystem)this).GetEntity(args.Input.EntityTarget);
			EntityTargetActionComponent entTarget = default(EntityTargetActionComponent);
			if (targetEntity.HasValue && (!((EntitySystem)this).TryComp<EntityTargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent), ref entTarget) || !ValidateEntityTarget(user, targetEntity.Value, Entity<EntityTargetActionComponent>.op_Implicit((Entity<WorldTargetActionComponent>.op_Implicit(ent), entTarget)))))
			{
				args.Invalid = true;
				return;
			}
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(57, 5);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" is performing the ");
			handler.AppendFormatted(((EntitySystem)this).Name(Entity<WorldTargetActionComponent>.op_Implicit(ent), (MetaDataComponent)null), 0, "action");
			handler.AppendLiteral(" action (provided by ");
			handler.AppendFormatted<EntityUid>(args.Provider, "args.Provider");
			handler.AppendLiteral(") targeting ");
			handler.AppendFormatted(targetEntity, "targetEntity");
			handler.AppendLiteral(" at ");
			handler.AppendFormatted<EntityCoordinates>(target, "target", "target");
			handler.AppendLiteral(".");
			adminLogger.Add(LogType.Action, ref handler);
			WorldTargetActionEvent ev = ent.Comp.Event;
			if (ev != null)
			{
				ev.Target = target;
				ev.Entity = targetEntity;
			}
		}
		else
		{
			args.Invalid = true;
		}
	}

	public bool ValidateEntityTarget(EntityUid user, EntityUid target, Entity<EntityTargetActionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Entity<EntityTargetActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		EntityTargetActionComponent entityTargetActionComponent = default(EntityTargetActionComponent);
		val.Deconstruct(ref val2, ref entityTargetActionComponent);
		EntityUid uid = val2;
		EntityTargetActionComponent comp = entityTargetActionComponent;
		if (!((EntityUid)(ref target)).IsValid() || ((EntitySystem)this).Deleted(target, (MetaDataComponent)null))
		{
			((EntitySystem)this).RaisePredictiveEvent<RMCMissedTargetActionEvent>(new RMCMissedTargetActionEvent(((EntitySystem)this).GetNetEntity(Entity<EntityTargetActionComponent>.op_Implicit(ent), (MetaDataComponent)null)));
			return false;
		}
		if (_whitelist.IsWhitelistFail(comp.Whitelist, target))
		{
			return false;
		}
		if (_whitelist.IsBlacklistPass(comp.Blacklist, target))
		{
			return false;
		}
		if (_actionQuery.Comp(uid).CheckCanInteract && !_actionBlocker.CanInteract(user, target) && ent.Comp.TargetCheckCanInteract)
		{
			return false;
		}
		if (user == target)
		{
			return comp.CanTargetSelf;
		}
		TargetActionComponent targetAction = ((EntitySystem)this).Comp<TargetActionComponent>(uid);
		if (targetAction.CheckCanAccess)
		{
			if (!_interaction.InRangeAndAccessible(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), targetAction.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, lagCompensated: true))
			{
				return _interaction.CanAccessViaStorage(user, target);
			}
			return true;
		}
		return true;
	}

	public bool ValidateWorldTarget(EntityUid user, EntityCoordinates target, Entity<WorldTargetActionComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		TargetActionComponent targetAction = ((EntitySystem)this).Comp<TargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent));
		return ValidateBaseTarget(user, target, Entity<TargetActionComponent>.op_Implicit((Entity<WorldTargetActionComponent>.op_Implicit(ent), targetAction)));
	}

	private bool ValidateBaseTarget(EntityUid user, EntityCoordinates coords, Entity<TargetActionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		TargetActionComponent comp = ent.Comp;
		if (comp.CheckCanAccess)
		{
			return _interaction.InRangeUnobstructed(user, coords, comp.Range);
		}
		TransformComponent xform = ((EntitySystem)this).Transform(user);
		if (xform.MapID != _transform.GetMapId(coords))
		{
			return false;
		}
		if (comp.Range <= 0f)
		{
			return true;
		}
		return _transform.InRange(coords, xform.Coordinates, comp.Range);
	}

	private void OnInstantGetEvent(Entity<InstantActionComponent> ent, ref ActionGetEventEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		InstantActionEvent ev = ent.Comp.Event;
		if (ev != null)
		{
			args.Event = ev;
		}
	}

	private void OnEntityGetEvent(Entity<EntityTargetActionComponent> ent, ref ActionGetEventEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent ev = ent.Comp.Event;
		if (ev != null)
		{
			args.Event = ev;
		}
	}

	private void OnWorldGetEvent(Entity<WorldTargetActionComponent> ent, ref ActionGetEventEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent ev = ent.Comp.Event;
		if (ev != null)
		{
			args.Event = ev;
		}
	}

	private void OnInstantSetEvent(Entity<InstantActionComponent> ent, ref ActionSetEventEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Event is InstantActionEvent ev)
		{
			ent.Comp.Event = ev;
			args.Handled = true;
		}
	}

	private void OnEntitySetEvent(Entity<EntityTargetActionComponent> ent, ref ActionSetEventEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Event is EntityTargetActionEvent ev)
		{
			ent.Comp.Event = ev;
			args.Handled = true;
		}
	}

	private void OnWorldSetEvent(Entity<WorldTargetActionComponent> ent, ref ActionSetEventEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Event is WorldTargetActionEvent ev)
		{
			ent.Comp.Event = ev;
			args.Handled = true;
		}
	}

	private void OnEntitySetTarget(Entity<EntityTargetActionComponent> ent, ref ActionSetTargetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent ev = ent.Comp.Event;
		if (ev != null)
		{
			ev.Target = args.Target;
			args.Handled = true;
		}
	}

	private void OnWorldSetTarget(Entity<WorldTargetActionComponent> ent, ref ActionSetTargetEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent ev = ent.Comp.Event;
		if (ev != null)
		{
			ev.Target = ((EntitySystem)this).Transform(args.Target).Coordinates;
			ev.Entity = (((EntitySystem)this).HasComp<EntityTargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent)) ? new EntityUid?(args.Target) : ((EntityUid?)null));
			args.Handled = true;
		}
	}

	public void PerformAction(Entity<ActionsComponent?> performer, Entity<ActionComponent> action, BaseActionEvent? actionEvent = null, bool predicted = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		_ = action.Comp.Toggled;
		if (action.Comp.AttachedEntity.HasValue)
		{
			EntityUid? attachedEntity = action.Comp.AttachedEntity;
			EntityUid val = Entity<ActionsComponent>.op_Implicit(performer);
			if (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() != val)
			{
				((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionsComponent>.op_Implicit(performer), (MetaDataComponent)null)} is attempting to perform an action {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionComponent>.op_Implicit(action), (MetaDataComponent)null)} that is attached to another entity {((EntitySystem)this).ToPrettyString(action.Comp.AttachedEntity, (MetaDataComponent)null)}");
				return;
			}
		}
		if (actionEvent == null)
		{
			actionEvent = GetEvent(Entity<ActionComponent>.op_Implicit(action));
		}
		if (actionEvent == null)
		{
			return;
		}
		BaseActionEvent ev = actionEvent;
		ev.Performer = Entity<ActionsComponent>.op_Implicit(performer);
		((HandledEntityEventArgs)ev).Handled = false;
		EntityUid target = performer.Owner;
		ev.Performer = Entity<ActionsComponent>.op_Implicit(performer);
		ev.Action = action;
		if (!action.Comp.RaiseOnUser)
		{
			EntityUid? attachedEntity = action.Comp.Container;
			if (attachedEntity.HasValue)
			{
				EntityUid container = attachedEntity.GetValueOrDefault();
				if (!_mindQuery.HasComp(container))
				{
					target = container;
				}
			}
		}
		if (action.Comp.RaiseOnAction)
		{
			target = Entity<ActionComponent>.op_Implicit(action);
		}
		((EntitySystem)this).RaiseLocalEvent(target, (object)ev, true);
		if (((HandledEntityEventArgs)ev).Handled)
		{
			if (ev != null && ev.Toggle)
			{
				SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), !action.Comp.Toggled);
			}
			_audio.PlayPredicted(action.Comp.Sound, Entity<ActionsComponent>.op_Implicit(performer), predicted ? new EntityUid?(Entity<ActionsComponent>.op_Implicit(performer)) : ((EntityUid?)null), (AudioParams?)null);
			RemoveCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))));
			StartUseDelay(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))));
			UpdateAction(action);
			ActionPerformedEvent performed = new ActionPerformedEvent(Entity<ActionsComponent>.op_Implicit(performer));
			((EntitySystem)this).RaiseLocalEvent<ActionPerformedEvent>(Entity<ActionComponent>.op_Implicit(action), ref performed, false);
		}
	}

	public EntityUid? AddAction(EntityUid performer, [ForbidLiteral] string? actionPrototypeId, EntityUid container = default(EntityUid), ActionsComponent? component = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? actionId = null;
		AddAction(performer, ref actionId, out ActionComponent _, actionPrototypeId, container, component);
		return actionId;
	}

	public bool AddAction(EntityUid performer, [NotNullWhen(true)] ref EntityUid? actionId, [ForbidLiteral] string? actionPrototypeId, EntityUid container = default(EntityUid), ActionsComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ActionComponent action;
		return AddAction(performer, ref actionId, out action, actionPrototypeId, container, component);
	}

	public bool AddAction(EntityUid performer, [NotNullWhen(true)] ref EntityUid? actionId, [NotNullWhen(true)] out ActionComponent? action, [ForbidLiteral] string? actionPrototypeId, EntityUid container = default(EntityUid), ActionsComponent? component = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityUid)(ref container)).IsValid())
		{
			container = performer;
		}
		if (!_actionContainer.EnsureAction(container, ref actionId, out action, actionPrototypeId))
		{
			return false;
		}
		return AddActionDirect(Entity<ActionsComponent>.op_Implicit((performer, component)), Entity<ActionComponent>.op_Implicit((actionId.Value, action)));
	}

	public bool AddAction(Entity<ActionsComponent?> performer, Entity<ActionComponent?> action, Entity<ActionsContainerComponent?> container)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			EntityUid? container2 = ent.Comp.Container;
			EntityUid owner = container.Owner;
			if (!container2.HasValue || container2.GetValueOrDefault() != owner || !((EntitySystem)this).Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true) || !((BaseContainer)container.Comp.Container).Contains(Entity<ActionComponent>.op_Implicit(ent)))
			{
				((EntitySystem)this).Log.Error($"Attempted to add an action with an invalid container: {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
				return false;
			}
			return AddActionDirect(performer, Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
		}
		return false;
	}

	public bool AddActionDirect(Entity<ActionsComponent?> performer, Entity<ActionComponent?>? action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (action2.HasValue)
		{
			Entity<ActionComponent> ent = action2.GetValueOrDefault();
			EntityUid? attachedEntity = ent.Comp.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				RemoveAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
			}
			if (ent.Comp.StartDelay && ent.Comp.UseDelay.HasValue)
			{
				SetCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))), ent.Comp.UseDelay.Value);
			}
			ref ActionsComponent comp = ref performer.Comp;
			if (comp == null)
			{
				comp = ((EntitySystem)this).EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
			}
			ent.Comp.AttachedEntity = Entity<ActionsComponent>.op_Implicit(performer);
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "AttachedEntity", (MetaDataComponent)null);
			performer.Comp.Actions.Add(Entity<ActionComponent>.op_Implicit(ent));
			((EntitySystem)this).Dirty(Entity<ActionsComponent>.op_Implicit(performer), (IComponent)(object)performer.Comp, (MetaDataComponent)null);
			ActionAdded(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(performer), performer.Comp)), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), ent.Comp)));
			return true;
		}
		return false;
	}

	protected virtual void ActionAdded(Entity<ActionsComponent> performer, Entity<ActionComponent> action)
	{
	}

	public void GrantActions(Entity<ActionsComponent?> performer, IEnumerable<EntityUid> actions, Entity<ActionsContainerComponent?> container)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true))
		{
			return;
		}
		ref ActionsComponent comp = ref performer.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
		}
		foreach (EntityUid actionId in actions)
		{
			AddAction(performer, Entity<ActionComponent>.op_Implicit(actionId), container);
		}
	}

	public void GrantContainedActions(Entity<ActionsComponent?> performer, Entity<ActionsContainerComponent?> container)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true))
		{
			return;
		}
		ref ActionsComponent comp = ref performer.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
		}
		foreach (EntityUid actionId in ((BaseContainer)container.Comp.Container).ContainedEntities)
		{
			Entity<ActionComponent>? action = GetAction(Entity<ActionComponent>.op_Implicit(actionId));
			if (action.HasValue)
			{
				Entity<ActionComponent> action2 = action.GetValueOrDefault();
				AddActionDirect(performer, Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action2), Entity<ActionComponent>.op_Implicit(action2))));
			}
		}
	}

	public void GrantContainedAction(Entity<ActionsComponent?> performer, Entity<ActionsContainerComponent?> container, EntityUid actionId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActionsContainerComponent>(Entity<ActionsContainerComponent>.op_Implicit(container), ref container.Comp, true))
		{
			ref ActionsComponent comp = ref performer.Comp;
			if (comp == null)
			{
				comp = ((EntitySystem)this).EnsureComp<ActionsComponent>(Entity<ActionsComponent>.op_Implicit(performer));
			}
			AddActionDirect(performer, Entity<ActionComponent>.op_Implicit(actionId));
		}
	}

	public IEnumerable<Entity<ActionComponent>> GetActions(EntityUid holderId, ActionsComponent? actions = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ActionsComponent>(holderId, ref actions, false))
		{
			yield break;
		}
		foreach (EntityUid actionId in actions.Actions)
		{
			Entity<ActionComponent>? action = GetAction(Entity<ActionComponent>.op_Implicit(actionId));
			if (action.HasValue)
			{
				yield return action.GetValueOrDefault();
			}
		}
	}

	public void RemoveProvidedActions(EntityUid performer, EntityUid container, ActionsComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ActionsComponent>(performer, ref comp, false))
		{
			return;
		}
		EntityUid[] array = comp.Actions.ToArray();
		foreach (EntityUid actionId in array)
		{
			Entity<ActionComponent>? action = GetAction(Entity<ActionComponent>.op_Implicit(actionId));
			if (action.HasValue)
			{
				Entity<ActionComponent> ent = action.GetValueOrDefault();
				EntityUid? container2 = ent.Comp.Container;
				if (container2.HasValue && container2.GetValueOrDefault() == container)
				{
					RemoveAction(Entity<ActionsComponent>.op_Implicit((performer, comp)), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
				}
				continue;
			}
			break;
		}
	}

	public void RemoveProvidedAction(EntityUid performer, EntityUid container, EntityUid actionId, ActionsComponent? comp = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!_actionsQuery.Resolve(performer, ref comp, false))
		{
			return;
		}
		Entity<ActionComponent>? action = GetAction(Entity<ActionComponent>.op_Implicit(actionId));
		if (action.HasValue)
		{
			Entity<ActionComponent> ent = action.GetValueOrDefault();
			EntityUid? container2 = ent.Comp.Container;
			if (container2.HasValue && container2.GetValueOrDefault() == container)
			{
				RemoveAction(Entity<ActionsComponent>.op_Implicit((performer, comp)), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
			}
		}
	}

	public void RemoveAction(Entity<ActionComponent?>? action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (!action2.HasValue)
		{
			return;
		}
		Entity<ActionComponent> ent = action2.GetValueOrDefault();
		EntityUid? attachedEntity = ent.Comp.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid actions = attachedEntity.GetValueOrDefault();
			ActionsComponent comp = default(ActionsComponent);
			if (_actionsQuery.TryComp(actions, ref comp))
			{
				RemoveAction(Entity<ActionsComponent>.op_Implicit((actions, comp)), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ent), Entity<ActionComponent>.op_Implicit(ent))));
			}
		}
	}

	public void RemoveAction(Entity<ActionsComponent?> performer, Entity<ActionComponent?>? action)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent>? action2 = GetAction(action);
		if (!action2.HasValue)
		{
			return;
		}
		Entity<ActionComponent> ent = action2.GetValueOrDefault();
		EntityUid? attachedEntity = ent.Comp.AttachedEntity;
		EntityUid owner = performer.Owner;
		if (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() != owner)
		{
			if (!GameTiming.ApplyingState)
			{
				((EntitySystem)this).Log.Error($"Attempted to remove an action {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionComponent>.op_Implicit(ent), (MetaDataComponent)null)} from an entity that it was never attached to: {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<ActionsComponent>.op_Implicit(performer), (MetaDataComponent)null)}. Trace: {Environment.StackTrace}");
			}
		}
		else if (!_actionsQuery.Resolve(Entity<ActionsComponent>.op_Implicit(performer), ref performer.Comp, false))
		{
			ent.Comp.AttachedEntity = null;
		}
		else
		{
			performer.Comp.Actions.Remove(ent.Owner);
			((EntitySystem)this).Dirty(Entity<ActionsComponent>.op_Implicit(performer), (IComponent)(object)performer.Comp, (MetaDataComponent)null);
			ent.Comp.AttachedEntity = null;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "AttachedEntity", (MetaDataComponent)null);
			ActionRemoved(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(performer), performer.Comp)), ent);
			if (ent.Comp.Temporary)
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<ActionComponent>.op_Implicit(ent));
			}
		}
	}

	protected virtual void ActionRemoved(Entity<ActionsComponent> performer, Entity<ActionComponent> action)
	{
	}

	public bool ValidAction(Entity<ActionComponent> ent, bool canReach = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Entity<ActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		val.Deconstruct(ref val2, ref actionComponent);
		ActionComponent comp = actionComponent;
		if (!comp.Enabled)
		{
			return false;
		}
		TimeSpan curTime = GameTiming.CurTime;
		if (comp.Cooldown.HasValue && comp.Cooldown.Value.End > curTime)
		{
			return false;
		}
		if (!canReach)
		{
			TargetActionComponent targetActionComponent = ((EntitySystem)this).Comp<TargetActionComponent>(Entity<ActionComponent>.op_Implicit(ent));
			if (targetActionComponent == null)
			{
				return false;
			}
			return !targetActionComponent.CheckCanAccess;
		}
		return true;
	}

	private void OnRelayActionCompChange(Entity<ActionsComponent> ent, ref RelayedActionComponentChangeEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			AttemptRelayActionComponentChangeEvent ev = default(AttemptRelayActionComponentChangeEvent);
			((EntitySystem)this).RaiseLocalEvent<AttemptRelayActionComponentChangeEvent>(ent.Owner, ref ev, false);
			EntityUid target = (EntityUid)(((_003F?)ev.Target) ?? ent.Owner);
			((HandledEntityEventArgs)args).Handled = true;
			args.Toggle = true;
			if (!args.Action.Comp.Toggled)
			{
				base.EntityManager.AddComponents(target, args.Components, true);
			}
			else
			{
				base.EntityManager.RemoveComponents(target, args.Components);
			}
		}
	}

	private void OnActionCompChange(Entity<ActionsComponent> ent, ref ActionComponentChangeEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			args.Toggle = true;
			EntityUid target = ent.Owner;
			if (!args.Action.Comp.Toggled)
			{
				base.EntityManager.AddComponents(target, args.Components, true);
			}
			else
			{
				base.EntityManager.RemoveComponents(target, args.Components);
			}
		}
	}

	private void OnDidEquip(Entity<ActionsComponent> ent, ref DidEquipEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!GameTiming.ApplyingState)
		{
			GetItemActionsEvent ev = new GetItemActionsEvent(_actionContainer, args.Equipee, args.Equipment, args.SlotFlags);
			((EntitySystem)this).RaiseLocalEvent<GetItemActionsEvent>(args.Equipment, ev, false);
			if (ev.Actions.Count != 0)
			{
				GrantActions(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(ent), Entity<ActionsComponent>.op_Implicit(ent))), ev.Actions, Entity<ActionsContainerComponent>.op_Implicit(args.Equipment));
			}
		}
	}

	private void OnHandEquipped(Entity<ActionsComponent> ent, ref DidEquipHandEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!GameTiming.ApplyingState)
		{
			GetItemActionsEvent ev = new GetItemActionsEvent(_actionContainer, args.User, args.Equipped);
			((EntitySystem)this).RaiseLocalEvent<GetItemActionsEvent>(args.Equipped, ev, false);
			if (ev.Actions.Count != 0)
			{
				GrantActions(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(ent), Entity<ActionsComponent>.op_Implicit(ent))), ev.Actions, Entity<ActionsContainerComponent>.op_Implicit(args.Equipped));
			}
		}
	}

	private void OnDidUnequip(EntityUid uid, ActionsComponent component, DidUnequipEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!GameTiming.ApplyingState)
		{
			RemoveProvidedActions(uid, args.Equipment, component);
		}
	}

	private void OnHandUnequipped(EntityUid uid, ActionsComponent component, DidUnequipHandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!GameTiming.ApplyingState)
		{
			RemoveProvidedActions(uid, args.Unequipped, component);
		}
	}

	public void SetEntityIcon(Entity<ActionComponent?> ent, EntityUid? icon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (_actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			EntityUid? entityIcon = ent.Comp.EntityIcon;
			EntityUid? val = icon;
			if (entityIcon.HasValue != val.HasValue || (entityIcon.HasValue && !(entityIcon.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				ent.Comp.EntityIcon = icon;
				((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "EntIcon", (MetaDataComponent)null);
			}
		}
	}

	public void SetItemIconStyle(Entity<ActionComponent?> ent, ItemActionIconStyle style)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) && ent.Comp.ItemIconStyle != style)
		{
			ent.Comp.ItemIconStyle = style;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "ItemIconStyle", (MetaDataComponent)null);
		}
	}

	public void SetIcon(Entity<ActionComponent?> ent, SpriteSpecifier? icon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) && ent.Comp.Icon != icon)
		{
			ent.Comp.Icon = icon;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "Icon", (MetaDataComponent)null);
		}
	}

	public void SetIconOn(Entity<ActionComponent?> ent, SpriteSpecifier? iconOn)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) && ent.Comp.IconOn != iconOn)
		{
			ent.Comp.IconOn = iconOn;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "IconOn", (MetaDataComponent)null);
		}
	}

	public void SetIconColor(Entity<ActionComponent?> ent, Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (_actionQuery.Resolve(Entity<ActionComponent>.op_Implicit(ent), ref ent.Comp, true) && !(ent.Comp.IconColor == color))
		{
			ent.Comp.IconColor = color;
			((EntitySystem)this).DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(ent), ent.Comp, "IconColor", (MetaDataComponent)null);
		}
	}

	public void SetEvent(EntityUid uid, BaseActionEvent ev)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		ActionSetEventEvent setEv = new ActionSetEventEvent(ev);
		((EntitySystem)this).RaiseLocalEvent<ActionSetEventEvent>(uid, ref setEv, false);
		if (!setEv.Handled)
		{
			((EntitySystem)this).Log.Error($"Tried to set event of {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)):action} but nothing handled it!");
		}
	}

	public BaseActionEvent? GetEvent(EntityUid uid)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ActionGetEventEvent ev = default(ActionGetEventEvent);
		((EntitySystem)this).RaiseLocalEvent<ActionGetEventEvent>(uid, ref ev, false);
		return ev.Event;
	}

	public bool SetEventTarget(EntityUid uid, EntityUid target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		ActionSetTargetEvent ev = new ActionSetTargetEvent(target);
		((EntitySystem)this).RaiseLocalEvent<ActionSetTargetEvent>(uid, ref ev, false);
		return ev.Handled;
	}

	public bool IsCooldownActive(ActionComponent action, TimeSpan? curTime = null)
	{
		if (action.Cooldown.HasValue)
		{
			return action.Cooldown.Value.End > curTime;
		}
		return false;
	}

	public void SetTemporary(Entity<ActionComponent?> ent, bool temporary)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActionComponent>(ent.Owner, ref ent.Comp, false))
		{
			ent.Comp.Temporary = temporary;
			((EntitySystem)this).Dirty<ActionComponent>(ent, (MetaDataComponent)null);
		}
	}
}
