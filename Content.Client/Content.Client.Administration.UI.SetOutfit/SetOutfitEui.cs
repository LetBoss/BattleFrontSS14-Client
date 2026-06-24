using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Administration.UI.SetOutfit;

public sealed class SetOutfitEui : BaseEui
{
	private readonly SetOutfitMenu _window;

	private IEntityManager _entManager;

	public SetOutfitEui()
	{
		_entManager = IoCManager.Resolve<IEntityManager>();
		_window = new SetOutfitMenu();
		((BaseWindow)_window).OnClose += OnClosed;
	}

	private void OnClosed()
	{
		SendMessage(new CloseEuiMessage());
	}

	public override void Opened()
	{
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_window).Close();
	}

	public override void HandleState(EuiStateBase state)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		SetOutfitEuiState setOutfitEuiState = (SetOutfitEuiState)state;
		_window.TargetEntityId = setOutfitEuiState.TargetNetEntity;
	}
}
