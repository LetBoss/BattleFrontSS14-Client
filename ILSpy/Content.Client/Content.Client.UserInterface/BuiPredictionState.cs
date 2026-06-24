using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Client.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface;

public sealed class BuiPredictionState
{
	private struct MessageData
	{
		public GameTick TickSent;

		public required BoundUserInterfaceMessage Message;

		public override string ToString()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			return $"{Message} @ {TickSent}";
		}
	}

	private readonly BoundUserInterface _parent;

	private readonly IClientGameTiming _gameTiming;

	private readonly Queue<MessageData> _queuedMessages = new Queue<MessageData>();

	public BuiPredictionState(BoundUserInterface parent, IClientGameTiming gameTiming)
	{
		_parent = parent;
		_gameTiming = gameTiming;
	}

	public void SendMessage(BoundUserInterfaceMessage message)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (((IGameTiming)_gameTiming).IsFirstTimePredicted)
		{
			MessageData item = new MessageData
			{
				TickSent = ((IGameTiming)_gameTiming).CurTick,
				Message = message
			};
			_queuedMessages.Enqueue(item);
		}
		_parent.SendPredictedMessage(message);
	}

	public IEnumerable<BoundUserInterfaceMessage> MessagesToReplay()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		GameTick lastRealTick = _gameTiming.LastRealTick;
		MessageData result;
		while (_queuedMessages.TryPeek(out result) && result.TickSent <= lastRealTick)
		{
			_queuedMessages.Dequeue();
		}
		if (_queuedMessages.Count == 0)
		{
			return Array.Empty<BoundUserInterfaceMessage>();
		}
		return _queuedMessages.Select((MessageData c) => c.Message);
	}
}
