using Robust.Shared.GameObjects;

namespace Content.Client.Computer;

public interface IComputerWindow<TState>
{
	void SetupComputerWindow(ComputerBoundUserInterfaceBase cb)
	{
	}

	void UpdateState(TState state)
	{
	}

	void ReceiveMessage(BoundUserInterfaceMessage message)
	{
	}
}
