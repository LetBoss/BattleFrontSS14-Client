using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Overwatch;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Overwatch;

public sealed class OverwatchConsoleSystem : SharedOverwatchConsoleSystem
{
	[Dependency]
	private AudioSystem _audio;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly List<(Entity<AudioComponent, OverwatchRelayedSoundComponent> Audio, EntityCoordinates Position)> _toRelay = new List<(Entity<AudioComponent, OverwatchRelayedSoundComponent>, EntityCoordinates)>();

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<OverwatchConsoleComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<OverwatchConsoleComponent, AfterAutoHandleStateEvent>)OnOverwatchAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchRelayedSoundComponent, ComponentRemove>((EntityEventRefHandler<OverwatchRelayedSoundComponent, ComponentRemove>)OnRelayedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchRelayedSoundComponent, EntityTerminatingEvent>((EntityEventRefHandler<OverwatchRelayedSoundComponent, EntityTerminatingEvent>)OnRelayedRemove, (Type[])null, (Type[])null);
	}

	private void OnOverwatchAfterState(Entity<OverwatchConsoleComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			UserInterfaceComponent val = default(UserInterfaceComponent);
			if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(Entity<OverwatchConsoleComponent>.op_Implicit(ent), ref val))
			{
				return;
			}
			foreach (BoundUserInterface value2 in val.ClientOpenInterfaces.Values)
			{
				if (value2 is OverwatchConsoleBui overwatchConsoleBui)
				{
					overwatchConsoleBui.Refresh();
				}
			}
		}
		catch (Exception value)
		{
			((EntitySystem)this).Log.Error($"Error refreshing {"OverwatchConsoleBui"}\n{value}");
		}
	}

	private void OnRelayedRemove<T>(Entity<OverwatchRelayedSoundComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryDeleteRelayed(ent.Comp.Relay);
	}

	private void TryDeleteRelayed(EntityUid? relay)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (relay.HasValue && ((EntitySystem)this).IsClientSide(relay.Value, (MetaDataComponent)null))
		{
			((EntitySystem)this).QueueDel(relay);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			TransformComponent val = default(TransformComponent);
			if (((EntitySystem)this).HasComp<OverwatchWatchingComponent>(valueOrDefault) && ((EntitySystem)this).TryComp(valueOrDefault, ref val))
			{
				_toRelay.Clear();
				MapCoordinates position = _eye.CurrentEye.Position;
				EntityCoordinates coordinates = val.Coordinates;
				AllEntityQueryEnumerator<AudioComponent, TransformComponent> val2 = ((EntitySystem)this).AllEntityQuery<AudioComponent, TransformComponent>();
				EntityUid val3 = default(EntityUid);
				AudioComponent val4 = default(AudioComponent);
				TransformComponent val5 = default(TransformComponent);
				Vector2 vector = default(Vector2);
				while (val2.MoveNext(ref val3, ref val4, ref val5))
				{
					EntityCoordinates coordinates2 = val5.Coordinates;
					if (!((EntityCoordinates)(ref coordinates2)).TryDelta((IEntityManager)(object)((EntitySystem)this).EntityManager, _transform, coordinates, ref vector))
					{
						continue;
					}
					if (position.MapId == val5.MapID && vector.LengthSquared() <= val4.MaxDistance * val4.MaxDistance)
					{
						((EntitySystem)this).RemCompDeferred<OverwatchRelayedSoundComponent>(val3);
						continue;
					}
					MapCoordinates val6 = ((MapCoordinates)(ref position)).Offset(vector);
					OverwatchRelayedSoundComponent overwatchRelayedSoundComponent = ((EntitySystem)this).EnsureComp<OverwatchRelayedSoundComponent>(val3);
					if (overwatchRelayedSoundComponent.Relay.HasValue && !((EntitySystem)this).TerminatingOrDeleted(overwatchRelayedSoundComponent.Relay, (MetaDataComponent)null))
					{
						_transform.SetMapCoordinates(overwatchRelayedSoundComponent.Relay.Value, val6);
						continue;
					}
					EntityCoordinates item = _transform.ToCoordinates(val6);
					_toRelay.Add((Entity<AudioComponent, OverwatchRelayedSoundComponent>.op_Implicit((val3, val4, overwatchRelayedSoundComponent)), item));
				}
				{
					foreach (var item5 in _toRelay)
					{
						Entity<AudioComponent, OverwatchRelayedSoundComponent> item2 = item5.Audio;
						EntityCoordinates item3 = item5.Position;
						(EntityUid, AudioComponent)? tuple = ((SharedAudioSystem)_audio).PlayStatic((SoundSpecifier)new SoundPathSpecifier(item2.Comp1.FileName, (AudioParams?)null), valueOrDefault, item3, (AudioParams?)item2.Comp1.Params);
						if (tuple.HasValue)
						{
							EntityUid item4 = tuple.GetValueOrDefault().Item1;
							((SharedAudioSystem)_audio).SetPlaybackPosition((Entity<AudioComponent>?)Entity<AudioComponent>.op_Implicit(item4), item2.Comp1.PlaybackPosition);
							item2.Comp2.Relay = item4;
						}
					}
					return;
				}
			}
		}
		AllEntityQueryEnumerator<OverwatchRelayedSoundComponent> val7 = ((EntitySystem)this).AllEntityQuery<OverwatchRelayedSoundComponent>();
		EntityUid val8 = default(EntityUid);
		OverwatchRelayedSoundComponent overwatchRelayedSoundComponent2 = default(OverwatchRelayedSoundComponent);
		while (val7.MoveNext(ref val8, ref overwatchRelayedSoundComponent2))
		{
			TryDeleteRelayed(overwatchRelayedSoundComponent2.Relay);
			((EntitySystem)this).RemCompDeferred<OverwatchRelayedSoundComponent>(val8);
		}
	}
}
