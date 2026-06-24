using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Storage.Containers;

[ByRefEvent]
public record struct RMCContainerDestructionEmptyEvent(bool Handled = false);
