using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Robust.Shared.Input.Binding;

public sealed class PointerStateInputCmdHandler : InputCmdHandler
{
	private PointerInputCmdDelegate _enabled;

	private PointerInputCmdDelegate _disabled;

	public override bool FireOutsidePrediction { get; }

	public PointerStateInputCmdHandler(PointerInputCmdDelegate enabled, PointerInputCmdDelegate disabled, bool outsidePrediction = false)
	{
		_enabled = enabled;
		_disabled = disabled;
		FireOutsidePrediction = outsidePrediction;
	}

	public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
	{
		if (!(message is ClientFullInputCmdMessage { State: var state } clientFullInputCmdMessage))
		{
			if (message is FullInputCmdMessage { State: var state2 } fullInputCmdMessage)
			{
				switch (state2)
				{
				case BoundKeyState.Up:
					return _disabled?.Invoke(session, entManager.GetCoordinates(fullInputCmdMessage.Coordinates), entManager.GetEntity(fullInputCmdMessage.Uid)) ?? false;
				case BoundKeyState.Down:
					return _enabled?.Invoke(session, entManager.GetCoordinates(fullInputCmdMessage.Coordinates), entManager.GetEntity(fullInputCmdMessage.Uid)) ?? false;
				}
			}
		}
		else
		{
			switch (state)
			{
			case BoundKeyState.Up:
				return _disabled?.Invoke(session, clientFullInputCmdMessage.Coordinates, clientFullInputCmdMessage.Uid) ?? false;
			case BoundKeyState.Down:
				return _enabled?.Invoke(session, clientFullInputCmdMessage.Coordinates, clientFullInputCmdMessage.Uid) ?? false;
			}
		}
		return false;
	}
}
