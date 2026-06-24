using System;
using System.Collections.Generic;
using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Construction;

public sealed class XenoChooseStructureBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly SpriteSystem _sprite;

	private readonly SharedXenoConstructionSystem _xenoConstruction;

	private readonly Dictionary<EntProtoId, XenoChoiceControl> _buttons = new Dictionary<EntProtoId, XenoChoiceControl>();

	[ViewVariables]
	private XenoChooseStructureWindow? _window;

	public XenoChooseStructureBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
		_xenoConstruction = base.EntMan.System<SharedXenoConstructionSystem>();
	}

	protected override void Open()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoChooseStructureWindow>((BoundUserInterface)(object)this);
		_buttons.Clear();
		XenoConstructionComponent xenoConstructionComponent = default(XenoConstructionComponent);
		if (base.EntMan.TryGetComponent<XenoConstructionComponent>(((BoundUserInterface)this).Owner, ref xenoConstructionComponent))
		{
			bool flag = base.EntMan.HasComponent<QueenBuildingBoostComponent>(((BoundUserInterface)this).Owner);
			EntityPrototype val = default(EntityPrototype);
			EntityPrototype val3 = default(EntityPrototype);
			foreach (EntProtoId structureId in xenoConstructionComponent.CanBuild)
			{
				if (!_prototype.TryIndex(structureId, ref val))
				{
					continue;
				}
				XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
				((BaseButton)xenoChoiceControl.Button).ToggleMode = true;
				EntProtoId val2 = structureId;
				string text = val.Name;
				if (flag)
				{
					EntProtoId queenVariant = GetQueenVariant(structureId);
					if (_prototype.TryIndex(queenVariant, ref val3) && queenVariant != structureId)
					{
						val2 = queenVariant;
						text = val3.Name;
					}
					text += " (0 plasma)";
				}
				else
				{
					FixedPoint2? structurePlasmaCost = _xenoConstruction.GetStructurePlasmaCost(structureId);
					if (structurePlasmaCost.HasValue)
					{
						FixedPoint2 valueOrDefault = structurePlasmaCost.GetValueOrDefault();
						text += $" ({valueOrDefault} plasma)";
					}
				}
				xenoChoiceControl.Set(text, _sprite.Frame0(_prototype.Index(val2)));
				((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoChooseStructureBuiMsg(structureId));
					UpdateButtonStates(structureId);
				};
				((Control)_window.StructureContainer).AddChild((Control)(object)xenoChoiceControl);
				_buttons.Add(structureId, xenoChoiceControl);
			}
		}
		Refresh();
	}

	private EntProtoId GetQueenVariant(EntProtoId originalId)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		return (EntProtoId)(((EntProtoId)(ref originalId)).Id switch
		{
			"WallXenoResin" => EntProtoId.op_Implicit("WallXenoResinQueen"), 
			"WallXenoMembrane" => EntProtoId.op_Implicit("WallXenoMembraneQueen"), 
			"DoorXenoResin" => EntProtoId.op_Implicit("DoorXenoResinQueen"), 
			_ => originalId, 
		});
	}

	private void UpdateButtonStates(EntProtoId selectedId)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (val2, xenoChoiceControl2) in _buttons)
		{
			((BaseButton)xenoChoiceControl2.Button).Pressed = val2 == selectedId;
		}
	}

	public void Refresh()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<EntProtoId, XenoChoiceControl> button in _buttons)
		{
			button.Deconstruct(out var _, out var value);
			((BaseButton)value.Button).Pressed = false;
		}
		EntProtoId? val = EntityManagerExt.GetComponentOrNull<XenoConstructionComponent>(base.EntMan, ((BoundUserInterface)this).Owner)?.BuildChoice;
		if (val.HasValue)
		{
			EntProtoId valueOrDefault = val.GetValueOrDefault();
			if (_buttons.TryGetValue(valueOrDefault, out XenoChoiceControl value2))
			{
				((BaseButton)value2.Button).Pressed = true;
			}
		}
	}
}
