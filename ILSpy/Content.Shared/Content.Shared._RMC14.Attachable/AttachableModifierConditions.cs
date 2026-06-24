using System;
using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct AttachableModifierConditions(bool UnwieldedOnly, bool WieldedOnly, bool ActiveOnly, bool InactiveOnly, EntityWhitelist? Whitelist, EntityWhitelist? Blacklist);
