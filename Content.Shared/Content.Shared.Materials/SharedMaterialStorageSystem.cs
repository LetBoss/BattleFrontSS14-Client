using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Research.Components;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Materials;

public abstract class SharedMaterialStorageSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	private const int DefaultSheetVolume = 100;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MaterialStorageComponent, MapInitEvent>((ComponentEventHandler<MaterialStorageComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaterialStorageComponent, InteractUsingEvent>((ComponentEventHandler<MaterialStorageComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaterialStorageComponent, TechnologyDatabaseModifiedEvent>((EntityEventRefHandler<MaterialStorageComponent, TechnologyDatabaseModifiedEvent>)OnDatabaseModified, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<InsertingMaterialStorageComponent> query = ((EntitySystem)this).EntityQueryEnumerator<InsertingMaterialStorageComponent>();
		EntityUid uid = default(EntityUid);
		InsertingMaterialStorageComponent inserting = default(InsertingMaterialStorageComponent);
		while (query.MoveNext(ref uid, ref inserting))
		{
			if (!(_timing.CurTime < inserting.EndTime))
			{
				_appearance.SetData(uid, (Enum)MaterialStorageVisuals.Inserting, (object)false, (AppearanceComponent)null);
				((EntitySystem)this).RemComp(uid, (IComponent)(object)inserting);
			}
		}
	}

	private void OnMapInit(EntityUid uid, MaterialStorageComponent component, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)MaterialStorageVisuals.Inserting, (object)false, (AppearanceComponent)null);
	}

	public Dictionary<ProtoId<MaterialPrototype>, int> GetStoredMaterials(Entity<MaterialStorageComponent?> ent, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(Entity<MaterialStorageComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return new Dictionary<ProtoId<MaterialPrototype>, int>();
		}
		Dictionary<ProtoId<MaterialPrototype>, int> mats = new Dictionary<ProtoId<MaterialPrototype>, int>(ent.Comp.Storage);
		GetStoredMaterialsEvent ev = new GetStoredMaterialsEvent(Entity<MaterialStorageComponent>.op_Implicit((Entity<MaterialStorageComponent>.op_Implicit(ent), ent.Comp)), mats, localOnly);
		((EntitySystem)this).RaiseLocalEvent<GetStoredMaterialsEvent>(Entity<MaterialStorageComponent>.op_Implicit(ent), ref ev, true);
		return ev.Materials;
	}

	public int GetMaterialAmount(EntityUid uid, MaterialPrototype material, MaterialStorageComponent? component = null, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetMaterialAmount(uid, material.ID, component, localOnly);
	}

	public int GetMaterialAmount(EntityUid uid, string material, MaterialStorageComponent? component = null, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, true))
		{
			return 0;
		}
		return GetStoredMaterials(Entity<MaterialStorageComponent>.op_Implicit((uid, component)), localOnly).GetValueOrDefault(ProtoId<MaterialPrototype>.op_Implicit(material), 0);
	}

	public int GetTotalMaterialAmount(EntityUid uid, MaterialStorageComponent? component = null, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, true))
		{
			return 0;
		}
		return GetStoredMaterials(Entity<MaterialStorageComponent>.op_Implicit((uid, component)), localOnly).Values.Sum();
	}

	public bool CanTakeVolume(EntityUid uid, int volume, MaterialStorageComponent? component = null, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, true))
		{
			return false;
		}
		if (component.StorageLimit.HasValue)
		{
			return GetTotalMaterialAmount(uid, component, localOnly: true) + volume <= component.StorageLimit;
		}
		return true;
	}

	public bool IsMaterialWhitelisted(Entity<MaterialStorageComponent?> ent, ProtoId<MaterialPrototype> material)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(Entity<MaterialStorageComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return false;
		}
		if (ent.Comp.MaterialWhiteList == null)
		{
			return true;
		}
		return ent.Comp.MaterialWhiteList.Contains(material);
	}

	public bool CanChangeMaterialAmount(EntityUid uid, string materialId, int volume, MaterialStorageComponent? component = null, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, true))
		{
			return false;
		}
		if (!CanTakeVolume(uid, volume, component))
		{
			return false;
		}
		if (!IsMaterialWhitelisted(Entity<MaterialStorageComponent>.op_Implicit((uid, component)), ProtoId<MaterialPrototype>.op_Implicit(materialId)))
		{
			return false;
		}
		return GetMaterialAmount(uid, materialId, component, localOnly) + volume >= 0;
	}

	public bool CanChangeMaterialAmount(Entity<MaterialStorageComponent?> entity, Dictionary<string, int> materials, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(Entity<MaterialStorageComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		int inVolume = materials.Values.Sum();
		Dictionary<ProtoId<MaterialPrototype>, int> stored = GetStoredMaterials(Entity<MaterialStorageComponent>.op_Implicit((Entity<MaterialStorageComponent>.op_Implicit(entity), entity.Comp)), localOnly);
		if (!CanTakeVolume(Entity<MaterialStorageComponent>.op_Implicit(entity), inVolume, entity.Comp))
		{
			return false;
		}
		foreach (var (material, amount) in materials)
		{
			if (!IsMaterialWhitelisted(entity, ProtoId<MaterialPrototype>.op_Implicit(material)))
			{
				return false;
			}
			if (stored.GetValueOrDefault(ProtoId<MaterialPrototype>.op_Implicit(material)) + amount < 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool TryChangeMaterialAmount(EntityUid uid, string materialId, int volume, MaterialStorageComponent? component = null, bool dirty = true, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, true))
		{
			return false;
		}
		if (!CanChangeMaterialAmount(uid, materialId, volume, component, localOnly))
		{
			return false;
		}
		ConsumeStoredMaterialsEvent changeEv = new ConsumeStoredMaterialsEvent(Entity<MaterialStorageComponent>.op_Implicit((uid, component)), new Dictionary<ProtoId<MaterialPrototype>, int> { 
		{
			ProtoId<MaterialPrototype>.op_Implicit(materialId),
			volume
		} }, localOnly);
		((EntitySystem)this).RaiseLocalEvent<ConsumeStoredMaterialsEvent>(uid, ref changeEv, false);
		int value = changeEv.Materials.Values.First();
		int existing = Extensions.GetOrNew<ProtoId<MaterialPrototype>, int>(component.Storage, ProtoId<MaterialPrototype>.op_Implicit(materialId));
		int localUpperLimit = ((!component.StorageLimit.HasValue) ? int.MaxValue : (component.StorageLimit.Value - existing));
		int localLowerLimit = -existing;
		int localChange = Math.Clamp(value, localLowerLimit, localUpperLimit);
		existing += localChange;
		if (existing == 0)
		{
			component.Storage.Remove(ProtoId<MaterialPrototype>.op_Implicit(materialId));
		}
		else
		{
			component.Storage[ProtoId<MaterialPrototype>.op_Implicit(materialId)] = existing;
		}
		MaterialAmountChangedEvent ev = default(MaterialAmountChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<MaterialAmountChangedEvent>(uid, ref ev, false);
		if (dirty)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		return true;
	}

	public bool TryChangeMaterialAmount(Entity<MaterialStorageComponent?> entity, Dictionary<string, int> materials, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TryChangeMaterialAmount(entity, materials.Select<KeyValuePair<string, int>, (ProtoId<MaterialPrototype>, int)>((KeyValuePair<string, int> p) => (new ProtoId<MaterialPrototype>(p.Key), Value: p.Value)).ToDictionary(), localOnly);
	}

	public bool TryChangeMaterialAmount(Entity<MaterialStorageComponent?> entity, Dictionary<ProtoId<MaterialPrototype>, int> materials, bool localOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(Entity<MaterialStorageComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		ProtoId<MaterialPrototype> key;
		int value;
		foreach (KeyValuePair<ProtoId<MaterialPrototype>, int> material3 in materials)
		{
			material3.Deconstruct(out key, out value);
			ProtoId<MaterialPrototype> material = key;
			int amount = value;
			if (!CanChangeMaterialAmount(Entity<MaterialStorageComponent>.op_Implicit(entity), ProtoId<MaterialPrototype>.op_Implicit(material), amount, Entity<MaterialStorageComponent>.op_Implicit(entity)))
			{
				return false;
			}
		}
		ConsumeStoredMaterialsEvent changeEv = new ConsumeStoredMaterialsEvent(Entity<MaterialStorageComponent>.op_Implicit((Entity<MaterialStorageComponent>.op_Implicit(entity), entity.Comp)), materials, localOnly);
		((EntitySystem)this).RaiseLocalEvent<ConsumeStoredMaterialsEvent>(Entity<MaterialStorageComponent>.op_Implicit(entity), ref changeEv, false);
		foreach (KeyValuePair<ProtoId<MaterialPrototype>, int> material4 in changeEv.Materials)
		{
			material4.Deconstruct(out key, out value);
			ProtoId<MaterialPrototype> material2 = key;
			int value2 = value;
			int existing = Extensions.GetOrNew<ProtoId<MaterialPrototype>, int>(entity.Comp.Storage, material2);
			int localUpperLimit = ((!entity.Comp.StorageLimit.HasValue) ? int.MaxValue : (entity.Comp.StorageLimit.Value - existing));
			int localLowerLimit = -existing;
			int localChange = Math.Clamp(value2, localLowerLimit, localUpperLimit);
			existing += localChange;
			if (existing == 0)
			{
				entity.Comp.Storage.Remove(material2);
			}
			else
			{
				entity.Comp.Storage[material2] = existing;
			}
		}
		MaterialAmountChangedEvent ev = default(MaterialAmountChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<MaterialAmountChangedEvent>(Entity<MaterialStorageComponent>.op_Implicit(entity), ref ev, false);
		((EntitySystem)this).Dirty(Entity<MaterialStorageComponent>.op_Implicit(entity), (IComponent)(object)entity.Comp, (MetaDataComponent)null);
		return true;
	}

	public bool TrySetMaterialAmount(EntityUid uid, string materialId, int volume, MaterialStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, true))
		{
			return false;
		}
		int curAmount = GetMaterialAmount(uid, materialId, component);
		int delta = volume - curAmount;
		return TryChangeMaterialAmount(uid, materialId, delta, component);
	}

	public virtual bool TryInsertMaterialEntity(EntityUid user, EntityUid toInsert, EntityUid receiver, MaterialStorageComponent? storage = null, MaterialComponent? material = null, PhysicalCompositionComponent? composition = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MaterialStorageComponent>(receiver, ref storage, true))
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<MaterialComponent, PhysicalCompositionComponent>(toInsert, ref material, ref composition, false))
		{
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(storage.Whitelist, toInsert))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<UnremoveableComponent>(toInsert))
		{
			return false;
		}
		StackComponent stackComponent = default(StackComponent);
		int multiplier = ((!((EntitySystem)this).TryComp<StackComponent>(toInsert, ref stackComponent)) ? 1 : stackComponent.Count);
		int totalVolume = 0;
		string key;
		int value;
		foreach (KeyValuePair<string, int> item in composition.MaterialComposition)
		{
			item.Deconstruct(out key, out value);
			string mat = key;
			int vol = value;
			if (!CanChangeMaterialAmount(receiver, mat, vol * multiplier, storage))
			{
				return false;
			}
			totalVolume += vol * multiplier;
		}
		if (!CanTakeVolume(receiver, totalVolume, storage, localOnly: true))
		{
			return false;
		}
		foreach (KeyValuePair<string, int> item2 in composition.MaterialComposition)
		{
			item2.Deconstruct(out key, out value);
			string mat2 = key;
			int vol2 = value;
			TryChangeMaterialAmount(receiver, mat2, vol2 * multiplier, storage);
		}
		InsertingMaterialStorageComponent insertingComp = ((EntitySystem)this).EnsureComp<InsertingMaterialStorageComponent>(receiver);
		insertingComp.EndTime = _timing.CurTime + storage.InsertionTime;
		if (!storage.IgnoreColor)
		{
			MaterialPrototype lastMat = default(MaterialPrototype);
			_prototype.TryIndex<MaterialPrototype>(composition.MaterialComposition.Keys.First(), ref lastMat);
			insertingComp.MaterialColor = lastMat?.Color;
		}
		_appearance.SetData(receiver, (Enum)MaterialStorageVisuals.Inserting, (object)true, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(receiver, (IComponent)(object)insertingComp, (MetaDataComponent)null);
		MaterialEntityInsertedEvent ev = new MaterialEntityInsertedEvent(material);
		((EntitySystem)this).RaiseLocalEvent<MaterialEntityInsertedEvent>(receiver, ref ev, false);
		return true;
	}

	public void UpdateMaterialWhitelist(EntityUid uid, MaterialStorageComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MaterialStorageComponent>(uid, ref component, false))
		{
			GetMaterialWhitelistEvent ev = new GetMaterialWhitelistEvent(uid);
			((EntitySystem)this).RaiseLocalEvent<GetMaterialWhitelistEvent>(uid, ref ev, false);
			component.MaterialWhiteList = ev.Whitelist;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnInteractUsing(EntityUid uid, MaterialStorageComponent component, InteractUsingEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && component.InsertOnInteract)
		{
			((HandledEntityEventArgs)args).Handled = TryInsertMaterialEntity(args.User, args.Used, uid, component);
		}
	}

	private void OnDatabaseModified(Entity<MaterialStorageComponent> ent, ref TechnologyDatabaseModifiedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateMaterialWhitelist(Entity<MaterialStorageComponent>.op_Implicit(ent));
	}

	public int GetSheetVolume(MaterialPrototype material)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!material.StackEntity.HasValue)
		{
			return 100;
		}
		IPrototypeManager prototype = _prototype;
		EntProtoId? stackEntity = material.StackEntity;
		PhysicalCompositionComponent composition = default(PhysicalCompositionComponent);
		if (!prototype.Index<EntityPrototype>(stackEntity.HasValue ? EntProtoId.op_Implicit(stackEntity.GetValueOrDefault()) : null).TryGetComponent<PhysicalCompositionComponent>(ref composition, base.EntityManager.ComponentFactory))
		{
			return 100;
		}
		return composition.MaterialComposition.FirstOrDefault<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Key == material.ID).Value;
	}
}
