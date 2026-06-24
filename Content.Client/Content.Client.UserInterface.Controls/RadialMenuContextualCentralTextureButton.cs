using System.Numerics;
using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Controls;

public sealed class RadialMenuContextualCentralTextureButton : RadialMenuTextureButtonBase
{
	public float InnerRadius { get; set; }

	public Vector2? ParentCenter { get; set; }

	protected override bool HasPoint(Vector2 point)
	{
		if (!ParentCenter.HasValue)
		{
			return ((Control)this).HasPoint(point);
		}
		float num = (point + ((Control)this).Position - ParentCenter.Value).LengthSquared();
		float num2 = InnerRadius * InnerRadius;
		return num < num2;
	}
}
