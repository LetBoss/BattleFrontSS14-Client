using System;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

public static class AdminFaxEuiMsg
{
	[Serializable]
	[NetSerializable]
	public sealed class Close : EuiMessageBase
	{
	}

	[Serializable]
	[NetSerializable]
	public sealed class Follow : EuiMessageBase
	{
		public NetEntity TargetFax { get; }

		public Follow(NetEntity targetFax)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			TargetFax = targetFax;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class Send : EuiMessageBase
	{
		public NetEntity Target { get; }

		public string Title { get; }

		public string From { get; }

		public string Content { get; }

		public string StampState { get; }

		public Color StampColor { get; }

		public bool Locked { get; }

		public Send(NetEntity target, string title, string from, string content, string stamp, Color stampColor, bool locked)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			Target = target;
			Title = title;
			From = from;
			Content = content;
			StampState = stamp;
			StampColor = stampColor;
			Locked = locked;
		}
	}
}
