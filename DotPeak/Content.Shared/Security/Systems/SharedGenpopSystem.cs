// Decompiled with JetBrains decompiler
// Type: Content.Shared.Security.Systems.SharedGenpopSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Security.Components;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Security.Systems;

public abstract class SharedGenpopSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _cfgManager;
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  protected SharedIdCardSystem IdCard;
  [Dependency]
  private LockSystem _lock;
  [Dependency]
  protected MetaDataSystem MetaDataSystem;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedUserInterfaceSystem _userInterface;
  private int _maxIdJobLength;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GenpopLockerComponent, GenpopLockerIdConfiguredMessage>(new EntityEventRefHandler<GenpopLockerComponent, GenpopLockerIdConfiguredMessage>(this.OnIdConfigured));
    this.SubscribeLocalEvent<GenpopLockerComponent, StorageCloseAttemptEvent>(new EntityEventRefHandler<GenpopLockerComponent, StorageCloseAttemptEvent>(this.OnCloseAttempt));
    this.SubscribeLocalEvent<GenpopLockerComponent, LockToggleAttemptEvent>(new EntityEventRefHandler<GenpopLockerComponent, LockToggleAttemptEvent>(this.OnLockToggleAttempt));
    this.SubscribeLocalEvent<GenpopLockerComponent, LockToggledEvent>(new EntityEventRefHandler<GenpopLockerComponent, LockToggledEvent>(this.OnLockToggled));
    this.SubscribeLocalEvent<GenpopLockerComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<GenpopLockerComponent, GetVerbsEvent<Verb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<GenpopIdCardComponent, ExaminedEvent>(new EntityEventRefHandler<GenpopIdCardComponent, ExaminedEvent>(this.OnExamine));
    this.Subs.CVar<int>(this._cfgManager, CCVars.MaxIdJobLength, (Action<int>) (value => this._maxIdJobLength = value), true);
  }

  private void OnIdConfigured(
    Entity<GenpopLockerComponent> ent,
    ref GenpopLockerIdConfiguredMessage args)
  {
    if (string.IsNullOrWhiteSpace(args.Name) || args.Name.Length > this._maxIdJobLength || (double) args.Sentence < 0.0 || string.IsNullOrWhiteSpace(args.Crime) || args.Crime.Length > 48 /*0x30*/ || !this._accessReader.IsAllowed(args.Actor, (EntityUid) ent))
      return;
    ent.Comp.LinkedId = new EntityUid?(EntityUid.Invalid);
    this._lock.Lock(ent.Owner, new EntityUid?());
    this._entityStorage.CloseStorage((EntityUid) ent);
    this.CreateId(ent, args.Name, args.Sentence, args.Crime);
  }

  private void OnCloseAttempt(Entity<GenpopLockerComponent> ent, ref StorageCloseAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    if (!ent.Comp.LinkedId.HasValue)
      args.Cancelled = true;
    EntityUid? user = args.User;
    if (!user.HasValue)
      return;
    EntityUid valueOrDefault = user.GetValueOrDefault();
    if (!this._accessReader.IsAllowed(valueOrDefault, (EntityUid) ent))
      this._popup.PopupClient(this.Loc.GetString("lock-comp-has-user-access-fail"), new EntityUid?(valueOrDefault));
    else
      this._userInterface.TryOpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) GenpopLockerUiKey.Key, valueOrDefault);
  }

  private void OnLockToggleAttempt(
    Entity<GenpopLockerComponent> ent,
    ref LockToggleAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    if (!ent.Comp.LinkedId.HasValue)
      args.Cancelled = true;
    else if (!this._accessReader.FindPotentialAccessItems(args.User).Contains(ent.Comp.LinkedId.Value))
    {
      if (!args.Silent)
        this._popup.PopupClient(this.Loc.GetString("lock-comp-has-user-access-fail"), (EntityUid) ent, new EntityUid?(args.User));
      args.Cancelled = true;
    }
    else
    {
      ExpireIdCardComponent comp;
      if (this.TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId.Value, out comp) && comp.Expired)
        return;
      if (!args.Silent)
        this._popup.PopupClient(this.Loc.GetString("genpop-prisoner-id-popup-not-served"), (EntityUid) ent, new EntityUid?(args.User));
      args.Cancelled = true;
    }
  }

  private void OnLockToggled(Entity<GenpopLockerComponent> ent, ref LockToggledEvent args)
  {
    if (args.Locked)
      return;
    this.CancelIdCard(ent);
  }

  private void OnGetVerbs(Entity<GenpopLockerComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    ExpireIdCardComponent expire;
    GenpopIdCardComponent genpopId;
    if (!ent.Comp.LinkedId.HasValue || !args.CanAccess || !args.CanComplexInteract || !args.CanInteract || !this.TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId, out expire) || !this.TryComp<GenpopIdCardComponent>(ent.Comp.LinkedId, out genpopId))
      return;
    EntityUid user = args.User;
    bool flag = this._accessReader.IsAllowed(args.User, (EntityUid) ent);
    args.Verbs.Add(new Verb()
    {
      Act = (Action) (() => this.IdCard.ExpireId((Entity<ExpireIdCardComponent>) (ent.Comp.LinkedId.Value, expire))),
      Priority = 13,
      Text = this.Loc.GetString("genpop-locker-action-end-early"),
      Impact = LogImpact.Medium,
      DoContactInteraction = new bool?(true),
      Disabled = !flag
    });
    args.Verbs.Add(new Verb()
    {
      Act = (Action) (() => this.CancelIdCard(ent, new EntityUid?(user))),
      Priority = 12,
      Text = this.Loc.GetString("genpop-locker-action-clear-id"),
      Impact = LogImpact.Medium,
      DoContactInteraction = new bool?(true),
      Disabled = !flag
    });
    double num = 1.0 - (expire.ExpireTime - this.Timing.CurTime).TotalSeconds / genpopId.SentenceDuration.TotalSeconds;
    if (expire.Expired)
      return;
    args.Verbs.Add(new Verb()
    {
      Act = (Action) (() => this.IdCard.SetExpireTime((Entity<ExpireIdCardComponent>) (ent.Comp.LinkedId.Value, expire), this.Timing.CurTime + genpopId.SentenceDuration)),
      Priority = 11,
      Text = this.Loc.GetString("genpop-locker-action-reset-sentence", ("percent", (object) (Math.Clamp(num, 0.0, 1.0) * 100.0))),
      Impact = LogImpact.Medium,
      DoContactInteraction = new bool?(true),
      Disabled = !flag
    });
  }

  private void CancelIdCard(Entity<GenpopLockerComponent> ent, EntityUid? user = null)
  {
    if (!ent.Comp.LinkedId.HasValue)
      return;
    MetaDataComponent metadata = this.MetaData((EntityUid) ent);
    this.MetaDataSystem.SetEntityName((EntityUid) ent, this.Loc.GetString("genpop-locker-name-default"), metadata);
    this.MetaDataSystem.SetEntityDescription((EntityUid) ent, this.Loc.GetString("genpop-locker-desc-default"), metadata);
    ent.Comp.LinkedId = new EntityUid?();
    this._lock.Unlock(ent.Owner, user);
    this._entityStorage.OpenStorage(ent.Owner);
    ExpireIdCardComponent comp;
    if (this.TryComp<ExpireIdCardComponent>(ent.Comp.LinkedId, out comp))
      this.IdCard.ExpireId((Entity<ExpireIdCardComponent>) (ent.Comp.LinkedId.Value, comp));
    this.Dirty<GenpopLockerComponent>(ent);
  }

  private void OnExamine(Entity<GenpopIdCardComponent> ent, ref ExaminedEvent args)
  {
    ExpireIdCardComponent comp;
    if (!this.TryComp<ExpireIdCardComponent>((EntityUid) ent, out comp))
      return;
    if (comp.Permanent)
      args.PushText(this.Loc.GetString("genpop-prisoner-id-examine-wait-perm", ("crime", (object) ent.Comp.Crime)));
    else if (comp.Expired)
    {
      args.PushText(this.Loc.GetString("genpop-prisoner-id-examine-served", ("crime", (object) ent.Comp.Crime)));
    }
    else
    {
      TimeSpan sentenceDuration = ent.Comp.SentenceDuration;
      TimeSpan timeSpan = ent.Comp.SentenceDuration - (comp.ExpireTime - this.Timing.CurTime);
      args.PushText(this.Loc.GetString("genpop-prisoner-id-examine-wait", ("minutes", (object) timeSpan.Minutes), ("seconds", (object) timeSpan.Seconds), ("sentence", (object) sentenceDuration.TotalMinutes), ("crime", (object) ent.Comp.Crime)));
    }
  }

  protected virtual void CreateId(
    Entity<GenpopLockerComponent> ent,
    string name,
    float sentence,
    string crime)
  {
  }
}
