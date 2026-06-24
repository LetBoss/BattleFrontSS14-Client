using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.NameIdentifier;

[Prototype(null, 1)]
public sealed class NameIdentifierGroupPrototype : IPrototype
{
	[DataField("fullName", false, 1, false, false, null)]
	public bool FullName;

	[DataField("prefix", false, 1, false, false, null)]
	public string? Prefix;

	[DataField("maxValue", false, 1, false, false, null)]
	public int MaxValue = 1000;

	[DataField("minValue", false, 1, false, false, null)]
	public int MinValue;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
