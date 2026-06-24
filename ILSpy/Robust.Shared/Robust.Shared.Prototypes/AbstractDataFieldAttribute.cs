using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

public sealed class AbstractDataFieldAttribute : DataFieldAttribute
{
	public const string Name = "abstract";

	public AbstractDataFieldAttribute(int priority = 1)
		: base("abstract", readOnly: false, priority)
	{
	}
}
