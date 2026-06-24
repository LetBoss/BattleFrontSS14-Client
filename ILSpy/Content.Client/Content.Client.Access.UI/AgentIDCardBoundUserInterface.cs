using System;
using Content.Shared.Access.Systems;
using Content.Shared.StatusIcon;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Access.UI;

public sealed class AgentIDCardBoundUserInterface : BoundUserInterface
{
	private AgentIDCardWindow? _window;

	public AgentIDCardBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<AgentIDCardWindow>((BoundUserInterface)(object)this);
		_window.OnNameChanged += OnNameChanged;
		_window.OnJobChanged += OnJobChanged;
		_window.OnJobIconChanged += OnJobIconChanged;
	}

	private void OnNameChanged(string newName)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AgentIDCardNameChangedMessage(newName));
	}

	private void OnJobChanged(string newJob)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AgentIDCardJobChangedMessage(newJob));
	}

	public void OnJobIconChanged(ProtoId<JobIconPrototype> newJobIconId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AgentIDCardJobIconChangedMessage(newJobIconId));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is AgentIDCardBoundUserInterfaceState agentIDCardBoundUserInterfaceState)
		{
			_window.SetCurrentName(agentIDCardBoundUserInterfaceState.CurrentName);
			_window.SetCurrentJob(agentIDCardBoundUserInterfaceState.CurrentJob);
			_window.SetAllowedIcons(agentIDCardBoundUserInterfaceState.CurrentJobIconId);
		}
	}
}
