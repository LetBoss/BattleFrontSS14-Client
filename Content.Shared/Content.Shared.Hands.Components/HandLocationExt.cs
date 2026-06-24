using System;

namespace Content.Shared.Hands.Components;

public static class HandLocationExt
{
	public static HandUILocation GetUILocation(this HandLocation location)
	{
		return location switch
		{
			HandLocation.Left => HandUILocation.Left, 
			HandLocation.Middle => HandUILocation.Right, 
			HandLocation.Right => HandUILocation.Right, 
			_ => throw new ArgumentOutOfRangeException("location", location, null), 
		};
	}
}
