// Decompiled with JetBrains decompiler
// Type: Content.Client.Store.Ui.StoreBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Store.Ui;

public sealed class StoreBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private IPrototypeManager _prototypeManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private StoreMenu? _menu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private string _search = string.Empty;
  [Robust.Shared.ViewVariables.ViewVariables]
  private HashSet<ListingDataWithCostModifiers> _listings = new HashSet<ListingDataWithCostModifiers>();

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<StoreMenu>((BoundUserInterface) this);
    StoreComponent storeComponent;
    if (this.EntMan.TryGetComponent<StoreComponent>(this.Owner, ref storeComponent))
      this._menu.Title = Loc.GetString(LocId.op_Implicit(storeComponent.Name));
    this._menu.OnListingButtonPressed += (Action<BaseButton.ButtonEventArgs, ListingDataWithCostModifiers>) ((_, listing) => this.SendMessage((BoundUserInterfaceMessage) new StoreBuyListingMessage(ProtoId<ListingPrototype>.op_Implicit(listing.ID))));
    this._menu.OnCategoryButtonPressed += (Action<BaseButton.ButtonEventArgs, string>) ((_, category) =>
    {
      this._menu.CurrentCategory = category;
      this._menu?.UpdateListing();
    });
    this._menu.OnWithdrawAttempt += (Action<BaseButton.ButtonEventArgs, string, int>) ((_, type, amount) => this.SendMessage((BoundUserInterfaceMessage) new StoreRequestWithdrawMessage(type, amount)));
    this._menu.SearchTextUpdated += (EventHandler<string>) ((_, search) =>
    {
      this._search = search.Trim().ToLowerInvariant();
      this.UpdateListingsWithSearchFilter();
    });
    this._menu.OnRefundAttempt += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new StoreRequestRefundMessage()));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is StoreUpdateState storeUpdateState))
      return;
    this._listings = storeUpdateState.Listings;
    this._menu?.UpdateBalance(storeUpdateState.Balance);
    this.UpdateListingsWithSearchFilter();
    this._menu?.SetFooterVisibility(storeUpdateState.ShowFooter);
    this._menu?.UpdateRefund(storeUpdateState.AllowRefund);
  }

  private void UpdateListingsWithSearchFilter()
  {
    if (this._menu == null)
      return;
    HashSet<ListingDataWithCostModifiers> withCostModifiersSet = new HashSet<ListingDataWithCostModifiers>((IEnumerable<ListingDataWithCostModifiers>) this._listings);
    if (!string.IsNullOrEmpty(this._search))
      withCostModifiersSet.RemoveWhere((Predicate<ListingDataWithCostModifiers>) (listingData => !ListingLocalisationHelpers.GetLocalisedNameOrEntityName((ListingData) listingData, this._prototypeManager).Trim().ToLowerInvariant().Contains(this._search) && !ListingLocalisationHelpers.GetLocalisedDescriptionOrEntityDescription((ListingData) listingData, this._prototypeManager).Trim().ToLowerInvariant().Contains(this._search)));
    this._menu.PopulateStoreCategoryButtons(withCostModifiersSet);
    this._menu.UpdateListing(withCostModifiersSet.ToList<ListingDataWithCostModifiers>());
  }
}
