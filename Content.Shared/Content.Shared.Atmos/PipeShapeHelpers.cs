using System;

namespace Content.Shared.Atmos;

public static class PipeShapeHelpers
{
	public static PipeDirection ToBaseDirection(this PipeShape shape)
	{
		return shape switch
		{
			PipeShape.Half => PipeDirection.South, 
			PipeShape.Straight => PipeDirection.Longitudinal, 
			PipeShape.Bend => PipeDirection.SWBend, 
			PipeShape.TJunction => PipeDirection.TSouth, 
			PipeShape.Fourway => PipeDirection.Fourway, 
			_ => throw new ArgumentOutOfRangeException("shape", $"{shape} does not have an associated {"PipeDirection"}."), 
		};
	}
}
