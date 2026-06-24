using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Controls;

public sealed class HSpacer : Control
{
	public float Spacing
	{
		get
		{
			return ((Control)this).MinHeight;
		}
		set
		{
			((Control)this).MinHeight = value;
		}
	}

	public HSpacer()
	{
		((Control)this).MinHeight = Spacing;
	}

	public HSpacer(float height = 5f)
	{
		Spacing = height;
		((Control)this).MinHeight = height;
	}
}
