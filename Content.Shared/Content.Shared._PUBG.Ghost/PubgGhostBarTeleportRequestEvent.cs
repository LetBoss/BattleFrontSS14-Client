using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Ghost;

[Serializable]
[NetSerializable]
public sealed class PubgGhostBarTeleportRequestEvent : EntityEventArgs
{
}
