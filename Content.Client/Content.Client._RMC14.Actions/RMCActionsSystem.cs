using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Content.Client.Actions;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Actions;

public sealed class RMCActionsSystem : SharedRMCActionsSystem
{
	[Dependency]
	private ActionsSystem _actions;

	[Dependency]
	private IPlayerManager _player;

	private EntityUid? _sortEnt;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<RMCActionOrderLoadedEvent>((EntityEventHandler<RMCActionOrderLoadedEvent>)OnActionOrderLoaded, (Type[])null, (Type[])null);
	}

	private void OnActionOrderLoaded(RMCActionOrderLoadedEvent ev)
	{
		_sortEnt = null;
	}

	public void ActionsChanged(List<EntityUid?> actions)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		List<EntProtoId> list = new List<EntProtoId>();
		MetaDataComponent val = default(MetaDataComponent);
		foreach (EntityUid? action in actions)
		{
			if (!action.HasValue)
			{
				continue;
			}
			EntityUid valueOrDefault = action.GetValueOrDefault();
			if (((EntitySystem)this).Exists(valueOrDefault) && ((EntitySystem)this).TryComp(valueOrDefault, ref val))
			{
				EntityPrototype entityPrototype = val.EntityPrototype;
				if (entityPrototype != null)
				{
					list.Add(EntProtoId.op_Implicit(entityPrototype.ID));
				}
			}
		}
		RMCActionOrderChangeEvent rMCActionOrderChangeEvent = new RMCActionOrderChangeEvent(list);
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)rMCActionOrderChangeEvent);
	}

	private void SortDefault(EntityUid player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		XenoComponent xenoComponent = default(XenoComponent);
		if (!((EntitySystem)this).TryComp<XenoComponent>(player, ref xenoComponent))
		{
			return;
		}
		foreach (KeyValuePair<EntProtoId, EntityUid> action in xenoComponent.Actions)
		{
			var (_, val3) = (KeyValuePair<EntProtoId, EntityUid>)(ref action);
			if (!((EntityUid)(ref val3)).IsValid())
			{
				return;
			}
		}
		_sortEnt = player;
		List<Entity<ActionComponent>> list = new List<Entity<ActionComponent>>();
		foreach (Entity<ActionComponent> action2 in _actions.GetActions(player))
		{
			list.Add(action2);
		}
		List<EntityUid> xenoActions = xenoComponent.Actions.Values.ToList();
		list.Sort(delegate(Entity<ActionComponent> a, Entity<ActionComponent> b)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			int num = xenoActions.FindIndex((EntityUid e) => e == a.Owner);
			int num2 = xenoActions.FindIndex((EntityUid e) => e == b.Owner);
			return (num != -1 && num2 != -1) ? (num - num2) : ActionsSystem.ActionComparer(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(a), Entity<ActionComponent>.op_Implicit(a))), Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(b), Entity<ActionComponent>.op_Implicit(b))));
		});
		List<ActionsSystem.SlotAssignment> assignments = list.Select((Entity<ActionComponent> t, int i) => new ActionsSystem.SlotAssignment(0, (byte)i, Entity<ActionComponent>.op_Implicit(t))).ToList();
		_actions.SetAssignments(assignments);
	}

	public override void Update(float frameTime)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		localEntity = _sortEnt;
		EntityUid val = valueOrDefault;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
		{
			return;
		}
		_sortEnt = null;
		RMCActionOrderComponent rMCActionOrderComponent = default(RMCActionOrderComponent);
		if (((EntitySystem)this).TryComp<RMCActionOrderComponent>(valueOrDefault, ref rMCActionOrderComponent))
		{
			ImmutableArray<EntProtoId>? order = rMCActionOrderComponent.Order;
			if (order.HasValue)
			{
				ImmutableArray<EntProtoId> valueOrDefault2 = order.GetValueOrDefault();
				if (valueOrDefault2.Length > 0)
				{
					Entity<ActionComponent>[] array = _actions.GetClientActions().ToArray();
					Entity<ActionComponent>[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						Entity<ActionComponent> val2 = array2[i];
						if (!((EntityUid)(ref val2.Owner)).IsValid())
						{
							return;
						}
					}
					_sortEnt = valueOrDefault;
					Entity<ActionComponent>[] array3 = new Entity<ActionComponent>[valueOrDefault2.Length];
					List<Entity<ActionComponent>> list = new List<Entity<ActionComponent>>();
					array2 = array;
					MetaDataComponent val4 = default(MetaDataComponent);
					foreach (Entity<ActionComponent> val3 in array2)
					{
						if (((EntitySystem)this).TryComp(Entity<ActionComponent>.op_Implicit(val3), ref val4))
						{
							EntityPrototype entityPrototype = val4.EntityPrototype;
							if (entityPrototype != null)
							{
								int num = valueOrDefault2.IndexOf(EntProtoId.op_Implicit(entityPrototype.ID));
								if (num < 0)
								{
									list.Add(val3);
								}
								else
								{
									array3[num] = val3;
								}
								continue;
							}
						}
						list.Add(val3);
					}
					List<ActionsSystem.SlotAssignment> list2 = new List<ActionsSystem.SlotAssignment>();
					Entity<ActionComponent>[] array4 = (from a in array3.Concat(list)
						where a != default(Entity<ActionComponent>)
						select a).ToArray();
					for (int num2 = 0; num2 < array4.Length; num2++)
					{
						list2.Add(new ActionsSystem.SlotAssignment(0, (byte)num2, Entity<ActionComponent>.op_Implicit(array4[num2])));
					}
					_actions.SetAssignments(list2);
					return;
				}
			}
		}
		SortDefault(valueOrDefault);
	}
}
