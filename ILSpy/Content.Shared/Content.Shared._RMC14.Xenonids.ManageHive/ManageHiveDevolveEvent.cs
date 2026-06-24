using System;
using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.ManageHive;

[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
[NetSerializable]
public readonly record struct ManageHiveDevolveEvent;
