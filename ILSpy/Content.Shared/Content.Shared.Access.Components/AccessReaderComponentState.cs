using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Components;

[Serializable]
[NetSerializable]
public sealed class AccessReaderComponentState : ComponentState
{
	public bool Enabled;

	public HashSet<ProtoId<AccessLevelPrototype>> DenyTags;

	public List<HashSet<ProtoId<AccessLevelPrototype>>> AccessLists;

	public List<(NetEntity, uint)> AccessKeys;

	public Queue<AccessRecord> AccessLog;

	public int AccessLogLimit;

	public AccessReaderComponentState(bool enabled, HashSet<ProtoId<AccessLevelPrototype>> denyTags, List<HashSet<ProtoId<AccessLevelPrototype>>> accessLists, List<(NetEntity, uint)> accessKeys, Queue<AccessRecord> accessLog, int accessLogLimit)
	{
		Enabled = enabled;
		DenyTags = denyTags;
		AccessLists = accessLists;
		AccessKeys = accessKeys;
		AccessLog = accessLog;
		AccessLogLimit = accessLogLimit;
	}
}
