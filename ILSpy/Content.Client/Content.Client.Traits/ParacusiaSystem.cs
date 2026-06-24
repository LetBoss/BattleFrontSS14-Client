using System;
using System.Numerics;
using Content.Shared.Traits.Assorted;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Client.Traits;

public sealed class ParacusiaSystem : SharedParacusiaSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ParacusiaComponent, ComponentStartup>((ComponentEventHandler<ParacusiaComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ParacusiaComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<ParacusiaComponent, LocalPlayerDetachedEvent>)OnPlayerDetach, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_timing.IsFirstTimePredicted)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault = localEntity.GetValueOrDefault();
				PlayParacusiaSounds(valueOrDefault);
			}
		}
	}

	private void OnComponentStartup(EntityUid uid, ParacusiaComponent component, ComponentStartup args)
	{
		component.NextIncidentTime = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(component.MinTimeBetweenIncidents, component.MaxTimeBetweenIncidents));
	}

	private void OnPlayerDetach(EntityUid uid, ParacusiaComponent component, LocalPlayerDetachedEvent args)
	{
		component.Stream = _audio.Stop(component.Stream, (AudioComponent)null);
	}

	private void PlayParacusiaSounds(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		ParacusiaComponent paracusiaComponent = default(ParacusiaComponent);
		if (((EntitySystem)this).TryComp<ParacusiaComponent>(uid, ref paracusiaComponent) && !(_timing.CurTime <= paracusiaComponent.NextIncidentTime))
		{
			float num = _random.NextFloat(paracusiaComponent.MinTimeBetweenIncidents, paracusiaComponent.MaxTimeBetweenIncidents);
			paracusiaComponent.NextIncidentTime += TimeSpan.FromSeconds(num);
			Vector2 vector = new Vector2(_random.NextFloat(0f - paracusiaComponent.MaxSoundDistance, paracusiaComponent.MaxSoundDistance), _random.NextFloat(0f - paracusiaComponent.MaxSoundDistance, paracusiaComponent.MaxSoundDistance));
			EntityCoordinates coordinates = ((EntitySystem)this).Transform(uid).Coordinates;
			EntityCoordinates val = ((EntityCoordinates)(ref coordinates)).Offset(vector);
			paracusiaComponent.Stream = _audio.PlayStatic(paracusiaComponent.Sounds, uid, val, (AudioParams?)null)?.Item1;
		}
	}
}
