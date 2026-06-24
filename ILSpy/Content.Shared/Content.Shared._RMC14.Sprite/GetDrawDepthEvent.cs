using Content.Shared.DrawDepth;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Sprite;

[ByRefEvent]
public record struct GetDrawDepthEvent(Content.Shared.DrawDepth.DrawDepth DrawDepth);
