// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Suicide.RMCSuicideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Suicide;

public sealed class RMCSuicideSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _admin;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCSuicideComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<RMCSuicideComponent, GetVerbsEvent<Verb>>(this.OnSuicideGetVerbs));
    this.SubscribeLocalEvent<RMCSuicideComponent, RMCSuicideDoAfterEvent>(new EntityEventRefHandler<RMCSuicideComponent, RMCSuicideDoAfterEvent>(this.OnSuicideDoAfter));
    this.SubscribeLocalEvent<RMCHasSuicidedComponent, UpdateMobStateEvent>(new EntityEventRefHandler<RMCHasSuicidedComponent, UpdateMobStateEvent>(this.OnHasSuicidedUpdateMobState));
  }

  private void OnSuicideGetVerbs(Entity<RMCSuicideComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    if (!args.CanInteract)
      return;
    EntityUid user = args.User;
    EntityUid? uid;
    if (user != args.Target || args.Hands == null || !this._hands.TryGetActiveItem((Entity<HandsComponent>) args.Target, out uid) || !this.HasComp<GunComponent>(uid))
      return;
    args.Verbs.Add(new Verb()
    {
      Text = this.Loc.GetString("rmc-suicide"),
      Act = (Action) (() =>
      {
        TimeSpan curTime = this._timing.CurTime;
        if (curTime < ent.Comp.LastAttempt + ent.Comp.Cooldown)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-suicide-fumble-self"), user, new EntityUid?(user), PopupType.SmallCaution);
        }
        else
        {
          ent.Comp.LastAttempt = curTime;
          if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.Delay, (DoAfterEvent) new RMCSuicideDoAfterEvent(), new EntityUid?(user))
          {
            BreakOnMove = true,
            NeedHand = true,
            BreakOnHandChange = true,
            ForceVisible = true
          }))
            return;
          ISharedAdminLogManager admin = this._admin;
          LogStringHandler logStringHandler = new LogStringHandler(20, 1);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" started to suicide.");
          ref LogStringHandler local = ref logStringHandler;
          admin.Add(LogType.RMCSuicide, LogImpact.High, ref local);
          this._popup.PopupPredicted(this.Loc.GetString("rmc-suicide-start-self"), this.Loc.GetString("rmc-suicide-start-others", ("user", (object) user)), user, new EntityUid?(user), PopupType.LargeCaution);
        }
      })
    });
  }

  private void OnSuicideDoAfter(Entity<RMCSuicideComponent> ent, ref RMCSuicideDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled)
    {
      ISharedAdminLogManager admin = this._admin;
      LogStringHandler logStringHandler = new LogStringHandler(25, 1);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral("'s suicide was cancelled.");
      ref LogStringHandler local = ref logStringHandler;
      admin.Add(LogType.RMCSuicide, LogImpact.High, ref local);
      this._popup.PopupPredicted(this.Loc.GetString("rmc-suicide-cancel-self"), this.Loc.GetString("rmc-suicide-cancel-others", ("user", (object) user)), user, new EntityUid?(user), PopupType.MediumCaution);
    }
    else
    {
      if (args.Handled)
        return;
      args.Handled = true;
      EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) user);
      if (activeItem.HasValue)
      {
        EntityUid valueOrDefault = activeItem.GetValueOrDefault();
        GunComponent comp;
        if (this.TryComp<GunComponent>(valueOrDefault, out comp))
        {
          TakeAmmoEvent args1 = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), this.Transform(user).Coordinates, new EntityUid?(user));
          this.RaiseLocalEvent<TakeAmmoEvent>(valueOrDefault, args1);
          if (args1.Ammo.Count == 0)
          {
            ISharedAdminLogManager admin = this._admin;
            LogStringHandler logStringHandler = new LogStringHandler(28, 1);
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
            logStringHandler.AppendLiteral(" failed to suicide: no ammo.");
            ref LogStringHandler local = ref logStringHandler;
            admin.Add(LogType.RMCSuicide, LogImpact.High, ref local);
            this._audio.PlayPredicted(comp.SoundEmpty, valueOrDefault, new EntityUid?((EntityUid) ent));
            return;
          }
          foreach ((EntityUid? Entity, IShootable Shootable) tuple in args1.Ammo)
            this.QueueDel(tuple.Entity);
          ISharedAdminLogManager admin1 = this._admin;
          LogStringHandler logStringHandler1 = new LogStringHandler(10, 1);
          logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
          logStringHandler1.AppendLiteral(" suicided.");
          ref LogStringHandler local1 = ref logStringHandler1;
          admin1.Add(LogType.RMCSuicide, LogImpact.High, ref local1);
          this._damageable.TryChangeDamage(new EntityUid?(user), ent.Comp.Damage, true);
          this._mobState.ChangeMobState(user, MobState.Dead);
          this._unrevivable.MakeUnrevivable((Entity<RMCRevivableComponent>) user);
          this._audio.PlayPredicted(comp.SoundGunshot, valueOrDefault, new EntityUid?((EntityUid) ent));
          this._unrevivable.MakeUnrevivable((Entity<RMCRevivableComponent>) user, false);
          this.EnsureComp<RMCHasSuicidedComponent>(user);
          return;
        }
      }
      ISharedAdminLogManager admin2 = this._admin;
      LogStringHandler logStringHandler2 = new LogStringHandler(27, 1);
      logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler2.AppendLiteral(" failed to suicide: no gun.");
      ref LogStringHandler local2 = ref logStringHandler2;
      admin2.Add(LogType.RMCSuicide, LogImpact.High, ref local2);
    }
  }

  private void OnHasSuicidedUpdateMobState(
    Entity<RMCHasSuicidedComponent> ent,
    ref UpdateMobStateEvent args)
  {
    args.State = MobState.Dead;
  }
}
