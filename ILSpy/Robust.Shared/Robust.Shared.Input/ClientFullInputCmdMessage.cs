using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

public sealed class ClientFullInputCmdMessage : InputCmdMessage, IFullInputCmdMessage
{
	public BoundKeyState State { get; init; }

	public EntityCoordinates Coordinates { get; init; }

	public ScreenCoordinates ScreenCoordinates { get; init; }

	public EntityUid Uid { get; init; }

	public ClientFullInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId)
		: base(tick, subTick, inputFunctionId)
	{
	}

	public ClientFullInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId, EntityCoordinates coordinates, ScreenCoordinates screenCoordinates, BoundKeyState state, EntityUid uid)
		: base(tick, subTick, inputFunctionId)
	{
		Coordinates = coordinates;
		ScreenCoordinates = screenCoordinates;
		State = state;
		Uid = uid;
	}
}
