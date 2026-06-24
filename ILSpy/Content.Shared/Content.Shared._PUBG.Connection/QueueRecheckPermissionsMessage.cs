using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Connection;

[Serializable]
[NetSerializable]
public sealed class QueueRecheckPermissionsMessage : EntityEventArgs
{
}
