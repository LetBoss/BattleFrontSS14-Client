using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Stun;

[Serializable]
[NetSerializable]
public sealed class DazedComponentShutdownEvent : EntityEventArgs
{
}
