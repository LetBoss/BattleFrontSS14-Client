using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Store;

public static class ListingLocalisationHelpers
{
	public static string GetLocalisedNameOrEntityName(ListingData listingData, IPrototypeManager prototypeManager)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string name = string.Empty;
		if (listingData.Name != null)
		{
			name = Loc.GetString(listingData.Name);
		}
		else if (listingData.ProductEntity.HasValue)
		{
			name = prototypeManager.Index(listingData.ProductEntity.Value).Name;
		}
		return name;
	}

	public static string GetLocalisedDescriptionOrEntityDescription(ListingData listingData, IPrototypeManager prototypeManager)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		string desc = string.Empty;
		if (listingData.Description != null)
		{
			desc = Loc.GetString(listingData.Description);
		}
		else if (listingData.ProductEntity.HasValue)
		{
			desc = prototypeManager.Index(listingData.ProductEntity.Value).Description;
		}
		return desc;
	}
}
