using System;
using System.Numerics;
using Content.Shared._PUBG;
using Content.Shared._PUBG.Lobby;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.UserInterface.Systems.Stats;

public sealed class PubgHudEventsSystem : EntitySystem
{
	public PubgModeStatusEvent? LastPubgModeStatus { get; private set; }

	public PubgKillsChangedEvent? LastKillsChanged { get; private set; }

	public PubgWarmupStatusEvent? LastWarmupStatus { get; private set; }

	public LobbyStatusEvent? LastLobbyStatus { get; private set; }

	public PubgZoneStateEvent? LastZoneState { get; private set; }

	public PubgGameEndEvent? LastGameEnd { get; private set; }

	public event Action<PubgModeStatusEvent>? OnPubgModeStatusReceived;

	public event Action<PubgKillsChangedEvent>? OnKillsChangedReceived;

	public event Action<PubgWarmupStatusEvent>? OnWarmupStatusReceived;

	public event Action<LobbyStatusEvent>? OnLobbyStatusReceived;

	public event Action<PubgZoneStateEvent>? OnZoneStateReceived;

	public event Action<PubgGameEndEvent>? OnGameEndReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgModeStatusEvent>((EntitySessionEventHandler<PubgModeStatusEvent>)OnPubgModeStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgKillsChangedEvent>((EntitySessionEventHandler<PubgKillsChangedEvent>)OnKillsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgWarmupStatusEvent>((EntitySessionEventHandler<PubgWarmupStatusEvent>)OnWarmupStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<LobbyStatusEvent>((EntitySessionEventHandler<LobbyStatusEvent>)OnLobbyStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgZoneStateEvent>((EntitySessionEventHandler<PubgZoneStateEvent>)OnZoneState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgGameEndEvent>((EntitySessionEventHandler<PubgGameEndEvent>)OnGameEnd, (Type[])null, (Type[])null);
	}

	private void OnPubgModeStatus(PubgModeStatusEvent msg, EntitySessionEventArgs args)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		LastPubgModeStatus = msg;
		if (!msg.Enabled)
		{
			LastKillsChanged = new PubgKillsChangedEvent(0);
			LastWarmupStatus = new PubgWarmupStatusEvent(inWarmup: false, 0, 0);
			LastLobbyStatus = new LobbyStatusEvent(inLobby: false, 0, 0);
			LastZoneState = new PubgZoneStateEvent(Vector2.Zero, 0f, Vector2.Zero, 0f, 0, ZoneState.Waiting, 0f, active: false, visible: false, NetEntity.Invalid);
			LastGameEnd = null;
		}
		this.OnPubgModeStatusReceived?.Invoke(msg);
	}

	private void OnKillsChanged(PubgKillsChangedEvent msg, EntitySessionEventArgs args)
	{
		LastKillsChanged = msg;
		this.OnKillsChangedReceived?.Invoke(msg);
	}

	private void OnWarmupStatus(PubgWarmupStatusEvent msg, EntitySessionEventArgs args)
	{
		LastWarmupStatus = msg;
		this.OnWarmupStatusReceived?.Invoke(msg);
	}

	private void OnLobbyStatus(LobbyStatusEvent msg, EntitySessionEventArgs args)
	{
		LastLobbyStatus = msg;
		this.OnLobbyStatusReceived?.Invoke(msg);
	}

	private void OnZoneState(PubgZoneStateEvent msg, EntitySessionEventArgs args)
	{
		LastZoneState = msg;
		this.OnZoneStateReceived?.Invoke(msg);
	}

	private void OnGameEnd(PubgGameEndEvent msg, EntitySessionEventArgs args)
	{
		LastGameEnd = msg;
		this.OnGameEndReceived?.Invoke(msg);
	}
}
