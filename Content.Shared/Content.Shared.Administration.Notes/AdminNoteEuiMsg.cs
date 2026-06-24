using System;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

public static class AdminNoteEuiMsg
{
	[Serializable]
	[NetSerializable]
	public sealed class CreateNoteRequest : EuiMessageBase
	{
		public NoteType NoteType { get; set; }

		public string Message { get; set; }

		public NoteSeverity? NoteSeverity { get; set; }

		public bool Secret { get; set; }

		public DateTime? ExpiryTime { get; set; }

		public CreateNoteRequest(NoteType type, string message, NoteSeverity? severity, bool secret, DateTime? expiryTime)
		{
			NoteType = type;
			Message = message;
			NoteSeverity = severity;
			Secret = secret;
			ExpiryTime = expiryTime;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class DeleteNoteRequest : EuiMessageBase
	{
		public int Id { get; set; }

		public NoteType Type { get; set; }

		public DeleteNoteRequest(int id, NoteType type)
		{
			Id = id;
			Type = type;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class EditNoteRequest : EuiMessageBase
	{
		public int Id { get; set; }

		public NoteType Type { get; set; }

		public string Message { get; set; }

		public NoteSeverity? NoteSeverity { get; set; }

		public bool Secret { get; set; }

		public DateTime? ExpiryTime { get; set; }

		public EditNoteRequest(int id, NoteType type, string message, NoteSeverity? severity, bool secret, DateTime? expiryTime)
		{
			Id = id;
			Type = type;
			Message = message;
			NoteSeverity = severity;
			Secret = secret;
			ExpiryTime = expiryTime;
		}
	}
}
