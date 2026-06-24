using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Utility.Events;

[Serializable]
[NetSerializable]
public sealed class PrepareMedevacEvent : EntityEventArgs
{
	public NetEntity MedevacEntity;

	public bool ReadyForMedevac;

	public PrepareMedevacEvent(NetEntity medevacEntity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MedevacEntity = medevacEntity;
	}
}
