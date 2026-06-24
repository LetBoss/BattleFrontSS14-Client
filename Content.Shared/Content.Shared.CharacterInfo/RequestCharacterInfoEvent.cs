using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo;

[Serializable]
[NetSerializable]
public sealed class RequestCharacterInfoEvent : EntityEventArgs
{
	public readonly NetEntity NetEntity;

	public RequestCharacterInfoEvent(NetEntity netEntity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		NetEntity = netEntity;
	}
}
