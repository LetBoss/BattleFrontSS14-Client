using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

public static class AdminMessageEuiMsg
{
	[Serializable]
	[NetSerializable]
	public sealed class Dismiss(bool permanent) : EuiMessageBase
	{
		public bool Permanent { get; } = permanent;
	}
}
