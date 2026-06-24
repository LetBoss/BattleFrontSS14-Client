using System;
using System.Collections.Generic;
using Content.Shared.Ghost;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Skin;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgSkinItemComponent : Component, ISerializationGenerated<PubgSkinItemComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool IsBase;

	[DataField(null, false, 1, false, false, null)]
	public SkinRarity Rarity;

	[DataField(null, false, 1, false, false, null)]
	public int CraftPrice;

	[DataField(null, false, 1, false, false, null)]
	public int SellPrice = 100;

	[DataField(null, false, 1, false, false, null)]
	public int PremiumPrice;

	[DataField(null, false, 1, false, false, null)]
	public int CollectibleLimit;

	[DataField(null, false, 1, false, false, null)]
	public List<PubgSkinShopOfferPrototype> ShopOffers = new List<PubgSkinShopOfferPrototype>();

	[DataField(null, false, 1, false, false, null)]
	public string Category = "";

	[DataField(null, false, 1, false, false, null)]
	public bool Disabled;

	[DataField(null, false, 1, false, false, null)]
	public bool CanDropFromCase;

	[DataField(null, false, 1, false, false, null)]
	public string Description = "";

	[DataField(null, false, 1, false, false, null)]
	public List<string> Tags = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public int? ExpiresIn;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<GhostComponent>? GhostPrototype;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<GhostComponent>? AdminGhostPrototype;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgSkinItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgSkinItemComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgSkinItemComponent>(this, ref target, hookCtx, false, context))
		{
			bool IsBaseTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsBase, ref IsBaseTemp, hookCtx, false, context))
			{
				IsBaseTemp = IsBase;
			}
			target.IsBase = IsBaseTemp;
			SkinRarity RarityTemp = SkinRarity.Common;
			if (!serialization.TryCustomCopy<SkinRarity>(Rarity, ref RarityTemp, hookCtx, false, context))
			{
				RarityTemp = Rarity;
			}
			target.Rarity = RarityTemp;
			int CraftPriceTemp = 0;
			if (!serialization.TryCustomCopy<int>(CraftPrice, ref CraftPriceTemp, hookCtx, false, context))
			{
				CraftPriceTemp = CraftPrice;
			}
			target.CraftPrice = CraftPriceTemp;
			int SellPriceTemp = 0;
			if (!serialization.TryCustomCopy<int>(SellPrice, ref SellPriceTemp, hookCtx, false, context))
			{
				SellPriceTemp = SellPrice;
			}
			target.SellPrice = SellPriceTemp;
			int PremiumPriceTemp = 0;
			if (!serialization.TryCustomCopy<int>(PremiumPrice, ref PremiumPriceTemp, hookCtx, false, context))
			{
				PremiumPriceTemp = PremiumPrice;
			}
			target.PremiumPrice = PremiumPriceTemp;
			int CollectibleLimitTemp = 0;
			if (!serialization.TryCustomCopy<int>(CollectibleLimit, ref CollectibleLimitTemp, hookCtx, false, context))
			{
				CollectibleLimitTemp = CollectibleLimit;
			}
			target.CollectibleLimit = CollectibleLimitTemp;
			List<PubgSkinShopOfferPrototype> ShopOffersTemp = null;
			if (ShopOffers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<PubgSkinShopOfferPrototype>>(ShopOffers, ref ShopOffersTemp, hookCtx, true, context))
			{
				ShopOffersTemp = serialization.CreateCopy<List<PubgSkinShopOfferPrototype>>(ShopOffers, hookCtx, context, false);
			}
			target.ShopOffers = ShopOffersTemp;
			string CategoryTemp = null;
			if (Category == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Category, ref CategoryTemp, hookCtx, false, context))
			{
				CategoryTemp = Category;
			}
			target.Category = CategoryTemp;
			bool DisabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Disabled, ref DisabledTemp, hookCtx, false, context))
			{
				DisabledTemp = Disabled;
			}
			target.Disabled = DisabledTemp;
			bool CanDropFromCaseTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanDropFromCase, ref CanDropFromCaseTemp, hookCtx, false, context))
			{
				CanDropFromCaseTemp = CanDropFromCase;
			}
			target.CanDropFromCase = CanDropFromCaseTemp;
			string DescriptionTemp = null;
			if (Description == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = Description;
			}
			target.Description = DescriptionTemp;
			List<string> TagsTemp = null;
			if (Tags == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Tags, ref TagsTemp, hookCtx, true, context))
			{
				TagsTemp = serialization.CreateCopy<List<string>>(Tags, hookCtx, context, false);
			}
			target.Tags = TagsTemp;
			int? ExpiresInTemp = null;
			if (!serialization.TryCustomCopy<int?>(ExpiresIn, ref ExpiresInTemp, hookCtx, false, context))
			{
				ExpiresInTemp = ExpiresIn;
			}
			target.ExpiresIn = ExpiresInTemp;
			EntProtoId<GhostComponent>? GhostPrototypeTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId<GhostComponent>?>(GhostPrototype, ref GhostPrototypeTemp, hookCtx, false, context))
			{
				GhostPrototypeTemp = serialization.CreateCopy<EntProtoId<GhostComponent>?>(GhostPrototype, hookCtx, context, false);
			}
			target.GhostPrototype = GhostPrototypeTemp;
			EntProtoId<GhostComponent>? AdminGhostPrototypeTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId<GhostComponent>?>(AdminGhostPrototype, ref AdminGhostPrototypeTemp, hookCtx, false, context))
			{
				AdminGhostPrototypeTemp = serialization.CreateCopy<EntProtoId<GhostComponent>?>(AdminGhostPrototype, hookCtx, context, false);
			}
			target.AdminGhostPrototype = AdminGhostPrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgSkinItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSkinItemComponent cast = (PubgSkinItemComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSkinItemComponent cast = (PubgSkinItemComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSkinItemComponent def = (PubgSkinItemComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgSkinItemComponent Instantiate()
	{
		return new PubgSkinItemComponent();
	}
}
