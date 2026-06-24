using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgStatCard : Control
{
	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color OrangeWarning = Color.FromHex((ReadOnlySpan<char>)"#FF9500", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color ProgressBg = Color.FromHex((ReadOnlySpan<char>)"#0d0d15", (Color?)null);

	private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>)"#00FFB3", (Color?)null);

	[Dependency]
	private IGameTiming _timing;

	private string _title = "";

	private string _value = "";

	private float _progress;

	private bool _showProgress;

	private bool _isPulse;

	private Color _valueColor = Color.White;

	private Label? _titleLabel;

	private Label? _valueLabel;

	public string Title
	{
		get
		{
			return _title;
		}
		set
		{
			_title = value;
			if (_titleLabel != null)
			{
				_titleLabel.Text = value;
			}
		}
	}

	public string Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			if (_valueLabel != null)
			{
				_valueLabel.Text = value;
			}
		}
	}

	public float Progress
	{
		get
		{
			return _progress;
		}
		set
		{
			_progress = Math.Clamp(value, 0f, 1f);
		}
	}

	public bool ShowProgress
	{
		get
		{
			return _showProgress;
		}
		set
		{
			_showProgress = value;
		}
	}

	public bool IsPulse
	{
		get
		{
			return _isPulse;
		}
		set
		{
			_isPulse = value;
		}
	}

	public Color ValueColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _valueColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			_valueColor = value;
			if (_valueLabel != null)
			{
				_valueLabel.FontColorOverride = value;
			}
		}
	}

	public PubgStatCard()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<PubgStatCard>(this);
		((Control)this).MinHeight = 70f;
		((Control)this).HorizontalExpand = true;
		BuildUI();
	}

	private void BuildUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		_titleLabel = new Label
		{
			Text = _title,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#c0c0c0", (Color?)null),
			HorizontalAlignment = (HAlignment)1,
			Margin = new Thickness(12f, 8f, 12f, 0f)
		};
		((Control)this).AddChild((Control)(object)_titleLabel);
		_valueLabel = new Label
		{
			Text = _value,
			FontColorOverride = _valueColor,
			HorizontalAlignment = (HAlignment)1,
			Margin = new Thickness(12f, 28f, 12f, 0f)
		};
		((Control)_valueLabel).SetOnlyStyleClass("LabelHeadingBigger");
		((Control)this).AddChild((Control)(object)_valueLabel);
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (_titleLabel != null)
		{
			((Control)_titleLabel).Arrange(new UIBox2(0f, 0f, finalSize.X, 20f));
		}
		if (_valueLabel != null)
		{
			((Control)_valueLabel).Arrange(new UIBox2(0f, 0f, finalSize.X, finalSize.Y - 10f));
		}
		return finalSize;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		handle.DrawRect(val, ((Color)(ref DarkPanel)).WithAlpha(0.6f), true);
		((DrawingHandleBase)handle).DrawLine(val.TopLeft, ((UIBox2)(ref val)).TopRight, CardBorderColor);
		((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val)).TopRight, val.BottomRight, CardBorderColor);
		((DrawingHandleBase)handle).DrawLine(val.BottomRight, ((UIBox2)(ref val)).BottomLeft, CardBorderColor);
		((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val)).BottomLeft, val.TopLeft, CardBorderColor);
		if (_showProgress)
		{
			int num = ((Control)this).PixelSize.Y - 6;
			float num2 = 4f;
			UIBox2 val2 = default(UIBox2);
			((UIBox2)(ref val2))._002Ector(2f, (float)num, (float)(((Control)this).PixelSize.X - 2), (float)num + num2);
			handle.DrawRect(val2, ProgressBg, true);
			if (_progress > 0f)
			{
				float num3 = (float)(((Control)this).PixelSize.X - 4) * _progress;
				UIBox2 val3 = default(UIBox2);
				((UIBox2)(ref val3))._002Ector(2f, (float)num, 2f + num3, (float)num + num2);
				Color val4 = ((_progress >= 0.9f) ? CompletedGlow : ((_progress >= 0.6f) ? LerpColor(OrangeWarning, GreenSuccess, (_progress - 0.6f) / 0.4f) : OrangeWarning));
				handle.DrawRect(val3, val4, true);
			}
		}
		if (_isPulse)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num4 = 0.15f + MathF.Sin((float)totalSeconds * 2.5f) * 0.1f;
			UIBox2 val5 = default(UIBox2);
			((UIBox2)(ref val5))._002Ector(val.Left - 2f, val.Top - 2f, val.Right + 2f, val.Bottom + 2f);
			handle.DrawRect(val5, ((Color)(ref GoldAccent)).WithAlpha(num4), true);
		}
	}

	private static Color LerpColor(Color a, Color b, float t)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		t = Math.Clamp(t, 0f, 1f);
		return new Color(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_isPulse)
		{
			((Control)this).InvalidateMeasure();
		}
	}
}
