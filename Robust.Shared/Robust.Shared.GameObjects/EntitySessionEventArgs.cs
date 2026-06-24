using Robust.Shared.Player;

namespace Robust.Shared.GameObjects;

public readonly struct EntitySessionEventArgs(ICommonSession senderSession)
{
	public ICommonSession SenderSession { get; } = senderSession;
}
