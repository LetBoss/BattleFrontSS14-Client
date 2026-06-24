using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

public static class AdminAnnounceEuiMsg
{
	[Serializable]
	[NetSerializable]
	public sealed class DoAnnounce : EuiMessageBase
	{
		public bool CloseAfter;

		public string Announcer;

		public string Announcement;

		public AdminAnnounceType AnnounceType;
	}
}
