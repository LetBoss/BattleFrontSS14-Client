using System;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

public sealed class ClipControl : Control
{
	private bool _clipHorizontal = true;

	private bool _clipVertical = true;

	public bool ClipHorizontal
	{
		get
		{
			return _clipHorizontal;
		}
		set
		{
			_clipHorizontal = value;
			((Control)this).InvalidateMeasure();
		}
	}

	public bool ClipVertical
	{
		get
		{
			return _clipVertical;
		}
		set
		{
			_clipVertical = value;
			((Control)this).InvalidateMeasure();
		}
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		if (ClipHorizontal)
		{
			Vector2 vector = availableSize;
			vector.X = float.PositiveInfinity;
			availableSize = vector;
		}
		if (ClipVertical)
		{
			Vector2 vector = availableSize;
			vector.Y = float.PositiveInfinity;
			availableSize = vector;
		}
		return ((Control)this).MeasureOverride(availableSize);
	}

	protected unsafe override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				current.Arrange(UIBox2.FromDimensions(Vector2.Zero, current.DesiredSize));
			}
			return finalSize;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
