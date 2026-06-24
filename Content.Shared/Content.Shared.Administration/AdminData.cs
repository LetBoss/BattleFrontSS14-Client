namespace Content.Shared.Administration;

public sealed class AdminData
{
	public bool Active;

	public bool Stealth;

	public string? Title;

	public AdminFlags Flags;

	public bool HasFlag(AdminFlags flag, bool includeDeAdmin = false)
	{
		if (includeDeAdmin || Active)
		{
			return (Flags & flag) == flag;
		}
		return false;
	}

	public bool CanAdminPlace()
	{
		return HasFlag(AdminFlags.Spawn);
	}

	public bool CanScript()
	{
		return HasFlag(AdminFlags.Host);
	}

	public bool CanAdminMenu()
	{
		return HasFlag(AdminFlags.Admin);
	}

	public bool CanStealth()
	{
		return HasFlag(AdminFlags.Stealth);
	}

	public bool CanAdminReloadPrototypes()
	{
		return HasFlag(AdminFlags.Host);
	}
}
