// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.CMArmorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
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
  private static readonly ProtoId<DamageGroupPrototype> ArmorGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  private static readonly ProtoId<DamageGroupPrototype> BioGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  private static readonly int MaxXenoArmor = 55;
  private Robust.Shared.GameObjects.EntityQuery<RMCAllowSuitStorageUserWhitelistComponent> _rmcAllowSuitStorageUserWhitelistQuery;

  public override void Initialize()
  {
    this._rmcAllowSuitStorageUserWhitelistQuery = this.GetEntityQuery<RMCAllowSuitStorageUserWhitelistComponent>();
    this.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawnComplete));
    this.SubscribeLocalEvent<CMArmorComponent, MapInitEvent>(new EntityEventRefHandler<CMArmorComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<CMArmorComponent, ComponentRemove>(new EntityEventRefHandler<CMArmorComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<CMArmorComponent, DamageModifyEvent>(new EntityEventRefHandler<CMArmorComponent, DamageModifyEvent>(this.OnDamageModify));
    this.SubscribeLocalEvent<CMArmorComponent, CMGetArmorEvent>(new EntityEventRefHandler<CMArmorComponent, CMGetArmorEvent>(this.OnGetArmor));
    this.SubscribeLocalEvent<CMArmorComponent, InventoryRelayedEvent<CMGetArmorEvent>>(new EntityEventRefHandler<CMArmorComponent, InventoryRelayedEvent<CMGetArmorEvent>>(this.OnGetArmorRelayed));
    this.SubscribeLocalEvent<CMArmorComponent, InventoryRelayedEvent<GetExplosionResistanceEvent>>(new EntityEventRefHandler<CMArmorComponent, InventoryRelayedEvent<GetExplosionResistanceEvent>>(this.OnGetExplosionResistanceRelayed));
    this.SubscribeLocalEvent<CMArmorComponent, GetExplosionResistanceEvent>(new EntityEventRefHandler<CMArmorComponent, GetExplosionResistanceEvent>(this.OnGetExplosionResistance));
    this.SubscribeLocalEvent<CMArmorComponent, GotEquippedEvent>(new EntityEventRefHandler<CMArmorComponent, GotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<CMArmorComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<CMArmorComponent, GetVerbsEvent<ExamineVerb>>(this.OnArmorVerbExamine));
    this.SubscribeLocalEvent<CMArmorComponent, ExaminedEvent>(new EntityEventRefHandler<CMArmorComponent, ExaminedEvent>(this.OnArmorExamined));
    this.SubscribeLocalEvent<CMHardArmorComponent, InventoryRelayedEvent<HitBySlowingSpitEvent>>(new EntityEventRefHandler<CMHardArmorComponent, InventoryRelayedEvent<HitBySlowingSpitEvent>>(this.OnArmorHitBySlowingSpit));
    this.SubscribeLocalEvent<CMHardArmorComponent, InventoryRelayedEvent<CMSurgeryCanPerformStepEvent>>(new EntityEventRefHandler<CMHardArmorComponent, InventoryRelayedEvent<CMSurgeryCanPerformStepEvent>>(this.OnArmorCanPerformStep));
    this.SubscribeLocalEvent<InventoryComponent, CMSurgeryCanPerformStepEvent>(new EntityEventRefHandler<InventoryComponent, CMSurgeryCanPerformStepEvent>(this._inventory.RelayEvent<CMSurgeryCanPerformStepEvent>));
    this.SubscribeLocalEvent<CMArmorUserComponent, DamageModifyEvent>(new EntityEventRefHandler<CMArmorUserComponent, DamageModifyEvent>(this.OnUserDamageModify));
    this.SubscribeLocalEvent<CMArmorPiercingComponent, CMGetArmorPiercingEvent>(new EntityEventRefHandler<CMArmorPiercingComponent, CMGetArmorPiercingEvent>(this.OnPiercingGetArmor));
    this.SubscribeLocalEvent<InventoryComponent, CMGetArmorEvent>(new EntityEventRefHandler<InventoryComponent, CMGetArmorEvent>(this._inventory.RelayEvent<CMGetArmorEvent>));
    this.SubscribeLocalEvent<ClothingBlockBackpackComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<ClothingBlockBackpackComponent, BeingEquippedAttemptEvent>(this.OnBlockBackpackEquippedAttempt));
    this.SubscribeLocalEvent<ClothingBlockBackpackComponent, InventoryRelayedEvent<RMCEquipAttemptEvent>>(new EntityEventRefHandler<ClothingBlockBackpackComponent, InventoryRelayedEvent<RMCEquipAttemptEvent>>(this.OnBlockBackpackEquipAttempt));
    this.SubscribeLocalEvent<ClothingComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<ClothingComponent, BeingEquippedAttemptEvent>(this.OnClothingEquippedAttempt));
    this.SubscribeLocalEvent<RMCArmorSpeedTierComponent, GotEquippedEvent>(new EntityEventRefHandler<RMCArmorSpeedTierComponent, GotEquippedEvent>(this.OnArmorSpeedTierGotEquipped));
    this.SubscribeLocalEvent<RMCArmorSpeedTierComponent, GotUnequippedEvent>(new EntityEventRefHandler<RMCArmorSpeedTierComponent, GotUnequippedEvent>(this.OnArmorSpeedTierGotUnequipped));
    this.SubscribeLocalEvent<RMCArmorSpeedTierComponent, InventoryRelayedEvent<RefreshArmorSpeedTierEvent>>(new EntityEventRefHandler<RMCArmorSpeedTierComponent, InventoryRelayedEvent<RefreshArmorSpeedTierEvent>>(this.OnRefreshArmorSpeedTier));
    this.SubscribeLocalEvent<InventoryComponent, RMCEquipAttemptEvent>(new EntityEventRefHandler<InventoryComponent, RMCEquipAttemptEvent>(this._inventory.RelayEvent<RMCEquipAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshArmorSpeedTierEvent>(new EntityEventRefHandler<InventoryComponent, RefreshArmorSpeedTierEvent>(this._inventory.RelayEvent<RefreshArmorSpeedTierEvent>));
    this.SubscribeLocalEvent<RMCAllowSuitStorageUserWhitelistComponent, GotEquippedEvent>(new EntityEventRefHandler<RMCAllowSuitStorageUserWhitelistComponent, GotEquippedEvent>(this.OnAllowSuitStorageUserWhitelistGotEquipped));
    this.SubscribeLocalEvent<RMCAllowSuitStorageUserWhitelistComponent, GotUnequippedEvent>(new EntityEventRefHandler<RMCAllowSuitStorageUserWhitelistComponent, GotUnequippedEvent>(this.OnAllowSuitStorageUserWhitelistGotUnequipped));
  }

  private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
  {
    InventoryComponent comp;
    if (!this.TryComp<InventoryComponent>(ev.Mob, out comp))
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) (ev.Mob, comp));
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      RMCAllowSuitStorageUserWhitelistComponent component;
      if (this._rmcAllowSuitStorageUserWhitelistQuery.TryComp(container.ContainedEntity, out component))
        this.OnAllowSuitStorageWhitelistEquipped((Entity<RMCAllowSuitStorageUserWhitelistComponent>) (container.ContainedEntity.Value, component), ev.Mob);
    }
  }

  private void OnMapInit(Entity<CMArmorComponent> armored, ref MapInitEvent args)
  {
    this.UpdateArmorValue((Entity<CMArmorComponent>) ((EntityUid) armored, armored.Comp));
  }

  public void UpdateArmorValue(Entity<CMArmorComponent?> armored)
  {
    XenoComponent comp;
    if (!this.Resolve<CMArmorComponent>((EntityUid) armored, ref armored.Comp, false) || !this.TryComp<XenoComponent>((EntityUid) armored, out comp))
      return;
    CMGetArmorEvent args = new CMGetArmorEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
    this.RaiseLocalEvent<CMGetArmorEvent>((EntityUid) armored, ref args);
    string str1;
    if (args.FrontalArmor != 0 || args.SideArmor != 0 || armored.Comp.FrontalArmor != 0 || armored.Comp.SideArmor != 0)
      str1 = $"Overall: {FixedPoint2.New((double) args.XenoArmor * args.ArmorModifier)} / {armored.Comp.XenoArmor}";
    else
      str1 = $"{FixedPoint2.New((double) args.XenoArmor * args.ArmorModifier)} / {armored.Comp.XenoArmor}";
    string str2 = str1;
    if (armored.Comp.FrontalArmor != 0 || args.FrontalArmor != 0)
      str2 = $"{str2}\nFrontal: {FixedPoint2.New((double) (args.XenoArmor + args.FrontalArmor) * args.ArmorModifier)} / {armored.Comp.XenoArmor + armored.Comp.FrontalArmor}";
    if (armored.Comp.SideArmor != 0 || args.SideArmor != 0)
      str2 = $"{str2}\nSide: {FixedPoint2.New((double) (args.XenoArmor + args.SideArmor) * args.ArmorModifier)} / {armored.Comp.XenoArmor + armored.Comp.SideArmor}";
    short maxSeverity = this._alerts.GetMaxSeverity(comp.ArmorAlert);
    int num = (int) maxSeverity - ContentHelpers.RoundToLevels((double) args.XenoArmor * args.ArmorModifier, (double) CMArmorSystem.MaxXenoArmor, (int) maxSeverity + 1);
    AlertsSystem alerts = this._alerts;
    EntityUid euid = (EntityUid) armored;
    ProtoId<AlertPrototype> armorAlert = comp.ArmorAlert;
    short? severity = new short?((short) num);
    string str3 = str2;
    (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
    string dynamicMessage = str3;
    alerts.ShowAlert(euid, armorAlert, severity, cooldown, dynamicMessage: dynamicMessage);
  }

  private void OnRemove(Entity<CMArmorComponent> armored, ref ComponentRemove args)
  {
    XenoComponent comp;
    if (!this.TryComp<XenoComponent>((EntityUid) armored, out comp))
      return;
    this._alerts.ClearAlert((EntityUid) armored, comp.ArmorAlert);
  }

  private void OnDamageModify(Entity<CMArmorComponent> armored, ref DamageModifyEvent args)
  {
    this.ModifyDamage((EntityUid) armored, ref args);
  }

  private void OnGetArmor(Entity<CMArmorComponent> armored, ref CMGetArmorEvent args)
  {
    args.ExplosionArmor += armored.Comp.ExplosionArmor;
    args.FrontalArmor += armored.Comp.FrontalArmor;
    args.SideArmor += armored.Comp.SideArmor;
    if (this.HasComp<XenoComponent>((EntityUid) armored))
    {
      args.XenoArmor += armored.Comp.XenoArmor;
    }
    else
    {
      args.Melee += armored.Comp.Melee;
      args.Bullet += armored.Comp.Bullet;
      args.Bio += armored.Comp.Bio;
    }
  }

  private void OnGetArmorRelayed(
    Entity<CMArmorComponent> armored,
    ref InventoryRelayedEvent<CMGetArmorEvent> args)
  {
    args.Args.ExplosionArmor += armored.Comp.ExplosionArmor;
    args.Args.FrontalArmor += armored.Comp.FrontalArmor;
    args.Args.SideArmor += armored.Comp.SideArmor;
    if (this.HasComp<XenoComponent>((EntityUid) armored))
    {
      args.Args.XenoArmor += armored.Comp.XenoArmor;
    }
    else
    {
      args.Args.Melee += armored.Comp.Melee;
      args.Args.Bullet += armored.Comp.Bullet;
      args.Args.Bio += armored.Comp.Bio;
    }
  }

  private void OnGetExplosionResistanceRelayed(
    Entity<CMArmorComponent> ent,
    ref InventoryRelayedEvent<GetExplosionResistanceEvent> args)
  {
    int explosionArmor = ent.Comp.ExplosionArmor;
    if (explosionArmor <= 0)
      return;
    float num = (float) Math.Pow(1.1, (double) explosionArmor / 5.0);
    args.Args.DamageCoefficient /= num;
  }

  private void OnGetExplosionResistance(
    Entity<CMArmorComponent> armored,
    ref GetExplosionResistanceEvent args)
  {
    int explosionArmor = armored.Comp.ExplosionArmor;
    if (explosionArmor <= 0)
      return;
    float num = (float) Math.Pow(1.1, (double) explosionArmor / 5.0);
    args.DamageCoefficient /= num;
  }

  private void OnGotEquipped(Entity<CMArmorComponent> armored, ref GotEquippedEvent args)
  {
    this.EnsureComp<CMArmorUserComponent>(args.Equipee);
  }

  private void OnArmorVerbExamine(
    EntityUid uid,
    CMArmorComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || this.HasComp<XenoComponent>(uid))
      return;
    FormattedMessage armorExamine = this.GetArmorExamine(component);
    this._examine.AddDetailedExamineVerb(args, (Component) component, armorExamine, this.Loc.GetString("armor-examinable-verb-text"), "/Textures/Interface/Actions/actions_fakemindshield.rsi/icon-on.png", this.Loc.GetString("armor-examinable-verb-message"));
  }

  private void OnArmorExamined(Entity<CMArmorComponent> ent, ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examined))
      return;
    using (args.PushGroup(nameof (CMArmorSystem), -10))
    {
      (string, int)[] valueTupleArray = new (string, int)[4]
      {
        ("rmc-examine-armor-xeno", ent.Comp.XenoArmor),
        ("rmc-examine-armor-xeno-frontal", ent.Comp.FrontalArmor),
        ("rmc-examine-armor-xeno-side", ent.Comp.SideArmor),
        ("rmc-examine-armor-xeno-explosion", ent.Comp.ExplosionArmor)
      };
      StringBuilder stringBuilder = new StringBuilder();
      foreach ((string messageId, int num) in valueTupleArray)
      {
        if (num != 0)
          stringBuilder.AppendLine(this.Loc.GetString(messageId, ("armor", (object) num)));
      }
      if (stringBuilder.Length == 0)
        return;
      stringBuilder.Insert(0, this.Loc.GetString("rmc-examine-armor-xeno-header", ("xeno", (object) ent)) + "\n", 1);
      args.AddMarkup(stringBuilder.ToString());
    }
  }

  private void OnArmorHitBySlowingSpit(
    Entity<CMHardArmorComponent> ent,
    ref InventoryRelayedEvent<HitBySlowingSpitEvent> args)
  {
    args.Args.Cancelled = true;
  }

  private void OnArmorCanPerformStep(
    Entity<CMHardArmorComponent> ent,
    ref InventoryRelayedEvent<CMSurgeryCanPerformStepEvent> args)
  {
    if (args.Args.Invalid != StepInvalidReason.None)
      return;
    args.Args.Invalid = StepInvalidReason.Armor;
  }

  private void OnUserDamageModify(Entity<CMArmorUserComponent> ent, ref DamageModifyEvent args)
  {
    this.ModifyDamage((EntityUid) ent, ref args);
  }

  private void OnPiercingGetArmor(
    Entity<CMArmorPiercingComponent> piercing,
    ref CMGetArmorPiercingEvent args)
  {
    args.Piercing += piercing.Comp.Amount;
  }

  private void OnBlockBackpackEquippedAttempt(
    Entity<ClothingBlockBackpackComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.EquipTarget, SlotFlags.BACK);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      if (container.ContainedEntity.HasValue)
      {
        if (this.HasComp<ClothingIgnoreBlockBackpackComponent>(container.ContainedEntity))
          break;
        args.Cancel();
        args.Reason = "rmc-block-backpack-cant-other";
        break;
      }
    }
  }

  private void OnBlockBackpackEquipAttempt(
    Entity<ClothingBlockBackpackComponent> ent,
    ref InventoryRelayedEvent<RMCEquipAttemptEvent> args)
  {
    ref readonly BeingEquippedAttemptEvent local = ref args.Args.Event;
    if (local.Cancelled || this.HasComp<ClothingIgnoreBlockBackpackComponent>(args.Args.Event.Equipment) || (local.SlotFlags & SlotFlags.BACK) == SlotFlags.NONE)
      return;
    local.Cancel();
    local.Reason = "rmc-block-backpack-cant-backpack";
  }

  private void OnClothingEquippedAttempt(
    Entity<ClothingComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    RMCEquipAttemptEvent args1 = new RMCEquipAttemptEvent(args, SlotFlags.All);
    this.RaiseLocalEvent<RMCEquipAttemptEvent>(args.EquipTarget, ref args1);
  }

  private void ModifyDamage(EntityUid ent, ref DamageModifyEvent args)
  {
    CMGetArmorEvent args1 = new CMGetArmorEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
    this.RaiseLocalEvent<CMGetArmorEvent>(ent, ref args1);
    int armorPiercing = args.ArmorPiercing;
    if (args.Tool.HasValue)
    {
      CMGetArmorPiercingEvent args2 = new CMGetArmorPiercingEvent(ent);
      this.RaiseLocalEvent<CMGetArmorPiercingEvent>(args.Tool.Value, ref args2);
      armorPiercing += args2.Piercing;
    }
    CMArmorComponent comp;
    bool flag = this.TryComp<CMArmorComponent>(ent, out comp) && comp.ImmuneToAP;
    if (this.HasComp<XenoComponent>(ent))
    {
      args1.XenoArmor = (int) ((double) args1.XenoArmor * args1.ArmorModifier);
      if (!flag)
        args1.XenoArmor -= armorPiercing;
    }
    else
    {
      args1.Melee = (int) ((double) args1.Melee * args1.ArmorModifier);
      args1.Bullet = (int) ((double) args1.Bullet * args1.ArmorModifier);
      if (!flag)
        args1.Melee -= armorPiercing;
      args1.Bullet -= armorPiercing;
      args1.Bio -= armorPiercing;
    }
    EntityUid? origin = args.Origin;
    if (origin.HasValue)
    {
      MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(origin.GetValueOrDefault());
      MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(ent);
      if (mapCoordinates1.MapId == mapCoordinates2.MapId)
      {
        Angle worldAngle = DirectionExtensions.ToWorldAngle(mapCoordinates1.Position - mapCoordinates2.Position);
        Direction cardinalDir1 = ((Angle) ref worldAngle).GetCardinalDir();
        Angle worldRotation = this._transform.GetWorldRotation(ent);
        Direction cardinalDir2 = ((Angle) ref worldRotation).GetCardinalDir();
        if (cardinalDir2 == cardinalDir1)
        {
          args1.XenoArmor += args1.FrontalArmor;
        }
        else
        {
          (Direction First, Direction Second) perpendiculars = cardinalDir1.GetPerpendiculars();
          if (cardinalDir2 == perpendiculars.First || cardinalDir2 == perpendiculars.Second)
            args1.XenoArmor += args1.SideArmor;
        }
      }
    }
    RMCArmorModifierComponent modifierComponent = this.EnsureComp<RMCArmorModifierComponent>(ent);
    args.Damage = new DamageSpecifier(args.Damage);
    if (!this.HasComp<XenoComponent>(ent))
    {
      if (this.HasComp<RMCBulletComponent>(args.Tool))
        this.Resist(args.Damage, args1.Bullet, CMArmorSystem.ArmorGroup, modifierComponent.RangedArmorModifier);
      else if (this.HasComp<MeleeWeaponComponent>(args.Tool))
        this.Resist(args.Damage, args1.Melee, CMArmorSystem.ArmorGroup, modifierComponent.MeleeArmorModifier);
      this.Resist(args.Damage, args1.Bio, CMArmorSystem.BioGroup, modifierComponent.RangedArmorModifier);
    }
    else
      this.Resist(args.Damage, args1.XenoArmor, CMArmorSystem.ArmorGroup, modifierComponent.RangedArmorModifier);
  }

  private void Resist(
    DamageSpecifier damage,
    int armor,
    ProtoId<DamageGroupPrototype> group,
    int mult)
  {
    armor = Math.Max(armor, 0);
    if (armor <= 0)
      return;
    double num = Math.Pow(1.1, (double) armor / 5.0);
    List<string> damageTypes = this._prototypes.Index<DamageGroupPrototype>(group).DamageTypes;
    foreach (string key in damageTypes)
    {
      FixedPoint2 fixedPoint2;
      if (damage.DamageDict.TryGetValue(key, out fixedPoint2) && fixedPoint2 > FixedPoint2.Zero)
        damage.DamageDict[key] = fixedPoint2 / (FixedPoint2) num;
    }
    FixedPoint2 total = damage.GetTotal();
    if (!(total != FixedPoint2.Zero) || !(total < armor * 2))
      return;
    FixedPoint2 fixedPoint2_1 = FixedPoint2.Max((FixedPoint2) 0, total * mult - (FixedPoint2) armor);
    foreach (string key in damageTypes)
    {
      FixedPoint2 fixedPoint2_2;
      if (damage.DamageDict.TryGetValue(key, out fixedPoint2_2) && fixedPoint2_2 > FixedPoint2.Zero)
        damage.DamageDict[key] = fixedPoint2_2 * fixedPoint2_1 / (total * mult);
    }
  }

  public void SetArmorPiercing(Entity<CMArmorPiercingComponent> ent, int amount)
  {
    ent.Comp.Amount = amount;
    this.Dirty<CMArmorPiercingComponent>(ent);
  }

  public EntProtoId GetArmorVariant(
    Entity<RMCArmorVariantComponent> ent,
    ArmorPreference preference)
  {
    RMCArmorVariantComponent comp = ent.Comp;
    EntProtoId armorVariant = comp.DefaultType;
    EntProtoId entProtoId;
    if (comp.Types.TryGetValue(preference.ToString(), out entProtoId))
      armorVariant = entProtoId;
    if (preference == ArmorPreference.Random)
    {
      Random random = new Random();
      armorVariant = comp.Types.ElementAt<KeyValuePair<string, EntProtoId>>(random.Next(0, comp.Types.Count)).Value;
    }
    return armorVariant;
  }

  private void OnArmorSpeedTierGotEquipped(
    Entity<RMCArmorSpeedTierComponent> armour,
    ref GotEquippedEvent args)
  {
    RMCArmorSpeedTierUserComponent comp;
    this.EnsureComp<RMCArmorSpeedTierUserComponent>(args.Equipee, out comp);
    this.RefreshArmorSpeedTier((Entity<RMCArmorSpeedTierUserComponent>) (args.Equipee, comp));
  }

  private void OnArmorSpeedTierGotUnequipped(
    Entity<RMCArmorSpeedTierComponent> armour,
    ref GotUnequippedEvent args)
  {
    RMCArmorSpeedTierUserComponent comp;
    this.EnsureComp<RMCArmorSpeedTierUserComponent>(args.Equipee, out comp);
    this.RefreshArmorSpeedTier((Entity<RMCArmorSpeedTierUserComponent>) (args.Equipee, comp));
  }

  private void RefreshArmorSpeedTier(Entity<RMCArmorSpeedTierUserComponent> user)
  {
    RefreshArmorSpeedTierEvent args = new RefreshArmorSpeedTierEvent(SlotFlags.WITHOUT_POCKET);
    this.RaiseLocalEvent<RefreshArmorSpeedTierEvent>(user.Owner, ref args);
    user.Comp.SpeedTier = args.SpeedTier;
    float num1;
    switch (user.Comp.SpeedTier)
    {
      case "light":
        num1 = 0.483f;
        break;
      case "medium":
        num1 = 0.526f;
        break;
      case "heavy":
        num1 = 0.565f;
        break;
      default:
        num1 = 0.35f;
        break;
    }
    float num2 = num1;
    MobCollisionComponent comp;
    if (!this.TryComp<MobCollisionComponent>((EntityUid) user, out comp))
      return;
    comp.MinimumSpeedModifier = num2;
    this.Dirty((EntityUid) user, (IComponent) comp);
  }

  private void OnRefreshArmorSpeedTier(
    Entity<RMCArmorSpeedTierComponent> armor,
    ref InventoryRelayedEvent<RefreshArmorSpeedTierEvent> args)
  {
    args.Args.SpeedTier = armor.Comp.SpeedTier;
  }

  private void OnAllowSuitStorageUserWhitelistGotEquipped(
    Entity<RMCAllowSuitStorageUserWhitelistComponent> ent,
    ref GotEquippedEvent args)
  {
    this.OnAllowSuitStorageWhitelistEquipped(ent, args.Equipee);
  }

  private void OnAllowSuitStorageUserWhitelistGotUnequipped(
    Entity<RMCAllowSuitStorageUserWhitelistComponent> ent,
    ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    AllowSuitStorageComponent storageComponent = this.EnsureComp<AllowSuitStorageComponent>((EntityUid) ent);
    storageComponent.Whitelist = this._serializationManager.CreateCopy<EntityWhitelist>(ent.Comp.DefaultWhitelist, notNullableOverride: true);
    this.Dirty((EntityUid) ent, (IComponent) storageComponent);
  }

  private void OnAllowSuitStorageWhitelistEquipped(
    Entity<RMCAllowSuitStorageUserWhitelistComponent> ent,
    EntityUid user)
  {
    if (this._timing.ApplyingState)
      return;
    if (!this._entityWhitelist.IsWhitelistPass(ent.Comp.User, user))
    {
      AllowSuitStorageComponent storageComponent = this.EnsureComp<AllowSuitStorageComponent>((EntityUid) ent);
      storageComponent.Whitelist = this._serializationManager.CreateCopy<EntityWhitelist>(ent.Comp.DefaultWhitelist, notNullableOverride: true);
      this.Dirty((EntityUid) ent, (IComponent) storageComponent);
    }
    else
    {
      Robust.Shared.Prototypes.EntityPrototype prototype;
      if (!this._prototypes.TryIndex((EntProtoId) ent.Comp.AllowedWhitelist, out prototype))
        return;
      this.EntityManager.AddComponents((EntityUid) ent, prototype, true);
    }
  }

  private FormattedMessage GetArmorExamine(CMArmorComponent armorComponent)
  {
    FormattedMessage armorExamine = new FormattedMessage();
    armorExamine.AddMarkupOrThrow(this.Loc.GetString("armor-examine"));
    (string, int)[] valueTupleArray = new (string, int)[4]
    {
      (this.Loc.GetString("rmc-armor-melee"), armorComponent.Melee),
      (this.Loc.GetString("rmc-armor-bullet"), armorComponent.Bullet),
      (this.Loc.GetString("rmc-armor-bio"), armorComponent.Bio),
      (this.Loc.GetString("rmc-armor-explosion-armor"), armorComponent.ExplosionArmor)
    };
    foreach ((string str, int num) in valueTupleArray)
    {
      if (num != 0)
      {
        armorExamine.PushNewline();
        armorExamine.AddMarkupOrThrow(this.Loc.GetString("rmc-examine-armor", ("text", (object) str), ("value", (object) num)));
      }
    }
    if (armorComponent.ImmuneToAP)
    {
      armorExamine.PushNewline();
      armorExamine.AddMarkupOrThrow(this.Loc.GetString("rmc-examine-armor-piercing-immune"));
    }
    return armorExamine;
  }
}
