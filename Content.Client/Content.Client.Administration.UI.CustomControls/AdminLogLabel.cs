using System;
using Content.Shared.Administration.Logs;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogLabel : RichTextLabel
{
	public SharedAdminLog Log { get; }

	public HSeparator Separator { get; }

	public AdminLogLabel(ref SharedAdminLog log, HSeparator separator)
	{
		Log = log;
		Separator = separator;
		((RichTextLabel)this).SetMessage($"{log.Date:HH:mm:ss}: {log.Message}", (Color?)null);
		((Control)this).OnVisibilityChanged += VisibilityChanged;
	}

	private void VisibilityChanged(Control control)
	{
		((Control)Separator).Visible = ((Control)this).Visible;
	}

	[Obsolete]
	protected override void Dispose(bool disposing)
	{
		((Control)this).Dispose(disposing);
		((Control)this).OnVisibilityChanged -= VisibilityChanged;
	}
}
