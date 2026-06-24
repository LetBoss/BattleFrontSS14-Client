using System;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Projectile.Parasite;

public sealed class ReserveParasitesBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ReserveParasitesWindow? _window;

	public ReserveParasitesBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ReserveParasitesWindow>((BoundUserInterface)(object)this);
		XenoParasiteThrowerComponent xenoParasiteThrowerComponent = default(XenoParasiteThrowerComponent);
		if (base.EntMan.TryGetComponent<XenoParasiteThrowerComponent>(((BoundUserInterface)this).Owner, ref xenoParasiteThrowerComponent))
		{
			_window.SetReserveShown(xenoParasiteThrowerComponent.ReservedParasites);
		}
		((BaseButton)_window.ApplyButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new XenoChangeParasiteReserveMessage(_window.ReserveBar.Value));
			((BaseWindow)_window).Close();
		};
	}

	public void ChangeReserve(int newReserve)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new XenoChangeParasiteReserveMessage(newReserve));
	}
}
