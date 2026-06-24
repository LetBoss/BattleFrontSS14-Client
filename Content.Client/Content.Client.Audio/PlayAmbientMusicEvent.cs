using Robust.Shared.GameObjects;

namespace Content.Client.Audio;

[ByRefEvent]
public record struct PlayAmbientMusicEvent(bool Cancelled = false);
