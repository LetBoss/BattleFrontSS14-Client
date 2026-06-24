using Content.Shared.Eui;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Eui;

public abstract class BaseEui
{
	[Dependency]
	private IClientNetManager _netManager;

	public EuiManager Manager { get; private set; }

	public uint Id { get; private set; }

	protected BaseEui()
	{
		IoCManager.InjectDependencies<BaseEui>(this);
	}

	internal void Initialize(EuiManager mgr, uint id)
	{
		Manager = mgr;
		Id = id;
	}

	public virtual void Opened()
	{
	}

	public virtual void Closed()
	{
	}

	public virtual void HandleState(EuiStateBase state)
	{
	}

	public virtual void HandleMessage(EuiMessageBase msg)
	{
	}

	protected void SendMessage(EuiMessageBase msg)
	{
		MsgEuiMessage msgEuiMessage = new MsgEuiMessage();
		msgEuiMessage.Id = Id;
		msgEuiMessage.Message = msg;
		((INetManager)_netManager).ClientSendMessage((NetMessage)(object)msgEuiMessage);
	}
}
