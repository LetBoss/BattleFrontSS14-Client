// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.RehydratableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public sealed class RehydratableSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solutions;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RehydratableComponent, SolutionContainerChangedEvent>(new EntityEventRefHandler<RehydratableComponent, SolutionContainerChangedEvent>((object) this, __methodptr(OnSolutionChange)), (Type[]) null, (Type[]) null);
  }

  private void OnSolutionChange(
    Entity<RehydratableComponent> ent,
    ref SolutionContainerChangedEvent args)
  {
    FixedPoint2 prototypeQuantity = this._solutions.GetTotalPrototypeQuantity(Entity<RehydratableComponent>.op_Implicit(ent), ProtoId<ReagentPrototype>.op_Implicit(ent.Comp.CatalystPrototype));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(44, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
    logStringHandler.AppendLiteral(" was hydrated, now contains a solution of: ");
    logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(args.Solution));
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Medium, ref local);
    if (!(prototypeQuantity != FixedPoint2.Zero) || !(prototypeQuantity >= ent.Comp.CatalystMinimum))
      return;
    this.Expand(ent);
  }

  private void Expand(Entity<RehydratableComponent> ent)
  {
    if (this._net.IsClient)
      return;
    EntityUid entityUid1;
    RehydratableComponent rehydratableComponent;
    ent.Deconstruct(ref entityUid1, ref rehydratableComponent);
    EntityUid entityUid2 = entityUid1;
    EntityUid entityUid3 = this.Spawn(EntProtoId.op_Implicit(RandomExtensions.Pick<EntProtoId>(this._random, (IReadOnlyList<EntProtoId>) rehydratableComponent.PossibleSpawns)), this.Transform(entityUid2).Coordinates);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(43, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
    logStringHandler.AppendLiteral(" has been hydrated correctly and spawned: ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entityUid3)), "ToPrettyString(target)");
    logStringHandler.AppendLiteral(".");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Medium, ref local);
    this._popup.PopupEntity(this.Loc.GetString("rehydratable-component-expands-message", ("owner", (object) entityUid2)), entityUid3);
    this._xform.AttachToGridOrMap(entityUid3, (TransformComponent) null);
    GotRehydratedEvent gotRehydratedEvent = new GotRehydratedEvent(entityUid3);
    this.RaiseLocalEvent<GotRehydratedEvent>(entityUid2, ref gotRehydratedEvent, false);
    this.RemComp<RehydratableComponent>(entityUid2);
    this.QueueDel(new EntityUid?(entityUid2));
  }
}
