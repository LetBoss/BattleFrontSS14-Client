// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.SharpSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Kitchen.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Kitchen;

public sealed class SharpSystem : EntitySystem
{
  [Dependency]
  private SharedBodySystem _bodySystem;
  [Dependency]
  private SharedDestructibleSystem _destructibleSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private MobStateSystem _mobStateSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IRobustRandom _robustRandom;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SharpComponent, AfterInteractEvent>(new ComponentEventHandler<SharpComponent, AfterInteractEvent>(this.OnAfterInteract), new Type[1]
    {
      typeof (UtensilSystem)
    });
    this.SubscribeLocalEvent<SharpComponent, SharpDoAfterEvent>(new ComponentEventHandler<SharpComponent, SharpDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<ButcherableComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ButcherableComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs));
  }

  private void OnAfterInteract(EntityUid uid, SharpComponent component, AfterInteractEvent args)
  {
    if (args.Handled || !args.Target.HasValue || !args.CanReach || !this.TryStartButcherDoafter(uid, args.Target.Value, args.User))
      return;
    args.Handled = true;
  }

  private bool TryStartButcherDoafter(EntityUid knife, EntityUid target, EntityUid user)
  {
    ButcherableComponent comp1;
    SharpComponent comp2;
    MobStateComponent comp3;
    if (!this.TryComp<ButcherableComponent>(target, out comp1) || !this.TryComp<SharpComponent>(knife, out comp2) || this.TryComp<MobStateComponent>(target, out comp3) && !this._mobStateSystem.IsDead(target, comp3))
      return false;
    if (comp1.Type != ButcheringType.Knife && target != user)
    {
      this._popupSystem.PopupEntity(this.Loc.GetString("butcherable-different-tool", (nameof (target), (object) target)), knife, user);
      return false;
    }
    if (!comp2.Butchering.Add(target))
      return false;
    bool flag = user != knife;
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, comp2.ButcherDelayModifier * comp1.ButcherDelay, (DoAfterEvent) new SharpDoAfterEvent(), new EntityUid?(knife), new EntityUid?(target), new EntityUid?(knife))
    {
      BreakOnDamage = true,
      BreakOnMove = true,
      NeedHand = flag
    });
    return true;
  }

  private void OnDoAfter(EntityUid uid, SharpComponent component, DoAfterEvent args)
  {
    ButcherableComponent comp1;
    if (args.Handled || !this.TryComp<ButcherableComponent>(args.Args.Target, out comp1))
      return;
    if (args.Cancelled)
    {
      component.Butchering.Remove(args.Args.Target.Value);
    }
    else
    {
      component.Butchering.Remove(args.Args.Target.Value);
      if (this._containerSystem.IsEntityInContainer(args.Args.Target.Value))
      {
        args.Handled = true;
      }
      else
      {
        if (this._net.IsClient)
          return;
        List<string> spawns = EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) comp1.SpawnedEntities, this._robustRandom);
        MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(args.Args.Target.Value);
        EntityUid uid1 = new EntityUid();
        foreach (string prototype in spawns)
          uid1 = this.Spawn(prototype, mapCoordinates.Offset(this._robustRandom.NextVector2(0.25f)), rotation: new Angle());
        BodyComponent comp2;
        int num = this.TryComp<BodyComponent>(args.Args.Target.Value, out comp2) ? 1 : 0;
        PopupType type = PopupType.Small;
        if (num != 0)
          type = PopupType.LargeCaution;
        this._popupSystem.PopupEntity(this.Loc.GetString("butcherable-knife-butchered-success", ("target", (object) args.Args.Target.Value), ("knife", (object) Identity.Entity(uid, (IEntityManager) this.EntityManager))), uid1, args.Args.User, type);
        if (num != 0)
          this._bodySystem.GibBody(args.Args.Target.Value, body: comp2, splatCone: new Angle());
        this._destructibleSystem.DestroyEntity(args.Args.Target.Value);
        args.Handled = true;
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(21, 3);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
        logStringHandler.AppendLiteral(" ");
        logStringHandler.AppendLiteral("has butchered ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
        logStringHandler.AppendLiteral(" ");
        logStringHandler.AppendLiteral("with ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(args.Used), "knife", "ToPrettyString(args.Used)");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Gib, ref local);
      }
    }
  }

  private void OnGetInteractionVerbs(
    EntityUid uid,
    ButcherableComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    SharpComponent comp1;
    if (component.Type != ButcheringType.Knife || !args.CanAccess || !args.CanInteract || !this.TryComp<SharpComponent>(args.User, out comp1) && args.Hands == null)
      return;
    bool disabled = false;
    string str = (string) null;
    SharpComponent comp2;
    if (!this.TryComp<SharpComponent>(args.Using, out comp2) && comp1 == null)
    {
      disabled = true;
      str = this.Loc.GetString("butcherable-need-knife", ("target", (object) uid));
    }
    else if (this._containerSystem.IsEntityInContainer(uid))
    {
      disabled = true;
      str = this.Loc.GetString("butcherable-not-in-container", ("target", (object) uid));
    }
    else
    {
      MobStateComponent comp3;
      if (this.TryComp<MobStateComponent>(uid, out comp3) && !this._mobStateSystem.IsDead(uid, comp3))
      {
        disabled = true;
        str = this.Loc.GetString("butcherable-mob-isnt-dead");
      }
    }
    EntityUid sharpObject = new EntityUid();
    if (comp2 != null)
      sharpObject = args.Using.Value;
    else if (comp1 != null)
      sharpObject = args.User;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Act = (Action) (() =>
    {
      if (disabled)
        return;
      this.TryStartButcherDoafter(sharpObject, args.Target, args.User);
    });
    interactionVerb1.Message = str;
    interactionVerb1.Disabled = disabled;
    interactionVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png"));
    interactionVerb1.Text = this.Loc.GetString("butcherable-verb-name");
    InteractionVerb interactionVerb2 = interactionVerb1;
    args.Verbs.Add(interactionVerb2);
  }
}
