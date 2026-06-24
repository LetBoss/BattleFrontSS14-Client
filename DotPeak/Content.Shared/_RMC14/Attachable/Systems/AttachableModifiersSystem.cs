// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableModifiersSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Item;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Wieldable;
using Content.Shared._RMC14.Wieldable.Components;
using Content.Shared._RMC14.Wieldable.Events;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Item;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableModifiersSystem : EntitySystem
{
  [Dependency]
  private AttachableHolderSystem _attachableHolderSystem;
  [Dependency]
  private CMGunSystem _cmGunSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private ExamineSystemShared _examineSystem;
  [Dependency]
  private RMCSelectiveFireSystem _rmcSelectiveFireSystem;
  [Dependency]
  private RMCWieldableSystem _wieldableSystem;
  private const string modifierExamineColour = "yellow";
  private readonly Dictionary<string, FixedPoint2> _damage = new Dictionary<string, FixedPoint2>();
  [Dependency]
  private ItemSizeChangeSystem _itemSizeChangeSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableComponent, GetVerbsEvent<ExamineVerb>>(new EntityEventRefHandler<AttachableComponent, GetVerbsEvent<ExamineVerb>>(this.OnAttachableGetExamineVerbs));
    this.InitializeMelee();
    this.InitializeRanged();
    this.InitializeSize();
    this.InitializeSpeed();
    this.InitializeWieldDelay();
  }

  private void OnAttachableGetExamineVerbs(
    Entity<AttachableComponent> attachable,
    ref GetVerbsEvent<ExamineVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess)
      return;
    AttachableGetExamineDataEvent args1 = new AttachableGetExamineDataEvent(new Dictionary<byte, (AttachableModifierConditions?, List<string>)>());
    this.RaiseLocalEvent<AttachableGetExamineDataEvent>(attachable.Owner, ref args1);
    FormattedMessage message = new FormattedMessage();
    foreach (byte key in args1.Data.Keys)
    {
      string error;
      message.TryAddMarkup(this.GetExamineConditionText(attachable, args1.Data[key].conditions), out error);
      message.PushNewline();
      foreach (string str in args1.Data[key].effectStrings)
      {
        message.TryAddMarkup("    " + str, out error);
        message.PushNewline();
      }
    }
    if (message.IsEmpty)
      return;
    this._examineSystem.AddDetailedExamineVerb(args, (Component) attachable.Comp, message, this.Loc.GetString("rmc-attachable-examinable-verb-text"), "/Textures/Interface/VerbIcons/information.svg.192dpi.png", this.Loc.GetString("rmc-attachable-examinable-verb-message"));
  }

  private string GetExamineConditionText(
    Entity<AttachableComponent> attachable,
    AttachableModifierConditions? conditions)
  {
    string examineConditionText = this.Loc.GetString("rmc-attachable-examine-condition-always");
    if (!conditions.HasValue)
      return examineConditionText;
    AttachableModifierConditions modifierConditions = conditions.Value;
    bool conditionPlaced = false;
    string conditionText = this.Loc.GetString("rmc-attachable-examine-condition-when") + " ";
    this.ExamineConditionAddEntry(modifierConditions.WieldedOnly, this.Loc.GetString("rmc-attachable-examine-condition-wielded"), ref conditionText, ref conditionPlaced);
    this.ExamineConditionAddEntry(modifierConditions.UnwieldedOnly, this.Loc.GetString("rmc-attachable-examine-condition-unwielded"), ref conditionText, ref conditionPlaced);
    this.ExamineConditionAddEntry(modifierConditions.ActiveOnly, this.Loc.GetString("rmc-attachable-examine-condition-active", (nameof (attachable), (object) attachable.Owner)), ref conditionText, ref conditionPlaced);
    this.ExamineConditionAddEntry(modifierConditions.InactiveOnly, this.Loc.GetString("rmc-attachable-examine-condition-inactive", (nameof (attachable), (object) attachable.Owner)), ref conditionText, ref conditionPlaced);
    if (modifierConditions.Whitelist != null)
    {
      EntityWhitelist whitelist = modifierConditions.Whitelist;
      if (whitelist.Registrations != null)
        this.ExamineConditionAddEntry(modifierConditions.Whitelist != null, this.Loc.GetString("rmc-attachable-examine-condition-whitelist-comps", ("compNumber", whitelist.RequireAll ? (object) "all" : (object) "one"), ("comps", (object) string.Join<ComponentRegistration>(", ", (IEnumerable<ComponentRegistration>) whitelist.Registrations))), ref conditionText, ref conditionPlaced);
      if (whitelist.Sizes != null)
        this.ExamineConditionAddEntry(modifierConditions.Whitelist != null, this.Loc.GetString("rmc-attachable-examine-condition-whitelist-sizes", ("sizes", (object) string.Join<ProtoId<ItemSizePrototype>>(", ", (IEnumerable<ProtoId<ItemSizePrototype>>) whitelist.Sizes))), ref conditionText, ref conditionPlaced);
      if (whitelist.Tags != null)
        this.ExamineConditionAddEntry(modifierConditions.Whitelist != null, this.Loc.GetString("rmc-attachable-examine-condition-whitelist-tags", ("tagNumber", whitelist.RequireAll ? (object) "all" : (object) "one"), ("tags", (object) string.Join<ProtoId<TagPrototype>>(", ", (IEnumerable<ProtoId<TagPrototype>>) whitelist.Tags))), ref conditionText, ref conditionPlaced);
    }
    if (modifierConditions.Blacklist != null && modifierConditions.Blacklist.Tags != null)
    {
      EntityWhitelist blacklist = modifierConditions.Blacklist;
      if (blacklist.Registrations != null)
        this.ExamineConditionAddEntry(modifierConditions.Blacklist != null, this.Loc.GetString("rmc-attachable-examine-condition-blacklist-comps", ("compNumber", blacklist.RequireAll ? (object) "one" : (object) "all"), ("comps", (object) string.Join<ComponentRegistration>(", ", (IEnumerable<ComponentRegistration>) blacklist.Registrations))), ref conditionText, ref conditionPlaced);
      if (blacklist.Sizes != null)
        this.ExamineConditionAddEntry(modifierConditions.Blacklist != null, this.Loc.GetString("rmc-attachable-examine-condition-blacklist-sizes", ("sizes", (object) string.Join<ProtoId<ItemSizePrototype>>(", ", (IEnumerable<ProtoId<ItemSizePrototype>>) blacklist.Sizes))), ref conditionText, ref conditionPlaced);
      if (blacklist.Tags != null)
        this.ExamineConditionAddEntry(modifierConditions.Blacklist != null, this.Loc.GetString("rmc-attachable-examine-condition-blacklist-tags", ("tagNumber", blacklist.RequireAll ? (object) "one" : (object) "all"), ("tags", (object) string.Join<ProtoId<TagPrototype>>(", ", (IEnumerable<ProtoId<TagPrototype>>) blacklist.Tags))), ref conditionText, ref conditionPlaced);
    }
    conditionText += ":";
    return conditionText;
  }

  private void ExamineConditionAddEntry(
    bool condition,
    string text,
    ref string conditionText,
    ref bool conditionPlaced)
  {
    if (!condition)
      return;
    if (conditionPlaced)
      conditionText += "; ";
    conditionText += text;
    conditionPlaced = true;
  }

  private byte GetExamineKey(AttachableModifierConditions? conditions)
  {
    byte examineKey = 0;
    if (!conditions.HasValue)
      return examineKey;
    int num1 = (int) examineKey;
    AttachableModifierConditions modifierConditions = conditions.Value;
    int num2 = modifierConditions.WieldedOnly ? 1 : 0;
    int num3 = (int) (byte) (num1 | num2);
    modifierConditions = conditions.Value;
    int num4 = modifierConditions.UnwieldedOnly ? 2 : 0;
    int num5 = (int) (byte) (num3 | num4);
    modifierConditions = conditions.Value;
    int num6 = modifierConditions.ActiveOnly ? 4 : 0;
    int num7 = (int) (byte) (num5 | num6);
    modifierConditions = conditions.Value;
    int num8 = modifierConditions.InactiveOnly ? 8 : 0;
    int num9 = (int) (byte) (num7 | num8);
    modifierConditions = conditions.Value;
    int num10 = modifierConditions.Whitelist != null ? 16 /*0x10*/ : 0;
    int num11 = (int) (byte) (num9 | num10);
    modifierConditions = conditions.Value;
    int num12 = modifierConditions.Blacklist != null ? 32 /*0x20*/ : 0;
    return (byte) (num11 | num12);
  }

  private bool CanApplyModifiers(EntityUid attachableUid, AttachableModifierConditions? conditions)
  {
    if (!conditions.HasValue)
      return true;
    EntityUid? holderUid;
    this._attachableHolderSystem.TryGetHolder(attachableUid, out holderUid);
    if (holderUid.HasValue)
    {
      WieldableComponent comp;
      this.TryComp<WieldableComponent>(holderUid, out comp);
      if (conditions.Value.UnwieldedOnly && comp != null && comp.Wielded || conditions.Value.WieldedOnly && (comp == null || !comp.Wielded))
        return false;
    }
    AttachableToggleableComponent comp1;
    this.TryComp<AttachableToggleableComponent>(attachableUid, out comp1);
    return (!conditions.Value.InactiveOnly || comp1 == null || !comp1.Active) && (!conditions.Value.ActiveOnly || comp1 != null && comp1.Active) && (!holderUid.HasValue || (conditions.Value.Whitelist == null || !this._whitelistSystem.IsWhitelistFail(conditions.Value.Whitelist, holderUid.Value)) && (conditions.Value.Blacklist == null || !this._whitelistSystem.IsWhitelistPass(conditions.Value.Blacklist, holderUid.Value)));
  }

  private void InitializeMelee()
  {
    this.SubscribeLocalEvent<AttachableWeaponMeleeModsComponent, AttachableGetExamineDataEvent>(new EntityEventRefHandler<AttachableWeaponMeleeModsComponent, AttachableGetExamineDataEvent>(this.OnMeleeModsGetExamineData));
    this.SubscribeLocalEvent<AttachableWeaponMeleeModsComponent, AttachableRelayedEvent<MeleeHitEvent>>(new EntityEventRefHandler<AttachableWeaponMeleeModsComponent, AttachableRelayedEvent<MeleeHitEvent>>(this.OnMeleeModsHitEvent));
  }

  private void OnMeleeModsGetExamineData(
    Entity<AttachableWeaponMeleeModsComponent> attachable,
    ref AttachableGetExamineDataEvent args)
  {
    foreach (AttachableWeaponMeleeModifierSet modifier in attachable.Comp.Modifiers)
    {
      byte examineKey = this.GetExamineKey(modifier.Conditions);
      if (!args.Data.ContainsKey(examineKey))
        args.Data[examineKey] = (modifier.Conditions, this.GetEffectStrings(modifier));
      else
        args.Data[examineKey].effectStrings.AddRange((IEnumerable<string>) this.GetEffectStrings(modifier));
    }
  }

  private List<string> GetEffectStrings(AttachableWeaponMeleeModifierSet modSet)
  {
    List<string> effectStrings = new List<string>();
    if (modSet.BonusDamage != null)
    {
      FixedPoint2 total = modSet.BonusDamage.GetTotal();
      if (total != 0)
        effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-melee-damage", ("colour", (object) "yellow"), ("sign", total > 0 ? (object) '+' : (object) ""), ("damage", (object) total)));
    }
    return effectStrings;
  }

  private void OnMeleeModsHitEvent(
    Entity<AttachableWeaponMeleeModsComponent> attachable,
    ref AttachableRelayedEvent<MeleeHitEvent> args)
  {
    foreach (AttachableWeaponMeleeModifierSet modifier in attachable.Comp.Modifiers)
      this.ApplyModifierSet(attachable, modifier, ref args.Args);
  }

  private void ApplyModifierSet(
    Entity<AttachableWeaponMeleeModsComponent> attachable,
    AttachableWeaponMeleeModifierSet modSet,
    ref MeleeHitEvent args)
  {
    if (!this._attachableHolderSystem.TryGetHolder((EntityUid) attachable, out EntityUid? _) || !this.CanApplyModifiers(attachable.Owner, modSet.Conditions))
      return;
    if (modSet.BonusDamage != null)
      args.BonusDamage += modSet.BonusDamage;
    if (!(args.BonusDamage.GetTotal() < FixedPoint2.Zero))
      return;
    this._damage.Clear();
    foreach ((string key3, FixedPoint2 fixedPoint2_4) in args.BonusDamage.DamageDict)
    {
      string key2 = key3;
      FixedPoint2 fixedPoint2_2 = fixedPoint2_4;
      if (!(fixedPoint2_2 > FixedPoint2.Zero))
      {
        FixedPoint2 fixedPoint2_3;
        if (!args.BaseDamage.DamageDict.TryGetValue(key2, out fixedPoint2_3))
          this._damage[key2] = -fixedPoint2_2;
        else if (-fixedPoint2_2 > fixedPoint2_3)
          this._damage[key2] = -fixedPoint2_2 - fixedPoint2_3;
      }
    }
    foreach ((key3, fixedPoint2_4) in this._damage)
    {
      string key4 = key3;
      FixedPoint2 fixedPoint2_5 = fixedPoint2_4;
      args.BonusDamage.DamageDict[key4] = -fixedPoint2_5;
    }
  }

  private void InitializeRanged()
  {
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableAlteredEvent>(this.OnRangedModsAltered));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableGetExamineDataEvent>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableGetExamineDataEvent>(this.OnRangedModsGetExamineData));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModesEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModesEvent>>(this.OnRangedGetFireModes));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModeValuesEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetFireModeValuesEvent>>(this.OnRangedModsGetFireModeValues));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetDamageFalloffEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetDamageFalloffEvent>>(this.OnRangedModsGetDamageFalloff));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetGunDamageModifierEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetGunDamageModifierEvent>>(this.OnRangedModsGetGunDamage));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetWeaponAccuracyEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GetWeaponAccuracyEvent>>(this.OnRangedModsGetWeaponAccuracy));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunGetAmmoSpreadEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunGetAmmoSpreadEvent>>(this.OnRangedModsGetScatterFlat));
    this.SubscribeLocalEvent<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>(new EntityEventRefHandler<AttachableWeaponRangedModsComponent, AttachableRelayedEvent<GunRefreshModifiersEvent>>(this.OnRangedModsRefreshModifiers));
  }

  private void OnRangedModsGetExamineData(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableGetExamineDataEvent args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      byte examineKey = this.GetExamineKey(modifier.Conditions);
      if (!args.Data.ContainsKey(examineKey))
        args.Data[examineKey] = (modifier.Conditions, this.GetEffectStrings(modifier));
      else
        args.Data[examineKey].effectStrings.AddRange((IEnumerable<string>) this.GetEffectStrings(modifier));
    }
  }

  private List<string> GetEffectStrings(AttachableWeaponRangedModifierSet modSet)
  {
    List<string> effectStrings = new List<string>();
    if (modSet.AccuracyAddMult != 0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-accuracy", ("colour", (object) "yellow"), ("sign", modSet.AccuracyAddMult > 0 ? (object) '+' : (object) ""), ("accuracy", (object) modSet.AccuracyAddMult)));
    if (modSet.ScatterFlat != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-scatter", ("colour", (object) "yellow"), ("sign", modSet.ScatterFlat > 0.0 ? (object) '+' : (object) ""), ("scatter", (object) modSet.ScatterFlat)));
    if (modSet.BurstScatterAddMult != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-burst-scatter", ("colour", (object) "yellow"), ("sign", modSet.BurstScatterAddMult > 0.0 ? (object) '+' : (object) ""), ("burstScatterMult", (object) modSet.BurstScatterAddMult)));
    if (modSet.ShotsPerBurstFlat != 0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-shots-per-burst", ("colour", (object) "yellow"), ("sign", modSet.ShotsPerBurstFlat > 0 ? (object) '+' : (object) ""), ("shots", (object) modSet.ShotsPerBurstFlat)));
    if ((double) modSet.FireDelayFlat != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-fire-delay", ("colour", (object) "yellow"), ("sign", (double) modSet.FireDelayFlat > 0.0 ? (object) '+' : (object) ""), ("fireDelay", (object) modSet.FireDelayFlat)));
    if ((double) modSet.RecoilFlat != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-recoil", ("colour", (object) "yellow"), ("sign", (double) modSet.RecoilFlat > 0.0 ? (object) '+' : (object) ""), ("recoil", (object) modSet.RecoilFlat)));
    if (modSet.DamageAddMult != 0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-damage", ("colour", (object) "yellow"), ("sign", modSet.DamageAddMult > 0 ? (object) '+' : (object) ""), ("damage", (object) modSet.DamageAddMult)));
    if ((double) modSet.ProjectileSpeedFlat != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-projectile-speed", ("colour", (object) "yellow"), ("sign", (double) modSet.ProjectileSpeedFlat > 0.0 ? (object) '+' : (object) ""), ("projectileSpeed", (object) modSet.ProjectileSpeedFlat)));
    if (modSet.DamageFalloffAddMult != 0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-damage-falloff", ("colour", (object) "yellow"), ("sign", modSet.DamageFalloffAddMult > 0 ? (object) '+' : (object) ""), ("falloff", (object) modSet.DamageFalloffAddMult)));
    if ((double) modSet.RangeFlat != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-ranged-range", ("colour", (object) "yellow"), ("sign", (double) modSet.RangeFlat > 0.0 ? (object) '+' : (object) ""), ("falloff", (object) modSet.RangeFlat)));
    return effectStrings;
  }

  private void OnRangedModsAltered(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.DetachedDeactivated:
        break;
      case AttachableAlteredType.AppearanceChanged:
        break;
      default:
        this._cmGunSystem.RefreshGunDamageMultiplier((Entity<GunDamageModifierComponent>) args.Holder);
        if (attachable.Comp.FireModeMods != null)
          this._rmcSelectiveFireSystem.RefreshFireModes((Entity<RMCSelectiveFireComponent>) args.Holder, true);
        this._rmcSelectiveFireSystem.RefreshModifiableFireModeValues((Entity<RMCSelectiveFireComponent>) args.Holder);
        break;
    }
  }

  private void OnRangedModsRefreshModifiers(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GunRefreshModifiersEvent> args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
      {
        args.Args.ShotsPerBurst = Math.Max(args.Args.ShotsPerBurst + modifier.ShotsPerBurstFlat, 1);
        args.Args.CameraRecoilScalar = Math.Max(args.Args.CameraRecoilScalar + modifier.RecoilFlat, 0.0f);
        ref GunRefreshModifiersEvent local1 = ref args.Args;
        Angle minAngle = args.Args.MinAngle;
        Angle angle1 = Angle.FromDegrees(Math.Max(((Angle) ref minAngle).Degrees + modifier.ScatterFlat, 0.0));
        local1.MinAngle = angle1;
        ref GunRefreshModifiersEvent local2 = ref args.Args;
        Angle maxAngle = args.Args.MaxAngle;
        Angle angle2 = Angle.FromDegrees(Math.Max(((Angle) ref maxAngle).Degrees + modifier.ScatterFlat, Angle.op_Implicit(args.Args.MinAngle)));
        local2.MaxAngle = angle2;
        args.Args.ProjectileSpeed += modifier.ProjectileSpeedFlat;
        float num = args.Args.Gun.Comp.SelectedMode == SelectiveFire.Burst ? modifier.FireDelayFlat / 2f : modifier.FireDelayFlat;
        float f = (float) (1.0 / (1.0 / (double) args.Args.FireRate + (double) num));
        if (!float.IsInfinity(f))
          args.Args.FireRate = f;
      }
    }
  }

  private void OnRangedGetFireModes(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GetFireModesEvent> args)
  {
    if (attachable.Comp.FireModeMods == null)
      return;
    foreach (AttachableWeaponFireModesModifierSet fireModeMod in attachable.Comp.FireModeMods)
    {
      if (this.CanApplyModifiers(attachable.Owner, fireModeMod.Conditions))
      {
        args.Args.Modes |= fireModeMod.ExtraFireModes;
        args.Args.Set = fireModeMod.SetFireMode;
      }
    }
  }

  private void OnRangedModsGetDamageFalloff(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GetDamageFalloffEvent> args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
      {
        args.Args.FalloffMultiplier += modifier.DamageFalloffAddMult;
        args.Args.Range += modifier.RangeFlat;
      }
    }
  }

  private void OnRangedModsGetGunDamage(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GetGunDamageModifierEvent> args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
        args.Args.Multiplier += modifier.DamageAddMult;
    }
  }

  private void OnRangedModsGetWeaponAccuracy(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GetWeaponAccuracyEvent> args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
      {
        args.Args.AccuracyMultiplier += modifier.AccuracyAddMult;
        args.Args.Range += modifier.RangeFlat;
      }
    }
  }

  private void OnRangedModsGetFireModeValues(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GetFireModeValuesEvent> args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
        args.Args.BurstScatterMult += modifier.BurstScatterAddMult;
    }
  }

  private void OnRangedModsGetScatterFlat(
    Entity<AttachableWeaponRangedModsComponent> attachable,
    ref AttachableRelayedEvent<GunGetAmmoSpreadEvent> args)
  {
    foreach (AttachableWeaponRangedModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
      {
        ref GunGetAmmoSpreadEvent local = ref args.Args;
        local.Spread = Angle.op_Addition(local.Spread, Angle.op_Implicit(Angle.op_Implicit(Angle.FromDegrees(modifier.ScatterFlat)) / 2.0));
        if (Angle.op_Implicit(args.Args.Spread) < 0.0)
          args.Args.Spread = Angle.op_Implicit(0.0f);
      }
    }
  }

  private void InitializeSize()
  {
    this.SubscribeLocalEvent<AttachableSizeModsComponent, AttachableGetExamineDataEvent>(new EntityEventRefHandler<AttachableSizeModsComponent, AttachableGetExamineDataEvent>(this.OnSizeModsGetExamineData));
    this.SubscribeLocalEvent<AttachableSizeModsComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableSizeModsComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
    this.SubscribeLocalEvent<AttachableSizeModsComponent, AttachableRelayedEvent<GetItemSizeModifiersEvent>>(new EntityEventRefHandler<AttachableSizeModsComponent, AttachableRelayedEvent<GetItemSizeModifiersEvent>>(this.OnGetItemSizeModifiers));
  }

  private void OnSizeModsGetExamineData(
    Entity<AttachableSizeModsComponent> attachable,
    ref AttachableGetExamineDataEvent args)
  {
    foreach (AttachableSizeModifierSet modifier in attachable.Comp.Modifiers)
    {
      byte examineKey = this.GetExamineKey(modifier.Conditions);
      if (!args.Data.ContainsKey(examineKey))
        args.Data[examineKey] = (modifier.Conditions, this.GetEffectStrings(modifier));
      else
        args.Data[examineKey].effectStrings.AddRange((IEnumerable<string>) this.GetEffectStrings(modifier));
    }
  }

  private List<string> GetEffectStrings(AttachableSizeModifierSet modSet)
  {
    List<string> effectStrings = new List<string>();
    if (modSet.Size != 0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-size", ("colour", (object) "yellow"), ("sign", modSet.Size > 0 ? (object) '+' : (object) ""), ("size", (object) modSet.Size)));
    return effectStrings;
  }

  private void OnAttachableAltered(
    Entity<AttachableSizeModsComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    if (attachable.Comp.Modifiers.Count == 0)
      return;
    switch (args.Alteration)
    {
      case AttachableAlteredType.DetachedDeactivated:
        break;
      case AttachableAlteredType.AppearanceChanged:
        break;
      default:
        this._itemSizeChangeSystem.RefreshItemSizeModifiers((Entity<ItemSizeChangeComponent>) args.Holder);
        break;
    }
  }

  private void OnGetItemSizeModifiers(
    Entity<AttachableSizeModsComponent> attachable,
    ref AttachableRelayedEvent<GetItemSizeModifiersEvent> args)
  {
    foreach (AttachableSizeModifierSet modifier in attachable.Comp.Modifiers)
    {
      if (!this.CanApplyModifiers(attachable.Owner, modifier.Conditions))
        break;
      args.Args.Size += modifier.Size;
    }
  }

  private void InitializeSpeed()
  {
    this.SubscribeLocalEvent<AttachableSpeedModsComponent, AttachableGetExamineDataEvent>(new EntityEventRefHandler<AttachableSpeedModsComponent, AttachableGetExamineDataEvent>(this.OnSpeedModsGetExamineData));
    this.SubscribeLocalEvent<AttachableSpeedModsComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableSpeedModsComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
    this.SubscribeLocalEvent<AttachableSpeedModsComponent, AttachableRelayedEvent<GetWieldableSpeedModifiersEvent>>(new EntityEventRefHandler<AttachableSpeedModsComponent, AttachableRelayedEvent<GetWieldableSpeedModifiersEvent>>(this.OnGetSpeedModifiers));
  }

  private void OnSpeedModsGetExamineData(
    Entity<AttachableSpeedModsComponent> attachable,
    ref AttachableGetExamineDataEvent args)
  {
    foreach (AttachableSpeedModifierSet modifier in attachable.Comp.Modifiers)
    {
      byte examineKey = this.GetExamineKey(modifier.Conditions);
      if (!args.Data.ContainsKey(examineKey))
        args.Data[examineKey] = (modifier.Conditions, this.GetEffectStrings(modifier));
      else
        args.Data[examineKey].effectStrings.AddRange((IEnumerable<string>) this.GetEffectStrings(modifier));
    }
  }

  private List<string> GetEffectStrings(AttachableSpeedModifierSet modSet)
  {
    List<string> effectStrings = new List<string>();
    if ((double) modSet.Walk != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-speed-walk", ("colour", (object) "yellow"), ("sign", (double) modSet.Walk > 0.0 ? (object) '+' : (object) ""), ("speed", (object) modSet.Walk)));
    if ((double) modSet.Sprint != 0.0)
      effectStrings.Add(this.Loc.GetString("rmc-attachable-examine-speed-sprint", ("colour", (object) "yellow"), ("sign", (double) modSet.Sprint > 0.0 ? (object) '+' : (object) ""), ("speed", (object) modSet.Sprint)));
    return effectStrings;
  }

  private void OnAttachableAltered(
    Entity<AttachableSpeedModsComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.DetachedDeactivated:
        break;
      case AttachableAlteredType.AppearanceChanged:
        break;
      default:
        this._wieldableSystem.RefreshSpeedModifiers((Entity<WieldableSpeedModifiersComponent>) args.Holder);
        break;
    }
  }

  private void OnGetSpeedModifiers(
    Entity<AttachableSpeedModsComponent> attachable,
    ref AttachableRelayedEvent<GetWieldableSpeedModifiersEvent> args)
  {
    foreach (AttachableSpeedModifierSet modifier in attachable.Comp.Modifiers)
      this.ApplyModifierSet(attachable, modifier, ref args.Args);
  }

  private void ApplyModifierSet(
    Entity<AttachableSpeedModsComponent> attachable,
    AttachableSpeedModifierSet modSet,
    ref GetWieldableSpeedModifiersEvent args)
  {
    if (!this.CanApplyModifiers(attachable.Owner, modSet.Conditions))
      return;
    args.Walk += modSet.Walk;
    args.Sprint += modSet.Sprint;
  }

  private void InitializeWieldDelay()
  {
    this.SubscribeLocalEvent<AttachableWieldDelayModsComponent, AttachableGetExamineDataEvent>(new EntityEventRefHandler<AttachableWieldDelayModsComponent, AttachableGetExamineDataEvent>(this.OnWieldDelayModsGetExamineData));
    this.SubscribeLocalEvent<AttachableWieldDelayModsComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableWieldDelayModsComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
    this.SubscribeLocalEvent<AttachableWieldDelayModsComponent, AttachableRelayedEvent<GetWieldDelayEvent>>(new EntityEventRefHandler<AttachableWieldDelayModsComponent, AttachableRelayedEvent<GetWieldDelayEvent>>(this.OnGetWieldDelay));
  }

  private void OnWieldDelayModsGetExamineData(
    Entity<AttachableWieldDelayModsComponent> attachable,
    ref AttachableGetExamineDataEvent args)
  {
    foreach (AttachableWieldDelayModifierSet modifier in attachable.Comp.Modifiers)
    {
      byte examineKey = this.GetExamineKey(modifier.Conditions);
      if (!args.Data.ContainsKey(examineKey))
        args.Data[examineKey] = (modifier.Conditions, this.GetEffectStrings(modifier));
      else
        args.Data[examineKey].effectStrings.AddRange((IEnumerable<string>) this.GetEffectStrings(modifier));
    }
  }

  private List<string> GetEffectStrings(AttachableWieldDelayModifierSet modSet)
  {
    List<string> effectStrings = new List<string>();
    if (modSet.Delay != TimeSpan.Zero)
    {
      List<string> stringList = effectStrings;
      ILocalizationManager loc = this.Loc;
      (string, object)[] valueTupleArray = new (string, object)[3];
      valueTupleArray[0] = ("colour", (object) "yellow");
      TimeSpan delay = modSet.Delay;
      valueTupleArray[1] = ("sign", delay.TotalSeconds > 0.0 ? (object) '+' : (object) "");
      delay = modSet.Delay;
      valueTupleArray[2] = ("delay", (object) delay.TotalSeconds);
      string str = loc.GetString("rmc-attachable-examine-wield-delay", valueTupleArray);
      stringList.Add(str);
    }
    return effectStrings;
  }

  private void OnAttachableAltered(
    Entity<AttachableWieldDelayModsComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.Wielded:
        break;
      case AttachableAlteredType.Unwielded:
        break;
      case AttachableAlteredType.DetachedDeactivated:
        break;
      case AttachableAlteredType.AppearanceChanged:
        break;
      default:
        this._wieldableSystem.RefreshWieldDelay((Entity<WieldDelayComponent>) args.Holder);
        break;
    }
  }

  private void OnGetWieldDelay(
    Entity<AttachableWieldDelayModsComponent> attachable,
    ref AttachableRelayedEvent<GetWieldDelayEvent> args)
  {
    foreach (AttachableWieldDelayModifierSet modifier in attachable.Comp.Modifiers)
      this.ApplyModifierSet(attachable, modifier, ref args.Args);
  }

  private void ApplyModifierSet(
    Entity<AttachableWieldDelayModsComponent> attachable,
    AttachableWieldDelayModifierSet modSet,
    ref GetWieldDelayEvent args)
  {
    if (!this.CanApplyModifiers(attachable.Owner, modSet.Conditions))
      return;
    args.Delay += modSet.Delay;
  }
}
