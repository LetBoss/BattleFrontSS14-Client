using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ghost;

public sealed class GhostAttemptHandleEvent(MindComponent mind, bool canReturnGlobal) : HandledEntityEventArgs
{
	public MindComponent Mind { get; } = mind;

	public bool CanReturnGlobal { get; } = canReturnGlobal;

	public bool Result { get; set; }
}
