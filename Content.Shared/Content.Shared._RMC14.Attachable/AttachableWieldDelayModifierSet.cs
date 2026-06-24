using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct AttachableWieldDelayModifierSet(AttachableModifierConditions? Conditions, TimeSpan Delay);
