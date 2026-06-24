// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.SharedMaterialStorageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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
    base.Initialize();
    this.SubscribeLocalEvent<MaterialStorageComponent, MapInitEvent>(new ComponentEventHandler<MaterialStorageComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<MaterialStorageComponent, InteractUsingEvent>(new ComponentEventHandler<MaterialStorageComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<MaterialStorageComponent, TechnologyDatabaseModifiedEvent>(new EntityEventRefHandler<MaterialStorageComponent, TechnologyDatabaseModifiedEvent>(this.OnDatabaseModified));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<InsertingMaterialStorageComponent> entityQueryEnumerator = this.EntityQueryEnumerator<InsertingMaterialStorageComponent>();
    EntityUid uid;
    InsertingMaterialStorageComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(this._timing.CurTime < comp1.EndTime))
      {
        this._appearance.SetData(uid, (Enum) MaterialStorageVisuals.Inserting, (object) false);
        this.RemComp(uid, (IComponent) comp1);
      }
    }
  }

  private void OnMapInit(EntityUid uid, MaterialStorageComponent component, MapInitEvent args)
  {
    this._appearance.SetData(uid, (Enum) MaterialStorageVisuals.Inserting, (object) false);
  }

  public Dictionary<ProtoId<MaterialPrototype>, int> GetStoredMaterials(
    Entity<MaterialStorageComponent?> ent,
    bool localOnly = false)
  {
    if (!this.Resolve<MaterialStorageComponent>((EntityUid) ent, ref ent.Comp, false))
      return new Dictionary<ProtoId<MaterialPrototype>, int>();
    Dictionary<ProtoId<MaterialPrototype>, int> Materials = new Dictionary<ProtoId<MaterialPrototype>, int>((IDictionary<ProtoId<MaterialPrototype>, int>) ent.Comp.Storage);
    GetStoredMaterialsEvent args = new GetStoredMaterialsEvent((Entity<MaterialStorageComponent>) ((EntityUid) ent, ent.Comp), Materials, localOnly);
    this.RaiseLocalEvent<GetStoredMaterialsEvent>((EntityUid) ent, ref args, true);
    return args.Materials;
  }

  public int GetMaterialAmount(
    EntityUid uid,
    MaterialPrototype material,
    MaterialStorageComponent? component = null,
    bool localOnly = false)
  {
    return this.GetMaterialAmount(uid, material.ID, component, localOnly);
  }

  public int GetMaterialAmount(
    EntityUid uid,
    string material,
    MaterialStorageComponent? component = null,
    bool localOnly = false)
  {
    return !this.Resolve<MaterialStorageComponent>(uid, ref component) ? 0 : this.GetStoredMaterials((Entity<MaterialStorageComponent>) (uid, component), localOnly).GetValueOrDefault<ProtoId<MaterialPrototype>, int>((ProtoId<MaterialPrototype>) material, 0);
  }

  public int GetTotalMaterialAmount(
    EntityUid uid,
    MaterialStorageComponent? component = null,
    bool localOnly = false)
  {
    return !this.Resolve<MaterialStorageComponent>(uid, ref component) ? 0 : this.GetStoredMaterials((Entity<MaterialStorageComponent>) (uid, component), localOnly).Values.Sum();
  }

  public bool CanTakeVolume(
    EntityUid uid,
    int volume,
    MaterialStorageComponent? component = null,
    bool localOnly = false)
  {
    if (!this.Resolve<MaterialStorageComponent>(uid, ref component))
      return false;
    if (!component.StorageLimit.HasValue)
      return true;
    int num = this.GetTotalMaterialAmount(uid, component, true) + volume;
    int? storageLimit = component.StorageLimit;
    int valueOrDefault = storageLimit.GetValueOrDefault();
    return num <= valueOrDefault & storageLimit.HasValue;
  }

  public bool IsMaterialWhitelisted(
    Entity<MaterialStorageComponent?> ent,
    ProtoId<MaterialPrototype> material)
  {
    if (!this.Resolve<MaterialStorageComponent>((EntityUid) ent, ref ent.Comp))
      return false;
    return ent.Comp.MaterialWhiteList == null || ent.Comp.MaterialWhiteList.Contains(material);
  }

  public bool CanChangeMaterialAmount(
    EntityUid uid,
    string materialId,
    int volume,
    MaterialStorageComponent? component = null,
    bool localOnly = false)
  {
    return this.Resolve<MaterialStorageComponent>(uid, ref component) && this.CanTakeVolume(uid, volume, component) && this.IsMaterialWhitelisted((Entity<MaterialStorageComponent>) (uid, component), (ProtoId<MaterialPrototype>) materialId) && this.GetMaterialAmount(uid, materialId, component, localOnly) + volume >= 0;
  }

  public bool CanChangeMaterialAmount(
    Entity<MaterialStorageComponent?> entity,
    Dictionary<string, int> materials,
    bool localOnly = false)
  {
    if (!this.Resolve<MaterialStorageComponent>((EntityUid) entity, ref entity.Comp))
      return false;
    int volume = materials.Values.Sum();
    Dictionary<ProtoId<MaterialPrototype>, int> storedMaterials = this.GetStoredMaterials((Entity<MaterialStorageComponent>) ((EntityUid) entity, entity.Comp), localOnly);
    if (!this.CanTakeVolume((EntityUid) entity, volume, entity.Comp))
      return false;
    foreach ((string str, int num) in materials)
    {
      if (!this.IsMaterialWhitelisted(entity, (ProtoId<MaterialPrototype>) str) || storedMaterials.GetValueOrDefault<ProtoId<MaterialPrototype>, int>((ProtoId<MaterialPrototype>) str) + num < 0)
        return false;
    }
    return true;
  }

  public bool TryChangeMaterialAmount(
    EntityUid uid,
    string materialId,
    int volume,
    MaterialStorageComponent? component = null,
    bool dirty = true,
    bool localOnly = false)
  {
    if (!this.Resolve<MaterialStorageComponent>(uid, ref component) || !this.CanChangeMaterialAmount(uid, materialId, volume, component, localOnly))
      return false;
    ConsumeStoredMaterialsEvent args1;
    ref ConsumeStoredMaterialsEvent local = ref args1;
    Entity<MaterialStorageComponent> Entity = (Entity<MaterialStorageComponent>) (uid, component);
    Dictionary<ProtoId<MaterialPrototype>, int> Materials = new Dictionary<ProtoId<MaterialPrototype>, int>();
    Materials.Add((ProtoId<MaterialPrototype>) materialId, volume);
    int num1 = localOnly ? 1 : 0;
    local = new ConsumeStoredMaterialsEvent(Entity, Materials, num1 != 0);
    this.RaiseLocalEvent<ConsumeStoredMaterialsEvent>(uid, ref args1);
    int num2 = args1.Materials.Values.First<int>();
    int orNew = component.Storage.GetOrNew<ProtoId<MaterialPrototype>, int>((ProtoId<MaterialPrototype>) materialId);
    int num3 = !component.StorageLimit.HasValue ? int.MaxValue : component.StorageLimit.Value - orNew;
    int min = -orNew;
    int max = num3;
    int num4 = Math.Clamp(num2, min, max);
    int num5 = orNew + num4;
    if (num5 == 0)
      component.Storage.Remove((ProtoId<MaterialPrototype>) materialId);
    else
      component.Storage[(ProtoId<MaterialPrototype>) materialId] = num5;
    MaterialAmountChangedEvent args2 = new MaterialAmountChangedEvent();
    this.RaiseLocalEvent<MaterialAmountChangedEvent>(uid, ref args2);
    if (dirty)
      this.Dirty(uid, (IComponent) component);
    return true;
  }

  public bool TryChangeMaterialAmount(
    Entity<MaterialStorageComponent?> entity,
    Dictionary<string, int> materials,
    bool localOnly = false)
  {
    return this.TryChangeMaterialAmount(entity, materials.Select<KeyValuePair<string, int>, (ProtoId<MaterialPrototype>, int)>((Func<KeyValuePair<string, int>, (ProtoId<MaterialPrototype>, int)>) (p => (new ProtoId<MaterialPrototype>(p.Key), p.Value))).ToDictionary<ProtoId<MaterialPrototype>, int>(), localOnly);
  }

  public bool TryChangeMaterialAmount(
    Entity<MaterialStorageComponent?> entity,
    Dictionary<ProtoId<MaterialPrototype>, int> materials,
    bool localOnly = false)
  {
    if (!this.Resolve<MaterialStorageComponent>((EntityUid) entity, ref entity.Comp))
      return false;
    foreach ((ProtoId<MaterialPrototype> key1, int num1) in materials)
    {
      ProtoId<MaterialPrototype> materialId = key1;
      int volume = num1;
      if (!this.CanChangeMaterialAmount((EntityUid) entity, (string) materialId, volume, (MaterialStorageComponent) entity))
        return false;
    }
    ConsumeStoredMaterialsEvent args1 = new ConsumeStoredMaterialsEvent((Entity<MaterialStorageComponent>) ((EntityUid) entity, entity.Comp), materials, localOnly);
    this.RaiseLocalEvent<ConsumeStoredMaterialsEvent>((EntityUid) entity, ref args1);
    foreach ((key1, num1) in args1.Materials)
    {
      ProtoId<MaterialPrototype> key2 = key1;
      int num2 = num1;
      int orNew = entity.Comp.Storage.GetOrNew<ProtoId<MaterialPrototype>, int>(key2);
      int num3 = !entity.Comp.StorageLimit.HasValue ? int.MaxValue : entity.Comp.StorageLimit.Value - orNew;
      int min = -orNew;
      int max = num3;
      int num4 = Math.Clamp(num2, min, max);
      int num5 = orNew + num4;
      if (num5 == 0)
        entity.Comp.Storage.Remove(key2);
      else
        entity.Comp.Storage[key2] = num5;
    }
    MaterialAmountChangedEvent args2 = new MaterialAmountChangedEvent();
    this.RaiseLocalEvent<MaterialAmountChangedEvent>((EntityUid) entity, ref args2);
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
    return true;
  }

  public bool TrySetMaterialAmount(
    EntityUid uid,
    string materialId,
    int volume,
    MaterialStorageComponent? component = null)
  {
    if (!this.Resolve<MaterialStorageComponent>(uid, ref component))
      return false;
    int materialAmount = this.GetMaterialAmount(uid, materialId, component);
    int volume1 = volume - materialAmount;
    return this.TryChangeMaterialAmount(uid, materialId, volume1, component);
  }

  public virtual bool TryInsertMaterialEntity(
    EntityUid user,
    EntityUid toInsert,
    EntityUid receiver,
    MaterialStorageComponent? storage = null,
    MaterialComponent? material = null,
    PhysicalCompositionComponent? composition = null)
  {
    if (!this.Resolve<MaterialStorageComponent>(receiver, ref storage) || !this.Resolve<MaterialComponent, PhysicalCompositionComponent>(toInsert, ref material, ref composition, false) || this._whitelistSystem.IsWhitelistFail(storage.Whitelist, toInsert) || this.HasComp<UnremoveableComponent>(toInsert))
      return false;
    StackComponent comp;
    int num1 = this.TryComp<StackComponent>(toInsert, out comp) ? comp.Count : 1;
    int volume = 0;
    foreach ((string key, int num4) in composition.MaterialComposition)
    {
      string materialId = key;
      int num3 = num4;
      if (!this.CanChangeMaterialAmount(receiver, materialId, num3 * num1, storage))
        return false;
      volume += num3 * num1;
    }
    if (!this.CanTakeVolume(receiver, volume, storage, true))
      return false;
    foreach ((key, num4) in composition.MaterialComposition)
    {
      string materialId = key;
      int num5 = num4;
      this.TryChangeMaterialAmount(receiver, materialId, num5 * num1, storage);
    }
    InsertingMaterialStorageComponent storageComponent = this.EnsureComp<InsertingMaterialStorageComponent>(receiver);
    storageComponent.EndTime = this._timing.CurTime + storage.InsertionTime;
    if (!storage.IgnoreColor)
    {
      MaterialPrototype prototype;
      this._prototype.TryIndex<MaterialPrototype>(composition.MaterialComposition.Keys.First<string>(), out prototype);
      storageComponent.MaterialColor = prototype?.Color;
    }
    this._appearance.SetData(receiver, (Enum) MaterialStorageVisuals.Inserting, (object) true);
    this.Dirty(receiver, (IComponent) storageComponent);
    MaterialEntityInsertedEvent args = new MaterialEntityInsertedEvent(material);
    this.RaiseLocalEvent<MaterialEntityInsertedEvent>(receiver, ref args);
    return true;
  }

  public void UpdateMaterialWhitelist(EntityUid uid, MaterialStorageComponent? component = null)
  {
    if (!this.Resolve<MaterialStorageComponent>(uid, ref component, false))
      return;
    GetMaterialWhitelistEvent args = new GetMaterialWhitelistEvent(uid);
    this.RaiseLocalEvent<GetMaterialWhitelistEvent>(uid, ref args);
    component.MaterialWhiteList = args.Whitelist;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnInteractUsing(
    EntityUid uid,
    MaterialStorageComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled || !component.InsertOnInteract)
      return;
    args.Handled = this.TryInsertMaterialEntity(args.User, args.Used, uid, component);
  }

  private void OnDatabaseModified(
    Entity<MaterialStorageComponent> ent,
    ref TechnologyDatabaseModifiedEvent args)
  {
    this.UpdateMaterialWhitelist((EntityUid) ent);
  }

  public int GetSheetVolume(MaterialPrototype material)
  {
    if (!material.StackEntity.HasValue)
      return 100;
    IPrototypeManager prototype = this._prototype;
    EntProtoId? stackEntity = material.StackEntity;
    string valueOrDefault = stackEntity.HasValue ? (string) stackEntity.GetValueOrDefault() : (string) null;
    PhysicalCompositionComponent component;
    return !prototype.Index<Robust.Shared.Prototypes.EntityPrototype>(valueOrDefault).TryGetComponent<PhysicalCompositionComponent>(out component, this.EntityManager.ComponentFactory) ? 100 : component.MaterialComposition.FirstOrDefault<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (kvp => kvp.Key == material.ID)).Value;
  }
}
