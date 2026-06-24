using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Info;

public sealed class ServerInfo : BoxContainer
{
	private readonly RichTextLabel _richTextLabel;

	public ServerInfo()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		_richTextLabel = new RichTextLabel
		{
			VerticalExpand = true
		};
		((Control)this).AddChild((Control)(object)_richTextLabel);
	}

	public void SetInfoBlob(string markup)
	{
		_richTextLabel.SetMessage(FormattedMessage.FromMarkupOrThrow(markup), (Color?)null);
	}
}
