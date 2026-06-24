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

public sealed class PubgSubcategoryTab : Control
{
	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>)"#252530", (Color?)null);

	private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null);

	private static readonly Color DisabledColor = Color.FromHex((ReadOnlySpan<char>)"#4a4a5a", (Color?)null);

	[Dependency]
	private IGameTiming _timing;

	private string _text = "";

	private bool _isActive;

	private bool _hovered;

	private Label? _label;

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
			((Control)this).InvalidateMeasure();
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

	public PubgSubcategoryTab()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		IoCManager.InjectDependencies<PubgSubcategoryTab>(this);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).MinHeight = 45f;
		_label = new Label
		{
			Text = _text,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			FontColorOverride = Color.White
		};
		((Control)_label).SetOnlyStyleClass("LabelHeading");
		((Control)this).AddChild((Control)(object)_label);
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		return new Vector2(180f, 45f);
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (_label != null)
		{
			((Control)_label).Arrange(new UIBox2(0f, 0f, finalSize.X, finalSize.Y));
		}
		return finalSize;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		Color val2 = (_isActive ? Color.FromHex((ReadOnlySpan<char>)"#1a2a1a", (Color?)null) : ((!_hovered) ? DarkPanel : CardHoverColor));
		handle.DrawRect(val, val2, true);
		if (_isActive)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num = 0.2f + MathF.Sin((float)totalSeconds * 2f) * 0.1f;
			UIBox2 val3 = default(UIBox2);
			((UIBox2)(ref val3))._002Ector(val.Left - 2f, val.Top - 2f, val.Right + 2f, val.Bottom + 2f);
			handle.DrawRect(val3, ((Color)(ref GreenSuccess)).WithAlpha(num), true);
		}
		if (_hovered && !_isActive)
		{
			UIBox2 val4 = default(UIBox2);
			((UIBox2)(ref val4))._002Ector(val.Left - 1f, val.Top - 1f, val.Right + 1f, val.Bottom + 1f);
			handle.DrawRect(val4, ((Color)(ref CardBorderColor)).WithAlpha(0.5f), true);
		}
		Color val5 = (_isActive ? GreenSuccess : CardBorderColor);
		float num2 = (_isActive ? 2f : 1f);
		UIBox2 val6 = default(UIBox2);
		for (float num3 = 0f; num3 < num2; num3 += 1f)
		{
			((UIBox2)(ref val6))._002Ector(val.Left + num3, val.Top + num3, val.Right - num3, val.Bottom - num3);
			handle.DrawRect(val6, Color.Transparent, true);
			((DrawingHandleBase)handle).DrawLine(val6.TopLeft, ((UIBox2)(ref val6)).TopRight, val5);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val6)).TopRight, val6.BottomRight, val5);
			((DrawingHandleBase)handle).DrawLine(val6.BottomRight, ((UIBox2)(ref val6)).BottomLeft, val5);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val6)).BottomLeft, val6.TopLeft, val5);
		}
		if (_isActive)
		{
			UIBox2 val7 = default(UIBox2);
			((UIBox2)(ref val7))._002Ector(val.Left, val.Bottom - 3f, val.Right, val.Bottom);
			handle.DrawRect(val7, ((Color)(ref GreenSuccess)).WithAlpha(0.8f), true);
		}
		if (_label != null)
		{
			_label.FontColorOverride = (_isActive ? GreenSuccess : (_hovered ? Color.White : Color.FromHex((ReadOnlySpan<char>)"#c0c0c0", (Color?)null)));
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
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick))
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
