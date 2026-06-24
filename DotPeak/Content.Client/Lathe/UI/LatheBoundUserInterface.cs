// Decompiled with JetBrains decompiler
// Type: Content.Client.Lathe.UI.LatheBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Lathe;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Lathe.UI;

public sealed class LatheBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private LatheMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindowCenteredRight<LatheMenu>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.OnServerListButtonPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ConsoleServerSelectionMessage()));
    this._menu.RecipeQueueAction += (Action<string, int>) ((recipe, amount) => this.SendMessage((BoundUserInterfaceMessage) new LatheQueueRecipeMessage(recipe, amount)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is LatheUpdateState latheUpdateState))
      return;
    if (this._menu != null)
      this._menu.Recipes = latheUpdateState.Recipes;
    this._menu?.PopulateRecipes();
    this._menu?.UpdateCategories();
    this._menu?.PopulateQueueList((IReadOnlyCollection<ProtoId<LatheRecipePrototype>>) latheUpdateState.Queue);
    this._menu?.SetQueueInfo(latheUpdateState.CurrentlyProducing);
  }
}
