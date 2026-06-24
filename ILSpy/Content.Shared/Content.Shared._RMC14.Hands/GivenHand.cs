using Content.Shared.Hands.Components;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Hands;

[DataRecord]
public record struct GivenHand(string Name, HandLocation Location);
