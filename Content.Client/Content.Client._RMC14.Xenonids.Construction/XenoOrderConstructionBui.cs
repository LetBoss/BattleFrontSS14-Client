using System;
using System.Collections.Generic;
using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Construction;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoOrderConstructionBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly SpriteSystem _sprite;

	private readonly Dictionary<EntProtoId, XenoChoiceControl> _buttons = new Dictionary<EntProtoId, XenoChoiceControl>();

	[ViewVariables]
	private XenoChooseStructureWindow? _window;

	public XenoOrderConstructionBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
	}

	protected override void Open()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoChooseStructureWindow>((BoundUserInterface)(object)this);
		((DefaultWindow)_window).Title = Loc.GetString("cm-xeno-order-construction");
		_buttons.Clear();
		XenoConstructionComponent xenoConstructionComponent = default(XenoConstructionComponent);
		if (!base.EntMan.TryGetComponent<XenoConstructionComponent>(((BoundUserInterface)this).Owner, ref xenoConstructionComponent))
		{
			return;
		}
		EntityPrototype val = default(EntityPrototype);
		foreach (EntProtoId structureId in xenoConstructionComponent.CanOrderConstruction)
		{
			if (_prototype.TryIndex(structureId, ref val))
			{
				XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
				((BaseButton)xenoChoiceControl.Button).ToggleMode = false;
				xenoChoiceControl.Set(val.Name, _sprite.Frame0(val));
				((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoOrderConstructionBuiMsg(structureId));
					((BoundUserInterface)this).Close();
				};
				((Control)_window.StructureContainer).AddChild((Control)(object)xenoChoiceControl);
				_buttons.Add(structureId, xenoChoiceControl);
			}
		}
	}
}
