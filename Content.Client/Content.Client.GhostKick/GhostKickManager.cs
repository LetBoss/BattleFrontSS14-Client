using Content.Shared.GhostKick;
using Robust.Client;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.GhostKick;

public sealed class GhostKickManager
{
	private bool _fakeLossEnabled;

	[Dependency]
	private IBaseClient _baseClient;

	[Dependency]
	private IClientNetManager _netManager;

	[Dependency]
	private IConfigurationManager _cfg;

	public void Initialize()
	{
		((INetManager)_netManager).RegisterNetMessage<MsgGhostKick>((ProcessMessage<MsgGhostKick>)RxCallback, (NetMessageAccept)3);
		_baseClient.RunLevelChanged += BaseClientOnRunLevelChanged;
	}

	private void BaseClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (_fakeLossEnabled && (int)e.OldLevel == 4)
		{
			_cfg.SetCVar<float>(CVars.NetFakeLoss, 0f, false);
			_fakeLossEnabled = false;
		}
	}

	private void RxCallback(MsgGhostKick message)
	{
		_fakeLossEnabled = true;
		_cfg.SetCVar<float>(CVars.NetFakeLoss, 1f, false);
	}
}
