using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Fruit.Events;

[ByRefEvent]
public readonly record struct XenoFruitPlanterVisualsChangedEvent(EntProtoId<XenoFruitComponent> Choice);
