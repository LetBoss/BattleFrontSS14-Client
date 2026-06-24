using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Message;

public static class RichTextLabelExt
{
	public static RichTextLabel SetMarkup(this RichTextLabel label, string markup)
	{
		label.SetMessage(FormattedMessage.FromMarkupOrThrow(markup), (Color?)null);
		return label;
	}

	public static RichTextLabel SetMarkupPermissive(this RichTextLabel label, string markup)
	{
		label.SetMessage(FormattedMessage.FromMarkupPermissive(markup), (Color?)null);
		return label;
	}
}
