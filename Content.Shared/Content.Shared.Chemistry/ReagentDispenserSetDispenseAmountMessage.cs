using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentDispenserSetDispenseAmountMessage : BoundUserInterfaceMessage
{
	public readonly ReagentDispenserDispenseAmount ReagentDispenserDispenseAmount;

	public ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount amount)
	{
		ReagentDispenserDispenseAmount = amount;
	}

	public ReagentDispenserSetDispenseAmountMessage(string s)
	{
		switch (s)
		{
		case "1":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U1;
			break;
		case "5":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U5;
			break;
		case "10":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U10;
			break;
		case "15":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U15;
			break;
		case "20":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U20;
			break;
		case "25":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U25;
			break;
		case "30":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U30;
			break;
		case "50":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U50;
			break;
		case "100":
			ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U100;
			break;
		default:
			throw new Exception("Cannot convert the string `" + s + "` into a valid ReagentDispenser DispenseAmount");
		}
	}
}
