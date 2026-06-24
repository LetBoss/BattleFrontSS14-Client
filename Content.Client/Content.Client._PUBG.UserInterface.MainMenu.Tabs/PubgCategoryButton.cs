using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgCategoryButton : Control
{
	[Dependency]
	private IGameTiming _timing;

	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>)"#252530", (Color?)null);

	private string _text = "";

	private string _icon = "";

	private bool _isActive;

	private bool _hovered;

	private Label? _label;

	private Label? _iconLabel;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			if (_label != null)
			{
				_label.Text = value;
			}
		}
	}

	public string Icon
	{
		get
		{
			return _icon;
		}
		set
		{
			_icon = value;
			if (_iconLabel != null)
			{
				_iconLabel.Text = value;
			}
		}
	}

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public event Action? OnPressed;

	public PubgCategoryButton()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		IoCManager.InjectDependencies<PubgCategoryButton>(this);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).MinHeight = 50f;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalAlignment = (HAlignment)1,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(12f, 0f, 12f, 0f)
		};
		_iconLabel = new Label
		{
			Text = _icon,
			FontColorOverride = Color.White,
			Margin = new Thickness(0f, 0f, 10f, 0f),
			MinWidth = 30f
		};
		((Control)_iconLabel).SetOnlyStyleClass("LabelHeading");
		_label = new Label
		{
			Text = _text,
			FontColorOverride = Color.White,
			HorizontalExpand = true
		};
		((Control)_label).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)_iconLabel);
		((Control)val).AddChild((Control)(object)_label);
		((Control)this).AddChild((Control)(object)val);
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		return new Vector2(Math.Min(200f, availableSize.X), 50f);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		Color val2 = (_isActive ? Color.FromHex((ReadOnlySpan<char>)"#1a2a1a", (Color?)null) : ((!_hovered) ? ((Color)(ref DarkPanel)).WithAlpha(0.6f) : CardHoverColor));
		handle.DrawRect(val, val2, true);
		if (_isActive)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num = 0.15f + MathF.Sin((float)totalSeconds * 2f) * 0.08f;
			UIBox2 val3 = default(UIBox2);
			((UIBox2)(ref val3))._002Ector(val.Left - 2f, val.Top - 2f, val.Right + 2f, val.Bottom + 2f);
			handle.DrawRect(val3, ((Color)(ref GreenSuccess)).WithAlpha(num), true);
		}
		Color val4 = (_isActive ? GreenSuccess : (_hovered ? GoldAccent : CardBorderColor));
		UIBox2 val5 = default(UIBox2);
		((UIBox2)(ref val5))._002Ector(0f, val.Top, 4f, val.Bottom);
		handle.DrawRect(val5, val4, true);
		Color val6 = (_isActive ? GreenSuccess : CardBorderColor);
		((DrawingHandleBase)handle).DrawLine(val.TopLeft, ((UIBox2)(ref val)).TopRight, val6);
		((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val)).TopRight, val.BottomRight, val6);
		((DrawingHandleBase)handle).DrawLine(val.BottomRight, ((UIBox2)(ref val)).BottomLeft, val6);
		((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val)).BottomLeft, val.TopLeft, val6);
		if (_label != null)
		{
			_label.FontColorOverride = (_isActive ? GreenSuccess : (_hovered ? Color.White : Color.FromHex((ReadOnlySpan<char>)"#c0c0c0", (Color?)null)));
		}
		if (_iconLabel != null)
		{
			_iconLabel.FontColorOverride = (_isActive ? GreenSuccess : (_hovered ? GoldAccent : Color.FromHex((ReadOnlySpan<char>)"#888888", (Color?)null)));
		}
	}

	protected override void MouseEntered()
	{
		((Control)this).MouseEntered();
		_hovered = true;
		((Control)this).UserInterfaceManager.HoverSound();
		((Control)this).InvalidateMeasure();
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		_hovered = false;
		((Control)this).InvalidateMeasure();
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).KeyBindDown(args);
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			((Control)this).UserInterfaceManager.ClickSound();
			this.OnPressed?.Invoke();
			((BoundKeyEventArgs)args).Handle();
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_isActive)
		{
			((Control)this).InvalidateMeasure();
		}
	}
}
