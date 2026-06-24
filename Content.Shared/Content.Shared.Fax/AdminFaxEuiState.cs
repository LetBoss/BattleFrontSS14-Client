using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public sealed class AdminFaxEuiState : EuiStateBase
{
	public List<AdminFaxEntry> Entries { get; }

	public AdminFaxEuiState(List<AdminFaxEntry> entries)
	{
		Entries = entries;
	}
}
