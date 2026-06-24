using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship;

[Serializable]
[NetSerializable]
public readonly record struct Destination(NetEntity Id, string Name, bool Occupied, bool Primary);
