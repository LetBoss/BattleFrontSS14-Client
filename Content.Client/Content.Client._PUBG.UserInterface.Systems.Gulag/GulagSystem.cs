using System;
using Content.Shared._PUBG.Gulag;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	private MapId? _gulagMapId;

	private bool _localOnGulagMap;

	public event Action<GulagStatusEvent, EntitySessionEventArgs>? OnGulagStatusReceived;

	public event Action<GulagQueueUpdateEvent, EntitySessionEventArgs>? OnQueueUpdateReceived;

	public event Action<GulagFightStartEvent, EntitySessionEventArgs>? OnFightStartReceived;

	public event Action<GulagFightUpdateEvent, EntitySessionEventArgs>? OnFightUpdateReceived;

	public event Action<GulagSpectatorUpdateEvent, EntitySessionEventArgs>? OnSpectatorUpdateReceived;

	public event Action<GulagAdminOfferEvent, EntitySessionEventArgs>? OnAdminOfferReceived;

	public event Action<GulagMapInfoEvent, EntitySessionEventArgs>? OnMapInfoReceived;

	public event Action<bool>? OnLocalGulagMapChanged;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<GulagStatusEvent>((EntitySessionEventHandler<GulagStatusEvent>)OnGulagStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagQueueUpdateEvent>((EntitySessionEventHandler<GulagQueueUpdateEvent>)OnQueueUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagFightStartEvent>((EntitySessionEventHandler<GulagFightStartEvent>)OnFightStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagFightUpdateEvent>((EntitySessionEventHandler<GulagFightUpdateEvent>)OnFightUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagSpectatorUpdateEvent>((EntitySessionEventHandler<GulagSpectatorUpdateEvent>)OnSpectatorUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagAdminOfferEvent>((EntitySessionEventHandler<GulagAdminOfferEvent>)OnAdminOffer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<GulagMapInfoEvent>((EntitySessionEventHandler<GulagMapInfoEvent>)OnMapInfo, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		bool flag = false;
		if (localEntity.HasValue && _gulagMapId.HasValue)
		{
			flag = ((EntitySystem)this).Transform(localEntity.Value).MapID == _gulagMapId.Value;
		}
		if (flag != _localOnGulagMap)
		{
			_localOnGulagMap = flag;
			this.OnLocalGulagMapChanged?.Invoke(flag);
		}
	}

	private void OnGulagStatus(GulagStatusEvent msg, EntitySessionEventArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.OnGulagStatusReceived?.Invoke(msg, args);
	}

	private void OnQueueUpdate(GulagQueueUpdateEvent msg, EntitySessionEventArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.OnQueueUpdateReceived?.Invoke(msg, args);
	}

	private void OnFightStart(GulagFightStartEvent msg, EntitySessionEventArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.OnFightStartReceived?.Invoke(msg, args);
	}

	private void OnFightUpdate(GulagFightUpdateEvent msg, EntitySessionEventArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.OnFightUpdateReceived?.Invoke(msg, args);
	}

	private void OnSpectatorUpdate(GulagSpectatorUpdateEvent msg, EntitySessionEventArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.OnSpectatorUpdateReceived?.Invoke(msg, args);
	}

	private void OnAdminOffer(GulagAdminOfferEvent msg, EntitySessionEventArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this.OnAdminOfferReceived?.Invoke(msg, args);
	}

	private void OnMapInfo(GulagMapInfoEvent msg, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_gulagMapId = msg.GulagMapId;
		this.OnMapInfoReceived?.Invoke(msg, args);
	}
}
