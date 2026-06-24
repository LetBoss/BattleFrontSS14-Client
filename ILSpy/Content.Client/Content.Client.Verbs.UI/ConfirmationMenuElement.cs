using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Verbs.UI;

public sealed class ConfirmationMenuElement : ContextMenuElement
{
	public const string StyleClassConfirmationContextMenuButton = "confirmationContextMenuButton";

	public readonly Verb Verb;

	public override string Text
	{
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			FormattedMessage val = new FormattedMessage();
			val.PushColor(Color.White);
			val.AddMarkupPermissive(value.Trim());
			base.Label.SetMessage(val, (Color?)null);
		}
	}

	public ConfirmationMenuElement(Verb verb, string? text)
		: base(text)
	{
		Verb = verb;
		((Control)base.Icon).Visible = false;
		((Control)this).SetOnlyStyleClass("confirmationContextMenuButton");
	}
}
