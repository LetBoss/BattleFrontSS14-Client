using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class RequestServerVerbsEvent : EntityEventArgs
{
	public readonly NetEntity EntityUid;

	public readonly List<string> VerbTypes = new List<string>();

	public readonly NetEntity? SlotOwner;

	public readonly bool AdminRequest;

	public RequestServerVerbsEvent(NetEntity entityUid, IEnumerable<Type> verbTypes, NetEntity? slotOwner = null, bool adminRequest = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		EntityUid = entityUid;
		SlotOwner = slotOwner;
		AdminRequest = adminRequest;
		foreach (Type type in verbTypes)
		{
			VerbTypes.Add(type.Name);
		}
	}
}
