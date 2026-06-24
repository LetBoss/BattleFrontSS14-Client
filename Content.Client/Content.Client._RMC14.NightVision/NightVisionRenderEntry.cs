using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client._RMC14.NightVision;

public record struct NightVisionRenderEntry((EntityUid, SpriteComponent, TransformComponent) Ent, MapId? Map, bool NightVisionSeeThroughContainers, int Priority, float? Transparency);
