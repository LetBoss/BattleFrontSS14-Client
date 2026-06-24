using Robust.Shared.GameObjects;

namespace Robust.Shared.Analyzers;

[ByRefEvent]
[ComponentEvent]
public record struct AfterAutoHandleStateEvent(IComponentState State);
