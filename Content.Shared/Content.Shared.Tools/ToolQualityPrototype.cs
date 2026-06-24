using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Shared.Tools;

[Prototype("tool", 1)]
public sealed class ToolQualityPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField("toolName", false, 1, false, false, null)]
	public string ToolName { get; private set; } = string.Empty;

	[DataField("icon", false, 1, false, false, null)]
	public SpriteSpecifier? Icon { get; private set; }

	[DataField("spawn", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string Spawn { get; private set; } = string.Empty;
}
