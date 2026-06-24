using Content.Client.Eui;
using Content.Shared.Administration.Notes;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Administration.UI.AdminRemarks;

public sealed class UserNotesEui : BaseEui
{
	private AdminRemarksWindow NoteWindow { get; }

	public UserNotesEui()
	{
		NoteWindow = new AdminRemarksWindow();
		((BaseWindow)NoteWindow).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void HandleState(EuiStateBase state)
	{
		if (state is UserNotesEuiState userNotesEuiState)
		{
			NoteWindow.SetNotes(userNotesEuiState.Notes);
		}
	}

	public override void Opened()
	{
		((BaseWindow)NoteWindow).OpenCentered();
	}
}
