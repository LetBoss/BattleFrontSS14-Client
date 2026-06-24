namespace Content.Shared._RMC14.Weapons;

public static class RMCWeaponProfileRuleHash
{
	public static int Compute(string? value, int salt)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return 0;
		}
		string trimmed = value.Trim();
		uint hash = (uint)(-2128831035 ^ salt);
		for (int i = 0; i < trimmed.Length; i++)
		{
			hash ^= char.ToUpperInvariant(trimmed[i]);
			hash *= 16777619;
		}
		hash ^= (uint)trimmed.Length;
		hash *= 16777619;
		return (int)(hash & 0x7FFFFFFF);
	}
}
