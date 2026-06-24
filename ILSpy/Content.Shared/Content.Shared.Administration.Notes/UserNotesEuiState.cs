using System;
using System.Collections.Generic;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

[Serializable]
[NetSerializable]
public sealed class UserNotesEuiState : EuiStateBase
{
	public Dictionary<(int, NoteType), SharedAdminNote> Notes { get; }

	public UserNotesEuiState(Dictionary<(int, NoteType), SharedAdminNote> notes)
	{
		Notes = notes;
	}
}
