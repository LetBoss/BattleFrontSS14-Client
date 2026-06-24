using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
public sealed class FullInputCmdMessage : InputCmdMessage, IFullInputCmdMessage
{
	public BoundKeyState State { get; }

	public NetCoordinates Coordinates { get; }

	public ScreenCoordinates ScreenCoordinates { get; }

	public NetEntity Uid { get; init; }

	public FullInputCmdMessage(GameTick tick, ushort subTick, int inputSequence, KeyFunctionId inputFunctionId, BoundKeyState state, NetCoordinates coordinates, ScreenCoordinates screenCoordinates)
		: this(tick, subTick, inputFunctionId, state, coordinates, screenCoordinates, NetEntity.Invalid)
	{
	}

	public FullInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId, BoundKeyState state, NetCoordinates coordinates, ScreenCoordinates screenCoordinates, NetEntity uid)
		: base(tick, subTick, inputFunctionId)
	{
		State = state;
		Coordinates = coordinates;
		ScreenCoordinates = screenCoordinates;
		Uid = uid;
	}
}
