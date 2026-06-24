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

public sealed class PubgEmoteCard : Control
{
	[Dependency]
	private IGameTiming _timing;

	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>)"#252530", (Color?)null);

	private static readonly Color IconBgColor = Color.FromHex((ReadOnlySpan<char>)"#0f0a1e", (Color?)null);

	private string _emoteName = "";

	private string _emoteId = "";

	private Texture? _emoteIcon;

	private bool _isEquipped;

	private bool _hovered;

	private PanelContainer? _iconContainer;

	private TextureRect? _iconView;

	private Label? _nameLabel;

	private Label? _statusLabel;

	public string EmoteName
	{
		get
		{
			return _emoteName;
		}
		set
		{
			_emoteName = value;
			if (_nameLabel != null)
			{
				_nameLabel.Text = value;
			}
		}
	}

	public string EmoteId
	{
		get
		{
			return _emoteId;
		}
		set
		{
			_emoteId = value;
		}
	}

	public Texture? EmoteIcon
	{
		get
		{
			return _emoteIcon;
		}
		set
		{
			_emoteIcon = value;
			UpdateIcon();
		}
	}

	public bool IsEquipped
	{
		get
		{
			return _isEquipped;
		}
		set
		{
			_isEquipped = value;
			UpdateStatus();
		}
	}

	public event Action<string>? OnCardClicked;

	public PubgEmoteCard()
	{
		IoCManager.InjectDependencies<PubgEmoteCard>(this);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).MinSize = new Vector2(140f, 180f);
		BuildUI();
	}

	private void BuildUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		_iconContainer = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ((Color)(ref IconBgColor)).WithAlpha(0.6f)
			},
			Margin = new Thickness(4f, 4f, 4f, 0f),
			MinSize = new Vector2(130f, 100f)
		};
		_iconView = new TextureRect
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			Stretch = (StretchMode)7
		};
		((Control)_iconContainer).AddChild((Control)(object)_iconView);
		((Control)this).AddChild((Control)(object)_iconContainer);
		_nameLabel = new Label
		{
			Text = _emoteName,
			FontColorOverride = Color.White,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(4f, 110f, 4f, 0f)
		};
		((Control)this).AddChild((Control)(object)_nameLabel);
		_statusLabel = new Label
		{
			Text = "",
			FontColorOverride = Color.Gray,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(4f, 130f, 4f, 0f)
		};
		((Control)this).AddChild((Control)(object)_statusLabel);
	}

	private void UpdateIcon()
	{
		if (_iconView != null && _emoteIcon != null)
		{
			_iconView.Texture = _emoteIcon;
		}
	}

	private void UpdateStatus()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (_statusLabel != null)
		{
			if (_isEquipped)
			{
				_statusLabel.Text = "✓ ВЫБРАНО";
				_statusLabel.FontColorOverride = GreenSuccess;
			}
			else
			{
				_statusLabel.Text = "";
			}
		}
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (_iconContainer != null)
		{
			((Control)_iconContainer).Arrange(new UIBox2(4f, 4f, finalSize.X - 4f, 105f));
		}
		if (_nameLabel != null)
		{
			((Control)_nameLabel).Arrange(new UIBox2(0f, 110f, finalSize.X, 130f));
		}
		if (_statusLabel != null)
		{
			((Control)_statusLabel).Arrange(new UIBox2(0f, 130f, finalSize.X, 150f));
		}
		return finalSize;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		handle.DrawRect(val, ((Color)(ref DarkPanel)).WithAlpha(0.8f), true);
		if (_hovered)
		{
			float num = 2f;
			UIBox2 val2 = default(UIBox2);
			((UIBox2)(ref val2))._002Ector(val.Left - num, val.Top - num, val.Right + num, val.Bottom + num);
			handle.DrawRect(val2, ((Color)(ref CardHoverColor)).WithAlpha(0.5f), true);
		}
		Color val3 = (_isEquipped ? GreenSuccess : CardBorderColor);
		float num2 = (_isEquipped ? 3f : 2f);
		UIBox2 val4 = default(UIBox2);
		for (float num3 = 0f; num3 < num2; num3 += 1f)
		{
			((UIBox2)(ref val4))._002Ector(val.Left + num3, val.Top + num3, val.Right - num3, val.Bottom - num3);
			handle.DrawRect(val4, Color.Transparent, true);
			((DrawingHandleBase)handle).DrawLine(val4.TopLeft, ((UIBox2)(ref val4)).TopRight, val3);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val4)).TopRight, val4.BottomRight, val3);
			((DrawingHandleBase)handle).DrawLine(val4.BottomRight, ((UIBox2)(ref val4)).BottomLeft, val3);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val4)).BottomLeft, val4.TopLeft, val3);
		}
		if (_isEquipped)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num4 = 0.2f + MathF.Sin((float)totalSeconds * 2f) * 0.1f;
			UIBox2 val5 = default(UIBox2);
			((UIBox2)(ref val5))._002Ector(val.Left - 3f, val.Top - 3f, val.Right + 3f, val.Bottom + 3f);
			handle.DrawRect(val5, ((Color)(ref GreenSuccess)).WithAlpha(num4), true);
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
			this.OnCardClicked?.Invoke(_emoteId);
			((BoundKeyEventArgs)args).Handle();
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_isEquipped)
		{
			((Control)this).InvalidateMeasure();
		}
	}
}
