using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Construction;

public sealed class RMCUnfoldCardboardSystem : EntitySystem
{
	[Dependency]
	private SharedCMInventorySystem _cmInventory;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCUnfoldCardboardComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<RMCUnfoldCardboardComponent, GetVerbsEvent<Verb>>)OnGetVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetVerbs(Entity<RMCUnfoldCardboardComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null)
		{
			EntityUid user = args.User;
			Verb v = new Verb
			{
				Priority = 1,
				Text = base.Loc.GetString(LocId.op_Implicit(ent.Comp.VerbText)),
				Impact = LogImpact.Low,
				DoContactInteraction = true,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					UnfoldCardboard(ent, user);
				}
			};
			args.Verbs.Add(v);
		}
	}

	private void UnfoldCardboard(Entity<RMCUnfoldCardboardComponent> ent, EntityUid user)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		BulletBoxComponent bulletBox = default(BulletBoxComponent);
		StorageComponent storage = default(StorageComponent);
		if (_cmInventory.GetItemSlotsFilled(Entity<ItemSlotsComponent>.op_Implicit(ent.Owner)).Filled != 0)
		{
			NotEmptyPopup();
		}
		else if (((EntitySystem)this).TryComp<BulletBoxComponent>(Entity<RMCUnfoldCardboardComponent>.op_Implicit(ent), ref bulletBox) && bulletBox.Amount > 0)
		{
			NotEmptyPopup();
		}
		else if (((EntitySystem)this).TryComp<StorageComponent>(Entity<RMCUnfoldCardboardComponent>.op_Implicit(ent), ref storage) && ((BaseContainer)storage.Container).Count > 0)
		{
			NotEmptyPopup();
		}
		else
		{
			if (!_net.IsServer)
			{
				return;
			}
			foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>)ent.Comp.Spawns, (IRobustRandom?)null))
			{
				EntityUid spawned = ((EntitySystem)this).SpawnNextToOrDrop(spawn, Entity<RMCUnfoldCardboardComponent>.op_Implicit(ent), (TransformComponent)null, (ComponentRegistry)null);
				_stack.TryMergeToHands(spawned, user);
			}
			((EntitySystem)this).Del((EntityUid?)Entity<RMCUnfoldCardboardComponent>.op_Implicit(ent));
		}
		void NotEmptyPopup()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(ent.Comp.FailedNotEmptyText), (ValueTuple<string, object>)("entityName", ent.Owner)), Entity<RMCUnfoldCardboardComponent>.op_Implicit(ent), user);
		}
	}
}
