using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public record struct OverwatchSquad(NetEntity Id, string Name, Color Color, NetEntity? Leader, bool CanSupplyDrop, Rsi LeaderIcon);
