using System;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
public sealed class StateInputCmdMessage : InputCmdMessage
{
	public BoundKeyState State { get; }

	public StateInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId, BoundKeyState state)
		: base(tick, subTick, inputFunctionId)
	{
		State = state;
	}
}
