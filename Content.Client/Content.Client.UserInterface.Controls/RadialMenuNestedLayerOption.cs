using System.Collections.Generic;

namespace Content.Client.UserInterface.Controls;

public sealed class RadialMenuNestedLayerOption(IReadOnlyCollection<RadialMenuOption> nested, float containerRadius = 100f) : RadialMenuOption
{
	public float? ContainerRadius { get; } = containerRadius;

	public IReadOnlyCollection<RadialMenuOption> Nested { get; } = nested;
}
