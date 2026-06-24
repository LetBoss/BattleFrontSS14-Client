using System.Collections.Generic;

namespace Robust.Shared.Network;

public record NetDenyReason(string Text, Dictionary<string, object> AdditionalProperties)
{
	public NetDenyReason(string Text)
		: this(Text, new Dictionary<string, object>())
	{
	}
}
