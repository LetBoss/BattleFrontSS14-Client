using System;
using System.Collections.Generic;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Materials;
using Content.Shared.Nutrition;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Tools.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared.Storage.EntitySystems;

public sealed class SecretStashSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ToolOpenableSystem _toolOpenableSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private DamageableSystem _damageableSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, ComponentInit>((EntityEventRefHandler<SecretStashComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, DestructionEventArgs>((EntityEventRefHandler<SecretStashComponent, DestructionEventArgs>)OnDestroyed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, GotReclaimedEvent>((EntityEventRefHandler<SecretStashComponent, GotReclaimedEvent>)OnReclaimed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, InteractUsingEvent>((EntityEventRefHandler<SecretStashComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, new Type[2]
		{
			typeof(ToolOpenableSystem),
			typeof(AnchorableSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, AfterFullyEatenEvent>((EntityEventRefHandler<SecretStashComponent, AfterFullyEatenEvent>)OnEaten, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, InteractHandEvent>((EntityEventRefHandler<SecretStashComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SecretStashComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<SecretStashComponent, GetVerbsEvent<InteractionVerb>>)OnGetVerb, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<SecretStashComponent> entity, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		entity.Comp.ItemContainer = _containerSystem.EnsureContainer<ContainerSlot>(Entity<SecretStashComponent>.op_Implicit(entity), "stash", ref flag, (ContainerManagerComponent)null);
	}

	private void OnDestroyed(Entity<SecretStashComponent> entity, ref DestructionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DropContentsAndAlert(entity);
	}

	private void OnReclaimed(Entity<SecretStashComponent> entity, ref GotReclaimedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DropContentsAndAlert(entity, args.ReclaimerCoordinates);
	}

	private void OnEaten(Entity<SecretStashComponent> entity, ref AfterFullyEatenEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damage = entity.Comp.DamageEatenItemInside;
		if (HasItemInside(entity) && damage != null)
		{
			_damageableSystem.TryChangeDamage(args.User, damage, ignoreResistances: true);
		}
	}

	private void OnInteractUsing(Entity<SecretStashComponent> entity, ref InteractUsingEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && IsStashOpen(entity))
		{
			((HandledEntityEventArgs)args).Handled = TryStashItem(entity, args.User, args.Used);
		}
	}

	private void OnInteractHand(Entity<SecretStashComponent> entity, ref InteractHandEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && IsStashOpen(entity))
		{
			((HandledEntityEventArgs)args).Handled = TryGetItem(entity, args.User);
		}
	}

	private bool TryStashItem(Entity<SecretStashComponent> entity, EntityUid userUid, EntityUid itemToHideUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		ItemComponent itemComp = default(ItemComponent);
		if (!((EntitySystem)this).TryComp<ItemComponent>(itemToHideUid, ref itemComp))
		{
			return false;
		}
		_audio.PlayPredicted(entity.Comp.TryInsertItemSound, Entity<SecretStashComponent>.op_Implicit(entity), (EntityUid?)userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.25f));
		ContainerSlot container = entity.Comp.ItemContainer;
		if (HasItemInside(entity))
		{
			string popup = base.Loc.GetString("comp-secret-stash-action-hide-container-not-empty");
			_popupSystem.PopupClient(popup, Entity<SecretStashComponent>.op_Implicit(entity), userUid);
			return false;
		}
		if (_item.GetSizePrototype(itemComp.Size) > _item.GetSizePrototype(entity.Comp.MaxItemSize) || _whitelistSystem.IsBlacklistPass(entity.Comp.Blacklist, itemToHideUid))
		{
			string msg = base.Loc.GetString("comp-secret-stash-action-hide-item-too-big", (ValueTuple<string, object>)("item", itemToHideUid), (ValueTuple<string, object>)("stashname", GetStashName(entity)));
			_popupSystem.PopupClient(msg, Entity<SecretStashComponent>.op_Implicit(entity), userUid);
			return false;
		}
		if (!_handsSystem.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit(userUid), itemToHideUid, (BaseContainer)(object)container))
		{
			return false;
		}
		string successMsg = base.Loc.GetString("comp-secret-stash-action-hide-success", (ValueTuple<string, object>)("item", itemToHideUid), (ValueTuple<string, object>)("stashname", GetStashName(entity)));
		_popupSystem.PopupClient(successMsg, Entity<SecretStashComponent>.op_Implicit(entity), userUid);
		return true;
	}

	private bool TryGetItem(Entity<SecretStashComponent> entity, EntityUid userUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(userUid, ref handsComp))
		{
			return false;
		}
		_audio.PlayPredicted(entity.Comp.TryRemoveItemSound, Entity<SecretStashComponent>.op_Implicit(entity), (EntityUid?)userUid, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVariation((float?)0.25f));
		EntityUid? itemInStash = entity.Comp.ItemContainer.ContainedEntity;
		if (!itemInStash.HasValue)
		{
			return false;
		}
		_handsSystem.PickupOrDrop(userUid, itemInStash.Value, checkActionBlocker: true, animateUser: false, animate: true, dropNear: false, handsComp);
		string successMsg = base.Loc.GetString("comp-secret-stash-action-get-item-found-something", (ValueTuple<string, object>)("stashname", GetStashName(entity)));
		_popupSystem.PopupClient(successMsg, Entity<SecretStashComponent>.op_Implicit(entity), userUid);
		return true;
	}

	private void OnGetVerb(Entity<SecretStashComponent> entity, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanInteract || !args.CanAccess || !entity.Comp.HasVerbs)
		{
			return;
		}
		EntityUid user = args.User;
		EntityUid? item = args.Using;
		string stashName = GetStashName(entity);
		InteractionVerb itemVerb = new InteractionVerb();
		if (!IsStashOpen(entity))
		{
			return;
		}
		if (item.HasValue)
		{
			itemVerb.Text = base.Loc.GetString("comp-secret-stash-verb-insert-into-stash");
			if (HasItemInside(entity))
			{
				itemVerb.Disabled = true;
				itemVerb.Message = base.Loc.GetString("comp-secret-stash-verb-insert-message-item-already-inside", (ValueTuple<string, object>)("stashname", stashName));
			}
			else
			{
				itemVerb.Message = base.Loc.GetString("comp-secret-stash-verb-insert-message-no-item", (ValueTuple<string, object>)("item", item), (ValueTuple<string, object>)("stashname", stashName));
			}
			itemVerb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				TryStashItem(entity, user, item.Value);
			};
		}
		else
		{
			itemVerb.Text = base.Loc.GetString("comp-secret-stash-verb-take-out-item");
			itemVerb.Message = base.Loc.GetString("comp-secret-stash-verb-take-out-message-something", (ValueTuple<string, object>)("stashname", stashName));
			if (!HasItemInside(entity))
			{
				itemVerb.Disabled = true;
				itemVerb.Message = base.Loc.GetString("comp-secret-stash-verb-take-out-message-nothing", (ValueTuple<string, object>)("stashname", stashName));
			}
			itemVerb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				TryGetItem(entity, user);
			};
		}
		args.Verbs.Add(itemVerb);
	}

	private string GetStashName(Entity<SecretStashComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.SecretStashName == null)
		{
			return Identity.Name(Entity<SecretStashComponent>.op_Implicit(entity), (IEntityManager)(object)base.EntityManager);
		}
		return base.Loc.GetString(entity.Comp.SecretStashName);
	}

	private bool IsStashOpen(Entity<SecretStashComponent> stash)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _toolOpenableSystem.IsOpen(Entity<SecretStashComponent>.op_Implicit(stash));
	}

	private bool HasItemInside(Entity<SecretStashComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return entity.Comp.ItemContainer.ContainedEntity.HasValue;
	}

	private void DropContentsAndAlert(Entity<SecretStashComponent> entity, EntityCoordinates? cords = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> storedInside = _containerSystem.EmptyContainer((BaseContainer)(object)entity.Comp.ItemContainer, true, cords, true);
		if (storedInside != null && storedInside.Count >= 1)
		{
			string popup = base.Loc.GetString("comp-secret-stash-on-destroyed-popup", (ValueTuple<string, object>)("stashname", GetStashName(entity)));
			_popupSystem.PopupPredicted(popup, storedInside[0], null, PopupType.MediumCaution);
		}
	}
}
