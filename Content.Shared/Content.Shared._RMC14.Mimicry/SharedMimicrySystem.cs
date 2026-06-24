using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Mimicry;

public sealed class SharedMimicrySystem : EntitySystem
{
	[Dependency]
	private readonly SharedActionsSystem _actions;

	[Dependency]
	private readonly TurfSystem _turf;

	[Dependency]
	private readonly ITileDefinitionManager _tiles;

	[Dependency]
	private readonly InventorySystem _inventory;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedDoAfterSystem _doAfter;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SharedHumanoidAppearanceSystem _humanoid;

	[Dependency]
	private readonly IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<MimicryComponent, BeingEquippedAttemptEvent>)OnEquipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, GotEquippedEvent>((EntityEventRefHandler<MimicryComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, GotUnequippedEvent>((EntityEventRefHandler<MimicryComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, MimicrySurfaceActionEvent>((EntityEventRefHandler<MimicryComponent, MimicrySurfaceActionEvent>)OnAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, MimicryHoodToggleActionEvent>((EntityEventRefHandler<MimicryComponent, MimicryHoodToggleActionEvent>)OnHoodToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, MimicryDoAfterEvent>((EntityEventRefHandler<MimicryComponent, MimicryDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnEquipAttempt(Entity<MimicryComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Hood.HasValue && _inventory.TryGetSlotEntity(args.EquipTarget, ent.Comp.HoodSlot, out var _))
		{
			((CancellableEntityEventArgs)args).Cancel();
			args.Reason = "mimicry-head-occupied";
			_popup.PopupClient(base.Loc.GetString("mimicry-head-occupied"), args.EquipTarget, args.EquipTarget);
		}
	}

	private void OnEquipped(Entity<MimicryComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		_actions.AddAction(args.Equipee, ref ent.Comp.ActionEntity, EntProtoId.op_Implicit(ent.Comp.Action), Entity<MimicryComponent>.op_Implicit(ent));
		_actions.AddAction(args.Equipee, ref ent.Comp.HoodToggleActionEntity, EntProtoId.op_Implicit(ent.Comp.HoodToggleAction), Entity<MimicryComponent>.op_Implicit(ent));
		if (_net.IsServer)
		{
			EntProtoId? hood = ent.Comp.Hood;
			if (hood.HasValue)
			{
				EntProtoId hoodProto = hood.GetValueOrDefault();
				if (!ent.Comp.HoodUid.HasValue)
				{
					EntityUid hood2 = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(hoodProto), ((EntitySystem)this).Transform(args.Equipee).Coordinates);
					if (_inventory.TryEquip(args.Equipee, hood2, ent.Comp.HoodSlot, silent: true, force: true))
					{
						ent.Comp.HoodUid = hood2;
					}
					else
					{
						((EntitySystem)this).QueueDel((EntityUid?)hood2);
					}
				}
			}
		}
		SetHoodLayers(args.Equipee, ent.Comp, !ent.Comp.HoodDown);
	}

	private void OnUnequipped(Entity<MimicryComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actions = _actions;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(args.Equipee);
		EntityUid? actionEntity = ent.Comp.ActionEntity;
		actions.RemoveAction(performer, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		ent.Comp.ActionEntity = null;
		SharedActionsSystem actions2 = _actions;
		Entity<ActionsComponent> performer2 = Entity<ActionsComponent>.op_Implicit(args.Equipee);
		actionEntity = ent.Comp.HoodToggleActionEntity;
		actions2.RemoveAction(performer2, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		ent.Comp.HoodToggleActionEntity = null;
		SetHoodLayers(args.Equipee, ent.Comp, hidden: false);
		if (_net.IsServer)
		{
			actionEntity = ent.Comp.HoodUid;
			if (actionEntity.HasValue)
			{
				EntityUid hood = actionEntity.GetValueOrDefault();
				((EntitySystem)this).QueueDel((EntityUid?)hood);
				ent.Comp.HoodUid = null;
			}
		}
	}

	private void OnHoodToggle(Entity<MimicryComponent> ent, ref MimicryHoodToggleActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			ent.Comp.HoodDown = !ent.Comp.HoodDown;
			((EntitySystem)this).Dirty<MimicryComponent>(ent, (MetaDataComponent)null);
			SetHoodLayers(args.Performer, ent.Comp, !ent.Comp.HoodDown);
			string msg = (ent.Comp.HoodDown ? "mimicry-hood-down" : "mimicry-hood-up");
			_popup.PopupClient(base.Loc.GetString(msg), args.Performer, args.Performer);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void SetHoodLayers(EntityUid wearer, MimicryComponent comp, bool hidden)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState || !((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(wearer))
		{
			return;
		}
		foreach (HumanoidVisualLayers layer in comp.HoodHiddenLayers)
		{
			_humanoid.SetLayerVisibility(Entity<HumanoidAppearanceComponent>.op_Implicit(wearer), layer, !hidden, SlotFlags.HEAD);
		}
	}

	private void OnAction(Entity<MimicryComponent> ent, ref MimicrySurfaceActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (ent.Comp.MaxUses > 0 && ent.Comp.UsesDone >= ent.Comp.MaxUses)
		{
			_popup.PopupClient(base.Loc.GetString("mimicry-used-up"), args.Performer, args.Performer);
		}
		else
		{
			if (!_turf.TryGetTileRef(args.Target, out var tileRef) || !(_tiles[tileRef.Value.Tile.TypeId] is ContentTileDefinition { Sprite: not null } tileDef))
			{
				return;
			}
			if (ent.Comp.SurfaceWhitelist.Count > 0 && !ent.Comp.SurfaceWhitelist.Contains(ProtoId<ContentTileDefinition>.op_Implicit(tileDef.ID)))
			{
				_popup.PopupClient(base.Loc.GetString("mimicry-bad-surface"), args.Performer, args.Performer);
				return;
			}
			MimicryDoAfterEvent ev = new MimicryDoAfterEvent(tileDef.ID);
			EntityManager entityManager = base.EntityManager;
			EntityUid performer = args.Performer;
			TimeSpan delay = TimeSpan.FromSeconds(ent.Comp.MaskSeconds);
			EntityUid? eventTarget = ent.Owner;
			EntityUid? used = ent.Owner;
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, performer, delay, ev, eventTarget, null, used)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = false
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnDoAfter(Entity<MimicryComponent> ent, ref MimicryDoAfterEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			ent.Comp.MimickedTile = ProtoId<ContentTileDefinition>.op_Implicit(args.Tile);
			ent.Comp.UsesDone++;
			((EntitySystem)this).Dirty<MimicryComponent>(ent, (MetaDataComponent)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
