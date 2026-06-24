using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class CatalogItem : ISerializationGenerated<CatalogItem>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string ItemPrototype = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public int Price;

	[DataField(null, false, 1, false, false, null)]
	public int PackQuantity = 1;

	[DataField(null, false, 1, false, false, null)]
	public bool IsWeapon;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CatalogItem target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CatalogItem>(this, ref target, hookCtx, false, context))
		{
			string ItemPrototypeTemp = null;
			if (ItemPrototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ItemPrototype, ref ItemPrototypeTemp, hookCtx, false, context))
			{
				ItemPrototypeTemp = ItemPrototype;
			}
			target.ItemPrototype = ItemPrototypeTemp;
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			int PriceTemp = 0;
			if (!serialization.TryCustomCopy<int>(Price, ref PriceTemp, hookCtx, false, context))
			{
				PriceTemp = Price;
			}
			target.Price = PriceTemp;
			int PackQuantityTemp = 0;
			if (!serialization.TryCustomCopy<int>(PackQuantity, ref PackQuantityTemp, hookCtx, false, context))
			{
				PackQuantityTemp = PackQuantity;
			}
			target.PackQuantity = PackQuantityTemp;
			bool IsWeaponTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsWeapon, ref IsWeaponTemp, hookCtx, false, context))
			{
				IsWeaponTemp = IsWeapon;
			}
			target.IsWeapon = IsWeaponTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CatalogItem target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CatalogItem cast = (CatalogItem)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CatalogItem Instantiate()
	{
		return new CatalogItem();
	}
}
