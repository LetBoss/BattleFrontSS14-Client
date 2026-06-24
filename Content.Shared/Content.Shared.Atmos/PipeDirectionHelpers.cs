using System;
using Robust.Shared.Maths;

namespace Content.Shared.Atmos;

public static class PipeDirectionHelpers
{
	public const int PipeDirections = 4;

	public const int AllPipeDirections = 6;

	public static bool HasDirection(this PipeDirection pipeDirection, PipeDirection other)
	{
		return (pipeDirection & other) == other;
	}

	public static Angle ToAngle(this PipeDirection pipeDirection)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return DirectionExtensions.ToAngle(pipeDirection.ToDirection());
	}

	public static PipeDirection ToPipeDirection(this Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		return (int)direction switch
		{
			4 => PipeDirection.North, 
			0 => PipeDirection.South, 
			2 => PipeDirection.East, 
			6 => PipeDirection.West, 
			_ => throw new ArgumentOutOfRangeException("direction"), 
		};
	}

	public static Direction ToDirection(this PipeDirection pipeDirection)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return (Direction)(pipeDirection switch
		{
			PipeDirection.North => 4, 
			PipeDirection.South => 0, 
			PipeDirection.East => 2, 
			PipeDirection.West => 6, 
			_ => throw new ArgumentOutOfRangeException("pipeDirection"), 
		});
	}

	public static PipeDirection GetOpposite(this PipeDirection pipeDirection)
	{
		return pipeDirection switch
		{
			PipeDirection.North => PipeDirection.South, 
			PipeDirection.South => PipeDirection.North, 
			PipeDirection.East => PipeDirection.West, 
			PipeDirection.West => PipeDirection.East, 
			_ => throw new ArgumentOutOfRangeException("pipeDirection"), 
		};
	}

	public static PipeShape PipeDirectionToPipeShape(this PipeDirection pipeDirection)
	{
		return pipeDirection switch
		{
			PipeDirection.North => PipeShape.Half, 
			PipeDirection.South => PipeShape.Half, 
			PipeDirection.East => PipeShape.Half, 
			PipeDirection.West => PipeShape.Half, 
			PipeDirection.Lateral => PipeShape.Straight, 
			PipeDirection.Longitudinal => PipeShape.Straight, 
			PipeDirection.NEBend => PipeShape.Bend, 
			PipeDirection.NWBend => PipeShape.Bend, 
			PipeDirection.SEBend => PipeShape.Bend, 
			PipeDirection.SWBend => PipeShape.Bend, 
			PipeDirection.TNorth => PipeShape.TJunction, 
			PipeDirection.TSouth => PipeShape.TJunction, 
			PipeDirection.TEast => PipeShape.TJunction, 
			PipeDirection.TWest => PipeShape.TJunction, 
			PipeDirection.Fourway => PipeShape.Fourway, 
			_ => throw new ArgumentOutOfRangeException("pipeDirection"), 
		};
	}

	public static PipeDirection RotatePipeDirection(this PipeDirection pipeDirection, double diff)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		PipeDirection newPipeDir = PipeDirection.None;
		for (int i = 0; i < 4; i++)
		{
			PipeDirection currentPipeDirection = (PipeDirection)(1 << i);
			if (pipeDirection.HasFlag(currentPipeDirection))
			{
				Angle angle = currentPipeDirection.ToAngle();
				angle += Angle.op_Implicit(diff);
				newPipeDir |= ((Angle)(ref angle)).GetCardinalDir().ToPipeDirection();
			}
		}
		return newPipeDir;
	}
}
