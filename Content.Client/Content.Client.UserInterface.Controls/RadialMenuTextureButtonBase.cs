using Content.Shared.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Input;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenuTextureButtonBase : TextureButton
{
	protected RadialMenuTextureButtonBase()
	{
		((BaseButton)this).EnableAllKeybinds = true;
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick || ((BoundKeyEventArgs)args).Function == ContentKeyFunctions.AltActivateItemInWorld)
		{
			((BaseButton)this).KeyBindUp(args);
		}
	}
}
