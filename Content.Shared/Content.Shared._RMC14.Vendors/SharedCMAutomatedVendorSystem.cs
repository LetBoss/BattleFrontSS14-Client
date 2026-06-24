using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Animations;
using Content.Shared._RMC14.Cryostorage;
using Content.Shared._RMC14.Holiday;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Scaling;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Webbing;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Clothing.Components;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Wall;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vendors;

public abstract class SharedCMAutomatedVendorSystem : EntitySystem
{
	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedCMInventorySystem _cmInventory;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedJobSystem _job;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private SharedStorageSystem _storage;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCAnimationSystem _rmcAnimation;

	[Dependency]
	private SharedRMCHolidaySystem _rmcHoliday;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SquadSystem _squads;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedWebbingSystem _webbing;

	[Dependency]
	private ThrowingSystem _throwingSystem;

	[Dependency]
	private SharedRankSystem _rank;

	public const string SpecialistPoints = "Specialist";

	private readonly Dictionary<EntProtoId, CMVendorEntry> _entries = new Dictionary<EntProtoId, CMVendorEntry>();

	private readonly List<CMVendorEntry> _boxEntries = new List<CMVendorEntry>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MarineScaleChangedEvent>((EntityEventRefHandler<MarineScaleChangedEvent>)OnMarineScaleChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, MapInitEvent>((EntityEventRefHandler<CMAutomatedVendorComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, ExaminedEvent>((EntityEventRefHandler<CMAutomatedVendorComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<CMAutomatedVendorComponent, ActivatableUIOpenAttemptEvent>)OnUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, InteractUsingEvent>((EntityEventRefHandler<CMAutomatedVendorComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, RMCAutomatedVendorHackDoAfterEvent>((EntityEventRefHandler<CMAutomatedVendorComponent, RMCAutomatedVendorHackDoAfterEvent>)OnHack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAutomatedVendorComponent, DestructionEventArgs>((EntityEventRefHandler<CMAutomatedVendorComponent, DestructionEventArgs>)OnVendorDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRecentlyVendedComponent, GotEquippedHandEvent>((EntityEventRefHandler<RMCRecentlyVendedComponent, GotEquippedHandEvent>)OnRecentlyGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRecentlyVendedComponent, GotEquippedEvent>((EntityEventRefHandler<RMCRecentlyVendedComponent, GotEquippedEvent>)OnRecentlyGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSpecCryoRefundComponent, EnteredCryostorageEvent>((EntityEventRefHandler<RMCSpecCryoRefundComponent, EnteredCryostorageEvent>)OnSpecEnteredCryostorageEvent, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<CMAutomatedVendorComponent>(((EntitySystem)this).Subs, (object)CMAutomatedVendorUI.Key, (BuiEventSubscriber<CMAutomatedVendorComponent>)delegate(Subscriber<CMAutomatedVendorComponent> subs)
		{
			subs.Event<CMVendorVendBuiMsg>((EntityEventRefHandler<CMAutomatedVendorComponent, CMVendorVendBuiMsg>)OnVendBui);
		});
	}

	private void OnMarineScaleChanged(ref MarineScaleChangedEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<CMAutomatedVendorComponent> vendors = ((EntitySystem)this).EntityQueryEnumerator<CMAutomatedVendorComponent>();
		EntityUid uid = default(EntityUid);
		CMAutomatedVendorComponent vendor = default(CMAutomatedVendorComponent);
		while (vendors.MoveNext(ref uid, ref vendor))
		{
			if (!vendor.Scaling)
			{
				continue;
			}
			bool changed = false;
			foreach (CMVendorSection section in vendor.Sections)
			{
				foreach (CMVendorEntry entry in section.Entries)
				{
					int? multiplier = entry.Multiplier;
					if (!multiplier.HasValue)
					{
						continue;
					}
					int multiplier2 = multiplier.GetValueOrDefault();
					multiplier = entry.Max;
					if (!multiplier.HasValue)
					{
						continue;
					}
					int max = multiplier.GetValueOrDefault();
					if (!entry.Box.HasValue)
					{
						int toAdd = (int)Math.Round(ev.New * (double)multiplier2) - max;
						if (toAdd > 0)
						{
							entry.Amount += toAdd;
							entry.Max += toAdd;
							changed = true;
							AmountUpdated(Entity<CMAutomatedVendorComponent>.op_Implicit((uid, vendor)), entry);
						}
					}
				}
			}
			if (changed)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)vendor, (MetaDataComponent)null);
			}
		}
	}

	private void OnMapInit(Entity<CMAutomatedVendorComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = ((EntitySystem)this).Transform(ent.Owner);
		_entries.Clear();
		_boxEntries.Clear();
		foreach (CMVendorSection section in ent.Comp.Sections)
		{
			foreach (CMVendorEntry entry in section.Entries)
			{
				_entries.TryAdd(entry.Id, entry);
				if (entry.Box.HasValue)
				{
					_boxEntries.Add(entry);
					continue;
				}
				entry.Multiplier = entry.Amount;
				entry.Max = entry.Amount;
				if (!_rmcPlanet.IsOnPlanet(transform))
				{
					continue;
				}
				int? amount = entry.Amount;
				if (!amount.HasValue)
				{
					continue;
				}
				int originalAmount = amount.GetValueOrDefault();
				amount = ent.Comp.RandomUnstockAmount;
				if (amount.HasValue)
				{
					int randomUnstock = amount.GetValueOrDefault();
					if (randomUnstock == -1)
					{
						entry.Amount = _random.Next(1, originalAmount);
					}
					else
					{
						entry.Amount = _random.Next(1, randomUnstock);
					}
				}
				float? randomEmptyChance = ent.Comp.RandomEmptyChance;
				if (randomEmptyChance.HasValue)
				{
					float emptyChance = randomEmptyChance.GetValueOrDefault();
					if (RandomExtensions.Prob(_random, emptyChance))
					{
						entry.Amount = 0;
					}
				}
			}
		}
		foreach (CMVendorEntry boxEntry in _boxEntries)
		{
			EntProtoId? box = boxEntry.Box;
			if (box.HasValue)
			{
				EntProtoId box2 = box.GetValueOrDefault();
				if (_entries.TryGetValue(box2, out CMVendorEntry entry2))
				{
					AmountUpdated(ent, entry2);
				}
			}
		}
		if (_boxEntries.Count > 0)
		{
			((EntitySystem)this).Dirty<CMAutomatedVendorComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnExamined(Entity<CMAutomatedVendorComponent> ent, ref ExaminedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.Examiner), ent.Comp.HackSkill, ent.Comp.HackSkillLevel) || !ent.Comp.Hackable)
		{
			return;
		}
		using (args.PushGroup("CMAutomatedVendorComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-vending-machine-can-hack"));
		}
	}

	private void OnUIOpenAttempt(Entity<CMAutomatedVendorComponent> vendor, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if (((CancellableEntityEventArgs)args).Cancelled)
		{
			return;
		}
		CMVendorUserComponent vendorUser = default(CMVendorUserComponent);
		RMCVendorUserRechargeComponent recharge = default(RMCVendorUserRechargeComponent);
		if (((EntitySystem)this).TryComp<CMVendorUserComponent>(args.User, ref vendorUser) && ((EntitySystem)this).TryComp<RMCVendorUserRechargeComponent>(args.User, ref recharge))
		{
			int points = (int)Math.Floor((_timing.CurTime - recharge.LastUpdate) / recharge.TimePerUpdate * (double)recharge.PointsPerUpdate);
			if (points > 0)
			{
				vendorUser.Points = Math.Min(recharge.MaxPoints, vendorUser.Points + points);
				recharge.LastUpdate = _timing.CurTime;
				((EntitySystem)this).DirtyEntity(args.User, (MetaDataComponent)null);
			}
		}
		if (((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(args.User))
		{
			return;
		}
		AccessReaderComponent reader = default(AccessReaderComponent);
		if (((EntitySystem)this).TryComp<AccessReaderComponent>(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), ref reader) && reader.Enabled && reader.AccessLists.Count > 0)
		{
			IdCardOwnerComponent owner = default(IdCardOwnerComponent);
			foreach (EntityUid item in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(args.User)))
			{
				if (((EntitySystem)this).HasComp<IdCardComponent>(item) && ((EntitySystem)this).TryComp<IdCardOwnerComponent>(item, ref owner) && owner.Id != args.User)
				{
					_popup.PopupClient(base.Loc.GetString("cm-vending-machine-wrong-card"), Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), args.User);
					((CancellableEntityEventArgs)args).Cancel();
					return;
				}
			}
		}
		if (!TryAuthorizeVendorUse(vendor, args.User, vendorUser))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnInteractUsing(Entity<CMAutomatedVendorComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<MultitoolComponent>(args.Used))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!ent.Comp.Hackable)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vending-machine-cannot-hack", (ValueTuple<string, object>)("vendor", ent)), Entity<CMAutomatedVendorComponent>.op_Implicit(ent), args.User);
			return;
		}
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.HackSkill, ent.Comp.HackSkillLevel))
		{
			string msg = base.Loc.GetString("rmc-vending-machine-hack-no-skill", (ValueTuple<string, object>)("vendor", ent));
			_popup.PopupClient(msg, Entity<CMAutomatedVendorComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
			return;
		}
		TimeSpan delay = ent.Comp.HackDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.HackSkill);
		RMCAutomatedVendorHackDoAfterEvent ev = new RMCAutomatedVendorHackDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, delay, ev, Entity<CMAutomatedVendorComponent>.op_Implicit(ent), Entity<CMAutomatedVendorComponent>.op_Implicit(ent), args.Used);
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-vending-machine-hack-start", (ValueTuple<string, object>)("vendor", ent)), Entity<CMAutomatedVendorComponent>.op_Implicit(ent), args.User);
		}
	}

	private void OnHack(Entity<CMAutomatedVendorComponent> ent, ref RMCAutomatedVendorHackDoAfterEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ent.Comp.Hacked = !ent.Comp.Hacked;
			((EntitySystem)this).Dirty<CMAutomatedVendorComponent>(ent, (MetaDataComponent)null);
			string msg = (ent.Comp.Hacked ? base.Loc.GetString("rmc-vending-machine-hack-finish-remove", (ValueTuple<string, object>)("vendor", ent)) : base.Loc.GetString("rmc-vending-machine-hack-finish-restore", (ValueTuple<string, object>)("vendor", ent)));
			_popup.PopupClient(msg, Entity<CMAutomatedVendorComponent>.op_Implicit(ent), args.User);
			AccessReaderComponent accessReader = default(AccessReaderComponent);
			if (((EntitySystem)this).TryComp<AccessReaderComponent>(Entity<CMAutomatedVendorComponent>.op_Implicit(ent), ref accessReader))
			{
				List<ProtoId<AccessLevelPrototype>> access = (List<ProtoId<AccessLevelPrototype>>)(ent.Comp.Hacked ? ((IList)new List<ProtoId<AccessLevelPrototype>>()) : ((IList)ent.Comp.Access));
				_accessReader.SetAccesses(Entity<AccessReaderComponent>.op_Implicit((Entity<CMAutomatedVendorComponent>.op_Implicit(ent), accessReader)), access);
			}
		}
	}

	private void OnVendorDestruction(Entity<CMAutomatedVendorComponent> vendor, ref DestructionEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (vendor.Comp.EjectContentsOnDestruction)
		{
			EjectAllVendorContents(vendor);
		}
	}

	private void EjectAllVendorContents(Entity<CMAutomatedVendorComponent> vendor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		foreach (var availableInventoryWithAmount in GetAvailableInventoryWithAmounts(vendor.Comp))
		{
			EntProtoId itemId = availableInventoryWithAmount.Id;
			int amount = availableInventoryWithAmount.Amount;
			for (int i = 0; i < amount; i++)
			{
				EntityCoordinates coords = ((EntitySystem)this).Transform(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor)).Coordinates;
				EntityUid spawnedItem = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(itemId), coords);
				Vector2 direction = new Vector2(_random.NextFloat(-1f, 1f), _random.NextFloat(-1f, 1f));
				float throwForce = _random.NextFloat(1f, 7f);
				_throwingSystem.TryThrow(spawnedItem, direction, throwForce);
			}
		}
	}

	private List<(EntProtoId Id, int Amount)> GetAvailableInventoryWithAmounts(CMAutomatedVendorComponent component)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		List<(EntProtoId, int)> inventory = new List<(EntProtoId, int)>();
		foreach (CMVendorSection section in component.Sections)
		{
			foreach (CMVendorEntry entry in section.Entries)
			{
				if (entry.Amount > 0)
				{
					inventory.Add((entry.Id, entry.Amount.Value));
				}
			}
		}
		return inventory;
	}

	private void OnRecentlyGotEquipped<T>(Entity<RMCRecentlyVendedComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<WallMountComponent>(Entity<RMCRecentlyVendedComponent>.op_Implicit(ent));
	}

	protected virtual void OnVendBui(Entity<CMAutomatedVendorComponent> vendor, ref CMVendorVendBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0967: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
		_audio.PlayPredicted(vendor.Comp.Sound, Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, (AudioParams?)null);
		_rmcAnimation.TryFlick(Entity<RMCAnimationComponent>.op_Implicit(vendor.Owner), vendor.Comp.AnimationSprite, vendor.Comp.BaseSprite);
		if (_net.IsClient)
		{
			return;
		}
		CMAutomatedVendorComponent comp = vendor.Comp;
		int sections = comp.Sections.Count;
		EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
		CMVendorUserComponent user = ((EntitySystem)this).CompOrNull<CMVendorUserComponent>(actor);
		if (!TryAuthorizeVendorUse(vendor, actor, user))
		{
			return;
		}
		if (args.Section < 0 || args.Section >= sections)
		{
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} sent an invalid vend section: {args.Section}. Max: {sections}");
			return;
		}
		CMVendorSection section = comp.Sections[args.Section];
		int entries = section.Entries.Count;
		if (args.Entry < 0 || args.Entry >= entries)
		{
			((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} sent an invalid vend entry: {args.Entry}. Max: {entries}");
			return;
		}
		CMVendorEntry entry = section.Entries[args.Entry];
		int? amount = entry.Amount;
		if (amount.HasValue && amount.GetValueOrDefault() <= 0)
		{
			return;
		}
		EntityPrototype entity = default(EntityPrototype);
		if (!_prototypes.TryIndex(entry.Id, ref entity))
		{
			((EntitySystem)this).Log.Error($"Tried to vend non-existent entity: {entry.Id}");
			return;
		}
		string takeAll = section.TakeAll;
		if (takeAll != null)
		{
			user = ((EntitySystem)this).EnsureComp<CMVendorUserComponent>(actor);
			if (!user.TakeAll.Add((takeAll, entry.Id)))
			{
				((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to buy too many take-alls.");
				return;
			}
			((EntitySystem)this).Dirty(actor, (IComponent)(object)user, (MetaDataComponent)null);
		}
		string takeOne = section.TakeOne;
		if (takeOne != null)
		{
			user = ((EntitySystem)this).EnsureComp<CMVendorUserComponent>(actor);
			if (!user.TakeOne.Add(takeOne))
			{
				((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to buy too many take-ones.");
				return;
			}
			((EntitySystem)this).Dirty(actor, (IComponent)(object)user, (MetaDataComponent)null);
		}
		bool validJob = true;
		if (_mind.TryGetMind(((BaseBoundUserInterfaceEvent)args).Actor, out EntityUid mindId, out MindComponent _))
		{
			foreach (ProtoId<JobPrototype> job in section.Jobs)
			{
				if (!_job.MindHasJobWithId(mindId, job.Id))
				{
					validJob = false;
					continue;
				}
				validJob = true;
				break;
			}
		}
		bool validRank = true;
		foreach (ProtoId<RankPrototype> rank in section.Ranks)
		{
			RankPrototype userRank = _rank.GetRank(actor);
			if (userRank == null || rank != ProtoId<RankPrototype>.op_Implicit(userRank))
			{
				validRank = false;
				continue;
			}
			validRank = true;
			break;
		}
		if (!validJob || !validRank)
		{
			return;
		}
		bool validHoliday = section.Holidays.Count == 0;
		foreach (string holiday in section.Holidays)
		{
			if (_rmcHoliday.IsActiveHoliday(holiday))
			{
				validHoliday = true;
			}
		}
		if (!validHoliday)
		{
			return;
		}
		(string, int)? choices = section.Choices;
		if (choices.HasValue)
		{
			(string, int) choices2 = choices.GetValueOrDefault();
			user = ((EntitySystem)this).EnsureComp<CMVendorUserComponent>(actor);
			if (!user.Choices.TryGetValue(choices2.Item1, out var playerChoices))
			{
				playerChoices = 0;
				user.Choices[choices2.Item1] = playerChoices;
				((EntitySystem)this).Dirty(actor, (IComponent)(object)user, (MetaDataComponent)null);
			}
			if (playerChoices >= choices2.Item2)
			{
				((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to buy too many choices.");
				return;
			}
			playerChoices = (user.Choices[choices2.Item1] = playerChoices + 1);
			((EntitySystem)this).Dirty(actor, (IComponent)(object)user, (MetaDataComponent)null);
		}
		amount = section.SharedSpecLimit;
		if (amount.HasValue)
		{
			amount.GetValueOrDefault();
			if (!((EntitySystem)this).HasComp<IgnoreSpecLimitsComponent>(actor) && ((EntitySystem)this).HasComp<RMCVendorSpecialistComponent>(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor)))
			{
				RMCVendorSpecialistComponent thisSpecVendor = ((EntitySystem)this).Comp<RMCVendorSpecialistComponent>(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor));
				if (thisSpecVendor.GlobalSharedVends.TryGetValue(args.Entry, out var vendCount) && vendCount >= section.SharedSpecLimit)
				{
					ResetChoices();
					_popup.PopupEntity(base.Loc.GetString("cm-vending-machine-specialist-max"), Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), actor);
					return;
				}
				EntityQueryEnumerator<RMCVendorSpecialistComponent> specVendors = ((EntitySystem)this).EntityQueryEnumerator<RMCVendorSpecialistComponent>();
				int maxAmongVendors = 0;
				if (thisSpecVendor.GlobalSharedVends.TryGetValue(args.Entry, out vendCount))
				{
					maxAmongVendors = vendCount;
				}
				EntityUid vendorId = default(EntityUid);
				RMCVendorSpecialistComponent rMCVendorSpecialistComponent = default(RMCVendorSpecialistComponent);
				while (specVendors.MoveNext(ref vendorId, ref rMCVendorSpecialistComponent))
				{
					RMCVendorSpecialistComponent specVendorComponent = ((EntitySystem)this).EnsureComp<RMCVendorSpecialistComponent>(vendorId);
					foreach (int linkedEntry in args.LinkedEntries)
					{
						specVendorComponent.GlobalSharedVends.TryGetValue(linkedEntry, out var linkedCount);
						maxAmongVendors += linkedCount;
					}
					if (specVendorComponent.GlobalSharedVends.TryGetValue(args.Entry, out vendCount))
					{
						if (vendCount > maxAmongVendors)
						{
							maxAmongVendors = specVendorComponent.GlobalSharedVends[args.Entry];
						}
						else
						{
							specVendorComponent.GlobalSharedVends[args.Entry] = maxAmongVendors;
						}
					}
					else
					{
						specVendorComponent.GlobalSharedVends.Add(args.Entry, maxAmongVendors);
					}
					((EntitySystem)this).Dirty(vendorId, (IComponent)(object)specVendorComponent, (MetaDataComponent)null);
				}
				thisSpecVendor.GlobalSharedVends[args.Entry] = maxAmongVendors;
				if (thisSpecVendor.GlobalSharedVends[args.Entry] >= section.SharedSpecLimit)
				{
					ResetChoices();
					_popup.PopupEntity(base.Loc.GetString("cm-vending-machine-specialist-max"), vendor.Owner, actor);
					return;
				}
				thisSpecVendor.GlobalSharedVends[args.Entry]++;
				((EntitySystem)this).Dirty(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), (IComponent)(object)thisSpecVendor, (MetaDataComponent)null);
				((EntitySystem)this).AddComp<RMCSpecCryoRefundComponent>(actor, new RMCSpecCryoRefundComponent
				{
					Vendor = Entity<CMAutomatedVendorComponent>.op_Implicit(vendor),
					Entry = args.Entry
				}, true);
			}
		}
		if (entry.Points.HasValue)
		{
			if (user == null)
			{
				((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} tried to buy {entry.Id} for {entry.Points} points without having points.");
				return;
			}
			int userPoints = ((vendor.Comp.PointsType == null) ? user.Points : (user.ExtraPoints?.GetValueOrDefault(vendor.Comp.PointsType) ?? 0));
			if (userPoints < entry.Points)
			{
				((EntitySystem)this).Log.Error($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor))} with {user.Points} tried to buy {entry.Id} for {entry.Points} points without having enough points.");
				return;
			}
			if (vendor.Comp.PointsType == null)
			{
				user.Points -= entry.Points.Value;
			}
			else if (user.ExtraPoints != null)
			{
				user.ExtraPoints[vendor.Comp.PointsType] = userPoints - entry.Points.GetValueOrDefault();
			}
			((EntitySystem)this).Dirty(actor, (IComponent)(object)user, (MetaDataComponent)null);
		}
		if (entry.Amount.HasValue)
		{
			EntProtoId? box = entry.Box;
			if (box.HasValue)
			{
				EntProtoId box2 = box.GetValueOrDefault();
				bool foundEntry = false;
				foreach (CMVendorSection section2 in vendor.Comp.Sections)
				{
					foreach (CMVendorEntry vendorEntry in section2.Entries)
					{
						if (!(vendorEntry.Id != box2))
						{
							vendorEntry.Amount -= GetBoxRemoveAmount(entry);
							((EntitySystem)this).Dirty<CMAutomatedVendorComponent>(vendor, (MetaDataComponent)null);
							AmountUpdated(vendor, vendorEntry);
							foundEntry = true;
							break;
						}
					}
					if (foundEntry)
					{
						break;
					}
				}
			}
			else
			{
				entry.Amount--;
				((EntitySystem)this).Dirty<CMAutomatedVendorComponent>(vendor, (MetaDataComponent)null);
				AmountUpdated(vendor, entry);
			}
		}
		if (entry.GiveSquadRoleName.HasValue || entry.GiveIcon != null)
		{
			RMCVendorRoleOverrideComponent overrideComp = ((EntitySystem)this).EnsureComp<RMCVendorRoleOverrideComponent>(actor);
			overrideComp.GiveSquadRoleName = entry.GiveSquadRoleName;
			overrideComp.IsAppendSquadRoleName = entry.IsAppendSquadRoleName;
			overrideComp.GiveIcon = entry.GiveIcon;
			((EntitySystem)this).Dirty(actor, (IComponent)(object)overrideComp, (MetaDataComponent)null);
			_squads.UpdateSquadTitle(actor);
		}
		if (entry.GiveMapBlip != null)
		{
			MapBlipIconOverrideComponent mapBlip = ((EntitySystem)this).EnsureComp<MapBlipIconOverrideComponent>(actor);
			mapBlip.Icon = entry.GiveMapBlip;
			((EntitySystem)this).Dirty(actor, (IComponent)(object)mapBlip, (MetaDataComponent)null);
		}
		if (entry.GivePrefix.HasValue)
		{
			JobPrefixComponent jobPrefix = ((EntitySystem)this).EnsureComp<JobPrefixComponent>(actor);
			if (entry.IsAppendPrefix)
			{
				jobPrefix.AdditionalPrefix = entry.GivePrefix;
			}
			else
			{
				jobPrefix.Prefix = entry.GivePrefix.Value;
			}
			((EntitySystem)this).Dirty(actor, (IComponent)(object)jobPrefix, (MetaDataComponent)null);
		}
		Vector2 min = comp.MinOffset;
		Vector2 max = comp.MaxOffset;
		CMVendorBundleComponent bundle = default(CMVendorBundleComponent);
		for (int i = 0; i < entry.Spawn; i++)
		{
			Vector2 offset = _random.NextVector2Box(min.X, min.Y, max.X, max.Y);
			if (entity.TryGetComponent<CMVendorBundleComponent>(ref bundle, _compFactory))
			{
				foreach (EntProtoId bundled in bundle.Bundle)
				{
					Vend(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), actor, bundled, offset, entry.ReplaceSlot);
				}
			}
			else
			{
				Vend(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), actor, entry.Id, offset, entry.ReplaceSlot);
			}
		}
		CMChangeUserOnVendComponent change = default(CMChangeUserOnVendComponent);
		if (entity.TryGetComponent<CMChangeUserOnVendComponent>(ref change, _compFactory) && change.AddComponents != null)
		{
			base.EntityManager.AddComponents(actor, change.AddComponents, true);
		}
		void ResetChoices()
		{
			(string, int)? choices3 = section.Choices;
			if (choices3.HasValue)
			{
				(string, int) choices4 = choices3.GetValueOrDefault();
				if (user != null)
				{
					user.Choices[choices4.Item1]--;
				}
			}
			string takeOne2 = section.TakeOne;
			if (takeOne2 != null && user != null)
			{
				user.TakeOne.Remove(takeOne2);
			}
		}
	}

	private bool TryAuthorizeVendorUse(Entity<CMAutomatedVendorComponent> vendor, EntityUid user, CMVendorUserComponent? vendorUser, bool showPopup = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		CMAutomatedVendorAccessAttemptEvent accessAttempt = new CMAutomatedVendorAccessAttemptEvent(user, showPopup);
		((EntitySystem)this).RaiseLocalEvent<CMAutomatedVendorAccessAttemptEvent>(vendor.Owner, accessAttempt, false);
		if (((CancellableEntityEventArgs)accessAttempt).Cancelled)
		{
			if (showPopup)
			{
				string message = (string.IsNullOrWhiteSpace(accessAttempt.Reason) ? base.Loc.GetString("cm-vending-machine-access-denied") : accessAttempt.Reason);
				_popup.PopupClient(message, Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), user, PopupType.SmallCaution);
			}
			return false;
		}
		if (vendor.Comp.Hacked)
		{
			return true;
		}
		bool validJob = false;
		if (vendor.Comp.Jobs.Count == 0)
		{
			validJob = true;
		}
		else
		{
			_mind.TryGetMind(user, out EntityUid mindId, out MindComponent _);
			foreach (ProtoId<JobPrototype> job in vendor.Comp.Jobs)
			{
				if (((EntityUid)(ref mindId)).Valid && _job.MindHasJobWithId(mindId, job.Id))
				{
					validJob = true;
				}
				else if (vendorUser != null)
				{
					ProtoId<JobPrototype>? id = vendorUser.Id;
					ProtoId<JobPrototype> val = job;
					if (id.HasValue && id.GetValueOrDefault() == val)
					{
						validJob = true;
					}
				}
				if (validJob)
				{
					break;
				}
			}
		}
		bool validRank = false;
		if (vendor.Comp.Ranks.Count == 0)
		{
			validRank = true;
		}
		else
		{
			RankPrototype userRank = _rank.GetRank(user);
			if (userRank != null)
			{
				foreach (ProtoId<RankPrototype> rank in vendor.Comp.Ranks)
				{
					if (ProtoId<RankPrototype>.op_Implicit(userRank) == rank)
					{
						validRank = true;
						break;
					}
				}
			}
		}
		if (validJob && validRank)
		{
			return true;
		}
		if (showPopup)
		{
			_popup.PopupClient(base.Loc.GetString("cm-vending-machine-access-denied"), Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), user);
		}
		return false;
	}

	private void Vend(EntityUid vendor, EntityUid player, EntProtoId toVend, Vector2 offset, SlotFlags? replaceSlot = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		CMVendorMapToSquadComponent mapTo = default(CMVendorMapToSquadComponent);
		if (_prototypes.Index(toVend).TryGetComponent<CMVendorMapToSquadComponent>(ref mapTo, _compFactory))
		{
			SquadMemberComponent member = default(SquadMemberComponent);
			if (((EntitySystem)this).TryComp<SquadMemberComponent>(player, ref member))
			{
				EntityUid? squad = member.Squad;
				if (squad.HasValue)
				{
					EntityUid squad2 = squad.GetValueOrDefault();
					MetaDataComponent obj = ((EntitySystem)this).CompOrNull<MetaDataComponent>(squad2);
					EntityPrototype squadPrototype = ((obj != null) ? obj.EntityPrototype : null);
					if (squadPrototype != null && mapTo.Map.TryGetValue(EntProtoId.op_Implicit(squadPrototype.ID), out var mapped))
					{
						toVend = mapped;
						goto IL_009f;
					}
				}
			}
			EntProtoId? val = mapTo.Default;
			if (!val.HasValue)
			{
				return;
			}
			EntProtoId defaultVend = val.GetValueOrDefault();
			toVend = defaultVend;
		}
		goto IL_009f;
		IL_009f:
		RMCRequisitionsVendorComponent vendorComponent = default(RMCRequisitionsVendorComponent);
		if (((EntitySystem)this).TryComp<RMCRequisitionsVendorComponent>(vendor, ref vendorComponent) && vendorComponent.Enabled && _rmcMap.HasAnchoredEntityEnumerator<RMCRequisitionsChairComponent>(player.ToCoordinates(), out Entity<RMCRequisitionsChairComponent> requisitionsChair, (Direction?)null, (DirectionFlag)0))
		{
			Vector2 itemPlacementOffset = requisitionsChair.Comp.OffsetItem;
			EntityCoordinates val2 = requisitionsChair.Owner.ToCoordinates();
			EntityCoordinates finalPlacementCoordinates = ((EntityCoordinates)(ref val2)).Offset(itemPlacementOffset);
			EntityUid spawn = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(toVend), finalPlacementCoordinates, (ComponentRegistry)null);
			AfterVend(spawn, player, vendor, offset, vended: true, replaceSlot);
		}
		else
		{
			EntityUid spawn2 = ((EntitySystem)this).SpawnNextToOrDrop(EntProtoId.op_Implicit(toVend), vendor, (TransformComponent)null, (ComponentRegistry)null);
			AfterVend(spawn2, player, vendor, offset, vended: false, replaceSlot);
		}
	}

	private void AfterVend(EntityUid spawn, EntityUid player, EntityUid vendor, Vector2 offset, bool vended = false, SlotFlags? replaceSlot = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		RMCRecentlyVendedComponent recently = ((EntitySystem)this).EnsureComp<RMCRecentlyVendedComponent>(spawn);
		RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(spawn, null, (DirectionFlag)0);
		EntityUid uid;
		while (anchored.MoveNext(out uid))
		{
			recently.PreventCollide.Add(uid);
		}
		((EntitySystem)this).Dirty(spawn, (IComponent)(object)recently, (MetaDataComponent)null);
		WallMountComponent mount = ((EntitySystem)this).EnsureComp<WallMountComponent>(spawn);
		mount.Arc = Angle.FromDegrees(360.0);
		((EntitySystem)this).Dirty(spawn, (IComponent)(object)mount, (MetaDataComponent)null);
		TransformComponent xform = default(TransformComponent);
		if (!vended && !Grab(player, spawn, replaceSlot) && ((EntitySystem)this).TryComp(spawn, ref xform))
		{
			_transform.SetLocalPosition(spawn, xform.LocalPosition + offset, xform);
		}
		RMCAutomatedVendedUserEvent ev = new RMCAutomatedVendedUserEvent(spawn, vendor);
		((EntitySystem)this).RaiseLocalEvent<RMCAutomatedVendedUserEvent>(player, ref ev, false);
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(21, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player)), "ToPrettyString(player)");
		handler.AppendLiteral(" vended ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(spawn)), "ToPrettyString(spawn)");
		handler.AppendLiteral(" from vendor ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(vendor)), "ToPrettyString(vendor)");
		adminLog.Add(LogType.RMCVend, ref handler);
	}

	private bool Grab(EntityUid player, EntityUid item, SlotFlags? replaceSlot = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ItemComponent>(item))
		{
			return false;
		}
		if (TryAttachWebbing(player, item))
		{
			return true;
		}
		ClothingComponent clothing = default(ClothingComponent);
		if (!((EntitySystem)this).TryComp<ClothingComponent>(item, ref clothing))
		{
			return _hands.TryPickupAnyHand(player, item);
		}
		if (replaceSlot.HasValue)
		{
			EntityUid? itemToReplace = null;
			InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(player), replaceSlot.Value);
			ContainerSlot slot;
			while (slots.MoveNext(out slot))
			{
				if (slot.ContainedEntity.HasValue)
				{
					itemToReplace = slot.ContainedEntity;
					_inventory.TryUnequip(player, ((BaseContainer)slot).ID, silent: true);
					break;
				}
			}
			if (itemToReplace.HasValue && ((EntitySystem)this).HasComp<StorageComponent>(item) && ((EntitySystem)this).HasComp<StorageComponent>(itemToReplace))
			{
				_storage.TransferEntities(itemToReplace.Value, item);
			}
		}
		if (_cmInventory.TryEquipClothing(player, Entity<ClothingComponent>.op_Implicit((item, clothing)), doRangeCheck: false))
		{
			return true;
		}
		return _hands.TryPickupAnyHand(player, item);
	}

	private bool TryAttachWebbing(EntityUid player, EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<WebbingComponent>(item) && _inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(player), out var enumerator))
		{
			ContainerSlot slot;
			WebbingClothingComponent clothing = default(WebbingClothingComponent);
			while (enumerator.MoveNext(out slot))
			{
				EntityUid? containedEntity = slot.ContainedEntity;
				if (containedEntity.HasValue)
				{
					EntityUid contained = containedEntity.GetValueOrDefault();
					if (((EntitySystem)this).TryComp<WebbingClothingComponent>(contained, ref clothing) && _webbing.Attach(Entity<WebbingClothingComponent>.op_Implicit((contained, clothing)), item, player, out var _))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void SetPoints(Entity<CMVendorUserComponent> user, int points)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		user.Comp.Points = points;
		((EntitySystem)this).Dirty<CMVendorUserComponent>(user, (MetaDataComponent)null);
	}

	public void SetExtraPoints(Entity<CMVendorUserComponent> user, string key, int points)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		CMVendorUserComponent comp = user.Comp;
		if (comp.ExtraPoints == null)
		{
			comp.ExtraPoints = new Dictionary<string, int>();
		}
		user.Comp.ExtraPoints[key] = points;
		((EntitySystem)this).Dirty<CMVendorUserComponent>(user, (MetaDataComponent)null);
	}

	public void AmountUpdated(Entity<CMAutomatedVendorComponent> vendor, CMVendorEntry entry)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		foreach (CMVendorSection section in vendor.Comp.Sections)
		{
			if (!section.HasBoxes)
			{
				continue;
			}
			foreach (CMVendorEntry sectionEntry in section.Entries)
			{
				EntProtoId? box = sectionEntry.Box;
				if (box.HasValue)
				{
					EntProtoId box2 = box.GetValueOrDefault();
					if (!(entry.Id != box2))
					{
						sectionEntry.Amount = entry.Amount / GetBoxRemoveAmount(sectionEntry);
					}
				}
			}
		}
	}

	public void SetSections(Entity<CMAutomatedVendorComponent?> vendor, List<CMVendorSection> sections)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CMAutomatedVendorComponent>(Entity<CMAutomatedVendorComponent>.op_Implicit(vendor), ref vendor.Comp, false))
		{
			vendor.Comp.Sections = sections;
			((EntitySystem)this).Dirty<CMAutomatedVendorComponent>(vendor, (MetaDataComponent)null);
		}
	}

	private int GetBoxRemoveAmount(CMVendorEntry entry)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		int? boxSlots = entry.BoxSlots;
		int boxSlots2;
		if (boxSlots.HasValue)
		{
			boxSlots2 = boxSlots.GetValueOrDefault();
			goto IL_0061;
		}
		EntityPrototype boxProto = default(EntityPrototype);
		CMItemSlotsComponent slots = default(CMItemSlotsComponent);
		if (_prototypes.TryIndex(entry.Id, ref boxProto) && boxProto.TryGetComponent<CMItemSlotsComponent>(ref slots, _compFactory))
		{
			boxSlots = slots.Count;
			if (boxSlots.HasValue)
			{
				int count = boxSlots.GetValueOrDefault();
				boxSlots2 = count;
				goto IL_0061;
			}
		}
		return 1;
		IL_0061:
		int amount = boxSlots2;
		boxSlots = entry.BoxAmount;
		if (boxSlots.HasValue)
		{
			int boxAmount = boxSlots.GetValueOrDefault();
			amount = boxAmount;
		}
		return Math.Max(1, amount);
	}

	private void OnSpecEnteredCryostorageEvent(Entity<RMCSpecCryoRefundComponent> ent, ref EnteredCryostorageEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		RMCVendorSpecialistComponent vendor = default(RMCVendorSpecialistComponent);
		if (((EntitySystem)this).TryComp<RMCVendorSpecialistComponent>(ent.Comp.Vendor, ref vendor) && vendor.GlobalSharedVends.TryGetValue(ent.Comp.Entry, out var current) && current >= 1)
		{
			vendor.GlobalSharedVends[ent.Comp.Entry] = current - 1;
			((EntitySystem)this).Dirty(ent.Comp.Vendor, (IComponent)(object)vendor, (MetaDataComponent)null);
		}
	}
}
