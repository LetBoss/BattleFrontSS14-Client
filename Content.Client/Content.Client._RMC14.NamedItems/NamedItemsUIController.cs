using Content.Client._RMC14.LinkAccount;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.NamedItems;

public sealed class NamedItemsUIController : UIController
{
	[Dependency]
	private LinkAccountManager _linkAccount;

	public bool Available => _linkAccount.Tier?.NamedItems ?? false;
}
