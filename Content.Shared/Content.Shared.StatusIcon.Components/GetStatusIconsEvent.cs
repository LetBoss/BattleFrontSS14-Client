using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.StatusIcon.Components;

[ByRefEvent]
public record struct GetStatusIconsEvent(List<StatusIconData> StatusIcons);
