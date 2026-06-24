using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Disposal.Unit.Events;

public sealed class BeforeDisposalFlushEvent : CancellableEntityEventArgs
{
	public readonly List<string> Tags = new List<string>();
}
