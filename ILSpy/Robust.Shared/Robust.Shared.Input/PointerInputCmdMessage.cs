using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
public sealed class PointerInputCmdMessage : EventInputCmdMessage
{
	public NetCoordinates Coordinates { get; }

	public NetEntity Uid { get; }

	public PointerInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId, NetCoordinates coordinates)
		: this(tick, subTick, inputFunctionId, coordinates, NetEntity.Invalid)
	{
	}

	public PointerInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId, NetCoordinates coordinates, NetEntity uid)
		: base(tick, subTick, inputFunctionId)
	{
		Coordinates = coordinates;
		Uid = uid;
	}
}
