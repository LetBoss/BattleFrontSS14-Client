using System;

namespace Robust.Shared.Audio.AudioLoading;

internal record AudioMetadata(TimeSpan Length, int ChannelCount, string? Title = null, string? Artist = null);
