using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class VSeparator : PanelContainer
{
	private static readonly Color SeparatorColor = Color.FromHex((ReadOnlySpan<char>)"#3D4059", (Color?)null);

	public VSeparator(Color color)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0038: Expected O, but got Unknown
		((Control)this).MinSize = new Vector2(2f, 5f);
		((Control)this).AddChild((Control)new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = color
			}
		});
	}

	public VSeparator()
		: this(SeparatorColor)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)

}
