using System.Collections.Generic;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo.Components;

[ByRefEvent]
public readonly record struct BankBalanceUpdatedEvent(EntityUid Station, Dictionary<ProtoId<CargoAccountPrototype>, int> Balance);
