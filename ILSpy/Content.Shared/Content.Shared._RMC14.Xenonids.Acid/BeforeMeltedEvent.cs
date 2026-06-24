using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Acid;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public record struct BeforeMeltedEvent();
