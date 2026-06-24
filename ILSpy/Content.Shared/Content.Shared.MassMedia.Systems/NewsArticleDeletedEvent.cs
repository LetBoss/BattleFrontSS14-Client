using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.MassMedia.Systems;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public record struct NewsArticleDeletedEvent;
