using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenuTextureButton : RadialMenuTextureButtonBase
{
	public Control? TargetLayer { get; set; }

	public string? TargetLayerControlName { get; set; }

	public RadialMenuTextureButton()
	{
		((BaseButton)this).EnableAllKeybinds = true;
		((BaseButton)this).OnButtonUp += OnClicked;
	}

	private void OnClicked(ButtonEventArgs args)
	{
		if (TargetLayer == null && TargetLayerControlName == null)
		{
			return;
		}
		RadialMenu radialMenu = FindParentMultiLayerContainer((Control)(object)this);
		if (radialMenu != null)
		{
			if (TargetLayer != null)
			{
				radialMenu.TryToMoveToNewLayer(TargetLayer);
			}
			else
			{
				radialMenu.TryToMoveToNewLayer(TargetLayerControlName);
			}
		}
	}

	private RadialMenu? FindParentMultiLayerContainer(Control control)
	{
		foreach (Control selfAndLogicalAncestor in LogicalExtensions.GetSelfAndLogicalAncestors(control))
		{
			if (selfAndLogicalAncestor is RadialMenu result)
			{
				return result;
			}
		}
		return null;
	}
}
