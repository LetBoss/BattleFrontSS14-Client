using Robust.Shared.Analyzers;

namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IEventBus : IDirectedEventBus, IBroadcastEventBus
{
}
