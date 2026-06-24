using Content.Client.Administration.Managers;
using Content.Shared.CCVar;
using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Client.DebugMon;

internal sealed class DebugMonitorManager
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IUserInterfaceManager _userInterface;

	[Dependency]
	private IBaseClient _baseClient;

	public void FrameUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)_baseClient.RunLevel == 4 && !_admin.IsActive() && _cfg.GetCVar<bool>(CCVars.DebugCoordinatesAdminOnly))
		{
			_userInterface.DebugMonitors.SetMonitor((DebugMonitor)1, false);
		}
	}
}
