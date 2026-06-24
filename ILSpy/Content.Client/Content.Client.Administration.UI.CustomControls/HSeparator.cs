using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class HSeparator : Control
{
	private static readonly Color SeparatorColor = Color.FromHex((ReadOnlySpan<char>)"#3D4059", (Color?)null);

	private readonly StyleBoxFlat _styleBox;

	public Color Color
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _styleBox.BackgroundColor;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_styleBox.BackgroundColor = value;
		}
	}

	public HSeparator(Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		_styleBox = new StyleBoxFlat
		{
			BackgroundColor = color,
			ContentMarginBottomOverride = 2f,
			ContentMarginLeftOverride = 2f
		};
		((Control)this).AddChild((Control)new PanelContainer
		{
			PanelOverride = (StyleBox)(object)_styleBox
		});
	}

	public HSeparator()
		: this(SeparatorColor)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)

}
