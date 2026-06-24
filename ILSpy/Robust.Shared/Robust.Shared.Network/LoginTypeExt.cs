namespace Robust.Shared.Network;

public static class LoginTypeExt
{
	public static bool HasStaticUserId(this LoginType type)
	{
		if (type != LoginType.LoggedIn)
		{
			return type == LoginType.GuestAssigned;
		}
		return true;
	}
}
