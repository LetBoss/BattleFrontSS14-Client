using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Light;
using Content.Shared.Dataset;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Weather;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weather;

public sealed class RMCWeatherSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedRoofSystem _roof;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedWeatherSystem _weather;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private RMCAmbientLightSystem _rmcLight;

	[Dependency]
	private SharedAudioSystem _audio;

	private EntityQuery<BlockWeatherComponent> _blockQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_blockQuery = ((EntitySystem)this).GetEntityQuery<BlockWeatherComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCWeatherCycleComponent, MapInitEvent>((EntityEventRefHandler<RMCWeatherCycleComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<RMCWeatherCycleComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.WeatherEvents.Count > 0)
		{
			((EntitySystem)this).EnsureComp<RMCAmbientLightComponent>(Entity<RMCWeatherCycleComponent>.op_Implicit(ent));
			((EntitySystem)this).EnsureComp<RMCAmbientLightEffectsComponent>(Entity<RMCWeatherCycleComponent>.op_Implicit(ent));
			ent.Comp.LastEventCooldown = _random.Next(ent.Comp.MinTimeBetweenEvents);
		}
	}

	public bool CanWeatherAffectArea(EntityUid uid, MapGridComponent grid, TileRef tileRef, RoofComponent? roofComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RoofComponent>(uid, ref roofComp, false) && _roof.IsRooved(Entity<MapGridComponent, RoofComponent>.op_Implicit((uid, grid, roofComp)), tileRef.GridIndices))
		{
			return false;
		}
		if (!_area.IsWeatherEnabled(Entity<MapGridComponent>.op_Implicit((uid, grid)), tileRef.GridIndices))
		{
			return false;
		}
		AnchoredEntitiesEnumerator anchoredEntities = _mapSystem.GetAnchoredEntitiesEnumerator(uid, grid, tileRef.GridIndices);
		EntityUid? ent = default(EntityUid?);
		BlockWeatherComponent block = default(BlockWeatherComponent);
		while (((AnchoredEntitiesEnumerator)(ref anchoredEntities)).MoveNext(ref ent))
		{
			if (_blockQuery.TryGetComponent(ent.Value, ref block) && block.Enabled)
			{
				return false;
			}
		}
		return true;
	}

	public void HandleWeatherEffects(Entity<RMCWeatherCycleComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CurrentEvent != null && !(ent.Comp.CurrentEvent.LightningChance <= 0f) && ent.Comp.CurrentEvent.LightningEffects.Count > 0 && RandomExtensions.Prob(_random, ent.Comp.CurrentEvent.LightningChance))
		{
			RMCAmbientLightComponent lightComp = default(RMCAmbientLightComponent);
			((EntitySystem)this).EnsureComp<RMCAmbientLightComponent>(Entity<RMCWeatherCycleComponent>.op_Implicit(ent), ref lightComp);
			MapLightComponent mapLightComp = default(MapLightComponent);
			((EntitySystem)this).EnsureComp<MapLightComponent>(Entity<RMCWeatherCycleComponent>.op_Implicit(ent), ref mapLightComp);
			if (!lightComp.IsAnimating && !(mapLightComp.AmbientLightColor != Color.Black))
			{
				List<Color> lightningEffect = _rmcLight.ProcessPrototype(ProtoId<DatasetPrototype>.op_Implicit(RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)ent.Comp.CurrentEvent.LightningEffects)));
				TimeSpan lightningDuration = ent.Comp.CurrentEvent.LightningDuration;
				_rmcLight.SetColor(Entity<RMCAmbientLightComponent>.op_Implicit((Entity<RMCWeatherCycleComponent>.op_Implicit(ent), lightComp)), lightningEffect, lightningDuration);
				SoundSpecifier sound = ent.Comp.CurrentEvent.LightningSound;
				_audio.PlayGlobal(_audio.ResolveSound(sound), Filter.BroadcastMap(((EntitySystem)this).Transform(Entity<RMCWeatherCycleComponent>.op_Implicit(ent)).MapID), true, (AudioParams?)null);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<RMCWeatherCycleComponent> weatherQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCWeatherCycleComponent>();
		EntityUid uid = default(EntityUid);
		RMCWeatherCycleComponent cycle = default(RMCWeatherCycleComponent);
		WeatherPrototype weatherProto = default(WeatherPrototype);
		while (weatherQuery.MoveNext(ref uid, ref cycle))
		{
			cycle.LastEventCooldown -= TimeSpan.FromSeconds(frameTime);
			if (cycle.LastEventCooldown <= TimeSpan.Zero)
			{
				RMCWeatherEvent weatherPick = RandomExtensions.Pick<RMCWeatherEvent>(_random, (IReadOnlyList<RMCWeatherEvent>)cycle.WeatherEvents);
				_proto.TryIndex<WeatherPrototype>(weatherPick.WeatherType, ref weatherProto);
				TimeSpan endTime = _timing.CurTime + weatherPick.Duration;
				cycle.CurrentEvent = weatherPick;
				cycle.CurrentEvent.DurationRemaining = weatherPick.Duration;
				_weather.SetWeather(((EntitySystem)this).Transform(uid).MapID, weatherProto, endTime);
				TimeSpan minTimeVariance = -cycle.MinTimeVariance * 0.5 + _random.Next(cycle.MinTimeVariance);
				cycle.LastEventCooldown = weatherPick.Duration + cycle.MinTimeBetweenEvents + minTimeVariance;
			}
			if (cycle.CurrentEvent == null)
			{
				continue;
			}
			cycle.CurrentEvent.DurationRemaining -= TimeSpan.FromSeconds(frameTime);
			if (cycle.CurrentEvent.DurationRemaining <= TimeSpan.Zero)
			{
				cycle.CurrentEvent = null;
				continue;
			}
			cycle.CurrentEvent.LightningCooldown -= TimeSpan.FromSeconds(frameTime);
			if (cycle.CurrentEvent.LightningCooldown <= TimeSpan.Zero)
			{
				HandleWeatherEffects(Entity<RMCWeatherCycleComponent>.op_Implicit((uid, cycle)));
				cycle.CurrentEvent.LightningCooldown = cycle.CurrentEvent.LightningCooldownDuration;
			}
		}
	}
}
