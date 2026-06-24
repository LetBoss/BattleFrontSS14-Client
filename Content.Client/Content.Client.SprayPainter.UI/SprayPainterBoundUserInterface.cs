using System;
using Content.Shared.SprayPainter;
using Content.Shared.SprayPainter.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.SprayPainter.UI;

public sealed class SprayPainterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private SprayPainterWindow? _window;

	public SprayPainterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<SprayPainterWindow>((BoundUserInterface)(object)this);
		_window.OnSpritePicked = OnSpritePicked;
		_window.OnColorPicked = OnColorPicked;
		SprayPainterComponent sprayPainterComponent = default(SprayPainterComponent);
		if (base.EntMan.TryGetComponent<SprayPainterComponent>(((BoundUserInterface)this).Owner, ref sprayPainterComponent))
		{
			_window.Populate(base.EntMan.System<SprayPainterSystem>().Entries, sprayPainterComponent.Index, sprayPainterComponent.PickedColor, sprayPainterComponent.ColorPalette);
		}
	}

	private void OnSpritePicked(ItemListSelectedEventArgs args)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SprayPainterSpritePickedMessage(args.ItemIndex));
	}

	private void OnColorPicked(ItemListSelectedEventArgs args)
	{
		string key = _window?.IndexToColorKey(args.ItemIndex);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SprayPainterColorPickedMessage(key));
	}
}
