using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Roles;

[ByRefEvent]
public readonly record struct MindGetAllRoleInfoEvent(List<RoleInfo> Roles);
