using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.UserInterface;

public abstract class RMCPopOutBui<T> : BoundUserInterface where T : RMCPopOutWindow
{
	[ViewVariables]
	protected abstract T? Window { get; set; }

	protected RMCPopOutBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			Window?.DisposePopOut();
		}
	}
}
