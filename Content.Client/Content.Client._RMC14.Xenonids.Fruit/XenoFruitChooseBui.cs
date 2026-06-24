using System;
using System.Collections.Generic;
using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Fruit;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Fruit;

public sealed class XenoFruitChooseBui : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly SpriteSystem _sprite;

	private readonly SharedXenoFruitSystem _xenoFruit;

	private readonly Dictionary<EntProtoId, XenoChoiceControl> _buttons = new Dictionary<EntProtoId, XenoChoiceControl>();

	[ViewVariables]
	private XenoFruitChooseWindow? _window;

	public XenoFruitChooseBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_sprite = base.EntMan.System<SpriteSystem>();
		_xenoFruit = base.EntMan.System<SharedXenoFruitSystem>();
	}

	protected override void Open()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoFruitChooseWindow>((BoundUserInterface)(object)this);
		_buttons.Clear();
		ButtonGroup val = new ButtonGroup(true);
		XenoFruitPlanterComponent xenoFruitPlanterComponent = default(XenoFruitPlanterComponent);
		if (base.EntMan.TryGetComponent<XenoFruitPlanterComponent>(((BoundUserInterface)this).Owner, ref xenoFruitPlanterComponent))
		{
			_window.FruitCountLabel.Text = Loc.GetString("rmc-xeno-fruit-ui-count", new(string, object)[2]
			{
				("count", xenoFruitPlanterComponent.PlantedFruit.Count),
				("max", xenoFruitPlanterComponent.MaxFruitAllowed)
			});
			EntityPrototype val2 = default(EntityPrototype);
			foreach (EntProtoId fruitId in xenoFruitPlanterComponent.CanPlant)
			{
				if (_prototype.TryIndex(fruitId, ref val2))
				{
					string fruitSprite = _xenoFruit.GetFruitSprite(val2);
					XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
					((BaseButton)xenoChoiceControl.Button).Group = val;
					string name = val2.Name;
					Rsi val3 = new Rsi(new ResPath("_RMC14/Structures/Xenos/xeno_fruit.rsi"), fruitSprite);
					xenoChoiceControl.Set(name, _sprite.Frame0((SpriteSpecifier)(object)val3));
					((BaseButton)xenoChoiceControl.Button).OnPressed += delegate
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new XenoFruitChooseBuiMsg(fruitId));
					};
					((Control)xenoChoiceControl.Button).ToolTip = val2.Description;
					((Control)xenoChoiceControl.Button).TooltipDelay = 0.1f;
					((Control)_window.FruitContainer).AddChild((Control)(object)xenoChoiceControl);
					_buttons.Add(fruitId, xenoChoiceControl);
				}
			}
		}
		Refresh();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is XenoFruitChooseBuiState state2)
		{
			UpdateState(state2);
		}
	}

	private void UpdateState(XenoFruitChooseBuiState state)
	{
		if (_window != null)
		{
			_window.FruitCountLabel.Text = Loc.GetString("rmc-xeno-fruit-ui-count", new(string, object)[2]
			{
				("count", state.Count),
				("max", state.Max)
			});
		}
	}

	public void Refresh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		XenoFruitPlanterComponent xenoFruitPlanterComponent = default(XenoFruitPlanterComponent);
		if (!base.EntMan.TryGetComponent<XenoFruitPlanterComponent>(((BoundUserInterface)this).Owner, ref xenoFruitPlanterComponent) || _window == null)
		{
			return;
		}
		EntProtoId? fruitChoice = xenoFruitPlanterComponent.FruitChoice;
		if (fruitChoice.HasValue)
		{
			EntProtoId valueOrDefault = fruitChoice.GetValueOrDefault();
			if (_buttons.TryGetValue(valueOrDefault, out XenoChoiceControl value))
			{
				((BaseButton)value.Button).Pressed = true;
			}
		}
	}
}
