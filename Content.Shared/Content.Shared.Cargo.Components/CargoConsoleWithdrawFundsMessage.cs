using System;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public sealed class CargoConsoleWithdrawFundsMessage : BoundUserInterfaceMessage
{
	public ProtoId<CargoAccountPrototype>? Account;

	public int Amount;

	public CargoConsoleWithdrawFundsMessage(ProtoId<CargoAccountPrototype>? account, int amount)
	{
		Account = account;
		Amount = amount;
	}
}
