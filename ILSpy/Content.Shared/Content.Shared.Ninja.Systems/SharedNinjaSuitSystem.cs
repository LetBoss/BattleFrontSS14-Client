using System;
using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Ninja.Systems;

public abstract class SharedNinjaSuitSystem : EntitySystem
{
	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedSpaceNinjaSystem _ninja;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, MapInitEvent>((EntityEventRefHandler<NinjaSuitComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, ClothingGotEquippedEvent>((EntityEventRefHandler<NinjaSuitComponent, ClothingGotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, GetItemActionsEvent>((EntityEventRefHandler<NinjaSuitComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, ToggleClothingCheckEvent>((EntityEventRefHandler<NinjaSuitComponent, ToggleClothingCheckEvent>)OnCloakCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, CheckItemCreatorEvent>((EntityEventRefHandler<NinjaSuitComponent, CheckItemCreatorEvent>)OnStarCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, CreateItemAttemptEvent>((EntityEventRefHandler<NinjaSuitComponent, CreateItemAttemptEvent>)OnCreateStarAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<NinjaSuitComponent, ItemToggleActivateAttemptEvent>)OnActivateAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NinjaSuitComponent, GotUnequippedEvent>((EntityEventRefHandler<NinjaSuitComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnEquipped(Entity<NinjaSuitComponent> ent, ref ClothingGotEquippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Wearer;
		SpaceNinjaComponent ninja = default(SpaceNinjaComponent);
		if (_ninja.NinjaQuery.TryComp(user, ref ninja))
		{
			NinjaEquipped(ent, Entity<SpaceNinjaComponent>.op_Implicit((user, ninja)));
		}
	}

	protected virtual void NinjaEquipped(Entity<NinjaSuitComponent> ent, Entity<SpaceNinjaComponent> user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_ninja.AssignSuit(user, Entity<NinjaSuitComponent>.op_Implicit(ent));
	}

	private void OnMapInit(Entity<NinjaSuitComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Entity<NinjaSuitComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		NinjaSuitComponent ninjaSuitComponent = default(NinjaSuitComponent);
		val.Deconstruct(ref val2, ref ninjaSuitComponent);
		EntityUid uid = val2;
		NinjaSuitComponent comp = ninjaSuitComponent;
		_actionContainer.EnsureAction(uid, ref comp.RecallKatanaActionEntity, EntProtoId.op_Implicit(comp.RecallKatanaAction));
		_actionContainer.EnsureAction(uid, ref comp.EmpActionEntity, EntProtoId.op_Implicit(comp.EmpAction));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void OnGetItemActions(Entity<NinjaSuitComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (_ninja.IsNinja(args.User))
		{
			NinjaSuitComponent comp = ent.Comp;
			args.AddAction(ref comp.RecallKatanaActionEntity, EntProtoId.op_Implicit(comp.RecallKatanaAction));
			args.AddAction(ref comp.EmpActionEntity, EntProtoId.op_Implicit(comp.EmpAction));
		}
	}

	private void OnCloakCheck(Entity<NinjaSuitComponent> ent, ref ToggleClothingCheckEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_ninja.IsNinja(args.User))
		{
			args.Cancelled = true;
		}
	}

	private void OnStarCheck(Entity<NinjaSuitComponent> ent, ref CheckItemCreatorEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_ninja.IsNinja(args.User))
		{
			args.Cancelled = true;
		}
	}

	private void OnCreateStarAttempt(Entity<NinjaSuitComponent> ent, ref CreateItemAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (CheckDisabled(ent, args.User))
		{
			args.Cancelled = true;
		}
	}

	private void OnUnequipped(Entity<NinjaSuitComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Equipee;
		SpaceNinjaComponent ninja = default(SpaceNinjaComponent);
		if (_ninja.NinjaQuery.TryComp(user, ref ninja))
		{
			UserUnequippedSuit(ent, Entity<SpaceNinjaComponent>.op_Implicit((user, ninja)));
		}
	}

	public void RevealNinja(Entity<NinjaSuitComponent?> ent, EntityUid user, bool disable = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<NinjaSuitComponent>(Entity<NinjaSuitComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			EntityUid uid = ent.Owner;
			NinjaSuitComponent comp = ent.Comp;
			if (!_toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(uid), user) && disable)
			{
				_audio.PlayPredicted(comp.RevealSound, uid, (EntityUid?)user, (AudioParams?)null);
				Popup.PopupClient(base.Loc.GetString("ninja-revealed"), user, user, PopupType.MediumCaution);
				_useDelay.TryResetDelay(uid, checkDelayed: false, null, comp.DisableDelayId);
			}
		}
	}

	private void OnActivateAttempt(Entity<NinjaSuitComponent> ent, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!_ninja.IsNinja(args.User))
		{
			args.Cancelled = true;
		}
		else if (IsDisabled(Entity<NinjaSuitComponent, UseDelayComponent>.op_Implicit((ValueTuple<EntityUid, NinjaSuitComponent, UseDelayComponent>)(Entity<NinjaSuitComponent>.op_Implicit(ent), ent.Comp, null))))
		{
			args.Cancelled = true;
			args.Popup = base.Loc.GetString("ninja-suit-cooldown");
		}
	}

	public bool IsDisabled(Entity<NinjaSuitComponent?, UseDelayComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<NinjaSuitComponent, UseDelayComponent>(Entity<NinjaSuitComponent, UseDelayComponent>.op_Implicit(ent), ref ent.Comp1, ref ent.Comp2, true))
		{
			return false;
		}
		return _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<NinjaSuitComponent, UseDelayComponent>.op_Implicit(ent), ent.Comp2)), ent.Comp1.DisableDelayId);
	}

	protected bool CheckDisabled(Entity<NinjaSuitComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (IsDisabled(Entity<NinjaSuitComponent, UseDelayComponent>.op_Implicit((ValueTuple<EntityUid, NinjaSuitComponent, UseDelayComponent>)(Entity<NinjaSuitComponent>.op_Implicit(ent), ent.Comp, null))))
		{
			Popup.PopupEntity(base.Loc.GetString("ninja-suit-cooldown"), user, user, PopupType.Medium);
			return true;
		}
		return false;
	}

	protected virtual void UserUnequippedSuit(Entity<NinjaSuitComponent> ent, Entity<SpaceNinjaComponent> user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_ninja.AssignSuit(user, null);
		EntityUid? gloves = user.Comp.Gloves;
		if (gloves.HasValue)
		{
			EntityUid uid = gloves.GetValueOrDefault();
			_toggle.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(uid), Entity<SpaceNinjaComponent>.op_Implicit(user));
		}
	}
}
