using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Maths;

public static class RMCDirectionExtensions
{
	public static string GetShorthand(this Direction direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		return (int)direction switch
		{
			0 => "S", 
			1 => "SE", 
			2 => "E", 
			3 => "NE", 
			4 => "N", 
			5 => "NW", 
			6 => "W", 
			7 => "SW", 
			_ => string.Empty, 
		};
	}
}
