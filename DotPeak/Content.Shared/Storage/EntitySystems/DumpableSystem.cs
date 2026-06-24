// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.DumpableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Unit;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Placeable;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public sealed class DumpableSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDisposalUnitSystem _disposalUnitSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  private Robust.Shared.GameObjects.EntityQuery<ItemComponent> _itemQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._itemQuery = this.GetEntityQuery<ItemComponent>();
    this.SubscribeLocalEvent<DumpableComponent, AfterInteractEvent>(new ComponentEventHandler<DumpableComponent, AfterInteractEvent>(this.OnAfterInteract), after: new Type[1]
    {
      typeof (SharedEntityStorageSystem)
    });
    this.SubscribeLocalEvent<DumpableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DumpableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDumpVerb));
    this.SubscribeLocalEvent<DumpableComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<DumpableComponent, GetVerbsEvent<UtilityVerb>>(this.AddUtilityVerbs));
    this.SubscribeLocalEvent<DumpableComponent, DumpableDoAfterEvent>(new ComponentEventHandler<DumpableComponent, DumpableDoAfterEvent>(this.OnDoAfter));
  }

  private void OnAfterInteract(EntityUid uid, DumpableComponent component, AfterInteractEvent args)
  {
    StorageComponent comp;
    if (!args.CanReach || args.Handled || !this.HasComp<DisposalUnitComponent>(args.Target) && !this.HasComp<PlaceableSurfaceComponent>(args.Target) || !this.TryComp<StorageComponent>(uid, out comp) || !comp.Container.ContainedEntities.Any<EntityUid>())
      return;
    this.StartDoAfter(uid, args.Target.Value, args.User, component);
    args.Handled = true;
  }

  private void AddDumpVerb(
    EntityUid uid,
    DumpableComponent dumpable,
    GetVerbsEvent<AlternativeVerb> args)
  {
    StorageComponent comp;
    if (!args.CanAccess || !args.CanInteract || !this.TryComp<StorageComponent>(uid, out comp) || !comp.Container.ContainedEntities.Any<EntityUid>())
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.StartDoAfter(uid, args.Target, args.User, dumpable));
    alternativeVerb1.Text = this.Loc.GetString("dump-verb-name");
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png"));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void AddUtilityVerbs(
    EntityUid uid,
    DumpableComponent dumpable,
    GetVerbsEvent<UtilityVerb> args)
  {
    StorageComponent comp;
    if (!args.CanAccess || !args.CanInteract || !this.TryComp<StorageComponent>(uid, out comp) || !comp.Container.ContainedEntities.Any<EntityUid>())
      return;
    if (this.HasComp<DisposalUnitComponent>(args.Target))
    {
      UtilityVerb utilityVerb1 = new UtilityVerb();
      utilityVerb1.Act = (Action) (() => this.StartDoAfter(uid, args.Target, args.User, dumpable));
      utilityVerb1.Text = this.Loc.GetString("dump-disposal-verb-name", ("unit", (object) args.Target));
      utilityVerb1.IconEntity = new NetEntity?(this.GetNetEntity(uid));
      UtilityVerb utilityVerb2 = utilityVerb1;
      args.Verbs.Add(utilityVerb2);
    }
    if (!this.HasComp<PlaceableSurfaceComponent>(args.Target))
      return;
    UtilityVerb utilityVerb3 = new UtilityVerb();
    utilityVerb3.Act = (Action) (() => this.StartDoAfter(uid, args.Target, args.User, dumpable));
    utilityVerb3.Text = this.Loc.GetString("dump-placeable-verb-name", ("surface", (object) args.Target));
    utilityVerb3.IconEntity = new NetEntity?(this.GetNetEntity(uid));
    UtilityVerb utilityVerb4 = utilityVerb3;
    args.Verbs.Add(utilityVerb4);
  }

  private void StartDoAfter(
    EntityUid storageUid,
    EntityUid targetUid,
    EntityUid userUid,
    DumpableComponent dumpable)
  {
    StorageComponent comp;
    if (!this.TryComp<StorageComponent>(storageUid, out comp))
      return;
    float num = 0.0f;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) comp.Container.ContainedEntities)
    {
      ItemComponent component;
      ItemSizePrototype prototype;
      if (this._itemQuery.TryGetComponent(containedEntity, out component) && this._prototypeManager.TryIndex<ItemSizePrototype>(component.Size, out prototype))
        num += (float) prototype.Weight;
    }
    float seconds = num * ((float) dumpable.DelayPerItem.TotalSeconds * dumpable.Multiplier);
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userUid, seconds, (DoAfterEvent) new DumpableDoAfterEvent(), new EntityUid?(storageUid), new EntityUid?(targetUid), new EntityUid?(storageUid))
    {
      BreakOnMove = true,
      NeedHand = true
    });
  }

  private void OnDoAfter(EntityUid uid, DumpableComponent component, DumpableDoAfterEvent args)
  {
    StorageComponent comp;
    if (args.Handled || args.Cancelled || !this.TryComp<StorageComponent>(uid, out comp) || comp.Container.ContainedEntities.Count == 0)
      return;
    Queue<EntityUid> entityUidQueue = new Queue<EntityUid>((IEnumerable<EntityUid>) comp.Container.ContainedEntities);
    bool flag = false;
    if (this.HasComp<DisposalUnitComponent>(args.Args.Target))
    {
      flag = true;
      foreach (EntityUid toInsert in entityUidQueue)
        this._disposalUnitSystem.DoInsertDisposalUnit(args.Args.Target.Value, toInsert, args.Args.User);
    }
    else if (this.HasComp<PlaceableSurfaceComponent>(args.Args.Target))
    {
      flag = true;
      (Vector2 WorldPosition, Angle angle) = this._transformSystem.GetWorldPositionRotation(args.Args.Target.Value);
      foreach (EntityUid uid1 in entityUidQueue)
        this._transformSystem.SetWorldPositionRotation(uid1, WorldPosition + this._random.NextVector2Box() / 4f, angle);
    }
    else
    {
      Vector2 worldPosition = this._transformSystem.GetWorldPosition(uid);
      foreach (EntityUid uid2 in entityUidQueue)
      {
        TransformComponent component1 = this.Transform(uid2);
        this._transformSystem.SetWorldPositionRotation(uid2, worldPosition + this._random.NextVector2Box() / 4f, this._random.NextAngle(), component1);
      }
    }
    if (!flag)
      return;
    this._audio.PlayPredicted(component.DumpSound, uid, new EntityUid?(args.User));
  }
}
