using Robust.Shared.Analyzers;
using Robust.Shared.Timing;

namespace Robust.Shared.Input;

[NotContentImplementable]
public interface IFullInputCmdMessage
{
	GameTick Tick { get; }

	BoundKeyState State { get; }

	KeyFunctionId InputFunctionId { get; }

	ushort SubTick { get; }

	uint InputSequence { get; set; }
}
