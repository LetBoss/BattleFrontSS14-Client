using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgTaskRowPanel : PanelContainer
{
	private static readonly Color HoverOverlay = Color.FromHex((ReadOnlySpan<char>)"#252530", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>)"#00FFB3", (Color?)null);

	private bool _isHoverable;

	private bool _isCompleted;

	private bool _hovered;

	public bool IsHoverable
	{
		get
		{
			return _isHoverable;
		}
		set
		{
			_isHoverable = value;
			((Control)this).MouseFilter = (MouseFilterMode)(!value);
		}
	}

	public bool IsCompleted
	{
		get
		{
			return _isCompleted;
		}
		set
		{
			_isCompleted = value;
		}
	}

	public PubgTaskRowPanel()
	{
		((Control)this).MouseFilter = (MouseFilterMode)1;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		((PanelContainer)this).Draw(handle);
		if (_hovered && _isHoverable)
		{
			UIBox2 val = default(UIBox2);
			((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
			handle.DrawRect(val, ((Color)(ref HoverOverlay)).WithAlpha(0.4f), true);
		}
		if (_isCompleted)
		{
			new UIBox2(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
			UIBox2 val2 = default(UIBox2);
			((UIBox2)(ref val2))._002Ector(0f, 0f, 3f, (float)((Control)this).PixelSize.Y);
			handle.DrawRect(val2, ((Color)(ref CompletedGlow)).WithAlpha(0.6f), true);
		}
	}

	protected override void MouseEntered()
	{
		((Control)this).MouseEntered();
		if (_isHoverable)
		{
			_hovered = true;
			((Control)this).UserInterfaceManager.HoverSound();
		}
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		_hovered = false;
	}
}
