using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

[Serializable]
[NetSerializable]
public sealed class GhostnadoRequestEvent : EntityEventArgs
{
}
