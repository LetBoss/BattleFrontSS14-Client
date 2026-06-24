// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sticky.Systems.StickySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Sticky.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Components;
using System;

#nullable enable
namespace Content.Shared.Sticky.Systems;

public sealed class StickySystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedPopupSystem _popup;
  private const string StickerSlotId = "stickers_container";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StickyComponent, AfterInteractEvent>(new EntityEventRefHandler<StickyComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<StickyComponent, StickyDoAfterEvent>(new EntityEventRefHandler<StickyComponent, StickyDoAfterEvent>(this.OnStickyDoAfter));
    this.SubscribeLocalEvent<StickyComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<StickyComponent, GetVerbsEvent<Verb>>(this.OnGetVerbs));
  }

  private void OnAfterInteract(Entity<StickyComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = this.StartSticking(ent, valueOrDefault, args.User);
  }

  private void OnGetVerbs(Entity<StickyComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    (EntityUid entityUid1, StickyComponent comp) = ent;
    if (!comp.StuckTo.HasValue || !comp.CanUnstick || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    if (!this._interaction.InRangeUnobstructed((Entity<TransformComponent>) entityUid1, (Entity<TransformComponent>) user, predicate: (SharedInteractionSystem.Ignored) (entity =>
    {
      EntityUid? stuckTo = comp.StuckTo;
      EntityUid entityUid2 = entity;
      return stuckTo.HasValue && stuckTo.GetValueOrDefault() == entityUid2;
    })))
      return;
    args.Verbs.Add(new Verb()
    {
      DoContactInteraction = new bool?(true),
      Text = this.Loc.GetString((string) comp.VerbText),
      Icon = comp.VerbIcon,
      Act = (Action) (() => this.StartUnsticking(ent, user))
    });
  }

  private bool StartSticking(Entity<StickyComponent> ent, EntityUid target, EntityUid user)
  {
    (EntityUid entityUid, StickyComponent comp) = ent;
    if (this._whitelist.IsWhitelistFail(comp.Whitelist, target) || this._whitelist.IsBlacklistPass(comp.Blacklist, target))
      return false;
    AttemptEntityStickEvent args = new AttemptEntityStickEvent(target, user);
    this.RaiseLocalEvent<AttemptEntityStickEvent>(entityUid, ref args);
    if (args.Cancelled)
      return false;
    if (comp.StickDelay <= TimeSpan.Zero)
    {
      this.StickToEntity(ent, target, user);
      return true;
    }
    if (comp.StickPopupStart.HasValue)
    {
      ILocalizationManager loc = this.Loc;
      LocId? stickPopupStart = comp.StickPopupStart;
      string valueOrDefault = stickPopupStart.HasValue ? (string) stickPopupStart.GetValueOrDefault() : (string) null;
      this._popup.PopupClient(loc.GetString(valueOrDefault), user, new EntityUid?(user));
    }
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, comp.StickDelay, (DoAfterEvent) new StickyDoAfterEvent(), new EntityUid?(entityUid), new EntityUid?(target), new EntityUid?(entityUid))
    {
      BreakOnMove = true,
      NeedHand = true,
      ForceVisible = true
    });
    return true;
  }

  private void OnStickyDoAfter(Entity<StickyComponent> ent, ref StickyDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    EntityUid user = args.User;
    if (!ent.Comp.StuckTo.HasValue)
      this.StickToEntity(ent, valueOrDefault, user);
    else
      this.UnstickFromEntity(ent, user);
    args.Handled = true;
  }

  private void StartUnsticking(Entity<StickyComponent> ent, EntityUid user)
  {
    (EntityUid entityUid, StickyComponent comp) = ent;
    EntityUid? stuckTo = comp.StuckTo;
    if (!stuckTo.HasValue)
      return;
    AttemptEntityUnstickEvent args = new AttemptEntityUnstickEvent(stuckTo.GetValueOrDefault(), user);
    this.RaiseLocalEvent<AttemptEntityUnstickEvent>(entityUid, ref args);
    if (args.Cancelled)
      return;
    if (comp.UnstickDelay <= TimeSpan.Zero)
    {
      this.UnstickFromEntity(ent, user);
    }
    else
    {
      if (comp.UnstickPopupStart.HasValue)
      {
        ILocalizationManager loc = this.Loc;
        LocId? unstickPopupStart = comp.UnstickPopupStart;
        string valueOrDefault = unstickPopupStart.HasValue ? (string) unstickPopupStart.GetValueOrDefault() : (string) null;
        this._popup.PopupClient(loc.GetString(valueOrDefault), user, new EntityUid?(user));
      }
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, comp.UnstickDelay, (DoAfterEvent) new StickyDoAfterEvent(), new EntityUid?(entityUid), new EntityUid?(entityUid))
      {
        BreakOnMove = true,
        NeedHand = true
      });
    }
  }

  public void StickToEntity(Entity<StickyComponent> ent, EntityUid target, EntityUid user)
  {
    (EntityUid entityUid, StickyComponent comp) = ent;
    AttemptEntityStickEvent args1 = new AttemptEntityStickEvent(target, user);
    this.RaiseLocalEvent<AttemptEntityStickEvent>(entityUid, ref args1);
    if (args1.Cancelled)
      return;
    Container container = this._container.EnsureContainer<Container>(target, "stickers_container");
    container.ShowContents = true;
    if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entityUid, (BaseContainer) container))
      return;
    if (comp.StickPopupSuccess.HasValue)
    {
      ILocalizationManager loc = this.Loc;
      LocId? stickPopupSuccess = comp.StickPopupSuccess;
      string valueOrDefault = stickPopupSuccess.HasValue ? (string) stickPopupSuccess.GetValueOrDefault() : (string) null;
      this._popup.PopupClient(loc.GetString(valueOrDefault), user, new EntityUid?(user));
    }
    this._appearance.SetData(entityUid, (Enum) StickyVisuals.IsStuck, (object) true);
    comp.StuckTo = new EntityUid?(target);
    this.Dirty(entityUid, (IComponent) comp);
    EntityStuckEvent args2 = new EntityStuckEvent(target, user);
    this.RaiseLocalEvent<EntityStuckEvent>(entityUid, ref args2);
  }

  public void UnstickFromEntity(Entity<StickyComponent> ent, EntityUid user)
  {
    (EntityUid entityUid, StickyComponent comp) = ent;
    EntityUid? stuckTo = comp.StuckTo;
    if (!stuckTo.HasValue)
      return;
    EntityUid valueOrDefault1 = stuckTo.GetValueOrDefault();
    AttemptEntityUnstickEvent args1 = new AttemptEntityUnstickEvent(valueOrDefault1, user);
    this.RaiseLocalEvent<AttemptEntityUnstickEvent>(entityUid, ref args1);
    BaseContainer container;
    if (args1.Cancelled || !this._container.TryGetContainer(valueOrDefault1, "stickers_container", out container) || !this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, container))
      return;
    if (container.ContainedEntities.Count == 0)
      this._container.ShutdownContainer(container);
    this._hands.PickupOrDrop(new EntityUid?(user), entityUid);
    this._appearance.SetData(entityUid, (Enum) StickyVisuals.IsStuck, (object) false);
    if (comp.UnstickPopupSuccess.HasValue)
    {
      ILocalizationManager loc = this.Loc;
      LocId? unstickPopupSuccess = comp.UnstickPopupSuccess;
      string valueOrDefault2 = unstickPopupSuccess.HasValue ? (string) unstickPopupSuccess.GetValueOrDefault() : (string) null;
      this._popup.PopupClient(loc.GetString(valueOrDefault2), user, new EntityUid?(user));
    }
    comp.StuckTo = new EntityUid?();
    this.Dirty(entityUid, (IComponent) comp);
    EntityUnstuckEvent args2 = new EntityUnstuckEvent(valueOrDefault1, user);
    this.RaiseLocalEvent<EntityUnstuckEvent>(entityUid, ref args2);
  }
}
