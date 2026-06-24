using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class StripeBack : Container
{
	private const float PadSize = 4f;

	private const float EdgeSize = 2f;

	private static readonly Color EdgeColor = Color.FromHex((ReadOnlySpan<char>)"#525252ff", (Color?)null);

	private bool _hasTopEdge = true;

	private bool _hasBottomEdge = true;

	private bool _hasMargins = true;

	public const string StylePropertyBackground = "background";

	public bool HasTopEdge
	{
		get
		{
			return _hasTopEdge;
		}
		set
		{
			_hasTopEdge = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool HasBottomEdge
	{
		get
		{
			return _hasBottomEdge;
		}
		set
		{
			_hasBottomEdge = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool HasMargins
	{
		get
		{
			return _hasMargins;
		}
		set
		{
			_hasMargins = value;
			((Control)this).InvalidateMeasure();
		}
	}

	protected unsafe override Vector2 MeasureOverride(Vector2 availableSize)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float num = (HasMargins ? 4f : 0f);
		float num2 = 0f;
		if (HasBottomEdge)
		{
			num2 += num + 2f;
		}
		if (HasTopEdge)
		{
			num2 += num + 2f;
		}
		Vector2 vector = Vector2.Zero;
		availableSize.Y -= num2;
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				current.Measure(availableSize);
				vector = Vector2.Max(vector, current.DesiredSize);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return vector + new Vector2(0f, num2);
	}

	protected unsafe override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = default(UIBox2);
		((UIBox2)(ref val))._002Ector(Vector2.Zero, finalSize);
		float num = (HasMargins ? 4f : 0f);
		if (HasTopEdge)
		{
			val += (0f, num + 2f, 0f, 0f);
		}
		if (HasBottomEdge)
		{
			val += (0f, 0f, 0f, 0f - (num + 2f));
		}
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				((Enumerator)(ref enumerator)).Current.Arrange(val);
			}
			return finalSize;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 val = UIBox2i.op_Implicit(((Control)this).PixelSizeBox);
		float num = (HasMargins ? 4f : 0f);
		if (HasTopEdge)
		{
			val += (0f, (num + 2f) * ((Control)this).UIScale, 0f, 0f);
			handle.DrawRect(new UIBox2(0f, num * ((Control)this).UIScale, (float)((Control)this).PixelWidth, val.Top), EdgeColor, true);
		}
		if (HasBottomEdge)
		{
			val += (0f, 0f, 0f, 0f - (num + 2f) * ((Control)this).UIScale);
			handle.DrawRect(new UIBox2(0f, val.Bottom, (float)((Control)this).PixelWidth, (float)((Control)this).PixelHeight - num * ((Control)this).UIScale), EdgeColor, true);
		}
		StyleBox? actualStyleBox = GetActualStyleBox();
		if (actualStyleBox != null)
		{
			actualStyleBox.Draw(handle, val, ((Control)this).UIScale);
		}
	}

	private StyleBox? GetActualStyleBox()
	{
		StyleBox result = default(StyleBox);
		if (!((Control)this).TryGetStyleProperty<StyleBox>("background", ref result))
		{
			return null;
		}
		return result;
	}
}
