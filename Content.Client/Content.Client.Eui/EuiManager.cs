using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Reflection;

namespace Content.Client.Eui;

public sealed class EuiManager
{
	private sealed class EuiData
	{
		public readonly BaseEui Eui;

		public EuiData(BaseEui eui)
		{
			Eui = eui;
		}
	}

	[Dependency]
	private IClientNetManager _net;

	[Dependency]
	private IReflectionManager _refl;

	[Dependency]
	private IDynamicTypeFactory _dtf;

	private readonly Dictionary<uint, EuiData> _openUis = new Dictionary<uint, EuiData>();

	public void Initialize()
	{
		((INetManager)_net).RegisterNetMessage<MsgEuiCtl>((ProcessMessage<MsgEuiCtl>)RxMsgCtl, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MsgEuiState>((ProcessMessage<MsgEuiState>)RxMsgState, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MsgEuiMessage>((ProcessMessage<MsgEuiMessage>)RxMsgMessage, (NetMessageAccept)3);
		((INetManager)_net).Disconnect += NetOnDisconnect;
	}

	private void NetOnDisconnect(object? sender, NetDisconnectedArgs e)
	{
		foreach (KeyValuePair<uint, EuiData> openUi in _openUis)
		{
			openUi.Value.Eui.Closed();
		}
		_openUis.Clear();
	}

	private void RxMsgMessage(MsgEuiMessage message)
	{
		_openUis[message.Id].Eui.HandleMessage(message.Message);
	}

	private void RxMsgState(MsgEuiState message)
	{
		_openUis[message.Id].Eui.HandleState(message.State);
	}

	private void RxMsgCtl(MsgEuiCtl message)
	{
		if (_openUis.TryGetValue(message.Id, out EuiData value))
		{
			value.Eui.Closed();
			_openUis.Remove(message.Id);
		}
		if (message.Type == MsgEuiCtl.CtlType.Open)
		{
			Type type = _refl.LooseGetType(message.OpenType);
			BaseEui baseEui = DynamicTypeFactoryExt.CreateInstance<BaseEui>(_dtf, type, false, true);
			baseEui.Initialize(this, message.Id);
			baseEui.Opened();
			_openUis.Add(message.Id, new EuiData(baseEui));
		}
	}
}
