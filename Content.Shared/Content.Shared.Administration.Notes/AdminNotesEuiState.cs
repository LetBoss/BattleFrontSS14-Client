using System;
using System.Collections.Generic;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

[Serializable]
[NetSerializable]
public sealed class AdminNotesEuiState : EuiStateBase
{
	public string NotedPlayerName { get; }

	public Dictionary<(int noteId, NoteType noteType), SharedAdminNote> Notes { get; }

	public bool CanCreate { get; }

	public bool CanDelete { get; }

	public bool CanEdit { get; }

	public AdminNotesEuiState(string notedPlayerName, Dictionary<(int, NoteType), SharedAdminNote> notes, bool canCreate, bool canDelete, bool canEdit)
	{
		NotedPlayerName = notedPlayerName;
		Notes = notes;
		CanCreate = canCreate;
		CanDelete = canDelete;
		CanEdit = canEdit;
	}
}
