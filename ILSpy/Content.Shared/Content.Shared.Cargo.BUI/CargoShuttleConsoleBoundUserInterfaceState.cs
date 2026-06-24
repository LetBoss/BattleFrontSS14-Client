using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.BUI;

[Serializable]
[NetSerializable]
public sealed class CargoShuttleConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
	public string AccountName;

	public string ShuttleName;

	public List<CargoOrderData> Orders;

	public CargoShuttleConsoleBoundUserInterfaceState(string accountName, string shuttleName, List<CargoOrderData> orders)
	{
		AccountName = accountName;
		ShuttleName = shuttleName;
		Orders = orders;
	}
}
