// Decompiled with JetBrains decompiler
// Type: Content.Shared.Materials.OreSilo.SharedOreSiloSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Materials.OreSilo;

public abstract class SharedOreSiloSystem : EntitySystem
{
  [Dependency]
  private SharedMaterialStorageSystem _materialStorage;
  [Dependency]
  private SharedPowerReceiverSystem _powerReceiver;
  [Dependency]
  private SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<OreSiloClientComponent> _clientQuery;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<OreSiloComponent, ToggleOreSiloClientMessage>(new EntityEventRefHandler<OreSiloComponent, ToggleOreSiloClientMessage>(this.OnToggleOreSiloClient));
    this.SubscribeLocalEvent<OreSiloComponent, ComponentShutdown>(new EntityEventRefHandler<OreSiloComponent, ComponentShutdown>(this.OnSiloShutdown));
    this.Subs.BuiEvents<OreSiloComponent>((object) OreSiloUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<OreSiloComponent>) (subs => subs.Event<BoundUIOpenedEvent>(new EntityEventRefHandler<OreSiloComponent, BoundUIOpenedEvent>(this.OnBoundUIOpened))));
    this.SubscribeLocalEvent<OreSiloClientComponent, GetStoredMaterialsEvent>(new EntityEventRefHandler<OreSiloClientComponent, GetStoredMaterialsEvent>(this.OnGetStoredMaterials));
    this.SubscribeLocalEvent<OreSiloClientComponent, ConsumeStoredMaterialsEvent>(new EntityEventRefHandler<OreSiloClientComponent, ConsumeStoredMaterialsEvent>(this.OnConsumeStoredMaterials));
    this.SubscribeLocalEvent<OreSiloClientComponent, ComponentShutdown>(new EntityEventRefHandler<OreSiloClientComponent, ComponentShutdown>(this.OnClientShutdown));
    this._clientQuery = this.GetEntityQuery<OreSiloClientComponent>();
  }

  private void OnToggleOreSiloClient(
    Entity<OreSiloComponent> ent,
    ref ToggleOreSiloClientMessage args)
  {
    EntityUid entity = this.GetEntity(args.Client);
    OreSiloClientComponent component;
    if (!this._clientQuery.TryComp(entity, out component))
      return;
    if (ent.Comp.Clients.Contains(entity))
    {
      component.Silo = new EntityUid?();
      this.Dirty(entity, (IComponent) component);
      ent.Comp.Clients.Remove(entity);
      this.Dirty<OreSiloComponent>(ent);
      this.UpdateOreSiloUi(ent);
    }
    else
    {
      if (!this.CanTransmitMaterials((Entity<OreSiloComponent, TransformComponent>) ((EntityUid) ent, (OreSiloComponent) ent), entity))
        return;
      Dictionary<ProtoId<MaterialPrototype>, int> storedMaterials = this._materialStorage.GetStoredMaterials((Entity<MaterialStorageComponent>) entity, true);
      Dictionary<string, int> materials = new Dictionary<string, int>();
      foreach ((ProtoId<MaterialPrototype> key, int num) in storedMaterials)
        materials.Add((string) key, -num);
      this._materialStorage.TryChangeMaterialAmount((Entity<MaterialStorageComponent>) entity, materials, true);
      this._materialStorage.TryChangeMaterialAmount((Entity<MaterialStorageComponent>) ent.Owner, storedMaterials);
      ent.Comp.Clients.Add(entity);
      this.Dirty<OreSiloComponent>(ent);
      component.Silo = new EntityUid?((EntityUid) ent);
      this.Dirty(entity, (IComponent) component);
      this.UpdateOreSiloUi(ent);
    }
  }

  private void OnBoundUIOpened(Entity<OreSiloComponent> ent, ref BoundUIOpenedEvent args)
  {
    this.UpdateOreSiloUi(ent);
  }

  private void OnSiloShutdown(Entity<OreSiloComponent> ent, ref ComponentShutdown args)
  {
    foreach (EntityUid client in ent.Comp.Clients)
    {
      OreSiloClientComponent component;
      if (this._clientQuery.TryComp(client, out component))
      {
        component.Silo = new EntityUid?();
        this.Dirty(client, (IComponent) component);
      }
    }
  }

  protected virtual void UpdateOreSiloUi(Entity<OreSiloComponent> ent)
  {
  }

  private void OnGetStoredMaterials(
    Entity<OreSiloClientComponent> ent,
    ref GetStoredMaterialsEvent args)
  {
    if (args.LocalOnly)
      return;
    EntityUid? silo = ent.Comp.Silo;
    if (!silo.HasValue)
      return;
    EntityUid valueOrDefault = silo.GetValueOrDefault();
    if (!this.CanTransmitMaterials((Entity<OreSiloComponent, TransformComponent>) valueOrDefault, (EntityUid) ent))
      return;
    foreach ((ProtoId<MaterialPrototype> protoId, int num) in this._materialStorage.GetStoredMaterials((Entity<MaterialStorageComponent>) valueOrDefault))
    {
      if (this._materialStorage.IsMaterialWhitelisted((Entity<MaterialStorageComponent>) ((EntityUid) args.Entity, (MaterialStorageComponent) args.Entity), protoId))
      {
        int orNew = args.Materials.GetOrNew<ProtoId<MaterialPrototype>, int>(protoId);
        args.Materials[protoId] = orNew + num;
      }
    }
  }

  private void OnConsumeStoredMaterials(
    Entity<OreSiloClientComponent> ent,
    ref ConsumeStoredMaterialsEvent args)
  {
    if (args.LocalOnly)
      return;
    EntityUid? silo = ent.Comp.Silo;
    if (!silo.HasValue)
      return;
    EntityUid valueOrDefault = silo.GetValueOrDefault();
    MaterialStorageComponent comp;
    if (!this.TryComp<MaterialStorageComponent>(valueOrDefault, out comp) || !this.CanTransmitMaterials((Entity<OreSiloComponent, TransformComponent>) valueOrDefault, (EntityUid) ent))
      return;
    foreach ((ProtoId<MaterialPrototype> protoId, int volume) in args.Materials)
    {
      if (this._materialStorage.TryChangeMaterialAmount(valueOrDefault, (string) protoId, volume, comp))
        args.Materials[protoId] = 0;
    }
  }

  private void OnClientShutdown(Entity<OreSiloClientComponent> ent, ref ComponentShutdown args)
  {
    OreSiloComponent comp;
    if (!this.TryComp<OreSiloComponent>(ent.Comp.Silo, out comp))
      return;
    comp.Clients.Remove((EntityUid) ent);
    this.Dirty(ent.Comp.Silo.Value, (IComponent) comp);
    this.UpdateOreSiloUi((Entity<OreSiloComponent>) (ent.Comp.Silo.Value, comp));
  }

  public bool CanTransmitMaterials(
    Entity<OreSiloComponent?, TransformComponent?> silo,
    EntityUid client)
  {
    if (!this.Resolve<OreSiloComponent, TransformComponent>((EntityUid) silo, ref silo.Comp1, ref silo.Comp2) || !this._powerReceiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) silo.Owner))
      return false;
    EntityUid? grid1 = this._transform.GetGrid((Entity<TransformComponent>) client);
    EntityUid? grid2 = this._transform.GetGrid((Entity<TransformComponent>) silo.Owner);
    return (grid1.HasValue == grid2.HasValue ? (grid1.HasValue ? (grid1.GetValueOrDefault() != grid2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && this._transform.InRange((Entity<TransformComponent>) (silo.Owner, silo.Comp2), (Entity<TransformComponent>) client, silo.Comp1.Range);
  }
}
