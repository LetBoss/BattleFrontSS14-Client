using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot;

[Serializable]
[NetSerializable]
public sealed class RequestAimedShotEvent : EntityEventArgs
{
	public NetEntity Gun;

	public NetEntity User;

	public NetEntity Target;
}
