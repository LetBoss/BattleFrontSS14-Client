using System.Numerics;
using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Controls;

public sealed class RadialMenuOuterAreaButton : RadialMenuTextureButtonBase
{
	public float OuterRadius { get; set; }

	public Vector2? ParentCenter { get; set; }

	protected override bool HasPoint(Vector2 point)
	{
		if (!ParentCenter.HasValue)
		{
			return ((Control)this).HasPoint(point);
		}
		float num = (point + ((Control)this).Position - ParentCenter.Value).LengthSquared();
		float num2 = OuterRadius * OuterRadius;
		return num > num2;
	}
}
