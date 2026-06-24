// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Item.ItemCamouflageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Survivor;
using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Content.Shared.Station;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Item;

public sealed class ItemCamouflageSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedStationSpawningSystem _stationSpawning;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private INetManager _net;
  private readonly Queue<Entity<ItemCamouflageComponent>> _items = new Queue<Entity<ItemCamouflageComponent>>();

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public CamouflageType CurrentMapCamouflage { get; set; } = CamouflageType.Jungle;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawnComplete), after: new Type[1]
    {
      typeof (SurvivorSystem)
    });
    this.SubscribeLocalEvent<ItemCamouflageComponent, MapInitEvent>(new EntityEventRefHandler<ItemCamouflageComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
  {
    if (ev.JobId == null)
      return;
    EntityUid mob = ev.Mob;
    JobPrototype prototype1;
    ProtoId<StartingGearPrototype> id;
    if (!this._prototypes.TryIndex<JobPrototype>(ev.JobId, out prototype1) || prototype1.CamouflageStartingGear == null || !prototype1.CamouflageStartingGear.TryGetValue(this.CurrentMapCamouflage, out id))
      return;
    StartingGearPrototype prototype2;
    this._prototypes.TryIndex<StartingGearPrototype>(id, out prototype2);
    this.EquipMapCamoGear(mob, (IEquipmentLoadout) prototype2);
  }

  private void EquipMapCamoGear(EntityUid mob, IEquipmentLoadout? startingGear)
  {
    if (startingGear == null)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) mob);
    EntityUid entityUid;
    SlotDefinition slot;
    while (slotEnumerator.NextItem(out entityUid, out slot))
    {
      if (!string.IsNullOrEmpty(startingGear.GetGear(slot.Name)))
      {
        this._inventory.TryUnequip(mob, slot.Name, true, true);
        this.QueueDel(new EntityUid?(entityUid));
      }
    }
    this._stationSpawning.EquipStartingGear(mob, startingGear);
  }

  private void OnMapInit(Entity<ItemCamouflageComponent> ent, ref MapInitEvent args)
  {
    if (this._net.IsClient)
      return;
    this._items.Enqueue(ent);
  }

  public override void Update(float frameTime)
  {
    if (this._items.Count == 0)
      return;
    Entity<ItemCamouflageComponent> result;
    while (this._items.TryDequeue(out result))
      this._appearance.SetData((EntityUid) result, (Enum) ItemCamouflageVisuals.Camo, (object) this.CurrentMapCamouflage);
  }
}
