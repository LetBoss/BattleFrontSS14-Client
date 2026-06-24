using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Medical.Surgery;
using Content.Shared._RMC14.Medical.Surgery.Steps;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Slowing;
using Content.Shared.Alert;
using Content.Shared.Armor;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Preferences;
using Content.Shared.Rounding;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Armor;

public sealed class CMArmorSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private ISerializationManager _serializationManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private ExamineSystemShared _examine;

	private static readonly ProtoId<DamageGroupPrototype> ArmorGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	private static readonly ProtoId<DamageGroupPrototype> BioGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Burn");

	private static readonly int MaxXenoArmor = 55;

	private EntityQuery<RMCAllowSuitStorageUserWhitelistComponent> _rmcAllowSuitStorageUserWhitelistQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_rmcAllowSuitStorageUserWhitelistQuery = ((EntitySystem)this).GetEntityQuery<RMCAllowSuitStorageUserWhitelistComponent>();
		((EntitySystem)this).SubscribeLocalEvent<PlayerSpawnCompleteEvent>((EntityEventHandler<PlayerSpawnCompleteEvent>)OnPlayerSpawnComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, MapInitEvent>((EntityEventRefHandler<CMArmorComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, ComponentRemove>((EntityEventRefHandler<CMArmorComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, DamageModifyEvent>((EntityEventRefHandler<CMArmorComponent, DamageModifyEvent>)OnDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, CMGetArmorEvent>((EntityEventRefHandler<CMArmorComponent, CMGetArmorEvent>)OnGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, InventoryRelayedEvent<CMGetArmorEvent>>((EntityEventRefHandler<CMArmorComponent, InventoryRelayedEvent<CMGetArmorEvent>>)OnGetArmorRelayed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, InventoryRelayedEvent<GetExplosionResistanceEvent>>((EntityEventRefHandler<CMArmorComponent, InventoryRelayedEvent<GetExplosionResistanceEvent>>)OnGetExplosionResistanceRelayed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, GetExplosionResistanceEvent>((EntityEventRefHandler<CMArmorComponent, GetExplosionResistanceEvent>)OnGetExplosionResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, GotEquippedEvent>((EntityEventRefHandler<CMArmorComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<CMArmorComponent, GetVerbsEvent<ExamineVerb>>)OnArmorVerbExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorComponent, ExaminedEvent>((EntityEventRefHandler<CMArmorComponent, ExaminedEvent>)OnArmorExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMHardArmorComponent, InventoryRelayedEvent<HitBySlowingSpitEvent>>((EntityEventRefHandler<CMHardArmorComponent, InventoryRelayedEvent<HitBySlowingSpitEvent>>)OnArmorHitBySlowingSpit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMHardArmorComponent, InventoryRelayedEvent<CMSurgeryCanPerformStepEvent>>((EntityEventRefHandler<CMHardArmorComponent, InventoryRelayedEvent<CMSurgeryCanPerformStepEvent>>)OnArmorCanPerformStep, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, CMSurgeryCanPerformStepEvent>((EntityEventRefHandler<InventoryComponent, CMSurgeryCanPerformStepEvent>)_inventory.RelayEvent<CMSurgeryCanPerformStepEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorUserComponent, DamageModifyEvent>((EntityEventRefHandler<CMArmorUserComponent, DamageModifyEvent>)OnUserDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMArmorPiercingComponent, CMGetArmorPiercingEvent>((EntityEventRefHandler<CMArmorPiercingComponent, CMGetArmorPiercingEvent>)OnPiercingGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, CMGetArmorEvent>((EntityEventRefHandler<InventoryComponent, CMGetArmorEvent>)_inventory.RelayEvent<CMGetArmorEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingBlockBackpackComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<ClothingBlockBackpackComponent, BeingEquippedAttemptEvent>)OnBlockBackpackEquippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingBlockBackpackComponent, InventoryRelayedEvent<RMCEquipAttemptEvent>>((EntityEventRefHandler<ClothingBlockBackpackComponent, InventoryRelayedEvent<RMCEquipAttemptEvent>>)OnBlockBackpackEquipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<ClothingComponent, BeingEquippedAttemptEvent>)OnClothingEquippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCArmorSpeedTierComponent, GotEquippedEvent>((EntityEventRefHandler<RMCArmorSpeedTierComponent, GotEquippedEvent>)OnArmorSpeedTierGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCArmorSpeedTierComponent, GotUnequippedEvent>((EntityEventRefHandler<RMCArmorSpeedTierComponent, GotUnequippedEvent>)OnArmorSpeedTierGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCArmorSpeedTierComponent, InventoryRelayedEvent<RefreshArmorSpeedTierEvent>>((EntityEventRefHandler<RMCArmorSpeedTierComponent, InventoryRelayedEvent<RefreshArmorSpeedTierEvent>>)OnRefreshArmorSpeedTier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RMCEquipAttemptEvent>((EntityEventRefHandler<InventoryComponent, RMCEquipAttemptEvent>)_inventory.RelayEvent<RMCEquipAttemptEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshArmorSpeedTierEvent>((EntityEventRefHandler<InventoryComponent, RefreshArmorSpeedTierEvent>)_inventory.RelayEvent<RefreshArmorSpeedTierEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCAllowSuitStorageUserWhitelistComponent, GotEquippedEvent>((EntityEventRefHandler<RMCAllowSuitStorageUserWhitelistComponent, GotEquippedEvent>)OnAllowSuitStorageUserWhitelistGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCAllowSuitStorageUserWhitelistComponent, GotUnequippedEvent>((EntityEventRefHandler<RMCAllowSuitStorageUserWhitelistComponent, GotUnequippedEvent>)OnAllowSuitStorageUserWhitelistGotUnequipped, (Type[])null, (Type[])null);
	}

	private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inventory = default(InventoryComponent);
		if (!((EntitySystem)this).TryComp<InventoryComponent>(ev.Mob, ref inventory))
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit((ev.Mob, inventory)));
		ContainerSlot slot;
		RMCAllowSuitStorageUserWhitelistComponent whitelist = default(RMCAllowSuitStorageUserWhitelistComponent);
		while (slots.MoveNext(out slot))
		{
			if (_rmcAllowSuitStorageUserWhitelistQuery.TryComp(slot.ContainedEntity, ref whitelist))
			{
				OnAllowSuitStorageWhitelistEquipped(Entity<RMCAllowSuitStorageUserWhitelistComponent>.op_Implicit((slot.ContainedEntity.Value, whitelist)), ev.Mob);
			}
		}
	}

	private void OnMapInit(Entity<CMArmorComponent> armored, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit((Entity<CMArmorComponent>.op_Implicit(armored), armored.Comp)));
	}

	public void UpdateArmorValue(Entity<CMArmorComponent?> armored)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		XenoComponent xeno = default(XenoComponent);
		if (((EntitySystem)this).Resolve<CMArmorComponent>(Entity<CMArmorComponent>.op_Implicit(armored), ref armored.Comp, false) && ((EntitySystem)this).TryComp<XenoComponent>(Entity<CMArmorComponent>.op_Implicit(armored), ref xeno))
		{
			CMGetArmorEvent ev = new CMGetArmorEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
			((EntitySystem)this).RaiseLocalEvent<CMGetArmorEvent>(Entity<CMArmorComponent>.op_Implicit(armored), ref ev, false);
			string armorMessage = ((ev.FrontalArmor == 0 && ev.SideArmor == 0 && armored.Comp.FrontalArmor == 0 && armored.Comp.SideArmor == 0) ? $"{FixedPoint2.New((double)ev.XenoArmor * ev.ArmorModifier)} / {armored.Comp.XenoArmor}" : $"Overall: {FixedPoint2.New((double)ev.XenoArmor * ev.ArmorModifier)} / {armored.Comp.XenoArmor}");
			if (armored.Comp.FrontalArmor != 0 || ev.FrontalArmor != 0)
			{
				armorMessage = $"{armorMessage}\nFrontal: {FixedPoint2.New((double)(ev.XenoArmor + ev.FrontalArmor) * ev.ArmorModifier)} / {armored.Comp.XenoArmor + armored.Comp.FrontalArmor}";
			}
			if (armored.Comp.SideArmor != 0 || ev.SideArmor != 0)
			{
				armorMessage = $"{armorMessage}\nSide: {FixedPoint2.New((double)(ev.XenoArmor + ev.SideArmor) * ev.ArmorModifier)} / {armored.Comp.XenoArmor + armored.Comp.SideArmor}";
			}
			short max = _alerts.GetMaxSeverity(xeno.ArmorAlert);
			int severity = max - ContentHelpers.RoundToLevels((double)ev.XenoArmor * ev.ArmorModifier, MaxXenoArmor, max + 1);
			AlertsSystem alerts = _alerts;
			EntityUid euid = Entity<CMArmorComponent>.op_Implicit(armored);
			ProtoId<AlertPrototype> armorAlert = xeno.ArmorAlert;
			short? severity2 = (short)severity;
			string dynamicMessage = armorMessage;
			alerts.ShowAlert(euid, armorAlert, severity2, null, autoRemove: false, showCooldown: true, dynamicMessage);
		}
	}

	private void OnRemove(Entity<CMArmorComponent> armored, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		XenoComponent xeno = default(XenoComponent);
		if (((EntitySystem)this).TryComp<XenoComponent>(Entity<CMArmorComponent>.op_Implicit(armored), ref xeno))
		{
			_alerts.ClearAlert(Entity<CMArmorComponent>.op_Implicit(armored), xeno.ArmorAlert);
		}
	}

	private void OnDamageModify(Entity<CMArmorComponent> armored, ref DamageModifyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ModifyDamage(Entity<CMArmorComponent>.op_Implicit(armored), ref args);
	}

	private void OnGetArmor(Entity<CMArmorComponent> armored, ref CMGetArmorEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		args.ExplosionArmor += armored.Comp.ExplosionArmor;
		args.FrontalArmor += armored.Comp.FrontalArmor;
		args.SideArmor += armored.Comp.SideArmor;
		if (((EntitySystem)this).HasComp<XenoComponent>(Entity<CMArmorComponent>.op_Implicit(armored)))
		{
			args.XenoArmor += armored.Comp.XenoArmor;
			return;
		}
		args.Melee += armored.Comp.Melee;
		args.Bullet += armored.Comp.Bullet;
		args.Bio += armored.Comp.Bio;
	}

	private void OnGetArmorRelayed(Entity<CMArmorComponent> armored, ref InventoryRelayedEvent<CMGetArmorEvent> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		args.Args.ExplosionArmor += armored.Comp.ExplosionArmor;
		args.Args.FrontalArmor += armored.Comp.FrontalArmor;
		args.Args.SideArmor += armored.Comp.SideArmor;
		if (((EntitySystem)this).HasComp<XenoComponent>(Entity<CMArmorComponent>.op_Implicit(armored)))
		{
			args.Args.XenoArmor += armored.Comp.XenoArmor;
			return;
		}
		args.Args.Melee += armored.Comp.Melee;
		args.Args.Bullet += armored.Comp.Bullet;
		args.Args.Bio += armored.Comp.Bio;
	}

	private void OnGetExplosionResistanceRelayed(Entity<CMArmorComponent> ent, ref InventoryRelayedEvent<GetExplosionResistanceEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		int armor = ent.Comp.ExplosionArmor;
		if (armor > 0)
		{
			float resist = (float)Math.Pow(1.1, (double)armor / 5.0);
			args.Args.DamageCoefficient /= resist;
		}
	}

	private void OnGetExplosionResistance(Entity<CMArmorComponent> armored, ref GetExplosionResistanceEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		int armor = armored.Comp.ExplosionArmor;
		if (armor > 0)
		{
			float resist = (float)Math.Pow(1.1, (double)armor / 5.0);
			args.DamageCoefficient /= resist;
		}
	}

	private void OnGotEquipped(Entity<CMArmorComponent> armored, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<CMArmorUserComponent>(args.Equipee);
	}

	private void OnArmorVerbExamine(EntityUid uid, CMArmorComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanInteract && args.CanAccess && !((EntitySystem)this).HasComp<XenoComponent>(uid))
		{
			FormattedMessage examineMarkup = GetArmorExamine(component);
			_examine.AddDetailedExamineVerb(args, (Component)(object)component, examineMarkup, base.Loc.GetString("armor-examinable-verb-text"), "/Textures/Interface/Actions/actions_fakemindshield.rsi/icon-on.png", base.Loc.GetString("armor-examinable-verb-message"));
		}
	}

	private void OnArmorExamined(Entity<CMArmorComponent> ent, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examined))
		{
			return;
		}
		using (args.PushGroup("CMArmorSystem", -10))
		{
			(string, int)[] obj = new(string, int)[4]
			{
				("rmc-examine-armor-xeno", ent.Comp.XenoArmor),
				("rmc-examine-armor-xeno-frontal", ent.Comp.FrontalArmor),
				("rmc-examine-armor-xeno-side", ent.Comp.SideArmor),
				("rmc-examine-armor-xeno-explosion", ent.Comp.ExplosionArmor)
			};
			StringBuilder examine = new StringBuilder();
			(string, int)[] array = obj;
			for (int i = 0; i < array.Length; i++)
			{
				var (locId, value) = array[i];
				if (value != 0)
				{
					examine.AppendLine(base.Loc.GetString(locId, (ValueTuple<string, object>)("armor", value)));
				}
			}
			if (examine.Length != 0)
			{
				examine.Insert(0, base.Loc.GetString("rmc-examine-armor-xeno-header", (ValueTuple<string, object>)("xeno", ent)) + "\n", 1);
				args.AddMarkup(examine.ToString());
			}
		}
	}

	private void OnArmorHitBySlowingSpit(Entity<CMHardArmorComponent> ent, ref InventoryRelayedEvent<HitBySlowingSpitEvent> args)
	{
		args.Args.Cancelled = true;
	}

	private void OnArmorCanPerformStep(Entity<CMHardArmorComponent> ent, ref InventoryRelayedEvent<CMSurgeryCanPerformStepEvent> args)
	{
		if (args.Args.Invalid == StepInvalidReason.None)
		{
			args.Args.Invalid = StepInvalidReason.Armor;
		}
	}

	private void OnUserDamageModify(Entity<CMArmorUserComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ModifyDamage(Entity<CMArmorUserComponent>.op_Implicit(ent), ref args);
	}

	private void OnPiercingGetArmor(Entity<CMArmorPiercingComponent> piercing, ref CMGetArmorPiercingEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Piercing += piercing.Comp.Amount;
	}

	private void OnBlockBackpackEquippedAttempt(Entity<ClothingBlockBackpackComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (((CancellableEntityEventArgs)args).Cancelled)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.EquipTarget), SlotFlags.BACK);
		ContainerSlot slot;
		while (slots.MoveNext(out slot))
		{
			if (slot.ContainedEntity.HasValue)
			{
				if (!((EntitySystem)this).HasComp<ClothingIgnoreBlockBackpackComponent>(slot.ContainedEntity))
				{
					((CancellableEntityEventArgs)args).Cancel();
					args.Reason = "rmc-block-backpack-cant-other";
				}
				break;
			}
		}
	}

	private void OnBlockBackpackEquipAttempt(Entity<ClothingBlockBackpackComponent> ent, ref InventoryRelayedEvent<RMCEquipAttemptEvent> args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		ref readonly BeingEquippedAttemptEvent ev = ref args.Args.Event;
		if (!((CancellableEntityEventArgs)ev).Cancelled && !((EntitySystem)this).HasComp<ClothingIgnoreBlockBackpackComponent>(args.Args.Event.Equipment) && (ev.SlotFlags & SlotFlags.BACK) != SlotFlags.NONE)
		{
			((CancellableEntityEventArgs)ev).Cancel();
			ev.Reason = "rmc-block-backpack-cant-backpack";
		}
	}

	private void OnClothingEquippedAttempt(Entity<ClothingComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			RMCEquipAttemptEvent ev = new RMCEquipAttemptEvent(args, SlotFlags.All);
			((EntitySystem)this).RaiseLocalEvent<RMCEquipAttemptEvent>(args.EquipTarget, ref ev, false);
		}
	}

	private void ModifyDamage(EntityUid ent, ref DamageModifyEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		CMGetArmorEvent ev = new CMGetArmorEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
		((EntitySystem)this).RaiseLocalEvent<CMGetArmorEvent>(ent, ref ev, false);
		int armorPiercing = args.ArmorPiercing;
		if (args.Tool.HasValue)
		{
			CMGetArmorPiercingEvent piercingEv = new CMGetArmorPiercingEvent(ent);
			((EntitySystem)this).RaiseLocalEvent<CMGetArmorPiercingEvent>(args.Tool.Value, ref piercingEv, false);
			armorPiercing += piercingEv.Piercing;
		}
		CMArmorComponent armorComp = default(CMArmorComponent);
		bool immuneToAP = ((EntitySystem)this).TryComp<CMArmorComponent>(ent, ref armorComp) && armorComp.ImmuneToAP;
		if (((EntitySystem)this).HasComp<XenoComponent>(ent))
		{
			ev.XenoArmor = (int)((double)ev.XenoArmor * ev.ArmorModifier);
			if (!immuneToAP)
			{
				ev.XenoArmor -= armorPiercing;
			}
		}
		else
		{
			ev.Melee = (int)((double)ev.Melee * ev.ArmorModifier);
			ev.Bullet = (int)((double)ev.Bullet * ev.ArmorModifier);
			if (!immuneToAP)
			{
				ev.Melee -= armorPiercing;
			}
			ev.Bullet -= armorPiercing;
			ev.Bio -= armorPiercing;
		}
		EntityUid? origin = args.Origin;
		if (origin.HasValue)
		{
			EntityUid origin2 = origin.GetValueOrDefault();
			MapCoordinates originCoords = _transform.GetMapCoordinates(origin2, (TransformComponent)null);
			MapCoordinates armorCoords = _transform.GetMapCoordinates(ent, (TransformComponent)null);
			if (originCoords.MapId == armorCoords.MapId)
			{
				Angle val = DirectionExtensions.ToWorldAngle(originCoords.Position - armorCoords.Position);
				Direction diff = ((Angle)(ref val)).GetCardinalDir();
				val = _transform.GetWorldRotation(ent);
				Direction dir = ((Angle)(ref val)).GetCardinalDir();
				if (dir == diff)
				{
					ev.XenoArmor += ev.FrontalArmor;
				}
				else
				{
					(Direction, Direction) perpendiculars = diff.GetPerpendiculars();
					if (dir == perpendiculars.Item1 || dir == perpendiculars.Item2)
					{
						ev.XenoArmor += ev.SideArmor;
					}
				}
			}
		}
		RMCArmorModifierComponent mod = ((EntitySystem)this).EnsureComp<RMCArmorModifierComponent>(ent);
		args.Damage = new DamageSpecifier(args.Damage);
		if (!((EntitySystem)this).HasComp<XenoComponent>(ent))
		{
			if (((EntitySystem)this).HasComp<RMCBulletComponent>(args.Tool))
			{
				Resist(args.Damage, ev.Bullet, ArmorGroup, mod.RangedArmorModifier);
			}
			else if (((EntitySystem)this).HasComp<MeleeWeaponComponent>(args.Tool))
			{
				Resist(args.Damage, ev.Melee, ArmorGroup, mod.MeleeArmorModifier);
			}
			Resist(args.Damage, ev.Bio, BioGroup, mod.RangedArmorModifier);
		}
		else
		{
			Resist(args.Damage, ev.XenoArmor, ArmorGroup, mod.RangedArmorModifier);
		}
	}

	private void Resist(DamageSpecifier damage, int armor, ProtoId<DamageGroupPrototype> group, int mult)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		armor = Math.Max(armor, 0);
		if (armor <= 0)
		{
			return;
		}
		double resist = Math.Pow(1.1, (double)armor / 5.0);
		List<string> types = _prototypes.Index<DamageGroupPrototype>(group).DamageTypes;
		foreach (string type in types)
		{
			if (damage.DamageDict.TryGetValue(type, out var amount) && amount > FixedPoint2.Zero)
			{
				damage.DamageDict[type] = amount / resist;
			}
		}
		FixedPoint2 newDamage = damage.GetTotal();
		if (!(newDamage != FixedPoint2.Zero) || !(newDamage < armor * 2))
		{
			return;
		}
		FixedPoint2 damageWithArmor = FixedPoint2.Max(0, newDamage * mult - armor);
		foreach (string type2 in types)
		{
			if (damage.DamageDict.TryGetValue(type2, out var amount2) && amount2 > FixedPoint2.Zero)
			{
				damage.DamageDict[type2] = amount2 * damageWithArmor / (newDamage * mult);
			}
		}
	}

	public void SetArmorPiercing(Entity<CMArmorPiercingComponent> ent, int amount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Amount = amount;
		((EntitySystem)this).Dirty<CMArmorPiercingComponent>(ent, (MetaDataComponent)null);
	}

	public EntProtoId GetArmorVariant(Entity<RMCArmorVariantComponent> ent, ArmorPreference preference)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		RMCArmorVariantComponent comp = ent.Comp;
		EntProtoId equipmentEntityID = comp.DefaultType;
		if (comp.Types.TryGetValue(preference.ToString(), out var equipment))
		{
			equipmentEntityID = equipment;
		}
		if (preference == ArmorPreference.Random)
		{
			System.Random random = new System.Random();
			equipmentEntityID = comp.Types.ElementAt(random.Next(0, comp.Types.Count)).Value;
		}
		return equipmentEntityID;
	}

	private void OnArmorSpeedTierGotEquipped(Entity<RMCArmorSpeedTierComponent> armour, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		RMCArmorSpeedTierUserComponent comp = default(RMCArmorSpeedTierUserComponent);
		((EntitySystem)this).EnsureComp<RMCArmorSpeedTierUserComponent>(args.Equipee, ref comp);
		RefreshArmorSpeedTier(Entity<RMCArmorSpeedTierUserComponent>.op_Implicit((args.Equipee, comp)));
	}

	private void OnArmorSpeedTierGotUnequipped(Entity<RMCArmorSpeedTierComponent> armour, ref GotUnequippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		RMCArmorSpeedTierUserComponent comp = default(RMCArmorSpeedTierUserComponent);
		((EntitySystem)this).EnsureComp<RMCArmorSpeedTierUserComponent>(args.Equipee, ref comp);
		RefreshArmorSpeedTier(Entity<RMCArmorSpeedTierUserComponent>.op_Implicit((args.Equipee, comp)));
	}

	private void RefreshArmorSpeedTier(Entity<RMCArmorSpeedTierUserComponent> user)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		RefreshArmorSpeedTierEvent ev = new RefreshArmorSpeedTierEvent(SlotFlags.WITHOUT_POCKET);
		((EntitySystem)this).RaiseLocalEvent<RefreshArmorSpeedTierEvent>(user.Owner, ref ev, false);
		user.Comp.SpeedTier = ev.SpeedTier;
		float speed = user.Comp.SpeedTier switch
		{
			"light" => 0.483f, 
			"medium" => 0.526f, 
			"heavy" => 0.565f, 
			_ => 0.35f, 
		};
		MobCollisionComponent mobCollision = default(MobCollisionComponent);
		if (((EntitySystem)this).TryComp<MobCollisionComponent>(Entity<RMCArmorSpeedTierUserComponent>.op_Implicit(user), ref mobCollision))
		{
			mobCollision.MinimumSpeedModifier = speed;
			((EntitySystem)this).Dirty(Entity<RMCArmorSpeedTierUserComponent>.op_Implicit(user), (IComponent)(object)mobCollision, (MetaDataComponent)null);
		}
	}

	private void OnRefreshArmorSpeedTier(Entity<RMCArmorSpeedTierComponent> armor, ref InventoryRelayedEvent<RefreshArmorSpeedTierEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Args.SpeedTier = armor.Comp.SpeedTier;
	}

	private void OnAllowSuitStorageUserWhitelistGotEquipped(Entity<RMCAllowSuitStorageUserWhitelistComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OnAllowSuitStorageWhitelistEquipped(ent, args.Equipee);
	}

	private void OnAllowSuitStorageUserWhitelistGotUnequipped(Entity<RMCAllowSuitStorageUserWhitelistComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			AllowSuitStorageComponent comp = ((EntitySystem)this).EnsureComp<AllowSuitStorageComponent>(Entity<RMCAllowSuitStorageUserWhitelistComponent>.op_Implicit(ent));
			comp.Whitelist = _serializationManager.CreateCopy<EntityWhitelist>(ent.Comp.DefaultWhitelist, (ISerializationContext)null, false, true);
			((EntitySystem)this).Dirty(Entity<RMCAllowSuitStorageUserWhitelistComponent>.op_Implicit(ent), (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void OnAllowSuitStorageWhitelistEquipped(Entity<RMCAllowSuitStorageUserWhitelistComponent> ent, EntityUid user)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			EntityPrototype allowed = default(EntityPrototype);
			if (!_entityWhitelist.IsWhitelistPass(ent.Comp.User, user))
			{
				AllowSuitStorageComponent comp = ((EntitySystem)this).EnsureComp<AllowSuitStorageComponent>(Entity<RMCAllowSuitStorageUserWhitelistComponent>.op_Implicit(ent));
				comp.Whitelist = _serializationManager.CreateCopy<EntityWhitelist>(ent.Comp.DefaultWhitelist, (ISerializationContext)null, false, true);
				((EntitySystem)this).Dirty(Entity<RMCAllowSuitStorageUserWhitelistComponent>.op_Implicit(ent), (IComponent)(object)comp, (MetaDataComponent)null);
			}
			else if (_prototypes.TryIndex(EntProtoId<AllowSuitStorageComponent>.op_Implicit(ent.Comp.AllowedWhitelist), ref allowed))
			{
				base.EntityManager.AddComponents(Entity<RMCAllowSuitStorageUserWhitelistComponent>.op_Implicit(ent), allowed, true);
			}
		}
	}

	private FormattedMessage GetArmorExamine(CMArmorComponent armorComponent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage msg = new FormattedMessage();
		msg.AddMarkupOrThrow(base.Loc.GetString("armor-examine"));
		(string, int)[] array = new(string, int)[4]
		{
			(base.Loc.GetString("rmc-armor-melee"), armorComponent.Melee),
			(base.Loc.GetString("rmc-armor-bullet"), armorComponent.Bullet),
			(base.Loc.GetString("rmc-armor-bio"), armorComponent.Bio),
			(base.Loc.GetString("rmc-armor-explosion-armor"), armorComponent.ExplosionArmor)
		};
		for (int i = 0; i < array.Length; i++)
		{
			var (text, value) = array[i];
			if (value != 0)
			{
				msg.PushNewline();
				msg.AddMarkupOrThrow(base.Loc.GetString("rmc-examine-armor", (ValueTuple<string, object>)("text", text), (ValueTuple<string, object>)("value", value)));
			}
		}
		if (armorComponent.ImmuneToAP)
		{
			msg.PushNewline();
			msg.AddMarkupOrThrow(base.Loc.GetString("rmc-examine-armor-piercing-immune"));
		}
		return msg;
	}
}
