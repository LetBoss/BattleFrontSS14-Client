using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Store;

[Serializable]
[NetSerializable]
public sealed class StoreRequestWithdrawMessage : BoundUserInterfaceMessage
{
	public string Currency;

	public int Amount;

	public StoreRequestWithdrawMessage(string currency, int amount)
	{
		Currency = currency;
		Amount = amount;
	}
}
