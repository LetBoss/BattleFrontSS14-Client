using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Roles;

public sealed record RoleAddedEvent(EntityUid MindId, MindComponent Mind, bool RoleTypeUpdate, bool Silent = false) : RoleEvent(MindId, Mind, RoleTypeUpdate);
