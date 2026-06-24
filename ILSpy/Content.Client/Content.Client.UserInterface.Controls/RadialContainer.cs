using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialContainer : LayoutContainer
{
	public enum RAlignment : byte
	{
		Clockwise,
		AntiClockwise
	}

	private const float RadiusIncrement = 5f;

	private Vector2 _angularRange = new Vector2(0f, MathF.PI * 2f);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 AngularRange
	{
		get
		{
			return _angularRange;
		}
		set
		{
			float x = value.X;
			float y = value.Y;
			x = ((x > MathF.PI * 2f) ? (x % (MathF.PI * 2f)) : x);
			y = ((y > MathF.PI * 2f) ? (y % (MathF.PI * 2f)) : y);
			x = ((x < 0f) ? (MathF.PI * 2f + x) : x);
			y = ((y < 0f) ? (MathF.PI * 2f + y) : y);
			_angularRange = new Vector2(x, y);
		}
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public RAlignment RadialAlignment { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float InitialRadius { get; set; } = 100f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float CalculatedRadius { get; private set; }

	public float InnerRadiusMultiplier { get; set; } = 0.5f;

	public float OuterRadiusMultiplier { get; set; } = 1.5f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool ReserveSpaceForHiddenChildren { get; set; } = true;

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		IEnumerable<Control> source;
		if (!ReserveSpaceForHiddenChildren)
		{
			source = ((IEnumerable<Control>)((Control)this).Children).Where((Control val) => val.Visible);
		}
		else
		{
			IEnumerable<Control> children = (IEnumerable<Control>)((Control)this).Children;
			source = children;
		}
		int num = source.Count();
		CalculatedRadius = InitialRadius + (float)num * 5f;
		bool flag = RadialAlignment == RAlignment.AntiClockwise;
		float num2 = AngularRange.Y - AngularRange.X;
		num2 = ((num2 < 0f) ? (MathF.PI * 2f + num2) : num2);
		num2 = (flag ? (MathF.PI * 2f - num2) : num2);
		int num3 = ((!MathHelper.CloseTo(num2, MathF.PI * 2f, 0.01f)) ? 1 : 0);
		float num4 = num2 / (float)(num - num3);
		num4 *= (flag ? (-1f) : 1f);
		Vector2 vector = finalSize * 0.5f;
		foreach (var item3 in source.Select((Control item3, int index) => (index: index, x: item3)))
		{
			int item = item3.index;
			Control item2 = item3.x;
			float x = AngularRange.X + num4 * ((float)item + 0.5f) + MathF.PI / 2f;
			Vector2 vector2 = new Vector2(MathF.Floor(CalculatedRadius * MathF.Cos(x)), MathF.Floor((0f - CalculatedRadius) * MathF.Sin(x))) + vector - item2.DesiredSize * 0.5f + ((Control)this).Position;
			LayoutContainer.SetPosition(item2, vector2);
			if (item2 is IRadialMenuItemWithSector radialMenuItemWithSector)
			{
				radialMenuItemWithSector.AngleSectorFrom = num4 * (float)item;
				radialMenuItemWithSector.AngleSectorTo = num4 * (float)(item + 1);
				radialMenuItemWithSector.AngleOffset = MathF.PI / 2f;
				radialMenuItemWithSector.InnerRadius = CalculatedRadius * InnerRadiusMultiplier;
				radialMenuItemWithSector.OuterRadius = CalculatedRadius * OuterRadiusMultiplier;
				radialMenuItemWithSector.ParentCenter = vector;
			}
		}
		return ((LayoutContainer)this).ArrangeOverride(finalSize);
	}
}
