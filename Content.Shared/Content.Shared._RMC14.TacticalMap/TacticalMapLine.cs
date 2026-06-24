using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.TacticalMap;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct TacticalMapLine(Vector2i Start, Vector2i End, Color Color, float Thickness = 2f);
