using System;
using System.Runtime.InteropServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Delivery;

[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 1)]
[NetSerializable]
public readonly record struct DeliveryPriorityExpiredEvent;
