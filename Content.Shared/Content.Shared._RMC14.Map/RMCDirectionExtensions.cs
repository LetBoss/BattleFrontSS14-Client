using System;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Map;

public static class RMCDirectionExtensions
{
	public static (Direction First, Direction Second) GetPerpendiculars(this Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		switch ((int)direction)
		{
		case 0:
		case 4:
			return (First: (Direction)6, Second: (Direction)2);
		case 1:
		case 5:
			return (First: (Direction)7, Second: (Direction)3);
		case 2:
		case 6:
			return (First: (Direction)4, Second: (Direction)0);
		case 3:
		case 7:
			return (First: (Direction)5, Second: (Direction)1);
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
	}

	public static bool IsCardinal(this Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		switch ((int)direction)
		{
		case 0:
		case 2:
		case 4:
		case 6:
			return true;
		default:
			return false;
		}
	}
}
