using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

[Serializable]
[NetSerializable]
[Virtual]
public class EventInputCmdMessage : InputCmdMessage
{
	public EventInputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId)
		: base(tick, subTick, inputFunctionId)
	{
	}
}
