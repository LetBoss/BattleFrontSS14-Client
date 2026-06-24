using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader;

[ByRefEvent]
public record struct CartridgeLoaderNotificationSentEvent(string Header, string Message);
