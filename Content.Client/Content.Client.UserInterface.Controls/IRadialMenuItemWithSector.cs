using System.Numerics;

namespace Content.Client.UserInterface.Controls;

public interface IRadialMenuItemWithSector
{
	float AngleSectorFrom { set; }

	float AngleSectorTo { set; }

	float OuterRadius { set; }

	float InnerRadius { set; }

	float AngleOffset { set; }

	Vector2 ParentCenter { set; }
}
