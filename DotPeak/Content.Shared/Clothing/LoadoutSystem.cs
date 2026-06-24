// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.LoadoutSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Systems;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Station;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Clothing;

public sealed class LoadoutSystem : EntitySystem
{
  [Dependency]
  private ActorSystem _actors;
  [Dependency]
  private SharedStationSpawningSystem _station;
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private IRobustRandom _random;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LoadoutComponent, MapInitEvent>(new ComponentEventHandler<LoadoutComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, new Type[1]
    {
      typeof (SharedBodySystem)
    });
  }

  public static string GetJobPrototype(string? loadout)
  {
    return string.IsNullOrEmpty(loadout) ? string.Empty : "Job" + loadout;
  }

  public EntProtoId? GetFirstOrNull(LoadoutPrototype loadout)
  {
    EntProtoId? firstOrNull = new EntProtoId?();
    StartingGearPrototype gear;
    if (this._protoMan.TryIndex<StartingGearPrototype>(loadout.StartingGear, ref gear))
      firstOrNull = this.GetFirstOrNull((IEquipmentLoadout) gear);
    if (!firstOrNull.HasValue)
      firstOrNull = this.GetFirstOrNull((IEquipmentLoadout) loadout);
    return firstOrNull;
  }

  public EntProtoId? GetFirstOrNull(IEquipmentLoadout? gear)
  {
    if (gear == null)
      return new EntProtoId?();
    if (gear.Equipment.Count + gear.Inhand.Count + gear.Storage.Values.Sum<List<EntProtoId>>((Func<List<EntProtoId>, int>) (x => x.Count)) == 1)
    {
      EntityPrototype entityPrototype;
      if (gear.Equipment.Count == 1 && this._protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Equipment.Values.First<EntProtoId>()), ref entityPrototype) || gear.Inhand.Count == 1 && this._protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Inhand[0]), ref entityPrototype))
        return EntProtoId.op_Implicit(entityPrototype.ID);
      foreach (List<EntProtoId> entProtoIdList in gear.Storage.Values)
      {
        using (List<EntProtoId>.Enumerator enumerator = entProtoIdList.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return new EntProtoId?(enumerator.Current);
        }
      }
    }
    return new EntProtoId?();
  }

  public string GetName(LoadoutPrototype loadout)
  {
    if (loadout.DummyEntity.HasValue)
    {
      IPrototypeManager protoMan = this._protoMan;
      EntProtoId? dummyEntity = loadout.DummyEntity;
      string str = dummyEntity.HasValue ? EntProtoId.op_Implicit(dummyEntity.GetValueOrDefault()) : (string) null;
      EntityPrototype entityPrototype;
      ref EntityPrototype local = ref entityPrototype;
      if (protoMan.TryIndex<EntityPrototype>(str, ref local))
        return entityPrototype.Name;
    }
    StartingGearPrototype gear;
    return this._protoMan.TryIndex<StartingGearPrototype>(loadout.StartingGear, ref gear) ? this.GetName((IEquipmentLoadout) gear) : this.GetName((IEquipmentLoadout) loadout);
  }

  public string GetName(IEquipmentLoadout? gear)
  {
    if (gear == null)
      return string.Empty;
    if (gear.Equipment.Count + gear.Storage.Values.Sum<List<EntProtoId>>((Func<List<EntProtoId>, int>) (o => o.Count)) + gear.Inhand.Count == 1)
    {
      EntityPrototype entityPrototype;
      if (gear.Equipment.Count == 1 && this._protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Equipment.Values.First<EntProtoId>()), ref entityPrototype) || gear.Inhand.Count == 1 && this._protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Inhand[0]), ref entityPrototype))
        return entityPrototype.Name;
      foreach (List<EntProtoId> entProtoIdList in gear.Storage.Values)
      {
        if (entProtoIdList.Count == 1)
        {
          if (this._protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(entProtoIdList[0]), ref entityPrototype))
            return entityPrototype.Name;
          break;
        }
      }
    }
    return this.Loc.GetString("unknown");
  }

  private void OnMapInit(EntityUid uid, LoadoutComponent component, MapInitEvent args)
  {
    this.Equip(uid, component.StartingGear, component.RoleLoadout);
  }

  public void Equip(
    EntityUid uid,
    List<ProtoId<StartingGearPrototype>>? startingGear,
    List<ProtoId<RoleLoadoutPrototype>>? loadoutGroups)
  {
    if (startingGear != null && startingGear.Count > 0)
      this._station.EquipStartingGear(uid, new ProtoId<StartingGearPrototype>?(RandomExtensions.Pick<ProtoId<StartingGearPrototype>>(this._random, (IReadOnlyList<ProtoId<StartingGearPrototype>>) startingGear)));
    if (loadoutGroups == null)
    {
      this.GearEquipped(uid);
    }
    else
    {
      ProtoId<RoleLoadoutPrototype> role = RandomExtensions.Pick<ProtoId<RoleLoadoutPrototype>>(this._random, (IReadOnlyList<ProtoId<RoleLoadoutPrototype>>) loadoutGroups);
      RoleLoadoutPrototype roleProto = this._protoMan.Index<RoleLoadoutPrototype>(role);
      RoleLoadout loadout = new RoleLoadout(role);
      loadout.SetDefault(this.GetProfile(new EntityUid?(uid)), this._actors.GetSession(new EntityUid?(uid)), this._protoMan, true);
      this._station.EquipRoleLoadout(uid, loadout, roleProto);
      this.GearEquipped(uid);
    }
  }

  public void GearEquipped(EntityUid uid)
  {
    StartingGearEquippedEvent gearEquippedEvent = new StartingGearEquippedEvent(uid);
    this.RaiseLocalEvent<StartingGearEquippedEvent>(uid, ref gearEquippedEvent, false);
  }

  public HumanoidCharacterProfile GetProfile(EntityUid? uid)
  {
    HumanoidAppearanceComponent appearanceComponent;
    return this.TryComp<HumanoidAppearanceComponent>(uid, ref appearanceComponent) ? HumanoidCharacterProfile.DefaultWithSpecies(ProtoId<SpeciesPrototype>.op_Implicit(appearanceComponent.Species)) : HumanoidCharacterProfile.Random();
  }
}
