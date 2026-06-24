using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface;

public static class ButtonHelpers
{
	public unsafe static void SetButtonDisabledRecursive(Control parent, bool val)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = parent.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				Button val2 = (Button)(object)((current is Button) ? current : null);
				if (val2 != null)
				{
					((BaseButton)val2).Disabled = val;
				}
				else if (current.ChildCount > 0)
				{
					SetButtonDisabledRecursive(current, val);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
