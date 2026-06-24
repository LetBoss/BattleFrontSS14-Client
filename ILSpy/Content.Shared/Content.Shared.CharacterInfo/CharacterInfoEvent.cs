using System;
using System.Collections.Generic;
using Content.Shared.Objectives;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo;

[Serializable]
[NetSerializable]
public sealed class CharacterInfoEvent : EntityEventArgs
{
	public readonly NetEntity NetEntity;

	public readonly string JobTitle;

	public readonly Dictionary<string, List<ObjectiveInfo>> Objectives;

	public readonly string? Briefing;

	public CharacterInfoEvent(NetEntity netEntity, string jobTitle, Dictionary<string, List<ObjectiveInfo>> objectives, string? briefing)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		NetEntity = netEntity;
		JobTitle = jobTitle;
		Objectives = objectives;
		Briefing = briefing;
	}
}
