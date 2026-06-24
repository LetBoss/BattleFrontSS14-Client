using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Kitchen.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Kitchen.UI;

public sealed class MicrowaveBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
	[ViewVariables]
	private MicrowaveMenu? _menu;

	[ViewVariables]
	private readonly Dictionary<int, EntityUid> _solids = new Dictionary<int, EntityUid>();

	[ViewVariables]
	private readonly Dictionary<int, ReagentQuantity> _reagents = new Dictionary<int, ReagentQuantity>();

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<MicrowaveMenu>((BoundUserInterface)(object)this);
		((BaseButton)_menu.StartButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MicrowaveStartCookMessage());
		};
		((BaseButton)_menu.EjectButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MicrowaveEjectMessage());
		};
		_menu.IngredientsList.OnItemSelected += delegate(ItemListSelectedEventArgs args)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MicrowaveEjectSolidIndexedMessage(base.EntMan.GetNetEntity(_solids[args.ItemIndex], (MetaDataComponent)null)));
		};
		_menu.OnCookTimeSelected += delegate(ButtonEventArgs args, int buttonIndex)
		{
			uint buttonIndex2 = 0u;
			if (args.Button is MicrowaveMenu.MicrowaveCookTimeButton)
			{
				MicrowaveMenu.MicrowaveCookTimeButton microwaveCookTimeButton = (MicrowaveMenu.MicrowaveCookTimeButton)(object)args.Button;
				buttonIndex2 = ((microwaveCookTimeButton.CookTime != 0) ? microwaveCookTimeButton.CookTime : 0u);
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MicrowaveSelectCookTimeMessage((int)buttonIndex2 / 5, microwaveCookTimeButton.CookTime));
				_menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new(string, object)[1] { ("time", buttonIndex2) });
			}
			else
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MicrowaveSelectCookTimeMessage((int)buttonIndex2, 0u));
				_menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new(string, object)[1] { ("time", Loc.GetString("microwave-menu-instant-button")) });
			}
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		((BoundUserInterface)this).UpdateState(state);
		if (state is MicrowaveUpdateUserInterfaceState microwaveUpdateUserInterfaceState && _menu != null)
		{
			_menu.IsBusy = microwaveUpdateUserInterfaceState.IsMicrowaveBusy;
			_menu.CurrentCooktimeEnd = microwaveUpdateUserInterfaceState.CurrentCookTimeEnd;
			_menu.ToggleBusyDisableOverlayPanel(microwaveUpdateUserInterfaceState.IsMicrowaveBusy || microwaveUpdateUserInterfaceState.ContainedSolids.Length == 0);
			RefreshContentsDisplay(base.EntMan.GetEntityArray(microwaveUpdateUserInterfaceState.ContainedSolids));
			string item = ((microwaveUpdateUserInterfaceState.ActiveButtonIndex == 0) ? Loc.GetString("microwave-menu-instant-button") : microwaveUpdateUserInterfaceState.CurrentCookTime.ToString());
			_menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new(string, object)[1] { ("time", item) });
			((BaseButton)_menu.StartButton).Disabled = microwaveUpdateUserInterfaceState.IsMicrowaveBusy || microwaveUpdateUserInterfaceState.ContainedSolids.Length == 0;
			((BaseButton)_menu.EjectButton).Disabled = microwaveUpdateUserInterfaceState.IsMicrowaveBusy || microwaveUpdateUserInterfaceState.ContainedSolids.Length == 0;
			if (microwaveUpdateUserInterfaceState.ActiveButtonIndex == 0)
			{
				((BaseButton)_menu.InstantCookButton).Pressed = true;
			}
			else
			{
				((BaseButton)(Button)((Control)_menu.CookTimeButtonVbox).GetChild(microwaveUpdateUserInterfaceState.ActiveButtonIndex - 1)).Pressed = true;
			}
			if (microwaveUpdateUserInterfaceState.IsMicrowaveBusy && microwaveUpdateUserInterfaceState.ContainedSolids.Length != 0)
			{
				_menu.IngredientsPanel.PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#947300", (Color?)null)
				};
			}
			else
			{
				_menu.IngredientsPanel.PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1B1B1E", (Color?)null)
				};
			}
		}
	}

	private void RefreshContentsDisplay(EntityUid[] containedSolids)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		_reagents.Clear();
		if (_menu == null)
		{
			return;
		}
		_solids.Clear();
		_menu.IngredientsList.Clear();
		IconComponent val2 = default(IconComponent);
		SpriteComponent val4 = default(SpriteComponent);
		foreach (EntityUid val in containedSolids)
		{
			if (base.EntMan.Deleted(val))
			{
				break;
			}
			Texture val3;
			if (base.EntMan.TryGetComponent<IconComponent>(val, ref val2))
			{
				val3 = base.EntMan.System<SpriteSystem>().GetIcon(val2);
			}
			else
			{
				if (!base.EntMan.TryGetComponent<SpriteComponent>(val, ref val4))
				{
					continue;
				}
				IRsiStateLike icon = val4.Icon;
				val3 = ((icon != null) ? ((IDirectionalTextureProvider)icon).Default : null);
			}
			Item val5 = _menu.IngredientsList.AddItem(base.EntMan.GetComponent<MetaDataComponent>(val).EntityName, val3, true, (object)null, 1f);
			int key = _menu.IngredientsList.IndexOf(val5);
			_solids.Add(key, val);
		}
	}
}
