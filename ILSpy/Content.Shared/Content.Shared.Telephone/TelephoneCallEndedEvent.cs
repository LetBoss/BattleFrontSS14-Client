using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Telephone;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public record struct TelephoneCallEndedEvent();
