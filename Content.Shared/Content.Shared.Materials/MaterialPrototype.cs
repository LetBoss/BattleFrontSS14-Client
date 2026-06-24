using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Materials;

[Prototype(null, 1)]
public sealed class MaterialPrototype : IPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? StackEntity;

	[DataField(null, false, 1, false, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string Unit = "materials-unit-sheet";

	[DataField(null, false, 1, true, false, null)]
	public double Price;

	[ViewVariables]
	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<MaterialPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[ViewVariables]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public Color Color { get; private set; } = Color.Gray;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Icon { get; private set; } = SpriteSpecifier.Invalid;
}
