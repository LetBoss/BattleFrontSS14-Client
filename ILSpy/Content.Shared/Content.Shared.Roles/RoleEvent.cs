using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Roles;

public abstract record RoleEvent(EntityUid MindId, MindComponent Mind, bool RoleTypeUpdate);
