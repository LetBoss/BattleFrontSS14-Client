using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared._RMC14.IdentityManagement;

public readonly record struct IdentityEntity(EntityUid Entity, string Name) : ILocValue
{
	public object Value => Entity;

	public static implicit operator EntityUid(IdentityEntity ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ent.Entity;
	}

	public static implicit operator string(IdentityEntity ent)
	{
		return ent.Name;
	}

	public string Format(LocContext ctx)
	{
		return Name;
	}

	public override string ToString()
	{
		return Name;
	}
}
