using System;
using System.Collections.Generic;
using Content.Shared.Humanoid.Markings;
using Content.Shared.MagicMirror;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.MagicMirror;

public sealed class MagicMirrorBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private MagicMirrorWindow? _window;

	public MagicMirrorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<MagicMirrorWindow>((BoundUserInterface)(object)this);
		MagicMirrorWindow? window = _window;
		window.OnHairSelected = (Action<(int, string)>)Delegate.Combine(window.OnHairSelected, (Action<(int, string)>)delegate((int slot, string id) tuple)
		{
			SelectHair(MagicMirrorCategory.Hair, tuple.id, tuple.slot);
		});
		MagicMirrorWindow? window2 = _window;
		window2.OnHairColorChanged = (Action<(int, Marking)>)Delegate.Combine(window2.OnHairColorChanged, (Action<(int, Marking)>)delegate((int slot, Marking marking) args)
		{
			ChangeColor(MagicMirrorCategory.Hair, args.marking, args.slot);
		});
		MagicMirrorWindow? window3 = _window;
		window3.OnHairSlotAdded = (Action)Delegate.Combine(window3.OnHairSlotAdded, (Action)delegate
		{
			AddSlot(MagicMirrorCategory.Hair);
		});
		MagicMirrorWindow? window4 = _window;
		window4.OnHairSlotRemoved = (Action<int>)Delegate.Combine(window4.OnHairSlotRemoved, (Action<int>)delegate(int args)
		{
			RemoveSlot(MagicMirrorCategory.Hair, args);
		});
		MagicMirrorWindow? window5 = _window;
		window5.OnFacialHairSelected = (Action<(int, string)>)Delegate.Combine(window5.OnFacialHairSelected, (Action<(int, string)>)delegate((int slot, string id) tuple)
		{
			SelectHair(MagicMirrorCategory.FacialHair, tuple.id, tuple.slot);
		});
		MagicMirrorWindow? window6 = _window;
		window6.OnFacialHairColorChanged = (Action<(int, Marking)>)Delegate.Combine(window6.OnFacialHairColorChanged, (Action<(int, Marking)>)delegate((int slot, Marking marking) args)
		{
			ChangeColor(MagicMirrorCategory.FacialHair, args.marking, args.slot);
		});
		MagicMirrorWindow? window7 = _window;
		window7.OnFacialHairSlotAdded = (Action)Delegate.Combine(window7.OnFacialHairSlotAdded, (Action)delegate
		{
			AddSlot(MagicMirrorCategory.FacialHair);
		});
		MagicMirrorWindow? window8 = _window;
		window8.OnFacialHairSlotRemoved = (Action<int>)Delegate.Combine(window8.OnFacialHairSlotRemoved, (Action<int>)delegate(int args)
		{
			RemoveSlot(MagicMirrorCategory.FacialHair, args);
		});
	}

	private void SelectHair(MagicMirrorCategory category, string marking, int slot)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MagicMirrorSelectMessage(category, marking, slot));
	}

	private void ChangeColor(MagicMirrorCategory category, Marking marking, int slot)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MagicMirrorChangeColorMessage(category, new List<Color>(marking.MarkingColors), slot));
	}

	private void RemoveSlot(MagicMirrorCategory category, int slot)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MagicMirrorRemoveSlotMessage(category, slot));
	}

	private void AddSlot(MagicMirrorCategory category)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MagicMirrorAddSlotMessage(category));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is MagicMirrorUiState state2 && _window != null)
		{
			_window.UpdateState(state2);
		}
	}
}
