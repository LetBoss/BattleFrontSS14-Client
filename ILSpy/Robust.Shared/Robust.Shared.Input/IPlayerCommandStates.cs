using Robust.Shared.Analyzers;

namespace Robust.Shared.Input;

[NotContentImplementable]
public interface IPlayerCommandStates
{
	BoundKeyState this[BoundKeyFunction function] { get; set; }

	BoundKeyState GetState(BoundKeyFunction function);

	void SetState(BoundKeyFunction function, BoundKeyState state);
}
