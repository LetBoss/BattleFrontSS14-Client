using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class ExecuteVerbEvent : EntityEventArgs
{
	public readonly NetEntity Target;

	public readonly Verb RequestedVerb;

	public ExecuteVerbEvent(NetEntity target, Verb requestedVerb)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
		RequestedVerb = requestedVerb;
	}
}
