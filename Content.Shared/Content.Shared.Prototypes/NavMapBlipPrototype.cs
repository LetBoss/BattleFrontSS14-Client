using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Prototypes;

[Prototype(null, 1)]
public sealed class NavMapBlipPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool Selectable;

	[DataField(null, false, 1, false, false, null)]
	public bool Blinks;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public Color Color { get; private set; } = Color.LightGray;

	[DataField(null, false, 1, false, false, null)]
	public ResPath[]? TexturePaths { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public float Scale { get; private set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	public NavMapBlipPlacement Placement { get; private set; }
}
