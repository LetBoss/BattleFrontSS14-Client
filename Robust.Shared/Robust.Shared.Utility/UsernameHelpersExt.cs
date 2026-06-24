using Robust.Shared.AuthLib;

namespace Robust.Shared.Utility;

public static class UsernameHelpersExt
{
	public static string ToText(this UsernameInvalidReason reason)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected I4, but got Unknown
		return (int)reason switch
		{
			0 => "Username is... valid?", 
			1 => "Username can't be empty.", 
			3 => "Username is too long.", 
			4 => "Contains invalid characters. Only use A-Z, 0-9 and underscores.", 
			_ => "Unknown reason", 
		};
	}
}
