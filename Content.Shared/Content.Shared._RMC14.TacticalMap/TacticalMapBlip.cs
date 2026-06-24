using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.TacticalMap;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct TacticalMapBlip(Vector2i Indices, Rsi Image, Color Color, TacticalMapBlipStatus Status, Rsi? Background, bool HiveLeader);
