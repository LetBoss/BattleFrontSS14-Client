using System;
using System.Collections.Generic;
using Content.Client.Clothing.Systems;
using Content.Shared.Clothing.Components;
using Content.Shared.Tag;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Clothing.UI;

public sealed class ChameleonBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _proto;

	private readonly ChameleonClothingSystem _chameleon;

	private readonly TagSystem _tag;

	[ViewVariables]
	private ChameleonMenu? _menu;

	public ChameleonBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_chameleon = base.EntMan.System<ChameleonClothingSystem>();
		_tag = base.EntMan.System<TagSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<ChameleonMenu>((BoundUserInterface)(object)this);
		_menu.OnIdSelected += OnIdSelected;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is ChameleonBoundUserInterfaceState chameleonBoundUserInterfaceState))
		{
			return;
		}
		IEnumerable<EntProtoId> validTargets = _chameleon.GetValidTargets(chameleonBoundUserInterfaceState.Slot);
		if (chameleonBoundUserInterfaceState.RequiredTag != null)
		{
			List<EntProtoId> list = new List<EntProtoId>();
			EntityPrototype val = default(EntityPrototype);
			TagComponent component = default(TagComponent);
			foreach (EntProtoId item in validTargets)
			{
				if (!string.IsNullOrEmpty(EntProtoId.op_Implicit(item)) && _proto.TryIndex(item, ref val) && val.TryGetComponent<TagComponent>(ref component, base.EntMan.ComponentFactory) && _tag.HasTag(component, ProtoId<TagPrototype>.op_Implicit(chameleonBoundUserInterfaceState.RequiredTag)))
				{
					list.Add(item);
				}
			}
			_menu?.UpdateState(list, chameleonBoundUserInterfaceState.SelectedId);
		}
		else
		{
			_menu?.UpdateState(validTargets, chameleonBoundUserInterfaceState.SelectedId);
		}
	}

	private void OnIdSelected(string selectedId)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ChameleonPrototypeSelectedMessage(selectedId));
	}
}
