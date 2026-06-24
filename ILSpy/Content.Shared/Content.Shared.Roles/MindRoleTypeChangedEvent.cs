using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Roles;

[Serializable]
[NetSerializable]
public sealed class MindRoleTypeChangedEvent : EntityEventArgs
{
}
