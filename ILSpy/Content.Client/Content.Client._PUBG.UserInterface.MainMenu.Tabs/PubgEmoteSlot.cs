using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgEmoteSlot : Control
{
	[Dependency]
	private IGameTiming _timing;

	private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>)"#00FF88", (Color?)null);

	private static readonly Color RedLocked = Color.FromHex((ReadOnlySpan<char>)"#AA3333", (Color?)null);

	private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>)"#0a0a0f", (Color?)null);

	private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a3a", (Color?)null);

	private static readonly Color IconBgColor = Color.FromHex((ReadOnlySpan<char>)"#0f0a1e", (Color?)null);

	private bool _isLocked;

	private bool _isFilled;

	private int _slotNumber;

	private BoxContainer? _contentContainer;

	public bool IsLocked
	{
		get
		{
			return _isLocked;
		}
		set
		{
			_isLocked = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool IsFilled
	{
		get
		{
			return _isFilled;
		}
		set
		{
			_isFilled = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public int SlotNumber
	{
		get
		{
			return _slotNumber;
		}
		set
		{
			_slotNumber = value;
		}
	}

	public PubgEmoteSlot()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		IoCManager.InjectDependencies<PubgEmoteSlot>(this);
		((Control)this).MinSize = new Vector2(180f, 140f);
		_contentContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		((Control)this).AddChild((Control)(object)_contentContainer);
	}

	public void SetContent(Control content)
	{
		BoxContainer? contentContainer = _contentContainer;
		if (contentContainer != null)
		{
			((Control)contentContainer).RemoveAllChildren();
		}
		BoxContainer? contentContainer2 = _contentContainer;
		if (contentContainer2 != null)
		{
			((Control)contentContainer2).AddChild(content);
		}
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (_contentContainer != null)
		{
			((Control)_contentContainer).Arrange(new UIBox2(0f, 0f, finalSize.X, finalSize.Y));
		}
		return finalSize;
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(0f, 0f, (float)((Control)this).PixelSize.X, (float)((Control)this).PixelSize.Y);
		float num = (_isLocked ? 0.3f : (_isFilled ? 0.7f : 0.5f));
		handle.DrawRect(val, ((Color)(ref DarkPanel)).WithAlpha(num), true);
		Color val2;
		float num2;
		if (_isLocked)
		{
			val2 = RedLocked;
			num2 = 2f;
		}
		else if (_isFilled)
		{
			val2 = GreenSuccess;
			num2 = 3f;
		}
		else
		{
			val2 = CardBorderColor;
			num2 = 2f;
		}
		UIBox2 val3 = default(UIBox2);
		for (float num3 = 0f; num3 < num2; num3 += 1f)
		{
			((UIBox2)(ref val3))._002Ector(val.Left + num3, val.Top + num3, val.Right - num3, val.Bottom - num3);
			handle.DrawRect(val3, Color.Transparent, true);
			((DrawingHandleBase)handle).DrawLine(val3.TopLeft, ((UIBox2)(ref val3)).TopRight, val2);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val3)).TopRight, val3.BottomRight, val2);
			((DrawingHandleBase)handle).DrawLine(val3.BottomRight, ((UIBox2)(ref val3)).BottomLeft, val2);
			((DrawingHandleBase)handle).DrawLine(((UIBox2)(ref val3)).BottomLeft, val3.TopLeft, val2);
		}
		if (_isFilled && !_isLocked)
		{
			double totalSeconds = _timing.RealTime.TotalSeconds;
			float num4 = 0.15f + MathF.Sin((float)totalSeconds * 2f) * 0.08f;
			UIBox2 val4 = default(UIBox2);
			((UIBox2)(ref val4))._002Ector(val.Left - 2f, val.Top - 2f, val.Right + 2f, val.Bottom + 2f);
			handle.DrawRect(val4, ((Color)(ref GreenSuccess)).WithAlpha(num4), true);
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_isFilled && !_isLocked)
		{
			((Control)this).InvalidateMeasure();
		}
	}
}
