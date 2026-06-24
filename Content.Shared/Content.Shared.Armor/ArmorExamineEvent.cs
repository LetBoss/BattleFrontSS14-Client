using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared.Armor;

[ByRefEvent]
public record struct ArmorExamineEvent(FormattedMessage Msg);
