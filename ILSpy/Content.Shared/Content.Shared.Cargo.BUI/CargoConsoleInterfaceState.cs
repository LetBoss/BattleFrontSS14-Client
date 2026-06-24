using System;
using System.Collections.Generic;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.BUI;

[Serializable]
[NetSerializable]
public sealed class CargoConsoleInterfaceState : BoundUserInterfaceState
{
	public string Name;

	public int Count;

	public int Capacity;

	public NetEntity Station;

	public List<CargoOrderData> Orders;

	public List<ProtoId<CargoProductPrototype>> Products;

	public CargoConsoleInterfaceState(string name, int count, int capacity, NetEntity station, List<CargoOrderData> orders, List<ProtoId<CargoProductPrototype>> products)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Name = name;
		Count = count;
		Capacity = capacity;
		Station = station;
		Orders = orders;
		Products = products;
	}
}
