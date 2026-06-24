using System;
using Content.Client.Eui;
using Content.Shared.Administration.Notes;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Administration.UI.Notes;

public sealed class AdminNotesEui : BaseEui
{
	private AdminNotesWindow NoteWindow { get; }

	private AdminNotesControl NoteControl { get; }

	public AdminNotesEui()
	{
		NoteWindow = new AdminNotesWindow();
		NoteControl = NoteWindow.Notes;
		NoteControl.NoteChanged += delegate(int id, NoteType type, string text, NoteSeverity? severity, bool secret, DateTime? expiryTime)
		{
			SendMessage(new AdminNoteEuiMsg.EditNoteRequest(id, type, text, severity, secret, expiryTime));
		};
		NoteControl.NewNoteEntered += delegate(NoteType type, string text, NoteSeverity? severity, bool secret, DateTime? expiryTime)
		{
			SendMessage(new AdminNoteEuiMsg.CreateNoteRequest(type, text, severity, secret, expiryTime));
		};
		NoteControl.NoteDeleted += delegate(int id, NoteType type)
		{
			SendMessage(new AdminNoteEuiMsg.DeleteNoteRequest(id, type));
		};
		((BaseWindow)NoteWindow).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)NoteWindow).Close();
	}

	public override void HandleState(EuiStateBase state)
	{
		if (state is AdminNotesEuiState adminNotesEuiState)
		{
			NoteWindow.SetTitlePlayer(adminNotesEuiState.NotedPlayerName);
			NoteControl.SetPlayerName(adminNotesEuiState.NotedPlayerName);
			NoteControl.SetNotes(adminNotesEuiState.Notes);
			NoteControl.SetPermissions(adminNotesEuiState.CanCreate, adminNotesEuiState.CanDelete, adminNotesEuiState.CanEdit);
		}
	}

	public override void Opened()
	{
		((BaseWindow)NoteWindow).OpenCentered();
	}
}
