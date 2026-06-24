using System;
using System.Collections.Generic;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Examine;

public static class ExamineSystemMessages
{
	[Serializable]
	[NetSerializable]
	public sealed class RequestExamineInfoMessage : EntityEventArgs
	{
		public readonly NetEntity NetEntity;

		public readonly int Id;

		public readonly bool GetVerbs;

		public RequestExamineInfoMessage(NetEntity netEntity, int id, bool getVerbs = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			NetEntity = netEntity;
			Id = id;
			GetVerbs = getVerbs;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class ExamineInfoResponseMessage : EntityEventArgs
	{
		public readonly NetEntity EntityUid;

		public readonly int Id;

		public readonly FormattedMessage Message;

		public List<Verb>? Verbs;

		public readonly bool CenterAtCursor;

		public readonly bool OpenAtOldTooltip;

		public readonly bool KnowTarget;

		public ExamineInfoResponseMessage(NetEntity entityUid, int id, FormattedMessage message, List<Verb>? verbs = null, bool centerAtCursor = true, bool openAtOldTooltip = true, bool knowTarget = true)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			EntityUid = entityUid;
			Id = id;
			Message = message;
			Verbs = verbs;
			CenterAtCursor = centerAtCursor;
			OpenAtOldTooltip = openAtOldTooltip;
			KnowTarget = knowTarget;
		}
	}
}
