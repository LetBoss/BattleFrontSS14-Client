using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

[Serializable]
[NetSerializable]
public sealed class AdminMessageEuiState(TimeSpan time, AdminMessageEuiState.Message[] messages) : EuiStateBase
{
	[Serializable]
	public sealed class Message(string text, string adminName, DateTime addedOn)
	{
		public string Text = text;

		public string AdminName = adminName;

		public DateTime AddedOn = addedOn;
	}

	public TimeSpan Time { get; } = time;

	public Message[] Messages { get; } = messages;
}
