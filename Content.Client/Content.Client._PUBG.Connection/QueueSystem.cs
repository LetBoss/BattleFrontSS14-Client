using System;
using Content.Client._CIV14merka.ModeMenu;
using Content.Client._PUBG.Lobby;
using Content.Shared._PUBG.Connection;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Connection;

public sealed class QueueSystem : EntitySystem
{
	[Dependency]
	private IStateManager _stateManager;

	private bool _isInQueue;

	private int _position;

	private int _total;

	public bool IsInQueue => _isInQueue;

	public int Position => _position;

	public int Total => _total;

	public event Action? OnQueueUpdated;

	public event Action? OnQueueAccepted;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<QueuePositionMessage>((EntityEventHandler<QueuePositionMessage>)OnQueuePosition, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<QueueAcceptedMessage>((EntityEventHandler<QueueAcceptedMessage>)OnAccepted, (Type[])null, (Type[])null);
	}

	private void OnQueuePosition(QueuePositionMessage msg)
	{
		_isInQueue = true;
		_position = msg.Position;
		_total = msg.TotalInQueue;
		if (!(_stateManager.CurrentState is QueueState))
		{
			_stateManager.RequestStateChange<QueueState>();
		}
		if (_stateManager.CurrentState is QueueState queueState)
		{
			queueState.UpdatePosition(_position, _total);
		}
		this.OnQueueUpdated?.Invoke();
	}

	private void OnAccepted(QueueAcceptedMessage msg)
	{
		_isInQueue = false;
		_position = 0;
		_total = 0;
		if (_stateManager.CurrentState is QueueState)
		{
			if (msg.OpenModeMenu)
			{
				_stateManager.RequestStateChange<ModeSelectState>();
			}
			else
			{
				_stateManager.RequestStateChange<PubgPreLobbyHubState>();
			}
		}
		this.OnQueueAccepted?.Invoke();
	}

	public void RequestPermissionsRecheck()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new QueueRecheckPermissionsMessage());
	}
}
