using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Roles;

public sealed record RoleRemovedEvent(EntityUid MindId, MindComponent Mind, bool RoleTypeUpdate) : RoleEvent(MindId, Mind, RoleTypeUpdate);
