using System;
using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Item;

[Serializable]
[DataRecord]
[NetSerializable]
public record MultiHandedItem(int Hands, EntityWhitelist Whitelist);
