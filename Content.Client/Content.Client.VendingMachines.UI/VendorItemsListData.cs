using Content.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client.VendingMachines.UI;

public record VendorItemsListData(EntProtoId ItemProtoID, int ItemIndex) : ListData
{
	public string ItemText = string.Empty;
}
