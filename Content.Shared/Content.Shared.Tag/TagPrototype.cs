using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Tag;

[Prototype("Tag", 1)]
public sealed class TagPrototype : IPrototype
{
	[IdDataField(1, null)]
	[ViewVariables]
	public string ID { get; private set; } = string.Empty;
}
