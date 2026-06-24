using Robust.Client.Graphics;

namespace Content.Client.SprayPainter;

public sealed class SprayPainterEntry
{
	public string Name;

	public Texture? Icon;

	public SprayPainterEntry(string name, Texture? icon)
	{
		Name = name;
		Icon = icon;
	}
}
