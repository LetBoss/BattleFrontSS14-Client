using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class VerbsResponseEvent : EntityEventArgs
{
	public readonly List<Verb>? Verbs;

	public readonly NetEntity Entity;

	public VerbsResponseEvent(NetEntity entity, SortedSet<Verb>? verbs)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		if (verbs != null)
		{
			Verbs = new List<Verb>(verbs);
		}
	}
}
