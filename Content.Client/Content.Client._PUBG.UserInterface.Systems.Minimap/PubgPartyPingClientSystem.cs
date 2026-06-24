using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Party;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgPartyPingClientSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAudioSystem _audio;

	private static readonly TimeSpan DoubleClickWindow = TimeSpan.FromMilliseconds(300L);

	private static readonly SoundSpecifier EnemyPingSound = (SoundSpecifier)new SoundCollectionSpecifier("PubgSoundCollectionEnemyPing", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-6f));

	private readonly List<PubgActivePingState> _activePings = new List<PubgActivePingState>();

	private PubgActivePingState[] _activePingSnapshot = Array.Empty<PubgActivePingState>();

	private MapCoordinates? _pendingClickCoords;

	private TimeSpan? _pendingClickSendAt;

	public IReadOnlyList<PubgActivePingState> ActivePings => _activePingSnapshot;

	public PubgActivePingState? LatestPing
	{
		get
		{
			if (_activePingSnapshot.Length == 0)
			{
				return null;
			}
			PubgActivePingState value = _activePingSnapshot[^1];
			if (!(value.ExpiresAtUtc > DateTime.UtcNow))
			{
				return null;
			}
			return value;
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgPartyPingBroadcastEvent>((EntitySessionEventHandler<PubgPartyPingBroadcastEvent>)OnPartyPingBroadcast, (Type[])null, (Type[])null);
	}

	public void QueueMapClick(MapCoordinates coords)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!(coords.MapId == MapId.Nullspace) && float.IsFinite(coords.Position.X) && float.IsFinite(coords.Position.Y))
		{
			if (_pendingClickCoords.HasValue && _pendingClickSendAt.HasValue && _timing.CurTime >= _pendingClickSendAt.Value)
			{
				SendPing(_pendingClickCoords.Value, PubgPartyPingKind.Normal);
				ClearPendingClick();
			}
			if (_pendingClickCoords.HasValue && _pendingClickSendAt.HasValue && _timing.CurTime <= _pendingClickSendAt.Value)
			{
				SendPing(coords, PubgPartyPingKind.Enemy);
				ClearPendingClick();
			}
			else
			{
				_pendingClickCoords = coords;
				_pendingClickSendAt = _timing.CurTime + DoubleClickWindow;
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_pendingClickCoords.HasValue && _pendingClickSendAt.HasValue && _timing.CurTime >= _pendingClickSendAt.Value)
		{
			SendPing(_pendingClickCoords.Value, PubgPartyPingKind.Normal);
			ClearPendingClick();
		}
		TrimExpiredPings();
	}

	private void OnPartyPingBroadcast(PubgPartyPingBroadcastEvent msg, EntitySessionEventArgs args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (msg.ExpiresAtUtc <= DateTime.UtcNow)
		{
			return;
		}
		for (int num = _activePings.Count - 1; num >= 0; num--)
		{
			if (!(_activePings[num].Source != msg.Source))
			{
				_activePings.RemoveAt(num);
			}
		}
		_activePings.Add(new PubgActivePingState(msg.Source, msg.MapId, msg.Position, msg.Kind, msg.ItemPrototypeId, msg.ExpiresAtUtc));
		RefreshSnapshot();
		if (msg.Kind == PubgPartyPingKind.Enemy)
		{
			_audio.PlayGlobal(EnemyPingSound, Filter.Local(), false, (AudioParams?)null);
		}
	}

	private void SendPing(MapCoordinates coords, PubgPartyPingKind kind)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgPartyPingRequestEvent(coords.MapId, coords.Position, kind));
	}

	private void TrimExpiredPings()
	{
		DateTime utcNow = DateTime.UtcNow;
		bool flag = false;
		for (int num = _activePings.Count - 1; num >= 0; num--)
		{
			if (!(_activePings[num].ExpiresAtUtc > utcNow))
			{
				_activePings.RemoveAt(num);
				flag = true;
			}
		}
		if (flag)
		{
			RefreshSnapshot();
		}
	}

	private void RefreshSnapshot()
	{
		_activePingSnapshot = _activePings.ToArray();
	}

	private void ClearPendingClick()
	{
		_pendingClickCoords = null;
		_pendingClickSendAt = null;
	}
}
