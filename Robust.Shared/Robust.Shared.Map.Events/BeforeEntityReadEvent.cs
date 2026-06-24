using System.Collections.Generic;

namespace Robust.Shared.Map.Events;

public sealed class BeforeEntityReadEvent
{
	public readonly HashSet<string> DeletedPrototypes = new HashSet<string>();

	public readonly Dictionary<string, string> RenamedPrototypes = new Dictionary<string, string>();
}
