// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Stasis.CMStasisBagSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.Wounds;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Body.Organ;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Medical.Stasis;

public sealed class CMStasisBagSystem : EntitySystem
{
  [Dependency]
  private SharedXenoParasiteSystem _parasite;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private MobStateSystem _mobstate;
  [Dependency]
  private SharedEntityStorageSystem _entStorage;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private INetManager _net;
  private Robust.Shared.GameObjects.EntityQuery<OrganComponent> _organQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._organQuery = this.GetEntityQuery<OrganComponent>();
    this.SubscribeLocalEvent<CMStasisBagComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<CMStasisBagComponent, ContainerIsInsertingAttemptEvent>(this.OnStasisInsert));
    this.SubscribeLocalEvent<CMStasisBagComponent, ContainerIsRemovingAttemptEvent>(new EntityEventRefHandler<CMStasisBagComponent, ContainerIsRemovingAttemptEvent>(this.OnStasisRemove));
    this.SubscribeLocalEvent<CMStasisBagComponent, ExaminedEvent>(new EntityEventRefHandler<CMStasisBagComponent, ExaminedEvent>(this.OnStasisExamine));
    this.SubscribeLocalEvent<CMInStasisComponent, CMMetabolizeAttemptEvent>(new EntityEventRefHandler<CMInStasisComponent, CMMetabolizeAttemptEvent>(this.OnBloodstreamMetabolizeAttempt));
    this.SubscribeLocalEvent<CMInStasisComponent, MapInitEvent>(new EntityEventRefHandler<CMInStasisComponent, MapInitEvent>(this.OnInStasisMapInit));
    this.SubscribeLocalEvent<CMInStasisComponent, ComponentRemove>(new EntityEventRefHandler<CMInStasisComponent, ComponentRemove>(this.OnInStasisRemove));
    this.SubscribeLocalEvent<CMInStasisComponent, GetInfectedIncubationMultiplierEvent>(new EntityEventRefHandler<CMInStasisComponent, GetInfectedIncubationMultiplierEvent>(this.OnInStasisGetInfectedIncubationMultiplier));
    this.SubscribeLocalEvent<CMInStasisComponent, CMBleedAttemptEvent>(new EntityEventRefHandler<CMInStasisComponent, CMBleedAttemptEvent>(this.OnInStasisBleedAttempt));
  }

  private void OnStasisInsert(
    Entity<CMStasisBagComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    this.OnInsert(ent, args.EntityUid);
  }

  private void OnStasisRemove(
    Entity<CMStasisBagComponent> ent,
    ref ContainerIsRemovingAttemptEvent args)
  {
    this.OnRemove(ent, args.EntityUid);
  }

  private void OnStasisExamine(Entity<CMStasisBagComponent> ent, ref ExaminedEvent args)
  {
    string messageId = "rmc-stasis-new";
    if (ent.Comp.StasisLeft / ent.Comp.StasisMaxTime < 0.33000001311302185)
      messageId = "rmc-stasis-very-used";
    else if (ent.Comp.StasisLeft / ent.Comp.StasisMaxTime < 0.6600000262260437)
      messageId = "rmc-stasis-used";
    args.PushMarkup(this.Loc.GetString(messageId));
  }

  private void OnBloodstreamMetabolizeAttempt(
    Entity<CMInStasisComponent> ent,
    ref CMMetabolizeAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnInStasisMapInit(Entity<CMInStasisComponent> ent, ref MapInitEvent args)
  {
    this._parasite.RefreshIncubationMultipliers((Entity<VictimInfectedComponent>) ent.Owner);
  }

  private void OnInStasisRemove(Entity<CMInStasisComponent> ent, ref ComponentRemove args)
  {
    this._parasite.RefreshIncubationMultipliers((Entity<VictimInfectedComponent>) ent.Owner);
  }

  private void OnInStasisGetInfectedIncubationMultiplier(
    Entity<CMInStasisComponent> ent,
    ref GetInfectedIncubationMultiplierEvent args)
  {
    if (!ent.Comp.Running)
      return;
    float incubationMultiplier = ent.Comp.IncubationMultiplier;
    if (args.stage >= ent.Comp.LessEffectiveStage)
      incubationMultiplier += incubationMultiplier / 3f;
    args.Multiply(incubationMultiplier);
  }

  private void OnInStasisBleedAttempt(Entity<CMInStasisComponent> ent, ref CMBleedAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnInsert(Entity<CMStasisBagComponent> bag, EntityUid target)
  {
    this.EnsureComp<CMInStasisComponent>(target);
  }

  private void OnRemove(Entity<CMStasisBagComponent> bag, EntityUid target)
  {
    this.RemCompDeferred<CMInStasisComponent>(target);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CMStasisBagComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CMStasisBagComponent>();
    EntityUid uid;
    CMStasisBagComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      BaseContainer container;
      if (this._container.TryGetContainer(uid, "entity_storage", out container) && container.ContainedEntities.Count > 0)
      {
        bool flag = false;
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
        {
          if (this._mobstate.IsDead(containedEntity))
          {
            this._entStorage.OpenStorage(uid);
            this._popup.PopupEntity(this.Loc.GetString("rmc-stasis-reject-dead"), uid, PopupType.SmallCaution);
          }
          else if (this.HasComp<CMInStasisComponent>(containedEntity))
            flag = true;
        }
        if (flag)
        {
          comp1.StasisLeft -= TimeSpan.FromSeconds((double) frameTime);
          if (comp1.StasisLeft <= TimeSpan.Zero)
          {
            this._entStorage.EmptyContents(uid);
            this.SpawnAtPosition((string) comp1.UsedBag, uid.ToCoordinates());
            this.QueueDel(new EntityUid?(uid));
          }
        }
      }
    }
  }

  public bool CanBodyMetabolize(EntityUid body)
  {
    CMMetabolizeAttemptEvent args = new CMMetabolizeAttemptEvent();
    this.RaiseLocalEvent<CMMetabolizeAttemptEvent>(body, ref args);
    return !args.Cancelled;
  }

  public bool CanOrganMetabolize(Entity<OrganComponent?> organ)
  {
    if (this._organQuery.Resolve((EntityUid) organ, ref organ.Comp, false))
    {
      EntityUid? body = organ.Comp.Body;
      if (body.HasValue)
      {
        EntityUid valueOrDefault = body.GetValueOrDefault();
        CMMetabolizeAttemptEvent args = new CMMetabolizeAttemptEvent();
        this.RaiseLocalEvent<CMMetabolizeAttemptEvent>(valueOrDefault, ref args);
        return !args.Cancelled;
      }
    }
    return true;
  }
}
