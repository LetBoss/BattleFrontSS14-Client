using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Controls;

public abstract class RadialMenuOption
{
	public string? ToolTip { get; init; }

	public SpriteSpecifier? Sprite { get; init; }

	public Color? BackgroundColor { get; set; }

	public Color? HoverBackgroundColor { get; set; }
}
