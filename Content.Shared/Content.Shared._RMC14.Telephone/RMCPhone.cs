using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Telephone;

[Serializable]
[NetSerializable]
public readonly record struct RMCPhone(NetEntity Id, string Category, string Name);
