using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Computer;

[Virtual]
public class ComputerBoundUserInterfaceBase : BoundUserInterface
{
	public ComputerBoundUserInterfaceBase(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public void SendMessage(BoundUserInterfaceMessage msg)
	{
		((BoundUserInterface)this).SendMessage(msg);
	}
}
