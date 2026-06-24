using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Station;
using Content.Shared.DisplacementMap;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Content.Shared.Whitelist;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Humanoid;

public sealed class RMCHumanoidAppearanceSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedRMCStationSpawningSystem _rmcStationSpawning;

	private EntityUid? _spawnMap;

	public bool HidePlayerIdentities { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PlayerSpawnCompleteEvent>((EntityEventHandler<PlayerSpawnCompleteEvent>)OnPlayerSpawnComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.HidePlayerIdentities, (Action<bool>)OnHidePlayerIdentitiesChanged, true);
	}

	private void OnRoundRestart(RoundRestartCleanupEvent ev)
	{
		_spawnMap = null;
	}

	private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		HumanoidAppearanceComponent appearance = default(HumanoidAppearanceComponent);
		if (((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(ev.Mob, ref appearance))
		{
			if (!_spawnMap.HasValue || ((EntitySystem)this).TerminatingOrDeleted(_spawnMap, (MetaDataComponent)null))
			{
				_spawnMap = _map.CreateMap(true);
			}
			EntityCoordinates coords = default(EntityCoordinates);
			((EntityCoordinates)(ref coords))._002Ector(_spawnMap.Value, Vector2.Zero);
			HumanoidCharacterProfile profile = HumanoidCharacterProfile.RandomWithSpecies(ProtoId<SpeciesPrototype>.op_Implicit(appearance.Species));
			EntityUid? random = _rmcStationSpawning.SpawnPlayerMob(coords, null, profile, null);
			HumanoidAppearanceComponent fakeLook = default(HumanoidAppearanceComponent);
			if (((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(random, ref fakeLook))
			{
				HiddenAppearanceComponent hidden = ((EntitySystem)this).EnsureComp<HiddenAppearanceComponent>(ev.Mob);
				hidden.Appearance = new RMCHumanoidAppearance
				{
					ClientOldMarkings = new MarkingSet(fakeLook.ClientOldMarkings),
					MarkingSet = new MarkingSet(fakeLook.MarkingSet),
					BaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>(fakeLook.BaseLayers),
					PermanentlyHidden = new HashSet<HumanoidVisualLayers>(fakeLook.PermanentlyHidden),
					Gender = fakeLook.Gender,
					Age = fakeLook.Age,
					CustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>(fakeLook.CustomBaseLayers),
					Species = fakeLook.Species,
					SkinColor = fakeLook.SkinColor,
					HiddenLayers = new Dictionary<HumanoidVisualLayers, SlotFlags>(fakeLook.HiddenLayers),
					Sex = fakeLook.Sex,
					EyeColor = fakeLook.EyeColor,
					CachedHairColor = fakeLook.CachedHairColor,
					CachedFacialHairColor = fakeLook.CachedFacialHairColor,
					HideLayersOnEquip = new HashSet<HumanoidVisualLayers>(fakeLook.HideLayersOnEquip),
					UndergarmentTop = fakeLook.UndergarmentTop,
					UndergarmentBottom = fakeLook.UndergarmentBottom,
					MarkingsDisplacement = new Dictionary<HumanoidVisualLayers, DisplacementData>(fakeLook.MarkingsDisplacement)
				};
				((EntitySystem)this).Dirty(ev.Mob, (IComponent)(object)hidden, (MetaDataComponent)null);
				((EntitySystem)this).QueueDel(random);
			}
		}
	}

	private void OnHidePlayerIdentitiesChanged(bool value)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		HidePlayerIdentities = value;
		if (!HidePlayerIdentities)
		{
			EntityQueryEnumerator<HiddenAppearanceComponent> query = ((EntitySystem)this).EntityQueryEnumerator<HiddenAppearanceComponent>();
			EntityUid uid = default(EntityUid);
			HiddenAppearanceComponent hiddenAppearanceComponent = default(HiddenAppearanceComponent);
			while (query.MoveNext(ref uid, ref hiddenAppearanceComponent))
			{
				((EntitySystem)this).RemCompDeferred<HiddenAppearanceComponent>(uid);
			}
		}
	}

	public bool TryGetLocalHiddenAppearance(EntityUid ent, [NotNullWhen(true)] out IRMCHumanoidAppearance? appearance)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		appearance = null;
		HiddenAppearanceComponent hiddenComp = default(HiddenAppearanceComponent);
		if (!((EntitySystem)this).TryComp<HiddenAppearanceComponent>(ent, ref hiddenComp) || hiddenComp.Appearance == null)
		{
			return false;
		}
		appearance = hiddenComp.Appearance;
		EntityUid? localEntity = _player.LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid player = localEntity.GetValueOrDefault();
			return _entityWhitelist.IsWhitelistPass(hiddenComp.Whitelist, player);
		}
		return false;
	}
}
