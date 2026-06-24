// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.BinSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Linq;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public sealed class BinSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedAdminLogManager _admin;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BinComponent, ComponentStartup>(new ComponentEventHandler<BinComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<BinComponent, MapInitEvent>(new ComponentEventHandler<BinComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<BinComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<BinComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted));
    this.SubscribeLocalEvent<BinComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<BinComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved));
    this.SubscribeLocalEvent<BinComponent, InteractHandEvent>(new ComponentEventHandler<BinComponent, InteractHandEvent>(this.OnInteractHand), new Type[1]
    {
      typeof (SharedItemSystem)
    });
    this.SubscribeLocalEvent<BinComponent, AfterInteractUsingEvent>(new ComponentEventHandler<BinComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing));
    this.SubscribeLocalEvent<BinComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<BinComponent, GetVerbsEvent<AlternativeVerb>>(this.OnAltInteractHand));
    this.SubscribeLocalEvent<BinComponent, ExaminedEvent>(new ComponentEventHandler<BinComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnExamined(EntityUid uid, BinComponent component, ExaminedEvent args)
  {
    args.PushText(this.Loc.GetString("bin-component-on-examine-text", ("count", (object) component.Items.Count)));
  }

  private void OnStartup(EntityUid uid, BinComponent component, ComponentStartup args)
  {
    component.ItemContainer = this._container.EnsureContainer<Container>(uid, component.ContainerId);
  }

  private void OnMapInit(EntityUid uid, BinComponent component, MapInitEvent args)
  {
    if (this._net.IsClient)
      return;
    TransformComponent transformComponent = this.Transform(uid);
    foreach (EntProtoId initialContent in component.InitialContents)
    {
      EntityUid toInsert = this.Spawn((string) initialContent, transformComponent.Coordinates);
      if (!this.TryInsertIntoBin(uid, toInsert, component))
      {
        this.Log.Error($"Entity {this.ToPrettyString((Entity<MetaDataComponent>) toInsert)} was unable to be initialized into bin {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
        break;
      }
    }
  }

  private void OnEntInserted(Entity<BinComponent> ent, ref EntInsertedIntoContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.ContainerId)
      return;
    ent.Comp.Items.Add(args.Entity);
  }

  private void OnEntRemoved(Entity<BinComponent> ent, ref EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.ContainerId)
      return;
    ent.Comp.Items.Remove(args.Entity);
  }

  private void OnInteractHand(EntityUid uid, BinComponent component, InteractHandEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? toRemove = new EntityUid?(component.Items.LastOrDefault<EntityUid>());
    if (!this.TryRemoveFromBin(uid, toRemove, component))
      return;
    this._hands.TryPickupAnyHand(args.User, toRemove.Value);
    ISharedAdminLogManager admin = this._admin;
    LogStringHandler logStringHandler = new LogStringHandler(20, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "player", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" removed ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) toRemove.Value), "ToPrettyString(toGrab.Value)");
    logStringHandler.AppendLiteral(" from bin ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    admin.Add(LogType.Pickup, LogImpact.Low, ref local);
    args.Handled = true;
  }

  private void OnAltInteractHand(
    EntityUid uid,
    BinComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.Using.HasValue)
      return;
    bool canReach = args.CanAccess && args.CanInteract;
    this.InsertIntoBin(args.User, args.Target, args.Using.Value, component, false, canReach);
  }

  private void OnAfterInteractUsing(
    EntityUid uid,
    BinComponent component,
    AfterInteractUsingEvent args)
  {
    this.InsertIntoBin(args.User, uid, args.Used, component, args.Handled, args.CanReach);
    args.Handled = true;
  }

  private void InsertIntoBin(
    EntityUid user,
    EntityUid target,
    EntityUid itemInHand,
    BinComponent component,
    bool handled,
    bool canReach)
  {
    if (handled || !canReach || !this.TryInsertIntoBin(target, itemInHand, component))
      return;
    ISharedAdminLogManager admin = this._admin;
    LogStringHandler logStringHandler = new LogStringHandler(21, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), "player", "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" inserted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" into bin ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    admin.Add(LogType.Pickup, LogImpact.Low, ref local);
  }

  public bool TryInsertIntoBin(EntityUid uid, EntityUid toInsert, BinComponent? component = null)
  {
    if (!this.Resolve<BinComponent>(uid, ref component) || component.Items.Count >= component.MaxItems || this._whitelistSystem.IsWhitelistFail(component.Whitelist, toInsert))
      return false;
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) toInsert, (BaseContainer) component.ItemContainer);
    this.Dirty(uid, (IComponent) component);
    return true;
  }

  public bool TryRemoveFromBin(EntityUid uid, EntityUid? toRemove, BinComponent? component = null)
  {
    if (!this.Resolve<BinComponent>(uid, ref component) || component.Items.Count == 0 || !toRemove.HasValue)
      return false;
    EntityUid? nullable = toRemove;
    EntityUid entityUid = component.Items.LastOrDefault<EntityUid>();
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || !this._container.Remove((Entity<TransformComponent, MetaDataComponent>) toRemove.Value, (BaseContainer) component.ItemContainer))
      return false;
    component.Items.Remove(toRemove.Value);
    this.Dirty(uid, (IComponent) component);
    return true;
  }
}
