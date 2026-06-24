using System;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Humanoid;

public sealed class HumanoidMarkingModifierBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private HumanoidMarkingModifierWindow? _window;

	public HumanoidMarkingModifierBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindowCenteredLeft<HumanoidMarkingModifierWindow>((BoundUserInterface)(object)this);
		HumanoidMarkingModifierWindow? window = _window;
		window.OnMarkingAdded = (Action<MarkingSet>)Delegate.Combine(window.OnMarkingAdded, new Action<MarkingSet>(SendMarkingSet));
		HumanoidMarkingModifierWindow? window2 = _window;
		window2.OnMarkingRemoved = (Action<MarkingSet>)Delegate.Combine(window2.OnMarkingRemoved, new Action<MarkingSet>(SendMarkingSet));
		HumanoidMarkingModifierWindow? window3 = _window;
		window3.OnMarkingColorChange = (Action<MarkingSet>)Delegate.Combine(window3.OnMarkingColorChange, new Action<MarkingSet>(SendMarkingSetNoResend));
		HumanoidMarkingModifierWindow? window4 = _window;
		window4.OnMarkingRankChange = (Action<MarkingSet>)Delegate.Combine(window4.OnMarkingRankChange, new Action<MarkingSet>(SendMarkingSet));
		HumanoidMarkingModifierWindow? window5 = _window;
		window5.OnLayerInfoModified = (Action<HumanoidVisualLayers, CustomBaseLayerInfo?>)Delegate.Combine(window5.OnLayerInfoModified, new Action<HumanoidVisualLayers, CustomBaseLayerInfo?>(SendBaseLayer));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is HumanoidMarkingModifierState humanoidMarkingModifierState)
		{
			_window.SetState(humanoidMarkingModifierState.MarkingSet, humanoidMarkingModifierState.Species, humanoidMarkingModifierState.Sex, humanoidMarkingModifierState.SkinColor, humanoidMarkingModifierState.CustomBaseLayers);
		}
	}

	private void SendMarkingSet(MarkingSet set)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HumanoidMarkingModifierMarkingSetMessage(set, resendState: true));
	}

	private void SendMarkingSetNoResend(MarkingSet set)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HumanoidMarkingModifierMarkingSetMessage(set, resendState: false));
	}

	private void SendBaseLayer(HumanoidVisualLayers layer, CustomBaseLayerInfo? info)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new HumanoidMarkingModifierBaseLayersSetMessage(layer, info, resendState: true));
	}
}
