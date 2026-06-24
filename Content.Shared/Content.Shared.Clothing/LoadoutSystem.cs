using System;
using System.Collections.Generic;
using System.Linq;
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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LoadoutComponent, MapInitEvent>((ComponentEventHandler<LoadoutComponent, MapInitEvent>)OnMapInit, (Type[])null, new Type[1] { typeof(SharedBodySystem) });
	}

	public static string GetJobPrototype(string? loadout)
	{
		if (string.IsNullOrEmpty(loadout))
		{
			return string.Empty;
		}
		return "Job" + loadout;
	}

	public EntProtoId? GetFirstOrNull(LoadoutPrototype loadout)
	{
		EntProtoId? proto = null;
		StartingGearPrototype gear = default(StartingGearPrototype);
		if (_protoMan.TryIndex<StartingGearPrototype>(loadout.StartingGear, ref gear))
		{
			proto = GetFirstOrNull(gear);
		}
		EntProtoId? val = proto;
		if (!val.HasValue)
		{
			proto = GetFirstOrNull((IEquipmentLoadout?)loadout);
		}
		return proto;
	}

	public EntProtoId? GetFirstOrNull(IEquipmentLoadout? gear)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (gear == null)
		{
			return null;
		}
		if (gear.Equipment.Count + gear.Inhand.Count + gear.Storage.Values.Sum((List<EntProtoId> x) => x.Count) == 1)
		{
			EntityPrototype proto = default(EntityPrototype);
			if (gear.Equipment.Count == 1 && _protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Equipment.Values.First()), ref proto))
			{
				return EntProtoId.op_Implicit(proto.ID);
			}
			if (gear.Inhand.Count == 1 && _protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Inhand[0]), ref proto))
			{
				return EntProtoId.op_Implicit(proto.ID);
			}
			foreach (List<EntProtoId> value in gear.Storage.Values)
			{
				using List<EntProtoId>.Enumerator enumerator2 = value.GetEnumerator();
				if (enumerator2.MoveNext())
				{
					return enumerator2.Current;
				}
			}
		}
		return null;
	}

	public string GetName(LoadoutPrototype loadout)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? dummyEntity = loadout.DummyEntity;
		if (dummyEntity.HasValue)
		{
			IPrototypeManager protoMan = _protoMan;
			dummyEntity = loadout.DummyEntity;
			EntityPrototype proto = default(EntityPrototype);
			if (protoMan.TryIndex<EntityPrototype>(dummyEntity.HasValue ? EntProtoId.op_Implicit(dummyEntity.GetValueOrDefault()) : null, ref proto))
			{
				return proto.Name;
			}
		}
		StartingGearPrototype gear = default(StartingGearPrototype);
		if (_protoMan.TryIndex<StartingGearPrototype>(loadout.StartingGear, ref gear))
		{
			return GetName(gear);
		}
		return GetName((IEquipmentLoadout?)loadout);
	}

	public string GetName(IEquipmentLoadout? gear)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if (gear == null)
		{
			return string.Empty;
		}
		if (gear.Equipment.Count + gear.Storage.Values.Sum((List<EntProtoId> o) => o.Count) + gear.Inhand.Count == 1)
		{
			EntityPrototype proto = default(EntityPrototype);
			if (gear.Equipment.Count == 1 && _protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Equipment.Values.First()), ref proto))
			{
				return proto.Name;
			}
			if (gear.Inhand.Count == 1 && _protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(gear.Inhand[0]), ref proto))
			{
				return proto.Name;
			}
			foreach (List<EntProtoId> values in gear.Storage.Values)
			{
				if (values.Count == 1)
				{
					if (_protoMan.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(values[0]), ref proto))
					{
						return proto.Name;
					}
					break;
				}
			}
		}
		return base.Loc.GetString("unknown");
	}

	private void OnMapInit(EntityUid uid, LoadoutComponent component, MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Equip(uid, component.StartingGear, component.RoleLoadout);
	}

	public void Equip(EntityUid uid, List<ProtoId<StartingGearPrototype>>? startingGear, List<ProtoId<RoleLoadoutPrototype>>? loadoutGroups)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (startingGear != null && startingGear.Count > 0)
		{
			_station.EquipStartingGear(uid, RandomExtensions.Pick<ProtoId<StartingGearPrototype>>(_random, (IReadOnlyList<ProtoId<StartingGearPrototype>>)startingGear));
		}
		if (loadoutGroups == null)
		{
			GearEquipped(uid);
			return;
		}
		ProtoId<RoleLoadoutPrototype> id = RandomExtensions.Pick<ProtoId<RoleLoadoutPrototype>>(_random, (IReadOnlyList<ProtoId<RoleLoadoutPrototype>>)loadoutGroups);
		RoleLoadoutPrototype proto = _protoMan.Index<RoleLoadoutPrototype>(id);
		RoleLoadout loadout = new RoleLoadout(id);
		loadout.SetDefault(GetProfile(uid), _actors.GetSession((EntityUid?)uid), _protoMan, force: true);
		_station.EquipRoleLoadout(uid, loadout, proto);
		GearEquipped(uid);
	}

	public void GearEquipped(EntityUid uid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		StartingGearEquippedEvent ev = new StartingGearEquippedEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<StartingGearEquippedEvent>(uid, ref ev, false);
	}

	public HumanoidCharacterProfile GetProfile(EntityUid? uid)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		HumanoidAppearanceComponent appearance = default(HumanoidAppearanceComponent);
		if (((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(uid, ref appearance))
		{
			return HumanoidCharacterProfile.DefaultWithSpecies(ProtoId<SpeciesPrototype>.op_Implicit(appearance.Species));
		}
		return HumanoidCharacterProfile.Random();
	}
}
