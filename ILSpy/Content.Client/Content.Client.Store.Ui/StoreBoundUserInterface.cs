using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Store.Ui;

public sealed class StoreBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
	private IPrototypeManager _prototypeManager;

	[ViewVariables]
	private StoreMenu? _menu;

	[ViewVariables]
	private string _search = string.Empty;

	[ViewVariables]
	private HashSet<ListingDataWithCostModifiers> _listings = new HashSet<ListingDataWithCostModifiers>();

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<StoreMenu>((BoundUserInterface)(object)this);
		StoreComponent storeComponent = default(StoreComponent);
		if (base.EntMan.TryGetComponent<StoreComponent>(((BoundUserInterface)this).Owner, ref storeComponent))
		{
			((DefaultWindow)_menu).Title = Loc.GetString(LocId.op_Implicit(storeComponent.Name));
		}
		_menu.OnListingButtonPressed += delegate(ButtonEventArgs _, ListingDataWithCostModifiers listing)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new StoreBuyListingMessage(ProtoId<ListingPrototype>.op_Implicit(listing.ID)));
		};
		_menu.OnCategoryButtonPressed += delegate(ButtonEventArgs _, string category)
		{
			_menu.CurrentCategory = category;
			_menu?.UpdateListing();
		};
		_menu.OnWithdrawAttempt += delegate(ButtonEventArgs _, string type, int amount)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new StoreRequestWithdrawMessage(type, amount));
		};
		_menu.SearchTextUpdated += delegate(object? _, string search)
		{
			_search = search.Trim().ToLowerInvariant();
			UpdateListingsWithSearchFilter();
		};
		_menu.OnRefundAttempt += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new StoreRequestRefundMessage());
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is StoreUpdateState storeUpdateState)
		{
			_listings = storeUpdateState.Listings;
			_menu?.UpdateBalance(storeUpdateState.Balance);
			UpdateListingsWithSearchFilter();
			_menu?.SetFooterVisibility(storeUpdateState.ShowFooter);
			_menu?.UpdateRefund(storeUpdateState.AllowRefund);
		}
	}

	private void UpdateListingsWithSearchFilter()
	{
		if (_menu == null)
		{
			return;
		}
		HashSet<ListingDataWithCostModifiers> hashSet = new HashSet<ListingDataWithCostModifiers>(_listings);
		if (!string.IsNullOrEmpty(_search))
		{
			hashSet.RemoveWhere((ListingDataWithCostModifiers listingData) => !ListingLocalisationHelpers.GetLocalisedNameOrEntityName(listingData, _prototypeManager).Trim().ToLowerInvariant()
				.Contains(_search) && !ListingLocalisationHelpers.GetLocalisedDescriptionOrEntityDescription(listingData, _prototypeManager).Trim().ToLowerInvariant()
				.Contains(_search));
		}
		_menu.PopulateStoreCategoryButtons(hashSet);
		_menu.UpdateListing(hashSet.ToList());
	}
}
