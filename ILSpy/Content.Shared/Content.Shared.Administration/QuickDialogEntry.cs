using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class QuickDialogEntry
{
	public string FieldId;

	public QuickDialogEntryType Type;

	public string Prompt;

	public string? Placeholder;

	public QuickDialogEntry(string fieldId, QuickDialogEntryType type, string prompt, string? placeholder = null)
	{
		FieldId = fieldId;
		Type = type;
		Prompt = prompt;
		Placeholder = placeholder;
	}
}
