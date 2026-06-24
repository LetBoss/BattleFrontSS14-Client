using System;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Strip;

public sealed class StrippingMenu : DefaultWindow
{
	public LayoutContainer InventoryContainer = new LayoutContainer();

	public LayoutContainer HandsContainer = new LayoutContainer();

	public BoxContainer SnareContainer = new BoxContainer();

	public bool Dirty = true;

	public event Action? OnDirty;

	public StrippingMenu()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(0f, 8f)
		};
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
		((Control)val).AddChild((Control)(object)SnareContainer);
		((Control)val).AddChild((Control)(object)HandsContainer);
		((Control)val).AddChild((Control)(object)InventoryContainer);
	}

	public void ClearButtons()
	{
		((Control)InventoryContainer).DisposeAllChildren();
		((Control)HandsContainer).DisposeAllChildren();
		((Control)SnareContainer).DisposeAllChildren();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		if (Dirty)
		{
			Dirty = false;
			this.OnDirty?.Invoke();
		}
	}
}
