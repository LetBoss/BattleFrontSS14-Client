using System;
using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Evolution;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Evolution;

public sealed class XenoDevolveBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly SpriteSystem _sprite;

	[ViewVariables]
	private XenoDevolveWindow? _window;

	public XenoDevolveBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoDevolveWindow>((BoundUserInterface)(object)this);
		XenoDevolveComponent xenoDevolveComponent = default(XenoDevolveComponent);
		if (!base.EntMan.TryGetComponent<XenoDevolveComponent>(((BoundUserInterface)this).Owner, ref xenoDevolveComponent))
		{
			return;
		}
		EntProtoId[] devolvesTo = xenoDevolveComponent.DevolvesTo;
		EntityPrototype val = default(EntityPrototype);
		foreach (EntProtoId devolvesTo2 in devolvesTo)
		{
			if (!_prototype.TryIndex(devolvesTo2, ref val))
			{
				break;
			}
			XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
			xenoChoiceControl.Set(val.Name, _sprite.Frame0(val));
			((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoDevolveBuiMsg(devolvesTo2));
				((BoundUserInterface)this).Close();
			};
			((Control)_window.DevolutionsContainer).AddChild((Control)(object)xenoChoiceControl);
		}
	}
}
