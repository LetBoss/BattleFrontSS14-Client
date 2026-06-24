using System;
using Content.Shared.Administration;

namespace Content.Client.Administration.Managers;

public interface IClientAdminManager
{
	event Action AdminStatusUpdated;

	AdminData? GetAdminData(bool includeDeAdmin = false);

	bool IsActive();

	bool HasFlag(AdminFlags flag);

	bool CanCommand(string cmdName);

	bool CanViewVar();

	bool CanAdminPlace();

	bool CanScript();

	bool CanAdminMenu();

	void Initialize();

	bool IsAdmin(bool includeDeAdmin = false)
	{
		return GetAdminData(includeDeAdmin) != null;
	}
}
