using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Shared.Administration.Managers;

public interface ISharedAdminManager
{
	AdminData? GetAdminData(EntityUid uid, bool includeDeAdmin = false);

	AdminData? GetAdminData(ICommonSession session, bool includeDeAdmin = false);

	bool HasAdminFlag(EntityUid player, AdminFlags flag, bool includeDeAdmin = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetAdminData(player, includeDeAdmin)?.HasFlag(flag, includeDeAdmin) ?? false;
	}

	bool HasAdminFlag(ICommonSession player, AdminFlags flag, bool includeDeAdmin = false)
	{
		return GetAdminData(player, includeDeAdmin)?.HasFlag(flag, includeDeAdmin) ?? false;
	}

	bool IsAdmin(EntityUid uid, bool includeDeAdmin = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetAdminData(uid, includeDeAdmin) != null;
	}

	bool IsAdmin(ICommonSession session, bool includeDeAdmin = false)
	{
		return GetAdminData(session, includeDeAdmin) != null;
	}
}
