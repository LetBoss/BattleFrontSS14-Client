using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Lobby;

[Serializable]
[NetSerializable]
public sealed class LobbyPermissionsRequestMessage : EntityEventArgs
{
}
