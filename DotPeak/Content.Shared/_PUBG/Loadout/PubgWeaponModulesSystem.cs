// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgWeaponModulesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Vision;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

public sealed class PubgWeaponModulesSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private UnpoweredFlashlightSystem _flashlight;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, GunGetAmmoSpreadEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, GunGetAmmoSpreadEvent>(this.OnWeaponGetAmmoSpread));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, GunRefreshModifiersEvent>(this.OnGunRefreshModifiers));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, GunMuzzleFlashAttemptEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, GunMuzzleFlashAttemptEvent>(this.OnGunMuzzleFlashAttempt));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, GetItemActionsEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, PubgToggleWeaponLightActionEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, PubgToggleWeaponLightActionEvent>(this.OnToggleWeaponLightAction));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, ExaminedEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, ExaminedEvent>(this.OnWeaponExamined));
    this.SubscribeLocalEvent<PubgWeaponModuleComponent, ExaminedEvent>(new EntityEventRefHandler<PubgWeaponModuleComponent, ExaminedEvent>(this.OnModuleExamined));
  }

  public bool TryGetSlotDefinition(
    EntityUid weapon,
    string slotId,
    out PubgWeaponModuleSlotDefinition slotDefinition,
    PubgWeaponModulesComponent? modules = null)
  {
    slotDefinition = (PubgWeaponModuleSlotDefinition) null;
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return false;
    foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
    {
      if (slotId.Equals(this.GetResolvedContainerId(slot), StringComparison.OrdinalIgnoreCase))
      {
        slotDefinition = slot;
        return true;
      }
    }
    return false;
  }

  public bool TryGetSlotDefinition(
    EntityUid weapon,
    PubgModuleSlotType slotType,
    out PubgWeaponModuleSlotDefinition slotDefinition,
    PubgWeaponModulesComponent? modules = null)
  {
    slotDefinition = (PubgWeaponModuleSlotDefinition) null;
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return false;
    foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
    {
      if (slot.Slot == slotType)
      {
        slotDefinition = slot;
        return true;
      }
    }
    return false;
  }

  public IEnumerable<(PubgWeaponModuleSlotDefinition Slot, EntityUid? Module)> EnumerateInstalledModules(
    EntityUid weapon,
    PubgWeaponModulesComponent? modules = null)
  {
    PubgWeaponModulesSystem weaponModulesSystem = this;
    if (weaponModulesSystem.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
    {
      foreach (PubgWeaponModuleSlotDefinition slot in modules.Slots)
      {
        EntityUid? nullable = new EntityUid?();
        string resolvedContainerId = weaponModulesSystem.GetResolvedContainerId(slot);
        BaseContainer container;
        if (weaponModulesSystem._container.TryGetContainer(weapon, resolvedContainerId, out container) && container is ContainerSlot containerSlot)
          nullable = containerSlot.ContainedEntity;
        yield return (slot, nullable);
      }
    }
  }

  public bool IsModuleAllowedInSlot(
    EntityUid module,
    PubgWeaponModuleSlotDefinition slotDefinition,
    PubgWeaponModuleComponent? moduleComp = null)
  {
    MetaDataComponent comp;
    if (!this.Resolve<PubgWeaponModuleComponent>(module, ref moduleComp, false) || moduleComp.Slot != slotDefinition.Slot || slotDefinition.AllowedModules.Count == 0 || !this.TryComp(module, out comp) || comp.EntityPrototype == null)
      return false;
    string id = comp.EntityPrototype.ID;
    foreach (EntProtoId allowedModule in slotDefinition.AllowedModules)
    {
      if (id.Equals(allowedModule.Id, StringComparison.Ordinal))
        return true;
    }
    return false;
  }

  public float GetSpreadMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 1f;
    float num = 1f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp1;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp1))
      {
        PubgBipodComponent comp2;
        if (this.TryComp<PubgBipodComponent>(Module.Value, out comp2) && comp2.Deployed)
          num *= comp2.DeployedSpreadMultiplier;
        else
          num *= comp1.SpreadMultiplier;
      }
    }
    return Math.Clamp(num, 0.5f, 1.5f);
  }

  public float GetHipfireSpreadMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 1f;
    float num = 1f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp1;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp1))
      {
        PubgBipodComponent comp2;
        if (this.TryComp<PubgBipodComponent>(Module.Value, out comp2) && comp2.Deployed)
          num *= comp2.DeployedHipfireSpreadMultiplier;
        else
          num *= comp1.HipfireSpreadMultiplier;
      }
    }
    return Math.Clamp(num, 0.5f, 1.75f);
  }

  public float GetRecoilMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 1f;
    float num = 1f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp1;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp1))
      {
        PubgBipodComponent comp2;
        if (this.TryComp<PubgBipodComponent>(Module.Value, out comp2) && comp2.Deployed)
          num *= comp2.DeployedRecoilMultiplier;
        else
          num *= comp1.RecoilMultiplier;
      }
    }
    return Math.Clamp(num, 0.25f, 2f);
  }

  public float GetReloadTimeMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 1f;
    float num = 1f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp))
        num *= comp.ReloadTimeMultiplier;
    }
    return Math.Clamp(num, 0.4f, 2f);
  }

  public int GetMagazineCapacityBonus(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 0;
    int num = 0;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp))
        num += comp.MagazineCapacityBonus;
    }
    return Math.Clamp(num, -20, 200);
  }

  public float GetRangeMultiplier(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 1f;
    float num = 1f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp))
        num *= comp.RangeMultiplier;
    }
    return Math.Clamp(num, 0.1f, 10f);
  }

  public float GetFocusBonusTiles(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return 0.0f;
    float num = 0.0f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp))
        num += comp.FocusBonusTiles;
    }
    return Math.Clamp(num, 0.0f, 8f);
  }

  public SoundSpecifier? GetGunshotOverrideSound(
    EntityUid weapon,
    PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return (SoundSpecifier) null;
    SoundSpecifier gunshotOverrideSound = (SoundSpecifier) null;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp) && comp.SoundGunshotOverride != null)
        gunshotOverrideSound = comp.SoundGunshotOverride;
    }
    return gunshotOverrideSound;
  }

  public bool ShouldSuppressMuzzleFlash(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return false;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp) && comp.SuppressMuzzleFlash)
        return true;
    }
    return false;
  }

  public PubgSpatialGunshotModifiers GetSpatialGunshotModifiers(
    EntityUid weapon,
    PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return PubgSpatialGunshotModifiers.Default;
    SoundSpecifier FarSoundOverride = (SoundSpecifier) null;
    bool DisableFarSound = false;
    float num1 = 1f;
    float num2 = 1f;
    float num3 = 1f;
    float num4 = 1f;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      PubgWeaponModuleComponent comp;
      if (Module.HasValue && this.TryComp<PubgWeaponModuleComponent>(Module.Value, out comp))
      {
        if (comp.SpatialFarSoundOverride != null)
          FarSoundOverride = comp.SpatialFarSoundOverride;
        DisableFarSound |= comp.DisableSpatialFarSound;
        num1 *= comp.SpatialAudioRangeMultiplier;
        num2 *= comp.SpatialNearRangeMultiplier;
        num3 *= comp.SpatialConeAngleMultiplier;
        num4 *= comp.SpatialNearVolumeMultiplier;
      }
    }
    return new PubgSpatialGunshotModifiers(FarSoundOverride, DisableFarSound, Math.Clamp(num1, 0.2f, 2f), Math.Clamp(num2, 0.2f, 2f), Math.Clamp(num3, 0.2f, 2f), Math.Clamp(num4, 0.2f, 2f));
  }

  public string GetResolvedContainerId(PubgWeaponModuleSlotDefinition slotDefinition)
  {
    return !string.IsNullOrWhiteSpace(slotDefinition.ContainerId) ? slotDefinition.ContainerId : "pubg_module_" + slotDefinition.Slot.ToString().ToLowerInvariant();
  }

  public bool HasInstalledModuleInSlot(
    EntityUid weapon,
    PubgModuleSlotType slotType,
    PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return false;
    foreach ((PubgWeaponModuleSlotDefinition Slot, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      if (Slot.Slot == slotType && Module.HasValue)
        return true;
    }
    return false;
  }

  public bool TryGetInstalledModule(
    EntityUid weapon,
    PubgModuleSlotType slotType,
    out EntityUid module,
    out PubgWeaponModuleSlotDefinition slotDefinition,
    PubgWeaponModulesComponent? modules = null)
  {
    module = EntityUid.Invalid;
    slotDefinition = (PubgWeaponModuleSlotDefinition) null;
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return false;
    foreach ((PubgWeaponModuleSlotDefinition Slot, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      if (Slot.Slot == slotType && Module.HasValue)
      {
        module = Module.Value;
        slotDefinition = Slot;
        return true;
      }
    }
    return false;
  }

  public bool HasRequiredModulesForReload(EntityUid weapon, PubgWeaponModulesComponent? modules = null)
  {
    if (!this.Resolve<PubgWeaponModulesComponent>(weapon, ref modules, false))
      return true;
    foreach ((PubgWeaponModuleSlotDefinition Slot, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      if (Slot.RequiredForReloading && !Module.HasValue)
        return false;
    }
    return true;
  }

  public static string GetSlotLocKey(PubgModuleSlotType slot)
  {
    string slotLocKey;
    switch (slot)
    {
      case PubgModuleSlotType.Optic:
        slotLocKey = "pubg-loadout-slot-optic";
        break;
      case PubgModuleSlotType.Muzzle:
        slotLocKey = "pubg-loadout-slot-muzzle";
        break;
      case PubgModuleSlotType.Underbarrel:
        slotLocKey = "pubg-loadout-slot-underbarrel";
        break;
      case PubgModuleSlotType.Tactical:
        slotLocKey = "pubg-loadout-slot-tactical";
        break;
      case PubgModuleSlotType.Magazine:
        slotLocKey = "pubg-loadout-slot-magazine";
        break;
      default:
        slotLocKey = "pubg-loadout-slot-custom";
        break;
    }
    return slotLocKey;
  }

  private void OnWeaponGetAmmoSpread(
    Entity<PubgWeaponModulesComponent> ent,
    ref GunGetAmmoSpreadEvent args)
  {
    float spreadMultiplier = this.GetSpreadMultiplier((EntityUid) ent, ent.Comp);
    PubgFocusViewComponent comp;
    if (this.TryComp<PubgFocusViewComponent>((EntityUid) ent, out comp) && !comp.Active)
      spreadMultiplier *= this.GetHipfireSpreadMultiplier((EntityUid) ent, ent.Comp);
    ref GunGetAmmoSpreadEvent local = ref args;
    local.Spread = Angle.op_Implicit(Angle.op_Implicit(local.Spread) * (double) spreadMultiplier);
  }

  private void OnGunRefreshModifiers(
    Entity<PubgWeaponModulesComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    args.CameraRecoilScalar *= this.GetRecoilMultiplier((EntityUid) ent, ent.Comp);
    SoundSpecifier gunshotOverrideSound = this.GetGunshotOverrideSound((EntityUid) ent, ent.Comp);
    if (gunshotOverrideSound == null)
      return;
    args.SoundGunshot = gunshotOverrideSound;
  }

  private void OnGunMuzzleFlashAttempt(
    Entity<PubgWeaponModulesComponent> ent,
    ref GunMuzzleFlashAttemptEvent args)
  {
    if (!this.ShouldSuppressMuzzleFlash((EntityUid) ent, ent.Comp))
      return;
    args.Cancelled = true;
  }

  private void OnGetItemActions(
    Entity<PubgWeaponModulesComponent> ent,
    ref GetItemActionsEvent args)
  {
    bool anyEnabled;
    EntityUid? toggleActionEntity;
    if (this.TryGetInstalledFlashlights((EntityUid) ent, ent.Comp, out anyEnabled))
    {
      args.AddAction(ref ent.Comp.FlashlightToggleActionEntity, ent.Comp.FlashlightToggleAction);
      SharedActionsSystem actions = this._actions;
      toggleActionEntity = ent.Comp.FlashlightToggleActionEntity;
      Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
      int num = anyEnabled ? 1 : 0;
      actions.SetToggled(action, num != 0);
    }
    EntityUid module;
    PubgBipodComponent comp;
    if (!this.TryGetInstalledModule((EntityUid) ent, PubgModuleSlotType.Underbarrel, out module, out PubgWeaponModuleSlotDefinition _, ent.Comp) || !this.TryComp<PubgBipodComponent>(module, out comp))
      return;
    args.AddAction(ref ent.Comp.BipodToggleActionEntity, ent.Comp.BipodToggleAction);
    SharedActionsSystem actions1 = this._actions;
    toggleActionEntity = ent.Comp.BipodToggleActionEntity;
    Entity<ActionComponent>? action1 = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num1 = comp.Deployed ? 1 : 0;
    actions1.SetToggled(action1, num1 != 0);
  }

  private void OnToggleWeaponLightAction(
    Entity<PubgWeaponModulesComponent> ent,
    ref PubgToggleWeaponLightActionEvent args)
  {
    HandsComponent comp1;
    if (args.Handled || !this.TryComp<HandsComponent>(args.Performer, out comp1) || !this._hands.IsHolding((Entity<HandsComponent>) (args.Performer, comp1), new EntityUid?(ent.Owner)))
      return;
    List<EntityUid> entityUidList = new List<EntityUid>();
    bool flag1 = false;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules((EntityUid) ent, ent.Comp))
    {
      UnpoweredFlashlightComponent comp2;
      if (Module.HasValue && this.TryComp<UnpoweredFlashlightComponent>(Module.Value, out comp2))
      {
        entityUidList.Add(Module.Value);
        flag1 |= comp2.LightOn;
      }
    }
    if (entityUidList.Count == 0)
      return;
    bool flag2 = !flag1;
    foreach (EntityUid entityUid in entityUidList)
      this._flashlight.SetLight((Entity<UnpoweredFlashlightComponent>) (entityUid, (UnpoweredFlashlightComponent) null), flag2, new EntityUid?(args.Performer));
    SharedActionsSystem actions = this._actions;
    EntityUid? toggleActionEntity = ent.Comp.FlashlightToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = flag2 ? 1 : 0;
    actions.SetToggled(action, num != 0);
    args.Handled = true;
  }

  private bool TryGetInstalledFlashlights(
    EntityUid weapon,
    PubgWeaponModulesComponent modules,
    out bool anyEnabled)
  {
    anyEnabled = false;
    bool installedFlashlights = false;
    foreach ((PubgWeaponModuleSlotDefinition _, EntityUid? Module) in this.EnumerateInstalledModules(weapon, modules))
    {
      UnpoweredFlashlightComponent comp;
      if (Module.HasValue && this.TryComp<UnpoweredFlashlightComponent>(Module.Value, out comp))
      {
        installedFlashlights = true;
        anyEnabled |= comp.LightOn;
      }
    }
    return installedFlashlights;
  }

  private void OnModuleExamined(Entity<PubgWeaponModuleComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-header"));
    string str1 = this.Loc.GetString(ent.Comp.UiCategoryLocKey);
    args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-category", ("category", (object) str1)));
    string str2 = this.Loc.GetString(PubgWeaponModulesSystem.GetSlotLocKey(ent.Comp.Slot));
    args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-slot", ("slot", (object) str2)));
    PubgMagazineModuleAmmoComponent comp;
    if (this.TryComp<PubgMagazineModuleAmmoComponent>((EntityUid) ent, out comp) && (comp.CurrentAmmo > 0 || comp.Capacity > 0))
      args.PushMarkup(comp.Capacity > 0 ? this.Loc.GetString("pubg-loadout-ammo-display", ("current", (object) comp.CurrentAmmo), ("capacity", (object) comp.Capacity)) : this.Loc.GetString("pubg-loadout-ammo-display-current", ("current", (object) comp.CurrentAmmo)));
    float x1 = (float) ((1.0 - (double) ent.Comp.SpreadMultiplier) * 100.0);
    if ((double) MathF.Abs(x1) > 0.0099999997764825821)
      args.PushMarkup((double) x1 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-spread-reduce", ("value", (object) MathF.Round(x1, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-spread-increase", ("value", (object) MathF.Round(MathF.Abs(x1), 1))));
    if ((double) MathF.Abs(ent.Comp.FocusBonusTiles) > 0.0099999997764825821)
      args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-focus", ("value", (object) MathF.Round(ent.Comp.FocusBonusTiles, 1))));
    float x2 = (float) ((1.0 - (double) ent.Comp.RecoilMultiplier) * 100.0);
    if ((double) MathF.Abs(x2) > 0.0099999997764825821)
      args.PushMarkup((double) x2 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-recoil-reduce", ("value", (object) MathF.Round(x2, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-recoil-increase", ("value", (object) MathF.Round(MathF.Abs(x2), 1))));
    float x3 = (float) ((1.0 - (double) ent.Comp.HipfireSpreadMultiplier) * 100.0);
    if ((double) MathF.Abs(x3) > 0.0099999997764825821)
      args.PushMarkup((double) x3 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-hipfire-reduce", ("value", (object) MathF.Round(x3, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-hipfire-increase", ("value", (object) MathF.Round(MathF.Abs(x3), 1))));
    float x4 = (float) ((1.0 - (double) ent.Comp.ReloadTimeMultiplier) * 100.0);
    if ((double) MathF.Abs(x4) > 0.0099999997764825821)
      args.PushMarkup((double) x4 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-reload-reduce", ("value", (object) MathF.Round(x4, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-reload-increase", ("value", (object) MathF.Round(MathF.Abs(x4), 1))));
    if (ent.Comp.MagazineCapacityBonus != 0)
      args.PushMarkup(ent.Comp.MagazineCapacityBonus > 0 ? this.Loc.GetString("pubg-loadout-tooltip-module-magazine-bonus", ("value", (object) ent.Comp.MagazineCapacityBonus)) : this.Loc.GetString("pubg-loadout-tooltip-module-magazine-penalty", ("value", (object) Math.Abs(ent.Comp.MagazineCapacityBonus))));
    if (ent.Comp.SoundGunshotOverride != null)
      args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-sound-override"));
    if (ent.Comp.DisableSpatialFarSound)
      args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-far-sound-disabled"));
    float x5 = (float) ((1.0 - (double) ent.Comp.SpatialAudioRangeMultiplier) * 100.0);
    if ((double) MathF.Abs(x5) > 0.0099999997764825821)
      args.PushMarkup((double) x5 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-audio-range-reduce", ("value", (object) MathF.Round(x5, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-audio-range-increase", ("value", (object) MathF.Round(MathF.Abs(x5), 1))));
    float x6 = (float) ((1.0 - (double) ent.Comp.SpatialNearRangeMultiplier) * 100.0);
    if ((double) MathF.Abs(x6) > 0.0099999997764825821)
      args.PushMarkup((double) x6 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-near-range-reduce", ("value", (object) MathF.Round(x6, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-near-range-increase", ("value", (object) MathF.Round(MathF.Abs(x6), 1))));
    float x7 = (float) ((1.0 - (double) ent.Comp.SpatialNearVolumeMultiplier) * 100.0);
    if ((double) MathF.Abs(x7) > 0.0099999997764825821)
      args.PushMarkup((double) x7 >= 0.0 ? this.Loc.GetString("pubg-loadout-tooltip-module-near-volume-reduce", ("value", (object) MathF.Round(x7, 1))) : this.Loc.GetString("pubg-loadout-tooltip-module-near-volume-increase", ("value", (object) MathF.Round(MathF.Abs(x7), 1))));
    if (ent.Comp.SuppressMuzzleFlash)
      args.PushMarkup(this.Loc.GetString("pubg-loadout-tooltip-module-muzzle-flash-hidden"));
    this.AppendCompatibilityInfo(ent, ref args);
  }

  private void AppendCompatibilityInfo(
    Entity<PubgWeaponModuleComponent> module,
    ref ExaminedEvent args)
  {
    HandsComponent comp1;
    if (!this.TryComp<HandsComponent>(args.Examiner, out comp1))
      return;
    bool flag = false;
    foreach (string key in comp1.Hands.Keys)
    {
      EntityUid? held;
      PubgWeaponModulesComponent comp2;
      if (this._hands.TryGetHeldItem((Entity<HandsComponent>) (args.Examiner, comp1), key, out held) && held.HasValue && this.TryComp<PubgWeaponModulesComponent>(held.Value, out comp2))
      {
        if (!flag)
        {
          args.PushMarkup(this.Loc.GetString("pubg-modules-examine-fit-header"));
          flag = true;
        }
        args.PushMarkup(this.BuildCompatibilityLine(module, held.Value, comp2));
      }
    }
  }

  private string BuildCompatibilityLine(
    Entity<PubgWeaponModuleComponent> module,
    EntityUid weapon,
    PubgWeaponModulesComponent modulesComp)
  {
    string str1 = this.Name(weapon);
    PubgWeaponModuleSlotDefinition slotDefinition = (PubgWeaponModuleSlotDefinition) null;
    foreach (PubgWeaponModuleSlotDefinition slot in modulesComp.Slots)
    {
      if (slot.Slot == module.Comp.Slot && this.IsModuleAllowedInSlot(module.Owner, slot, module.Comp))
      {
        slotDefinition = slot;
        break;
      }
    }
    if (slotDefinition == null)
      return this.Loc.GetString("pubg-modules-examine-fit-incompatible", (nameof (weapon), (object) str1));
    string str2 = this.Loc.GetString(slotDefinition.DisplayNameLocKey);
    string resolvedContainerId = this.GetResolvedContainerId(slotDefinition);
    BaseContainer container;
    if (this._container.TryGetContainer(weapon, resolvedContainerId, out container) && container is ContainerSlot containerSlot)
    {
      EntityUid? containedEntity = containerSlot.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        return this.Loc.GetString("pubg-modules-examine-fit-replaces", (nameof (weapon), (object) str1), ("slot", (object) str2), (nameof (module), (object) this.Name(valueOrDefault)));
      }
    }
    return this.Loc.GetString("pubg-modules-examine-fit-empty", (nameof (weapon), (object) str1), ("slot", (object) str2));
  }

  private void OnWeaponExamined(Entity<PubgWeaponModulesComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("PubgWeaponModulesComponent"))
    {
      args.PushMarkup(this.Loc.GetString("pubg-loadout-examine-weapon-header"));
      foreach ((PubgWeaponModuleSlotDefinition Slot, EntityUid? Module) in this.EnumerateInstalledModules((EntityUid) ent, ent.Comp))
      {
        string str1 = this.Loc.GetString(Slot.DisplayNameLocKey);
        string str2 = Module.HasValue ? this.Name(Module.Value) : this.Loc.GetString("pubg-loadout-slot-empty");
        args.PushMarkup(this.Loc.GetString("pubg-loadout-examine-weapon-module-entry", ("slot", (object) str1), ("module", (object) str2)));
        if (Slot.Slot == PubgModuleSlotType.Magazine && Slot.StoresAmmo)
        {
          PubgMagazineModuleAmmoComponent comp;
          if (Module.HasValue && this.TryComp<PubgMagazineModuleAmmoComponent>(Module.Value, out comp))
            args.PushMarkup(comp.Capacity > 0 ? this.Loc.GetString("pubg-loadout-ammo-display", ("current", (object) comp.CurrentAmmo), ("capacity", (object) comp.Capacity)) : this.Loc.GetString("pubg-loadout-ammo-display-current", ("current", (object) comp.CurrentAmmo)));
          else
            args.PushMarkup(this.Loc.GetString("pubg-loadout-ammo-empty"));
        }
      }
      float spreadMultiplier = this.GetSpreadMultiplier((EntityUid) ent, ent.Comp);
      float focusBonusTiles = this.GetFocusBonusTiles((EntityUid) ent, ent.Comp);
      float x = (float) ((1.0 - (double) spreadMultiplier) * 100.0);
      if ((double) x >= 0.0)
        args.PushMarkup(this.Loc.GetString("pubg-loadout-examine-weapon-spread-reduce-total", ("value", (object) MathF.Round(x, 1))));
      else
        args.PushMarkup(this.Loc.GetString("pubg-loadout-examine-weapon-spread-increase-total", ("value", (object) MathF.Round(MathF.Abs(x), 1))));
      args.PushMarkup(this.Loc.GetString("pubg-loadout-examine-weapon-focus-total", ("value", (object) MathF.Round(focusBonusTiles, 1))));
    }
  }
}
