using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Robust.Shared.Input.Binding;

public sealed class NullInputCmdHandler : InputCmdHandler
{
	public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
	{
		return true;
	}
}
