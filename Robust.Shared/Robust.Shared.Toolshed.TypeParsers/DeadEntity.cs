using Robust.Shared.GameObjects;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class DeadEntity(EntityUid entity) : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted($"The entity {entity} does not exist.");
	}
}
