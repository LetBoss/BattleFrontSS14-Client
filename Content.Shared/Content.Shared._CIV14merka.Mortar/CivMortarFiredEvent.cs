using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Mortar;

[Serializable]
[NetSerializable]
public sealed class CivMortarFiredEvent(NetEntity mortar) : EntityEventArgs
{
	public NetEntity Mortar = mortar;
}
