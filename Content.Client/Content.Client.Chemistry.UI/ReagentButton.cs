using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reagent;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Chemistry.UI;

public sealed class ReagentButton : Button
{
	public bool IsBuffer = true;

	public ChemMasterReagentAmount Amount { get; set; }

	public ReagentId Id { get; set; }

	public ReagentButton(string text, ChemMasterReagentAmount amount, ReagentId id, bool isBuffer, string styleClass)
	{
		((Control)this).AddStyleClass(styleClass);
		((Button)this).Text = text;
		Amount = amount;
		Id = id;
		IsBuffer = isBuffer;
	}
}
