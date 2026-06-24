// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleSpotlightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Input;
using Content.Shared.Vehicle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleSpotlightSystem : EntitySystem
{
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly VehicleSystem _rmcVehicles;
  [Dependency]
  private readonly SharedPointLightSystem _lights;
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleSpotlightComponent, ComponentStartup>(new EntityEventRefHandler<VehicleSpotlightComponent, ComponentStartup>(this.OnSpotlightStartup));
    this.SubscribeLocalEvent<HardpointSlotsChangedEvent>(new EntityEventHandler<HardpointSlotsChangedEvent>(this.OnHardpointSlotsChanged));
    if (this._net.IsClient)
      CommandBinds.Builder.Bind(ContentKeyFunctions.FlipObject, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
      {
        EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
        if (!attachedEntity.HasValue)
          return;
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        EntityUid? nullable = new EntityUid?();
        VehicleOperatorComponent comp;
        if (this.TryComp<VehicleOperatorComponent>(valueOrDefault, out comp) && comp.Vehicle.HasValue)
        {
          nullable = new EntityUid?(comp.Vehicle.Value);
        }
        else
        {
          EntityUid? vehicle;
          if (this._rmcVehicles.TryGetVehicleFromInterior(valueOrDefault, out vehicle) && vehicle.HasValue)
            nullable = new EntityUid?(vehicle.Value);
        }
        if (!nullable.HasValue)
          return;
        this.RaiseNetworkEvent((EntityEventArgs) new VehicleSpotlightToggleRequestEvent(this.GetNetEntity(nullable.Value)));
      }))).Register<VehicleSpotlightSystem>();
    this.SubscribeNetworkEvent<VehicleSpotlightToggleRequestEvent>(new EntitySessionEventHandler<VehicleSpotlightToggleRequestEvent>(this.OnSpotlightToggleRequest));
  }

  private void OnSpotlightStartup(Entity<VehicleSpotlightComponent> ent, ref ComponentStartup args)
  {
    VehicleSpotlightSystem.EnsureBase(ent.Comp);
    if (this._net.IsServer)
      this.RecalculateFromHardpoints(ent.Owner, ent.Comp);
    this.ApplySpotlight(ent.Owner, ent.Comp);
  }

  private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
  {
    VehicleSpotlightComponent comp;
    if (!this._net.IsServer || !this.TryComp<VehicleSpotlightComponent>(args.Vehicle, out comp))
      return;
    this.RecalculateFromHardpoints(args.Vehicle, comp);
    this.ApplySpotlight(args.Vehicle, comp);
    this.Dirty(args.Vehicle, (IComponent) comp);
  }

  private void OnSpotlightToggleRequest(
    VehicleSpotlightToggleRequestEvent ev,
    EntitySessionEventArgs args)
  {
    if (!this._net.IsServer)
      return;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid entity = this.GetEntity(ev.Vehicle);
    VehicleComponent comp1;
    if (!this.TryComp<VehicleComponent>(entity, out comp1))
      return;
    attachedEntity = comp1.Operator;
    EntityUid entityUid = valueOrDefault;
    VehicleSpotlightComponent comp2;
    if ((attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || !this.TryComp<VehicleSpotlightComponent>(entity, out comp2))
      return;
    comp2.Enabled = !comp2.Enabled;
    this.ApplySpotlight(entity, comp2);
    this.Dirty(entity, (IComponent) comp2);
  }

  private void ApplySpotlight(EntityUid uid, VehicleSpotlightComponent spotlight)
  {
    SharedPointLightComponent component = (SharedPointLightComponent) null;
    if (!this._lights.ResolveLight(uid, ref component))
      return;
    this._lights.SetRadius(uid, spotlight.Radius, component);
    this._lights.SetEnergy(uid, spotlight.Energy, component);
    this._lights.SetSoftness(uid, spotlight.Softness, component);
    this._lights.SetEnabled(uid, spotlight.Enabled, component);
  }

  private static void EnsureBase(VehicleSpotlightComponent spotlight)
  {
    if (spotlight.BaseInitialized)
      return;
    spotlight.BaseInitialized = true;
    spotlight.BaseRadius = spotlight.Radius;
    spotlight.BaseEnergy = spotlight.Energy;
    spotlight.BaseSoftness = spotlight.Softness;
  }

  private void RecalculateFromHardpoints(
    EntityUid vehicle,
    VehicleSpotlightComponent spotlight,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null)
  {
    VehicleSpotlightSystem.EnsureBase(spotlight);
    float num1 = spotlight.BaseRadius;
    float num2 = spotlight.BaseEnergy;
    float num3 = spotlight.BaseSoftness;
    if (this.Resolve<HardpointSlotsComponent, ItemSlotsComponent>(vehicle, ref hardpoints, ref itemSlots, false))
    {
      foreach (HardpointSlot slot in hardpoints.Slots)
      {
        ItemSlot itemSlot;
        VehicleSpotlightModifierComponent comp;
        if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(vehicle, slot.Id, out itemSlot, itemSlots) && itemSlot.HasItem && this.TryComp<VehicleSpotlightModifierComponent>(itemSlot.Item.Value, out comp))
        {
          num1 = num1 * comp.RadiusMultiplier + comp.RadiusAdd;
          num2 = num2 * comp.EnergyMultiplier + comp.EnergyAdd;
          num3 = num3 * comp.SoftnessMultiplier + comp.SoftnessAdd;
        }
      }
    }
    spotlight.Radius = num1;
    spotlight.Energy = num2;
    spotlight.Softness = num3;
  }
}
