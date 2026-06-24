using Content.Shared.Radio;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Chat;

[ByRefEvent]
public record struct ChatGetPrefixEvent(RadioChannelPrototype? Channel);
