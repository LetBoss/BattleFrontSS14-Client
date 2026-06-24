using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleViewToggleSystem : EntitySystem
{
	[Dependency]
	private readonly SharedActionsSystem _actions;

	[Dependency]
	private readonly SharedEyeSystem _eye;

	[Dependency]
	private readonly INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleViewToggleComponent, VehicleToggleViewActionEvent>((EntityEventRefHandler<VehicleViewToggleComponent, VehicleToggleViewActionEvent>)OnToggleViewAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleViewToggleComponent, ComponentShutdown>((EntityEventRefHandler<VehicleViewToggleComponent, ComponentShutdown>)OnToggleViewShutdown, (Type[])null, (Type[])null);
	}

	public void EnableViewToggle(EntityUid user, EntityUid outsideTarget, EntityUid source, EntityUid? insideTarget, bool isOutside)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		VehicleViewToggleComponent toggle = ((EntitySystem)this).EnsureComp<VehicleViewToggleComponent>(user);
		toggle.Sources.Add(source);
		toggle.Source = source;
		toggle.OutsideTarget = outsideTarget;
		toggle.InsideTarget = insideTarget;
		toggle.IsOutside = isOutside;
		EnsureSingleToggleAction(user, toggle);
		EntityUid? action = toggle.Action;
		if (action.HasValue)
		{
			EntityUid actionUid = action.GetValueOrDefault();
			ActionComponent actionComp = default(ActionComponent);
			if (((EntitySystem)this).TryComp<ActionComponent>(actionUid, ref actionComp))
			{
				_actions.SetItemIconStyle(Entity<ActionComponent>.op_Implicit((actionUid, actionComp)), ItemActionIconStyle.BigAction);
				_actions.SetEntityIcon(Entity<ActionComponent>.op_Implicit((actionUid, actionComp)), null);
				_actions.SetTemporary(Entity<ActionComponent>.op_Implicit((actionUid, actionComp)), temporary: false);
			}
		}
		UpdateActionState(toggle);
		((EntitySystem)this).Dirty(user, (IComponent)(object)toggle, (MetaDataComponent)null);
	}

	public void DisableViewToggle(EntityUid user, EntityUid source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		VehicleViewToggleComponent toggle = default(VehicleViewToggleComponent);
		if (!((EntitySystem)this).TryComp<VehicleViewToggleComponent>(user, ref toggle))
		{
			return;
		}
		toggle.Sources.Remove(source);
		if (toggle.Sources.Count > 0)
		{
			using (HashSet<EntityUid>.Enumerator enumerator = toggle.Sources.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					EntityUid remaining = enumerator.Current;
					toggle.Source = remaining;
				}
			}
			EnsureSingleToggleAction(user, toggle);
			UpdateActionState(toggle);
			((EntitySystem)this).Dirty(user, (IComponent)(object)toggle, (MetaDataComponent)null);
			return;
		}
		EntityUid? action = toggle.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			RemoveAndDeleteToggleAction(action2, user);
		}
		toggle.Action = null;
		toggle.Source = null;
		toggle.OutsideTarget = null;
		toggle.InsideTarget = null;
		toggle.IsOutside = false;
		((EntitySystem)this).RemComp<VehicleViewToggleComponent>(user);
	}

	private void OnToggleViewShutdown(Entity<VehicleViewToggleComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? action = ent.Comp.Action;
		if (action.HasValue)
		{
			EntityUid action2 = action.GetValueOrDefault();
			RemoveAndDeleteToggleAction(action2, ent.Owner);
		}
	}

	private void OnToggleViewAction(Entity<VehicleViewToggleComponent> ent, ref VehicleToggleViewActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Performer != ent.Owner)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EyeComponent eye = default(EyeComponent);
		if (ent.Comp.OutsideTarget.HasValue && ((EntitySystem)this).TryComp<EyeComponent>(ent.Owner, ref eye))
		{
			EntityUid outside = ent.Comp.OutsideTarget.Value;
			EntityUid? target = eye.Target;
			EntityUid val = outside;
			if (target.HasValue && target.GetValueOrDefault() == val)
			{
				_eye.SetTarget(ent.Owner, ent.Comp.InsideTarget, eye);
				ent.Comp.IsOutside = false;
			}
			else
			{
				ent.Comp.InsideTarget = eye.Target;
				_eye.SetTarget(ent.Owner, (EntityUid?)outside, eye);
				ent.Comp.IsOutside = true;
			}
			EnsureSingleToggleAction(ent.Owner, ent.Comp);
			UpdateActionState(ent.Comp);
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			((EntitySystem)this).RaiseLocalEvent<VehicleViewToggledEvent>(ent.Owner, new VehicleViewToggledEvent(ent.Comp.IsOutside), false);
		}
	}

	private void UpdateActionState(VehicleViewToggleComponent toggle)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (toggle.Action.HasValue)
		{
			SharedActionsSystem actions = _actions;
			EntityUid? action = toggle.Action;
			actions.SetToggled(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggle.IsOutside);
		}
	}

	private void EnsureSingleToggleAction(EntityUid user, VehicleViewToggleComponent toggle)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		ActionsContainerComponent containerComp = default(ActionsContainerComponent);
		EntityUid? attachedEntity;
		if (((EntitySystem)this).TryComp<ActionsContainerComponent>(user, ref containerComp))
		{
			EntityUid[] array = ((BaseContainer)containerComp.Container).ContainedEntities.ToArray();
			ActionComponent actionComp = default(ActionComponent);
			foreach (EntityUid action in array)
			{
				if (!IsToggleActionPrototype(action))
				{
					continue;
				}
				if (((EntitySystem)this).TryComp<ActionComponent>(action, ref actionComp))
				{
					attachedEntity = actionComp.AttachedEntity;
					EntityUid val = user;
					if (attachedEntity.HasValue && attachedEntity.GetValueOrDefault() == val)
					{
						continue;
					}
				}
				RemoveAndDeleteToggleAction(action, user);
			}
		}
		EntityUid? primaryAction = null;
		attachedEntity = toggle.Action;
		if (attachedEntity.HasValue)
		{
			EntityUid existing = attachedEntity.GetValueOrDefault();
			if (IsLiveToggleAction(existing))
			{
				primaryAction = existing;
			}
		}
		ActionsComponent actionsComp = default(ActionsComponent);
		if (((EntitySystem)this).TryComp<ActionsComponent>(user, ref actionsComp))
		{
			EntityUid[] array = actionsComp.Actions.ToArray();
			foreach (EntityUid action2 in array)
			{
				if (IsLiveToggleAction(action2))
				{
					if (!primaryAction.HasValue)
					{
						primaryAction = action2;
					}
					else if (!(action2 == primaryAction.Value))
					{
						RemoveAndDeleteToggleAction(action2, user);
					}
				}
			}
		}
		if (!primaryAction.HasValue)
		{
			primaryAction = _actions.AddAction(user, EntProtoId.op_Implicit(toggle.ActionId));
		}
		toggle.Action = primaryAction;
		attachedEntity = toggle.Action;
		if (attachedEntity.HasValue)
		{
			EntityUid ensuredAction = attachedEntity.GetValueOrDefault();
			_actions.SetEnabled(Entity<ActionComponent>.op_Implicit(ensuredAction), enabled: true);
		}
		bool IsLiveToggleAction(EntityUid actionUid)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if (!IsToggleActionPrototype(actionUid))
			{
				return false;
			}
			ActionComponent actionComp2 = default(ActionComponent);
			if (((EntitySystem)this).TryComp<ActionComponent>(actionUid, ref actionComp2))
			{
				EntityUid? attachedEntity2 = actionComp2.AttachedEntity;
				EntityUid val2 = user;
				if (attachedEntity2.HasValue && !(attachedEntity2.GetValueOrDefault() != val2))
				{
					return true;
				}
			}
			return false;
		}
		bool IsToggleActionPrototype(EntityUid actionUid)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			MetaDataComponent metaData = default(MetaDataComponent);
			if (((EntitySystem)this).TerminatingOrDeleted(actionUid, (MetaDataComponent)null) || !((EntitySystem)this).TryComp(actionUid, ref metaData))
			{
				return false;
			}
			EntityPrototype entityPrototype = metaData.EntityPrototype;
			return ((entityPrototype != null) ? entityPrototype.ID : null) == ((object)Unsafe.As<EntProtoId, EntProtoId>(ref toggle.ActionId)/*cast due to constrained. prefix*/).ToString();
		}
	}

	private void RemoveAndDeleteToggleAction(EntityUid action, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(action, (MetaDataComponent)null))
		{
			if (user.HasValue)
			{
				EntityUid actionUser = user.GetValueOrDefault();
				_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(actionUser), Entity<ActionComponent>.op_Implicit(action));
			}
			else
			{
				_actions.RemoveAction(Entity<ActionComponent>.op_Implicit(action));
			}
			if (!_net.IsClient && ((EntitySystem)this).Exists(action))
			{
				((EntitySystem)this).QueueDel((EntityUid?)action);
			}
		}
	}
}
