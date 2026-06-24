// Decompiled with JetBrains decompiler
// Type: Content.Client.Kitchen.UI.MicrowaveBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.Kitchen.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Kitchen.UI;

public sealed class MicrowaveBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private MicrowaveMenu? _menu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly Dictionary<int, EntityUid> _solids = new Dictionary<int, EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly Dictionary<int, ReagentQuantity> _reagents = new Dictionary<int, ReagentQuantity>();

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<MicrowaveMenu>((BoundUserInterface) this);
    ((BaseButton) this._menu.StartButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MicrowaveStartCookMessage()));
    ((BaseButton) this._menu.EjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new MicrowaveEjectMessage()));
    this._menu.IngredientsList.OnItemSelected += (Action<ItemList.ItemListSelectedEventArgs>) (args => this.SendPredictedMessage((BoundUserInterfaceMessage) new MicrowaveEjectSolidIndexedMessage(this.EntMan.GetNetEntity(this._solids[args.ItemIndex], (MetaDataComponent) null))));
    this._menu.OnCookTimeSelected += (Action<BaseButton.ButtonEventArgs, int>) ((args, buttonIndex) =>
    {
      uint buttonIndex1 = 0;
      if (args.Button is MicrowaveMenu.MicrowaveCookTimeButton)
      {
        MicrowaveMenu.MicrowaveCookTimeButton button = (MicrowaveMenu.MicrowaveCookTimeButton) args.Button;
        uint cookTime = button.CookTime == 0U ? 0U : button.CookTime;
        this.SendPredictedMessage((BoundUserInterfaceMessage) new MicrowaveSelectCookTimeMessage((int) cookTime / 5, button.CookTime));
        this._menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new (string, object)[1]
        {
          ("time", (object) cookTime)
        });
      }
      else
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) new MicrowaveSelectCookTimeMessage((int) buttonIndex1, 0U));
        this._menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new (string, object)[1]
        {
          ("time", (object) Loc.GetString("microwave-menu-instant-button"))
        });
      }
    });
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is MicrowaveUpdateUserInterfaceState userInterfaceState) || this._menu == null)
      return;
    this._menu.IsBusy = userInterfaceState.IsMicrowaveBusy;
    this._menu.CurrentCooktimeEnd = userInterfaceState.CurrentCookTimeEnd;
    this._menu.ToggleBusyDisableOverlayPanel(userInterfaceState.IsMicrowaveBusy || userInterfaceState.ContainedSolids.Length == 0);
    this.RefreshContentsDisplay(this.EntMan.GetEntityArray(userInterfaceState.ContainedSolids));
    this._menu.CookTimeInfoLabel.Text = Loc.GetString("microwave-bound-user-interface-cook-time-label", new (string, object)[1]
    {
      ("time", (object) (userInterfaceState.ActiveButtonIndex == 0 ? Loc.GetString("microwave-menu-instant-button") : userInterfaceState.CurrentCookTime.ToString()))
    });
    ((BaseButton) this._menu.StartButton).Disabled = userInterfaceState.IsMicrowaveBusy || userInterfaceState.ContainedSolids.Length == 0;
    ((BaseButton) this._menu.EjectButton).Disabled = userInterfaceState.IsMicrowaveBusy || userInterfaceState.ContainedSolids.Length == 0;
    if (userInterfaceState.ActiveButtonIndex == 0)
      ((BaseButton) this._menu.InstantCookButton).Pressed = true;
    else
      ((BaseButton) ((Control) this._menu.CookTimeButtonVbox).GetChild(userInterfaceState.ActiveButtonIndex - 1)).Pressed = true;
    if (userInterfaceState.IsMicrowaveBusy && userInterfaceState.ContainedSolids.Length != 0)
      this._menu.IngredientsPanel.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#947300", new Color?())
      };
    else
      this._menu.IngredientsPanel.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1B1B1E", new Color?())
      };
  }

  private void RefreshContentsDisplay(EntityUid[] containedSolids)
  {
    this._reagents.Clear();
    if (this._menu == null)
      return;
    this._solids.Clear();
    this._menu.IngredientsList.Clear();
    foreach (EntityUid containedSolid in containedSolids)
    {
      if (this.EntMan.Deleted(containedSolid))
        break;
      IconComponent iconComponent;
      Texture icon;
      if (this.EntMan.TryGetComponent<IconComponent>(containedSolid, ref iconComponent))
      {
        icon = this.EntMan.System<SpriteSystem>().GetIcon(iconComponent);
      }
      else
      {
        SpriteComponent spriteComponent;
        if (this.EntMan.TryGetComponent<SpriteComponent>(containedSolid, ref spriteComponent))
          icon = ((IDirectionalTextureProvider) spriteComponent.Icon)?.Default;
        else
          continue;
      }
      this._solids.Add(this._menu.IngredientsList.IndexOf(this._menu.IngredientsList.AddItem(this.EntMan.GetComponent<MetaDataComponent>(containedSolid).EntityName, icon, true, (object) null, 1f)), containedSolid);
    }
  }
}
