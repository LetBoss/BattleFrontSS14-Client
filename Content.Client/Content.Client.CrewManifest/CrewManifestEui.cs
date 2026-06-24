using Content.Client.Eui;
using Content.Shared.CrewManifest;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.CrewManifest;

public sealed class CrewManifestEui : BaseEui
{
	private readonly CrewManifestUi _window;

	public CrewManifestEui()
	{
		_window = new CrewManifestUi();
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void Opened()
	{
		base.Opened();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_window).Close();
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (state is CrewManifestEuiState crewManifestEuiState)
		{
			_window.Populate(crewManifestEuiState.StationName, crewManifestEuiState.Entries);
		}
	}
}
