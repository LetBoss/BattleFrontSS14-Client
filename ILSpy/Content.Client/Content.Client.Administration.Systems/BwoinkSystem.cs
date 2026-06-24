using System;
using Content.Shared.Administration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client.Administration.Systems;

public sealed class BwoinkSystem : SharedBwoinkSystem
{
	[Dependency]
	private IGameTiming _timing;

	private (TimeSpan Timestamp, bool Typing) _lastTypingUpdateSent;

	public event EventHandler<BwoinkTextMessage>? OnBwoinkTextMessageRecieved;

	protected override void OnBwoinkTextMessage(BwoinkTextMessage message, EntitySessionEventArgs eventArgs)
	{
		this.OnBwoinkTextMessageRecieved?.Invoke(this, message);
	}

	public void Send(NetUserId channelId, string text, bool playSound, bool adminOnly)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		bool playSound2 = playSound;
		bool adminOnly2 = adminOnly;
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new BwoinkTextMessage(channelId, channelId, text, null, playSound2, adminOnly2));
		SendInputTextUpdated(channelId, typing: false);
	}

	public void SendInputTextUpdated(NetUserId channel, bool typing)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (_lastTypingUpdateSent.Typing != typing || !(_lastTypingUpdateSent.Timestamp + TimeSpan.FromSeconds(1L) > _timing.RealTime))
		{
			_lastTypingUpdateSent = (Timestamp: _timing.RealTime, Typing: typing);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new BwoinkClientTypingUpdated(channel, typing));
		}
	}
}
