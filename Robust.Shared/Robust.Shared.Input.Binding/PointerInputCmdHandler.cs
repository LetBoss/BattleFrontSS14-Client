using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Robust.Shared.Input.Binding;

public sealed class PointerInputCmdHandler : InputCmdHandler
{
	public readonly struct PointerInputCmdArgs(ICommonSession? session, EntityCoordinates coordinates, ScreenCoordinates screenCoordinates, EntityUid entityUid, BoundKeyState state, IFullInputCmdMessage originalMessage)
	{
		public readonly ICommonSession? Session = session;

		public readonly EntityCoordinates Coordinates = coordinates;

		public readonly ScreenCoordinates ScreenCoordinates = screenCoordinates;

		public readonly EntityUid EntityUid = entityUid;

		public readonly BoundKeyState State = state;

		public readonly IFullInputCmdMessage OriginalMessage = originalMessage;
	}

	private PointerInputCmdDelegate2 _callback;

	private bool _ignoreUp;

	public override bool FireOutsidePrediction { get; }

	public PointerInputCmdHandler(PointerInputCmdDelegate callback, bool ignoreUp = true, bool outsidePrediction = false)
		: this(delegate(in PointerInputCmdArgs args)
		{
			return callback(args.Session, args.Coordinates, args.EntityUid);
		}, ignoreUp, outsidePrediction)
	{
	}

	public PointerInputCmdHandler(PointerInputCmdDelegate2 callback, bool ignoreUp = true, bool outsidePrediction = false)
	{
		_callback = callback;
		_ignoreUp = ignoreUp;
		FireOutsidePrediction = outsidePrediction;
	}

	public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
	{
		if (_ignoreUp && message.State != BoundKeyState.Down)
		{
			return false;
		}
		if (!(message is ClientFullInputCmdMessage clientFullInputCmdMessage))
		{
			if (message is FullInputCmdMessage fullInputCmdMessage)
			{
				bool? flag = _callback?.Invoke(new PointerInputCmdArgs(session, entManager.GetCoordinates(fullInputCmdMessage.Coordinates), fullInputCmdMessage.ScreenCoordinates, entManager.GetEntity(fullInputCmdMessage.Uid), fullInputCmdMessage.State, message));
				if (flag.HasValue)
				{
					return flag.Value;
				}
				return false;
			}
			throw new ArgumentOutOfRangeException();
		}
		bool? flag2 = _callback?.Invoke(new PointerInputCmdArgs(session, clientFullInputCmdMessage.Coordinates, clientFullInputCmdMessage.ScreenCoordinates, clientFullInputCmdMessage.Uid, message.State, message));
		if (flag2.HasValue)
		{
			return flag2.Value;
		}
		return false;
	}
}
