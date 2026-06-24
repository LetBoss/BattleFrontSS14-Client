// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Repairable;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Explosion.Components;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class HardpointSystem : EntitySystem
{
  private static readonly EntProtoId<SkillDefinitionComponent> EngineerSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;
  [Dependency]
  private readonly Content.Shared.Vehicle.VehicleSystem _vehicles;
  [Dependency]
  private readonly SharedDoAfterSystem _doAfter;
  [Dependency]
  private readonly SharedToolSystem _tool;
  [Dependency]
  private readonly VehicleWheelSystem _wheels;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly RMCRepairableSystem _repairable;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  [Dependency]
  private readonly SharedContainerSystem _containers;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SharedUserInterfaceSystem _ui;
  [Dependency]
  private readonly SharedHandsSystem _hands;
  [Dependency]
  private readonly SharedAppearanceSystem _appearance;
  [Dependency]
  private readonly EntityWhitelistSystem _whitelist;
  [Dependency]
  private readonly SharedGunSystem _guns;
  [Dependency]
  private readonly IPrototypeManager _prototypeManager;
  [Dependency]
  private readonly SharedExplosionSystem _explosion;
  [Dependency]
  private readonly VehicleTopologySystem _topology;
  [Dependency]
  private readonly SkillsSystem _skills;
  [Dependency]
  private readonly HardpointSlotSystem _hardpointSlots;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HardpointSlotsComponent, ComponentInit>(new EntityEventRefHandler<HardpointSlotsComponent, ComponentInit>(this.OnSlotsInit));
    this.SubscribeLocalEvent<HardpointSlotsComponent, MapInitEvent>(new EntityEventRefHandler<HardpointSlotsComponent, MapInitEvent>(this.OnSlotsMapInit));
    this.SubscribeLocalEvent<HardpointSlotsComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<HardpointSlotsComponent, EntInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<HardpointSlotsComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<HardpointSlotsComponent, EntRemovedFromContainerMessage>(this.OnRemoved));
    this.SubscribeLocalEvent<HardpointSlotsComponent, VehicleCanRunEvent>(new EntityEventRefHandler<HardpointSlotsComponent, VehicleCanRunEvent>(this.OnVehicleCanRun));
    this.SubscribeLocalEvent<HardpointSlotsComponent, DamageModifyEvent>(new EntityEventRefHandler<HardpointSlotsComponent, DamageModifyEvent>(this.OnVehicleDamageModify));
    this.SubscribeLocalEvent<HardpointIntegrityComponent, ComponentInit>(new EntityEventRefHandler<HardpointIntegrityComponent, ComponentInit>(this.OnHardpointIntegrityInit));
    this.SubscribeLocalEvent<HardpointIntegrityComponent, InteractUsingEvent>(new EntityEventRefHandler<HardpointIntegrityComponent, InteractUsingEvent>(this.OnHardpointRepair), new Type[1]
    {
      typeof (ItemSlotsSystem)
    });
    this.SubscribeLocalEvent<HardpointIntegrityComponent, ExaminedEvent>(new EntityEventRefHandler<HardpointIntegrityComponent, ExaminedEvent>(this.OnHardpointExamined));
    this.SubscribeLocalEvent<HardpointIntegrityComponent, HardpointRepairDoAfterEvent>(new EntityEventRefHandler<HardpointIntegrityComponent, HardpointRepairDoAfterEvent>(this.OnHardpointRepairDoAfter));
  }

  private void OnSlotsInit(Entity<HardpointSlotsComponent> ent, ref ComponentInit args)
  {
    this.EnsureSlots(ent.Owner, ent.Comp);
  }

  private void OnSlotsMapInit(Entity<HardpointSlotsComponent> ent, ref MapInitEvent args)
  {
    this.EnsureSlots(ent.Owner, ent.Comp);
    this._hardpointSlots.DisableEjectForAllSlots(ent);
  }

  private void OnInserted(
    Entity<HardpointSlotsComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    HardpointSlot slot;
    if (!this.TryGetSlot(ent.Comp, args.Container.ID, out slot))
      return;
    ent.Comp.PendingRemovals.Clear();
    if (!this.IsValidHardpoint(args.Entity, ent.Comp, slot))
    {
      ItemSlotsComponent comp;
      if (!this.TryComp<ItemSlotsComponent>(ent.Owner, out comp))
        return;
      this._itemSlots.TryEject(ent.Owner, args.Container.ID, new EntityUid?(), out EntityUid? _, comp, true);
    }
    else
    {
      ent.Comp.LastUiError = (string) null;
      GunComponent comp;
      if (this.TryComp<GunComponent>(args.Entity, out comp))
        this._guns.RefreshModifiers((Entity<GunComponent>) (args.Entity, comp));
      this.ApplyArmorHardpointModifiers(ent.Owner, args.Entity, true);
      this.RefreshSupportModifiers(ent.Owner);
      this.RefreshCanRun(ent.Owner);
      this.UpdateHardpointUi(ent.Owner, ent.Comp);
      this.UpdateContainingVehicleUi(ent.Owner);
      this.RaiseHardpointSlotsChanged(ent.Owner);
      this.RaiseVehicleSlotsChanged(ent.Owner);
    }
  }

  private void OnRemoved(
    Entity<HardpointSlotsComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!this.TryGetSlot(ent.Comp, args.Container.ID, out HardpointSlot _))
      return;
    this.ApplyArmorHardpointModifiers(ent.Owner, args.Entity, false);
    this.RefreshSupportModifiers(ent.Owner);
    ent.Comp.LastUiError = (string) null;
    this.RefreshCanRun(ent.Owner);
    ent.Comp.PendingRemovals.Remove(args.Container.ID);
    this.UpdateHardpointUi(ent.Owner, ent.Comp);
    this.UpdateContainingVehicleUi(ent.Owner);
    this.RaiseHardpointSlotsChanged(ent.Owner);
    this.RaiseVehicleSlotsChanged(ent.Owner);
  }

  private void RefreshSupportModifiers(EntityUid owner)
  {
    EntityUid vehicle = owner;
    HardpointSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if (!this.HasComp<VehicleComponent>(vehicle) && !this.TryGetContainingVehicleFrame(owner, out vehicle) || !this.TryComp<HardpointSlotsComponent>(vehicle, out comp1) || !this.TryComp<ItemSlotsComponent>(vehicle, out comp2))
      return;
    if (this._net.IsClient)
    {
      this.RefreshVehicleGunModifiers(vehicle, comp1, comp2);
    }
    else
    {
      FixedPoint2 accuracyMult = FixedPoint2.New(1);
      float fireRateMult = 1f;
      float speedMult = 1f;
      float accelMult = 1f;
      float viewScale = 0.0f;
      float cursorMaxOffset = 0.0f;
      float cursorOffsetSpeed = 0.5f;
      float cursorPvsIncrease = 0.0f;
      bool hasWeaponMods = false;
      bool hasSpeedMods = false;
      bool hasAccelMods = false;
      bool hasViewMods = false;
      foreach (HardpointSlot slot1 in comp1.Slots)
      {
        ItemSlot itemSlot1;
        if (!string.IsNullOrWhiteSpace(slot1.Id) && this._itemSlots.TryGetSlot(vehicle, slot1.Id, out itemSlot1, comp2) && itemSlot1.HasItem)
        {
          EntityUid uid = itemSlot1.Item.Value;
          Accumulate(uid);
          HardpointSlotsComponent comp3;
          ItemSlotsComponent comp4;
          if (this.TryComp<HardpointSlotsComponent>(uid, out comp3) && this.TryComp<ItemSlotsComponent>(uid, out comp4))
          {
            foreach (HardpointSlot slot2 in comp3.Slots)
            {
              ItemSlot itemSlot2;
              if (!string.IsNullOrWhiteSpace(slot2.Id) && this._itemSlots.TryGetSlot(uid, slot2.Id, out itemSlot2, comp4) && itemSlot2.HasItem)
                Accumulate(itemSlot2.Item.Value);
            }
          }
        }
      }
      if (hasWeaponMods)
      {
        VehicleWeaponSupportModifierComponent modifierComponent = this.EnsureComp<VehicleWeaponSupportModifierComponent>(vehicle);
        modifierComponent.AccuracyMultiplier = accuracyMult;
        modifierComponent.FireRateMultiplier = fireRateMult;
        this.Dirty(vehicle, (IComponent) modifierComponent);
      }
      else
        this.RemCompDeferred<VehicleWeaponSupportModifierComponent>(vehicle);
      if (hasSpeedMods)
      {
        VehicleSpeedModifierComponent modifierComponent = this.EnsureComp<VehicleSpeedModifierComponent>(vehicle);
        modifierComponent.SpeedMultiplier = speedMult;
        this.Dirty(vehicle, (IComponent) modifierComponent);
      }
      else
        this.RemCompDeferred<VehicleSpeedModifierComponent>(vehicle);
      if (hasAccelMods)
      {
        VehicleAccelerationModifierComponent modifierComponent = this.EnsureComp<VehicleAccelerationModifierComponent>(vehicle);
        modifierComponent.AccelerationMultiplier = accelMult;
        this.Dirty(vehicle, (IComponent) modifierComponent);
      }
      else
        this.RemCompDeferred<VehicleAccelerationModifierComponent>(vehicle);
      if (hasViewMods && (double) viewScale > 0.0)
      {
        VehicleGunnerViewComponent gunnerViewComponent = this.EnsureComp<VehicleGunnerViewComponent>(vehicle);
        gunnerViewComponent.PvsScale = viewScale;
        gunnerViewComponent.CursorMaxOffset = cursorMaxOffset;
        gunnerViewComponent.CursorOffsetSpeed = cursorOffsetSpeed;
        gunnerViewComponent.CursorPvsIncrease = cursorPvsIncrease;
        this.Dirty(vehicle, (IComponent) gunnerViewComponent);
      }
      else
        this.RemCompDeferred<VehicleGunnerViewComponent>(vehicle);
      this.RefreshVehicleGunModifiers(vehicle, comp1, comp2);

      void Accumulate(EntityUid item)
      {
        VehicleWeaponSupportAttachmentComponent comp1;
        if (this.TryComp<VehicleWeaponSupportAttachmentComponent>(item, out comp1))
        {
          accuracyMult *= comp1.AccuracyMultiplier;
          fireRateMult *= comp1.FireRateMultiplier;
          hasWeaponMods = true;
        }
        VehicleSpeedModifierAttachmentComponent comp2;
        if (this.TryComp<VehicleSpeedModifierAttachmentComponent>(item, out comp2))
        {
          speedMult *= comp2.SpeedMultiplier;
          hasSpeedMods = true;
        }
        VehicleAccelerationModifierAttachmentComponent comp3;
        if (this.TryComp<VehicleAccelerationModifierAttachmentComponent>(item, out comp3))
        {
          accelMult *= comp3.AccelerationMultiplier;
          hasAccelMods = true;
        }
        VehicleGunnerViewAttachmentComponent comp4;
        if (!this.TryComp<VehicleGunnerViewAttachmentComponent>(item, out comp4))
          return;
        viewScale = Math.Max(viewScale, comp4.PvsScale);
        cursorMaxOffset = Math.Max(cursorMaxOffset, comp4.CursorMaxOffset);
        cursorOffsetSpeed = MathF.Max(cursorOffsetSpeed, comp4.CursorOffsetSpeed);
        cursorPvsIncrease = Math.Max(cursorPvsIncrease, comp4.CursorPvsIncrease);
        hasViewMods = true;
      }
    }
  }

  private void RefreshVehicleGunModifiers(
    EntityUid vehicle,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots)
  {
    foreach (HardpointSlot slot1 in hardpoints.Slots)
    {
      ItemSlot itemSlot1;
      if (!string.IsNullOrWhiteSpace(slot1.Id) && this._itemSlots.TryGetSlot(vehicle, slot1.Id, out itemSlot1, itemSlots) && itemSlot1.HasItem)
      {
        EntityUid? nullable = itemSlot1.Item;
        this.RefreshGunModifiers(nullable.Value);
        nullable = itemSlot1.Item;
        HardpointSlotsComponent comp1;
        if (this.TryComp<HardpointSlotsComponent>(nullable.Value, out comp1))
        {
          nullable = itemSlot1.Item;
          ItemSlotsComponent comp2;
          if (this.TryComp<ItemSlotsComponent>(nullable.Value, out comp2))
          {
            foreach (HardpointSlot slot2 in comp1.Slots)
            {
              if (!string.IsNullOrWhiteSpace(slot2.Id))
              {
                ItemSlotsSystem itemSlots1 = this._itemSlots;
                nullable = itemSlot1.Item;
                EntityUid uid = nullable.Value;
                string id = slot2.Id;
                ItemSlot itemSlot2;
                ref ItemSlot local = ref itemSlot2;
                ItemSlotsComponent component = comp2;
                if (itemSlots1.TryGetSlot(uid, id, out local, component) && itemSlot2.HasItem)
                {
                  nullable = itemSlot2.Item;
                  this.RefreshGunModifiers(nullable.Value);
                }
              }
            }
          }
        }
      }
    }
  }

  private void RefreshGunModifiers(EntityUid item)
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>(item, out comp))
      return;
    this._guns.RefreshModifiers((Entity<GunComponent>) (item, comp));
  }

  private void ApplyArmorHardpointModifiers(
    EntityUid vehicle,
    EntityUid hardpointItem,
    bool adding)
  {
    VehicleArmorHardpointComponent comp1;
    if (this._net.IsClient || !this.TryComp<VehicleArmorHardpointComponent>(hardpointItem, out comp1))
      return;
    if (comp1.ModifierSets.Count > 0)
    {
      DamageProtectionBuffComponent protectionBuffComponent = this.EnsureComp<DamageProtectionBuffComponent>(vehicle);
      foreach (ProtoId<DamageModifierSetPrototype> modifierSet in comp1.ModifierSets)
      {
        DamageModifierSetPrototype prototype;
        if (this._prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSet, out prototype))
        {
          if (adding)
            protectionBuffComponent.Modifiers[(string) modifierSet] = prototype;
          else
            protectionBuffComponent.Modifiers.Remove((string) modifierSet);
        }
      }
      if (!adding && protectionBuffComponent.Modifiers.Count == 0)
        this.RemComp<DamageProtectionBuffComponent>(vehicle);
      else
        this.Dirty(vehicle, (IComponent) protectionBuffComponent);
    }
    if (!comp1.ExplosionCoefficient.HasValue)
      return;
    if (adding)
    {
      this._explosion.SetExplosionResistance(vehicle, comp1.ExplosionCoefficient.Value, false);
    }
    else
    {
      ExplosionResistanceComponent comp2;
      if (!this.TryComp<ExplosionResistanceComponent>(vehicle, out comp2) || (double) MathF.Abs(comp2.DamageCoefficient - comp1.ExplosionCoefficient.Value) >= 9.9999997473787516E-05)
        return;
      this.RemComp<ExplosionResistanceComponent>(vehicle);
    }
  }

  private void RaiseHardpointSlotsChanged(EntityUid vehicle)
  {
    HardpointSlotsChangedEvent args = new HardpointSlotsChangedEvent(vehicle);
    this.RaiseLocalEvent<HardpointSlotsChangedEvent>(vehicle, args, true);
  }

  private void RaiseVehicleSlotsChanged(EntityUid owner)
  {
    EntityUid vehicle;
    if (!this.TryGetContainingVehicleFrame(owner, out vehicle))
      return;
    this.RaiseHardpointSlotsChanged(vehicle);
  }

  private void OnVehicleCanRun(Entity<HardpointSlotsComponent> ent, ref VehicleCanRunEvent args)
  {
    if (!args.CanRun || this.HasAllRequired(ent.Owner, ent.Comp))
      return;
    args.CanRun = false;
  }

  private void EnsureSlots(
    EntityUid uid,
    HardpointSlotsComponent component,
    ItemSlotsComponent? itemSlots = null)
  {
    if (component.Slots.Count == 0)
      return;
    if (itemSlots == null)
      itemSlots = this.EnsureComp<ItemSlotsComponent>(uid);
    foreach (HardpointSlot slot1 in component.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot1.Id))
      {
        ItemSlot itemSlot;
        if (this._itemSlots.TryGetSlot(uid, slot1.Id, out itemSlot, itemSlots))
        {
          this._itemSlots.SetEjectFlags(uid, itemSlot, true, itemSlots: itemSlots);
        }
        else
        {
          EntityWhitelist entityWhitelist = slot1.Whitelist;
          if (entityWhitelist == null)
          {
            entityWhitelist = new EntityWhitelist()
            {
              Components = new string[1]{ "HardpointItem" }
            };
          }
          else
          {
            int num = entityWhitelist.Components == null ? 0 : (entityWhitelist.Components.Length != 0 ? 1 : 0);
            bool flag1 = entityWhitelist.Tags != null && entityWhitelist.Tags.Count > 0;
            bool flag2 = entityWhitelist.Sizes != null && entityWhitelist.Sizes.Count > 0;
            bool flag3 = entityWhitelist.Skills != null && entityWhitelist.Skills.Count > 0;
            bool hasValue = entityWhitelist.MinMobSize.HasValue;
            if (num == 0 && !flag1 && !flag2 && !flag3 && !hasValue)
              entityWhitelist.Components = new string[1]
              {
                "HardpointItem"
              };
          }
          ItemSlot slot2 = new ItemSlot()
          {
            Whitelist = entityWhitelist
          };
          this._itemSlots.AddItemSlot(uid, slot1.Id, slot2, itemSlots);
          this._itemSlots.SetEjectFlags(uid, slot2, true, itemSlots: itemSlots);
        }
      }
    }
  }

  internal bool TryGetSlot(HardpointSlotsComponent component, string? id, [NotNullWhen(true)] out HardpointSlot? slot)
  {
    slot = (HardpointSlot) null;
    if (id == null)
      return false;
    foreach (HardpointSlot slot1 in component.Slots)
    {
      if (slot1.Id == id)
      {
        slot = slot1;
        return true;
      }
    }
    return false;
  }

  internal bool IsValidHardpoint(EntityUid item, HardpointSlotsComponent slots, HardpointSlot slot)
  {
    HardpointItemComponent comp;
    if (!this.TryComp<HardpointItemComponent>(item, out comp))
      return false;
    if (slots.VehicleFamily.HasValue)
    {
      ProtoId<HardpointVehicleFamilyPrototype>? vehicleFamily = comp.VehicleFamily;
      if (!vehicleFamily.HasValue || vehicleFamily.GetValueOrDefault() != slots.VehicleFamily.Value)
        return false;
    }
    if (slot.SlotType.HasValue)
    {
      ProtoId<HardpointSlotTypePrototype>? slotType = comp.SlotType;
      if (!slotType.HasValue)
        return false;
      ProtoId<HardpointSlotTypePrototype> valueOrDefault = slotType.GetValueOrDefault();
      slotType = slot.SlotType;
      ProtoId<HardpointSlotTypePrototype> protoId = slotType.Value;
      if (valueOrDefault != protoId)
        return false;
    }
    if (!string.IsNullOrWhiteSpace(slot.CompatibilityId) && (string.IsNullOrWhiteSpace(comp.CompatibilityId) || !string.Equals(comp.CompatibilityId, slot.CompatibilityId, StringComparison.OrdinalIgnoreCase)))
      return false;
    if (string.IsNullOrWhiteSpace(slot.HardpointType))
      return slot.Whitelist == null || this._whitelist.IsValid(slot.Whitelist, item);
    if (!string.Equals(comp.HardpointType, slot.HardpointType, StringComparison.OrdinalIgnoreCase))
      return false;
    return slot.Whitelist == null || this._whitelist.IsValid(slot.Whitelist, item);
  }

  private bool HasAllRequired(
    EntityUid uid,
    HardpointSlotsComponent component,
    ItemSlotsComponent? itemSlots = null)
  {
    if (component.Slots.Count == 0 || !this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return true;
    foreach (HardpointSlot slot in component.Slots)
    {
      if (slot.Required)
      {
        ItemSlot itemSlot;
        if (!this._itemSlots.TryGetSlot(uid, slot.Id, out itemSlot, itemSlots) || !itemSlot.HasItem)
          return false;
        EntityUid? nullable = itemSlot.Item;
        HardpointIntegrityComponent comp;
        if (nullable.HasValue && this.TryComp<HardpointIntegrityComponent>(nullable.GetValueOrDefault(), out comp) && (double) comp.Integrity <= 0.0)
          return false;
      }
    }
    return true;
  }

  internal void RefreshCanRun(EntityUid uid)
  {
    VehicleComponent comp;
    if (!this.TryComp<VehicleComponent>(uid, out comp))
      return;
    this._vehicles.RefreshCanRun((Entity<VehicleComponent>) (uid, comp));
  }

  private void OnVehicleDamageModify(
    Entity<HardpointSlotsComponent> ent,
    ref DamageModifyEvent args)
  {
    ItemSlotsComponent comp1;
    if (this._net.IsClient || (double) args.Damage.GetTotal().Float() <= 0.0 || !this.TryComp<ItemSlotsComponent>(ent.Owner, out comp1))
      return;
    List<(EntityUid, HardpointIntegrityComponent)> intactHardpoints = new List<(EntityUid, HardpointIntegrityComponent)>();
    this.CollectIntactTopLevelHardpoints(ent.Owner, ent.Comp, comp1, intactHardpoints);
    bool flag = intactHardpoints.Count > 0;
    if (flag)
    {
      HashSet<EntityUid> visited = new HashSet<EntityUid>();
      foreach ((EntityUid hardpoint, HardpointIntegrityComponent integrity) in intactHardpoints)
        this.ApplyDamageToHardpointTree(ent.Owner, hardpoint, integrity, args.Damage, visited);
    }
    float fraction = flag ? ent.Comp.FrameDamageFractionWhileIntact : 1f;
    HardpointIntegrityComponent comp2;
    if (this.TryComp<HardpointIntegrityComponent>(ent.Owner, out comp2))
    {
      DamageSpecifier damage = this.ScaleDamage(args.Damage, fraction);
      float frameDamageAmount = this.GetVehicleFrameDamageAmount(ent.Owner, damage);
      if ((double) frameDamageAmount > 0.0)
        this.DamageHardpoint(ent.Owner, ent.Owner, frameDamageAmount, comp2);
    }
    args.Damage = this.ScaleDamage(args.Damage, fraction);
  }

  private void CollectIntactTopLevelHardpoints(
    EntityUid owner,
    HardpointSlotsComponent slots,
    ItemSlotsComponent itemSlots,
    List<(EntityUid Item, HardpointIntegrityComponent Integrity)> intactHardpoints)
  {
    foreach (HardpointSlot slot in slots.Slots)
    {
      ItemSlot itemSlot;
      if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(owner, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
      {
        EntityUid? nullable = itemSlot.Item;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          HardpointIntegrityComponent comp;
          if (this.TryComp<HardpointIntegrityComponent>(valueOrDefault, out comp) && (double) comp.Integrity > 0.0)
            intactHardpoints.Add((valueOrDefault, comp));
        }
      }
    }
  }

  private void ApplyDamageToHardpointTree(
    EntityUid vehicle,
    EntityUid hardpoint,
    HardpointIntegrityComponent integrity,
    DamageSpecifier damage,
    HashSet<EntityUid> visited)
  {
    if (!visited.Add(hardpoint))
      return;
    this.ApplyDamageToHardpoint(vehicle, hardpoint, integrity, damage);
    HardpointSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if (!this.TryComp<HardpointSlotsComponent>(hardpoint, out comp1) || !this.TryComp<ItemSlotsComponent>(hardpoint, out comp2))
      return;
    foreach (HardpointSlot slot in comp1.Slots)
    {
      ItemSlot itemSlot;
      if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(hardpoint, slot.Id, out itemSlot, comp2))
      {
        EntityUid? nullable = itemSlot.Item;
        if (nullable.HasValue)
        {
          EntityUid valueOrDefault = nullable.GetValueOrDefault();
          HardpointIntegrityComponent comp3;
          if (this.TryComp<HardpointIntegrityComponent>(valueOrDefault, out comp3) && (double) comp3.Integrity > 0.0)
            this.ApplyDamageToHardpointTree(vehicle, valueOrDefault, comp3, damage, visited);
        }
      }
    }
  }

  private DamageSpecifier ScaleDamage(DamageSpecifier source, float fraction)
  {
    if ((double) MathF.Abs(fraction - 1f) < 9.9999997473787516E-05)
      return source;
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    foreach ((string key, FixedPoint2 fixedPoint2) in source.DamageDict)
      damageSpecifier.DamageDict[key] = fixedPoint2 * fraction;
    return damageSpecifier;
  }

  private void ApplyDamageToHardpoint(
    EntityUid vehicle,
    EntityUid hardpoint,
    HardpointIntegrityComponent integrity,
    DamageSpecifier damage)
  {
    float hardpointDamageAmount = this.GetHardpointDamageAmount(hardpoint, damage);
    if ((double) hardpointDamageAmount <= 0.0)
      return;
    this.DamageHardpoint(vehicle, hardpoint, hardpointDamageAmount, integrity);
  }

  private float GetHardpointDamageAmount(EntityUid hardpoint, DamageSpecifier damage)
  {
    float hardpointDamageAmount = MathF.Max(damage.GetTotal().Float(), 0.0f);
    List<DamageModifierSet> modifierSets = new List<DamageModifierSet>();
    this.CollectHardpointDamageModifierSets(hardpoint, modifierSets);
    if (modifierSets.Count > 0)
      hardpointDamageAmount = MathF.Max(DamageSpecifier.ApplyModifierSets(damage, (IEnumerable<DamageModifierSet>) modifierSets).GetTotal().Float(), 0.0f);
    HardpointItemComponent comp;
    if (this.TryComp<HardpointItemComponent>(hardpoint, out comp))
      hardpointDamageAmount *= MathF.Max(comp.DamageMultiplier, 0.0f);
    return hardpointDamageAmount;
  }

  private void CollectHardpointDamageModifierSets(
    EntityUid hardpoint,
    List<DamageModifierSet> modifierSets)
  {
    HardpointDamageModifierComponent comp1;
    if (this.TryComp<HardpointDamageModifierComponent>(hardpoint, out comp1))
    {
      foreach (ProtoId<DamageModifierSetPrototype> modifierSet in comp1.ModifierSets)
      {
        DamageModifierSetPrototype prototype;
        if (this._prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSet, out prototype))
          modifierSets.Add((DamageModifierSet) prototype);
      }
    }
    VehicleArmorHardpointComponent comp2;
    if (!this.TryComp<VehicleArmorHardpointComponent>(hardpoint, out comp2))
      return;
    foreach (ProtoId<DamageModifierSetPrototype> modifierSet in comp2.ModifierSets)
    {
      DamageModifierSetPrototype prototype;
      if (this._prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSet, out prototype))
        modifierSets.Add((DamageModifierSet) prototype);
    }
  }

  private float GetVehicleFrameDamageAmount(EntityUid vehicle, DamageSpecifier damage)
  {
    float frameDamageAmount = MathF.Max(damage.GetTotal().Float(), 0.0f);
    DamageProtectionBuffComponent comp;
    if (!this.TryComp<DamageProtectionBuffComponent>(vehicle, out comp) || comp.Modifiers.Count == 0)
      return frameDamageAmount;
    DamageSpecifier damageSpec = damage;
    foreach (DamageModifierSetPrototype modifierSet in comp.Modifiers.Values)
      damageSpec = DamageSpecifier.ApplyModifierSet(damageSpec, (DamageModifierSet) modifierSet);
    return MathF.Max(damageSpec.GetTotal().Float(), 0.0f);
  }

  private void OnHardpointIntegrityInit(
    Entity<HardpointIntegrityComponent> ent,
    ref ComponentInit args)
  {
    if ((double) ent.Comp.Integrity <= 0.0)
      ent.Comp.Integrity = ent.Comp.MaxIntegrity;
    this.UpdateFrameDamageAppearance(ent.Owner, ent.Comp);
  }

  private void OnHardpointExamined(Entity<HardpointIntegrityComponent> ent, ref ExaminedEvent args)
  {
    float integrity = ent.Comp.Integrity;
    float maxIntegrity = ent.Comp.MaxIntegrity;
    float percent = (double) maxIntegrity > 0.0 ? integrity / maxIntegrity : 0.0f;
    if (this.HasComp<XenoComponent>(args.Examiner))
    {
      args.PushMarkup(this.Loc.GetString(this.GetHardpointConditionString(percent)));
    }
    else
    {
      string hardpointIntegrityColor = this.GetHardpointIntegrityColor(percent);
      args.PushMarkup(this.Loc.GetString("rmc-hardpoint-integrity-examine", ("color", (object) hardpointIntegrityColor), ("current", (object) (int) MathF.Ceiling(integrity)), ("max", (object) (int) MathF.Ceiling(maxIntegrity)), ("percent", (object) (int) MathF.Round(percent * 100f))));
      float acid;
      float slash;
      float bullet;
      float explosive;
      float blunt;
      if (!this.TryGetArmorExamineModifiers(ent.Owner, out acid, out slash, out bullet, out explosive, out blunt))
        return;
      args.PushMarkup(this.Loc.GetString("rmc-hardpoint-armor-modifiers-examine", ("acid", (object) HardpointSystem.FormatModifierValue(acid)), ("slash", (object) HardpointSystem.FormatModifierValue(slash)), ("bullet", (object) HardpointSystem.FormatModifierValue(bullet)), ("explosive", (object) HardpointSystem.FormatModifierValue(explosive)), ("blunt", (object) HardpointSystem.FormatModifierValue(blunt))));
    }
  }

  private bool TryGetArmorExamineModifiers(
    EntityUid uid,
    out float acid,
    out float slash,
    out float bullet,
    out float explosive,
    out float blunt)
  {
    acid = 1f;
    slash = 1f;
    bullet = 1f;
    explosive = 1f;
    blunt = 1f;
    VehicleArmorHardpointComponent comp1;
    if (!this.TryComp<VehicleArmorHardpointComponent>(uid, out comp1))
      return false;
    HardpointItemComponent comp2;
    if (this.TryComp<HardpointItemComponent>(uid, out comp2))
    {
      ProtoId<HardpointVehicleFamilyPrototype>? vehicleFamily = comp2.VehicleFamily;
      ProtoId<HardpointVehicleFamilyPrototype>? nullable = (ProtoId<HardpointVehicleFamilyPrototype>?) "Tank";
      DamageModifierSetPrototype prototype;
      if ((vehicleFamily.HasValue == nullable.HasValue ? (vehicleFamily.HasValue ? (vehicleFamily.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this._prototypeManager.TryIndex<DamageModifierSetPrototype>("VehicleFrameTank", out prototype))
        HardpointSystem.ApplyDamageModifierCoefficients((DamageModifierSet) prototype, ref acid, ref slash, ref bullet, ref explosive, ref blunt);
    }
    foreach (ProtoId<DamageModifierSetPrototype> modifierSet in comp1.ModifierSets)
    {
      DamageModifierSetPrototype prototype;
      if (this._prototypeManager.TryIndex<DamageModifierSetPrototype>(modifierSet, out prototype))
        HardpointSystem.ApplyDamageModifierCoefficients((DamageModifierSet) prototype, ref acid, ref slash, ref bullet, ref explosive, ref blunt);
    }
    return true;
  }

  private static void ApplyDamageModifierCoefficients(
    DamageModifierSet modifierSet,
    ref float acid,
    ref float slash,
    ref float bullet,
    ref float explosive,
    ref float blunt)
  {
    float num1;
    if (modifierSet.Coefficients.TryGetValue("Caustic", out num1))
      acid *= num1;
    float num2;
    if (modifierSet.Coefficients.TryGetValue("Slash", out num2))
      slash *= num2;
    float num3;
    if (modifierSet.Coefficients.TryGetValue("Piercing", out num3))
      bullet *= num3;
    float num4;
    if (modifierSet.Coefficients.TryGetValue("Structural", out num4))
      explosive *= num4;
    float num5;
    if (!modifierSet.Coefficients.TryGetValue("Blunt", out num5))
      return;
    blunt *= num5;
  }

  private static string FormatModifierValue(float value) => value.ToString("0.###");

  private string GetHardpointIntegrityColor(float percent)
  {
    if ((double) percent >= 0.89999997615814209)
      return "green";
    if ((double) percent >= 0.699999988079071)
      return "yellow";
    if ((double) percent >= 0.40000000596046448)
      return "orange";
    return (double) percent >= 0.15000000596046448 ? "red" : "crimson";
  }

  private string GetHardpointConditionString(float percent)
  {
    if ((double) percent >= 0.89999997615814209)
      return "rmc-hardpoint-condition-pristine";
    if ((double) percent >= 0.699999988079071)
      return "rmc-hardpoint-condition-good";
    if ((double) percent >= 0.40000000596046448)
      return "rmc-hardpoint-condition-worn";
    return (double) percent >= 0.15000000596046448 ? "rmc-hardpoint-condition-bad" : "rmc-hardpoint-condition-critical";
  }

  public bool DamageHardpoint(
    EntityUid vehicle,
    EntityUid hardpoint,
    float amount,
    HardpointIntegrityComponent? integrity = null)
  {
    if (this._net.IsClient || (double) amount <= 0.0 || !this.Resolve<HardpointIntegrityComponent>(hardpoint, ref integrity, false) || (double) integrity.Integrity <= 0.0)
      return false;
    if ((double) integrity.Integrity > (double) integrity.MaxIntegrity && (double) integrity.MaxIntegrity > 0.0)
      integrity.Integrity = integrity.MaxIntegrity;
    float integrity1 = integrity.Integrity;
    integrity.Integrity = MathF.Max(0.0f, integrity.Integrity - amount);
    if ((double) Math.Abs(integrity1 - integrity.Integrity) < 0.0099999997764825821)
      return false;
    this.Dirty(hardpoint, (IComponent) integrity);
    this.UpdateFrameDamageAppearance(hardpoint, integrity);
    if (this.TryComp<VehicleWheelItemComponent>(hardpoint, out VehicleWheelItemComponent _))
      this._wheels.OnWheelDamaged(vehicle);
    if ((double) integrity1 > 0.0 && (double) integrity.Integrity <= 0.0)
    {
      this.RefreshCanRun(vehicle);
      if (hardpoint == vehicle)
      {
        RMCVehicleFrameDestroyedEvent args = new RMCVehicleFrameDestroyedEvent(vehicle);
        this.RaiseLocalEvent<RMCVehicleFrameDestroyedEvent>(vehicle, args);
      }
    }
    this.UpdateHardpointUi(vehicle);
    return true;
  }

  private void OnHardpointRepair(
    Entity<HardpointIntegrityComponent> ent,
    ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    EntityUid used = args.Used;
    bool isFrame = this.HasComp<HardpointSlotsComponent>(ent.Owner);
    bool usedWelder = this._tool.HasQuality(used, (string) ent.Comp.RepairToolQuality) && this.HasComp<BlowtorchComponent>(used);
    bool usedWrench = isFrame && this._tool.HasQuality(used, (string) ent.Comp.FrameFinishToolQuality);
    if (!usedWelder && !usedWrench)
      return;
    float y = ent.Comp.MaxIntegrity * ent.Comp.RepairCapFraction;
    if ((double) ent.Comp.Integrity >= (double) y - (double) ent.Comp.FrameRepairEpsilon)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-hardpoint-intact"), ent.Owner, new EntityUid?(args.User), PopupType.SmallCaution);
      args.Handled = true;
    }
    else if (ent.Comp.Repairing)
    {
      args.Handled = true;
    }
    else
    {
      float num = MathF.Min(ent.Comp.MaxIntegrity * ent.Comp.FrameWeldCapFraction, y);
      if (usedWelder & isFrame && (double) ent.Comp.Integrity >= (double) num - (double) ent.Comp.FrameRepairEpsilon)
      {
        this._popup.PopupClient("Finish tightening the frame with a wrench.", ent.Owner, new EntityUid?(args.User), PopupType.SmallCaution);
        args.Handled = true;
      }
      else if (usedWrench && (double) ent.Comp.Integrity < (double) num - (double) ent.Comp.FrameRepairEpsilon)
      {
        this._popup.PopupClient("Weld the frame before tightening it.", ent.Owner, new EntityUid?(args.User), PopupType.SmallCaution);
        args.Handled = true;
      }
      else if (usedWelder && !this._repairable.UseFuel(used, args.User, ent.Comp.RepairFuelCost, true))
      {
        args.Handled = true;
      }
      else
      {
        float amountForCurrentStep = this.GetRepairAmountForCurrentStep(ent.Owner, ent.Comp, usedWelder, usedWrench, isFrame);
        if ((double) amountForCurrentStep <= 0.0)
        {
          args.Handled = true;
        }
        else
        {
          float timeForCurrentStep = this.GetRepairTimeForCurrentStep(ent.Owner, args.User, ent.Comp, amountForCurrentStep, isFrame);
          ent.Comp.Repairing = true;
          if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, timeForCurrentStep, (DoAfterEvent) new HardpointRepairDoAfterEvent(), new EntityUid?(ent.Owner), new EntityUid?(ent.Owner), new EntityUid?(used))
          {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true
          }))
            ent.Comp.Repairing = false;
          else
            args.Handled = true;
        }
      }
    }
  }

  private void OnHardpointRepairDoAfter(
    Entity<HardpointIntegrityComponent> ent,
    ref HardpointRepairDoAfterEvent args)
  {
    ent.Comp.Repairing = false;
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid? used = args.Used;
    bool isFrame = this.HasComp<HardpointSlotsComponent>(ent.Owner);
    bool usedWelder = used.HasValue && this._tool.HasQuality(used.Value, (string) ent.Comp.RepairToolQuality) && this.HasComp<BlowtorchComponent>(used);
    bool usedWrench = isFrame && used.HasValue && this._tool.HasQuality(used.Value, (string) ent.Comp.FrameFinishToolQuality);
    if (!usedWelder && !usedWrench || usedWelder && (!used.HasValue || !this._repairable.UseFuel(used.Value, args.User, ent.Comp.RepairFuelCost)))
      return;
    float amountForCurrentStep = this.GetRepairAmountForCurrentStep(ent.Owner, ent.Comp, usedWelder, usedWrench, isFrame);
    if ((double) amountForCurrentStep <= 0.0)
      return;
    float x = ent.Comp.MaxIntegrity * ent.Comp.RepairCapFraction;
    ent.Comp.Integrity = MathF.Min(x, ent.Comp.Integrity + amountForCurrentStep);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
    this.UpdateFrameDamageAppearance(ent.Owner, ent.Comp);
    if (ent.Comp.RepairSound != null)
      this._audio.PlayPredicted(ent.Comp.RepairSound, ent.Owner, new EntityUid?(args.User));
    this._popup.PopupClient(this.Loc.GetString("rmc-hardpoint-repaired"), ent.Owner, new EntityUid?(args.User));
    EntityUid entityUid = ent.Owner;
    if (this.TryComp<VehicleWheelItemComponent>(ent.Owner, out VehicleWheelItemComponent _))
    {
      entityUid = this.GetVehicleFromPart(ent.Owner) ?? ent.Owner;
      this._wheels.OnWheelDamaged(entityUid);
    }
    else
      this.RefreshCanRun(ent.Owner);
    if (ent.Comp.BypassEntryOnZero)
      this.RefreshCanRun(entityUid);
    this.UpdateHardpointUi(entityUid);
    if (!this.ShouldRepeatRepair(ent.Owner, ent.Comp, usedWelder, usedWrench, isFrame))
      return;
    args.Repeat = true;
  }

  private float GetRepairAmountForCurrentStep(
    EntityUid uid,
    HardpointIntegrityComponent integrity,
    bool usedWelder,
    bool usedWrench,
    bool isFrame)
  {
    if ((double) integrity.MaxIntegrity <= 0.0)
      return 0.0f;
    float x = MathF.Max(integrity.RepairChunkMinimum, integrity.MaxIntegrity * integrity.RepairChunkFraction);
    float y = integrity.MaxIntegrity * integrity.RepairCapFraction;
    float num1 = MathF.Min(integrity.MaxIntegrity * integrity.FrameWeldCapFraction, y);
    if (usedWelder)
    {
      float num2 = isFrame ? num1 : y;
      return !isFrame ? MathF.Max(0.0f, num2 - integrity.Integrity) : MathF.Max(0.0f, MathF.Min(x, num2 - integrity.Integrity));
    }
    return usedWrench ? MathF.Max(0.0f, MathF.Min(x, y - integrity.Integrity)) : 0.0f;
  }

  private float GetRepairTimeForCurrentStep(
    EntityUid uid,
    EntityUid user,
    HardpointIntegrityComponent integrity,
    float repairAmount,
    bool isFrame)
  {
    if ((double) integrity.MaxIntegrity <= 0.0 || (double) repairAmount <= 0.0)
      return 0.0f;
    float num = repairAmount / integrity.MaxIntegrity;
    float skillDelayMultiplier = this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user, HardpointSystem.EngineerSkill);
    return isFrame ? integrity.FrameRepairChunkSeconds * (num / integrity.RepairChunkFraction) * skillDelayMultiplier : integrity.ModuleRepairSeconds * skillDelayMultiplier;
  }

  private bool ShouldRepeatRepair(
    EntityUid uid,
    HardpointIntegrityComponent integrity,
    bool usedWelder,
    bool usedWrench,
    bool isFrame)
  {
    float y = integrity.MaxIntegrity * integrity.RepairCapFraction;
    if ((double) integrity.Integrity >= (double) y - (double) integrity.FrameRepairEpsilon)
      return false;
    if (isFrame)
    {
      float num = MathF.Min(integrity.MaxIntegrity * integrity.FrameWeldCapFraction, y);
      if (usedWelder)
        return (double) integrity.Integrity < (double) num - (double) integrity.FrameRepairEpsilon;
      return usedWrench && (double) integrity.Integrity >= (double) num - (double) integrity.FrameRepairEpsilon && (double) integrity.Integrity < (double) y;
    }
    return usedWelder && (double) integrity.Integrity > 0.0 && (double) integrity.Integrity < (double) y;
  }

  private EntityUid? GetVehicleFromPart(EntityUid part)
  {
    BaseContainer container;
    return !this._containers.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) part, out container) ? new EntityUid?() : new EntityUid?(container.Owner);
  }

  internal void UpdateHardpointUi(
    EntityUid uid,
    HardpointSlotsComponent? component = null,
    ItemSlotsComponent? itemSlots = null)
  {
    if (this._net.IsClient || !this.Resolve<HardpointSlotsComponent>(uid, ref component, false) || !this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return;
    List<HardpointUiEntry> hardpointUiEntryList = new List<HardpointUiEntry>(component.Slots.Count);
    float frameIntegrity = 0.0f;
    float frameMaxIntegrity = 0.0f;
    bool hasFrameIntegrity = false;
    HardpointIntegrityComponent comp1;
    if (this.TryComp<HardpointIntegrityComponent>(uid, out comp1))
    {
      frameIntegrity = comp1.Integrity;
      frameMaxIntegrity = comp1.MaxIntegrity;
      hasFrameIntegrity = true;
    }
    foreach (HardpointSlot slot in component.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        ItemSlot itemSlot;
        bool hasItem = this._itemSlots.TryGetSlot(uid, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem;
        string installedName = (string) null;
        NetEntity? installedEntity = new NetEntity?();
        float integrity = 0.0f;
        float maxIntegrity = 0.0f;
        bool hasIntegrity = false;
        EntityUid? nullable;
        if (hasItem)
        {
          nullable = itemSlot.Item;
          if (nullable.HasValue)
          {
            EntityUid valueOrDefault = nullable.GetValueOrDefault();
            installedEntity = new NetEntity?(this.GetNetEntity(valueOrDefault));
            installedName = this.Name(valueOrDefault);
            HardpointIntegrityComponent comp2;
            if (this.TryComp<HardpointIntegrityComponent>(valueOrDefault, out comp2))
            {
              integrity = comp2.Integrity;
              maxIntegrity = comp2.MaxIntegrity;
              hasIntegrity = true;
            }
          }
        }
        hardpointUiEntryList.Add(new HardpointUiEntry(slot.Id, slot.HardpointType, installedName, installedEntity, integrity, maxIntegrity, hasIntegrity, hasItem, slot.Required, component.PendingRemovals.Contains(slot.Id)));
        if (hasItem)
        {
          nullable = (EntityUid?) itemSlot?.Item;
          if (nullable.HasValue)
          {
            EntityUid valueOrDefault = nullable.GetValueOrDefault();
            HardpointSlotsComponent comp3;
            ItemSlotsComponent comp4;
            if (this.TryComp<HardpointSlotsComponent>(valueOrDefault, out comp3) && this.TryComp<ItemSlotsComponent>(valueOrDefault, out comp4))
              this.AppendTurretEntries(hardpointUiEntryList, slot.Id, valueOrDefault, comp3, comp4);
          }
        }
      }
    }
    this._ui.SetUiState((Entity<UserInterfaceComponent>) uid, (Enum) HardpointUiKey.Key, (BoundUserInterfaceState) new HardpointBoundUserInterfaceState(hardpointUiEntryList, frameIntegrity, frameMaxIntegrity, hasFrameIntegrity, component.LastUiError));
  }

  internal bool HasAttachedHardpoints(
    EntityUid owner,
    HardpointSlotsComponent slots,
    ItemSlotsComponent itemSlots)
  {
    foreach (HardpointSlot slot in slots.Slots)
    {
      ItemSlot itemSlot;
      if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(owner, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem)
        return true;
    }
    return false;
  }

  private void AppendTurretEntries(
    List<HardpointUiEntry> entries,
    string parentSlotId,
    EntityUid turretUid,
    HardpointSlotsComponent turretSlots,
    ItemSlotsComponent turretItemSlots)
  {
    foreach (HardpointSlot slot in turretSlots.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        string slotId = VehicleTurretSlotIds.Compose(parentSlotId, slot.Id);
        ItemSlot itemSlot;
        bool hasItem = this._itemSlots.TryGetSlot(turretUid, slot.Id, out itemSlot, turretItemSlots) && itemSlot.HasItem;
        string installedName = (string) null;
        NetEntity? installedEntity = new NetEntity?();
        float integrity = 0.0f;
        float maxIntegrity = 0.0f;
        bool hasIntegrity = false;
        if (hasItem)
        {
          EntityUid? nullable = itemSlot.Item;
          if (nullable.HasValue)
          {
            EntityUid valueOrDefault = nullable.GetValueOrDefault();
            installedEntity = new NetEntity?(this.GetNetEntity(valueOrDefault));
            installedName = this.Name(valueOrDefault);
            HardpointIntegrityComponent comp;
            if (this.TryComp<HardpointIntegrityComponent>(valueOrDefault, out comp))
            {
              integrity = comp.Integrity;
              maxIntegrity = comp.MaxIntegrity;
              hasIntegrity = true;
            }
          }
        }
        entries.Add(new HardpointUiEntry(slotId, slot.HardpointType, installedName, installedEntity, integrity, maxIntegrity, hasIntegrity, hasItem, slot.Required, turretSlots.PendingRemovals.Contains(slot.Id)));
      }
    }
  }

  internal void UpdateContainingVehicleUi(EntityUid owner)
  {
    EntityUid vehicle;
    if (!this.TryGetContainingVehicleFrame(owner, out vehicle))
      return;
    this.UpdateHardpointUi(vehicle);
  }

  internal void SetContainingVehicleUiError(EntityUid owner, string? error)
  {
    EntityUid vehicle;
    HardpointSlotsComponent comp;
    if (!this.TryGetContainingVehicleFrame(owner, out vehicle) || !this.TryComp<HardpointSlotsComponent>(vehicle, out comp))
      return;
    comp.LastUiError = error;
  }

  internal bool TryGetContainingVehicleFrame(EntityUid owner, out EntityUid vehicle)
  {
    return this._topology.TryGetVehicle(owner, out vehicle);
  }

  private void UpdateFrameDamageAppearance(EntityUid uid, HardpointIntegrityComponent component)
  {
    AppearanceComponent comp;
    if (this._net.IsClient || !this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    float num1 = (double) component.MaxIntegrity > 0.0 ? component.MaxIntegrity : 1f;
    float num2 = Math.Clamp((double) num1 > 0.0 ? component.Integrity / num1 : 1f, 0.0f, 1f);
    this._appearance.SetData(uid, (Enum) VehicleFrameDamageVisuals.IntegrityFraction, (object) num2, comp);
  }

  internal bool TryGetPryingTool(
    EntityUid user,
    ProtoId<ToolQualityPrototype> quality,
    out EntityUid tool)
  {
    tool = new EntityUid();
    HandsComponent comp1;
    if (!this.TryComp<HandsComponent>(user, out comp1))
      return false;
    string activeHand = this._hands.GetActiveHand((Entity<HandsComponent>) (user, comp1));
    EntityUid? held;
    ToolComponent comp2;
    if (activeHand == null || !this._hands.TryGetHeldItem((Entity<HandsComponent>) (user, comp1), activeHand, out held) || !this.TryComp<ToolComponent>(held.Value, out comp2) || !this._tool.HasQuality(held.Value, (string) quality, comp2))
      return false;
    tool = held.Value;
    return true;
  }
}
