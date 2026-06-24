// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleHardpointVisualsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleHardpointVisualsSystem : EntitySystem
{
  [Dependency]
  private readonly ItemSlotsSystem _itemSlots;
  [Dependency]
  private readonly INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentInit>(new EntityEventRefHandler<VehicleHardpointVisualsComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<VehicleHardpointVisualsComponent, MapInitEvent>(new EntityEventRefHandler<VehicleHardpointVisualsComponent, MapInitEvent>(this.OnInit));
    this.SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentGetState>(new EntityEventRefHandler<VehicleHardpointVisualsComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<HardpointSlotsChangedEvent>(new EntityEventHandler<HardpointSlotsChangedEvent>(this.OnHardpointSlotsChanged));
  }

  private void OnInit(Entity<VehicleHardpointVisualsComponent> ent, ref ComponentInit args)
  {
    if (this._net.IsClient)
      return;
    this.UpdateAppearance(ent.Owner);
  }

  private void OnInit(Entity<VehicleHardpointVisualsComponent> ent, ref MapInitEvent args)
  {
    if (this._net.IsClient)
      return;
    this.UpdateAppearance(ent.Owner);
  }

  private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
  {
    if (this._net.IsClient || !this.HasComp<VehicleHardpointVisualsComponent>(args.Vehicle))
      return;
    this.UpdateAppearance(args.Vehicle);
  }

  private void OnGetState(Entity<VehicleHardpointVisualsComponent> ent, ref ComponentGetState args)
  {
    if (this._net.IsClient)
      return;
    List<VehicleHardpointLayerState> layers = new List<VehicleHardpointLayerState>((IEnumerable<VehicleHardpointLayerState>) ent.Comp.Layers);
    args.State = (IComponentState) new VehicleHardpointVisualsComponentState(layers);
  }

  private void UpdateAppearance(
    EntityUid vehicle,
    HardpointSlotsComponent? hardpoints = null,
    ItemSlotsComponent? itemSlots = null,
    VehicleHardpointVisualsComponent? visuals = null)
  {
    if (!this.Resolve<HardpointSlotsComponent, ItemSlotsComponent, VehicleHardpointVisualsComponent>(vehicle, ref hardpoints, ref itemSlots, ref visuals, false))
      return;
    List<VehicleHardpointLayerState> hardpointLayerStateList = new List<VehicleHardpointLayerState>(hardpoints.Slots.Count);
    Dictionary<string, int> dictionary = new Dictionary<string, int>();
    foreach (HardpointSlot slot in hardpoints.Slots)
    {
      if (!string.IsNullOrWhiteSpace(slot.Id))
      {
        string visualLayer = slot.VisualLayer;
        visualLayer.ToLowerInvariant();
        string state = string.Empty;
        bool usesOverlay = false;
        ItemSlot itemSlot;
        if (this._itemSlots.TryGetSlot(vehicle, slot.Id, out itemSlot, itemSlots))
        {
          if (itemSlot.HasItem)
            state = this.ResolveVisualState(itemSlot.Item.Value, out usesOverlay);
          if (!string.IsNullOrWhiteSpace(visualLayer))
          {
            if (usesOverlay)
              state = string.Empty;
            string lowerInvariant = visualLayer.ToLowerInvariant();
            int index;
            if (dictionary.TryGetValue(lowerInvariant, out index))
            {
              if (!string.IsNullOrWhiteSpace(state))
                hardpointLayerStateList[index] = new VehicleHardpointLayerState(visualLayer, state);
            }
            else
            {
              dictionary[lowerInvariant] = hardpointLayerStateList.Count;
              hardpointLayerStateList.Add(new VehicleHardpointLayerState(visualLayer, state));
            }
          }
        }
      }
    }
    if (visuals.Layers.Count == hardpointLayerStateList.Count)
    {
      bool flag = true;
      for (int index = 0; index < visuals.Layers.Count; ++index)
      {
        if (!visuals.Layers[index].Equals(hardpointLayerStateList[index]))
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return;
    }
    visuals.Layers = hardpointLayerStateList;
    this.Dirty(vehicle, (IComponent) visuals);
  }

  private string ResolveVisualState(EntityUid item, out bool usesOverlay, int depth = 0)
  {
    usesOverlay = false;
    if (depth > 2)
      return string.Empty;
    VehicleTurretComponent comp1;
    if (this.TryComp<VehicleTurretComponent>(item, out comp1) && comp1.ShowOverlay)
      usesOverlay = true;
    HardpointSlotsComponent comp2;
    ItemSlotsComponent comp3;
    if (this.TryComp<HardpointSlotsComponent>(item, out comp2) && this.TryComp<ItemSlotsComponent>(item, out comp3))
    {
      foreach (HardpointSlot slot in comp2.Slots)
      {
        ItemSlot itemSlot;
        if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(item, slot.Id, out itemSlot, comp3) && itemSlot.HasItem)
        {
          bool usesOverlay1;
          string str = this.ResolveVisualState(itemSlot.Item.Value, out usesOverlay1, depth + 1);
          usesOverlay |= usesOverlay1;
          if (!string.IsNullOrWhiteSpace(str))
            return str;
        }
      }
    }
    HardpointVisualComponent comp4;
    if (this.TryComp<HardpointVisualComponent>(item, out comp4) && !string.IsNullOrWhiteSpace(comp4.VehicleState))
      return comp4.VehicleState;
    VehicleTurretComponent comp5;
    return this.TryComp<VehicleTurretComponent>(item, out comp5) && !string.IsNullOrWhiteSpace(comp5.OverlayState) ? comp5.OverlayState : string.Empty;
  }
}
