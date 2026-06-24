// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.SharedDrinkSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public abstract class SharedDrinkSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private FlavorProfileSystem _flavorProfile;
  [Dependency]
  private FoodSystem _food;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private OpenableSystem _openable;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DrinkComponent, AttemptShakeEvent>(new EntityEventRefHandler<DrinkComponent, AttemptShakeEvent>(this.OnAttemptShake));
    this.SubscribeLocalEvent<DrinkComponent, ExaminedEvent>(new EntityEventRefHandler<DrinkComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<DrinkComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DrinkComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDrinkVerb));
  }

  protected void OnAttemptShake(Entity<DrinkComponent> entity, ref AttemptShakeEvent args)
  {
    if (!this.IsEmpty((EntityUid) entity, entity.Comp))
      return;
    args.Cancelled = true;
  }

  protected void OnExamined(Entity<DrinkComponent> entity, ref ExaminedEvent args)
  {
    OpenableComponent comp;
    this.TryComp<OpenableComponent>((EntityUid) entity, out comp);
    if (this._openable.IsClosed(entity.Owner, comp: comp, predicted: true) || !args.IsInDetailsRange || !entity.Comp.Examinable)
      return;
    if (this.IsEmpty((EntityUid) entity, entity.Comp))
      args.PushMarkup(this.Loc.GetString("drink-component-on-examine-is-empty"));
    else if (this.HasComp<ExaminableSolutionComponent>((EntityUid) entity))
    {
      args.PushText(this.Loc.GetString("drink-component-on-examine-exact-volume", ("amount", (object) this.DrinkVolume((EntityUid) entity, entity.Comp))));
    }
    else
    {
      int num = (int) this._solutionContainer.PercentFull((EntityUid) entity);
      string messageId = num <= 66 ? (num > 33 ? this.HalfEmptyOrHalfFull(args) : "drink-component-on-examine-is-mostly-empty") : (num == 100 ? "drink-component-on-examine-is-full" : "drink-component-on-examine-is-mostly-full");
      args.PushMarkup(this.Loc.GetString(messageId));
    }
  }

  private void AddDrinkVerb(Entity<DrinkComponent> entity, ref GetVerbsEvent<AlternativeVerb> ev)
  {
    BodyComponent comp;
    if (entity.Owner == ev.User || !ev.CanInteract || !ev.CanAccess || !this.TryComp<BodyComponent>(ev.User, out comp) || !this._body.TryGetBodyOrganEntityComps<StomachComponent>((Entity<BodyComponent>) (ev.User, comp), out List<Entity<StomachComponent, OrganComponent>> _) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) entity.Owner, entity.Comp.Solution, out Entity<SolutionComponent>? _) || this._mobState.IsAlive((EntityUid) entity))
      return;
    EntityUid user = ev.User;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryDrink(user, user, entity.Comp, (EntityUid) entity));
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/drink.svg.192dpi.png"));
    alternativeVerb1.Text = this.Loc.GetString("drink-system-verb-drink");
    alternativeVerb1.Priority = 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    ev.Verbs.Add(alternativeVerb2);
  }

  protected FixedPoint2 DrinkVolume(EntityUid uid, DrinkComponent? component = null)
  {
    Solution solution;
    return !this.Resolve<DrinkComponent>(uid, ref component) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) uid, component.Solution, out Entity<SolutionComponent>? _, out solution) ? FixedPoint2.Zero : solution.Volume;
  }

  protected bool IsEmpty(EntityUid uid, DrinkComponent? component = null)
  {
    return !this.Resolve<DrinkComponent>(uid, ref component) || this.DrinkVolume(uid, component) <= 0;
  }

  private string HalfEmptyOrHalfFull(ExaminedEvent args)
  {
    string str = "drink-component-on-examine-is-half-full";
    MetaDataComponent comp;
    if (this.TryComp(args.Examiner, out comp) && comp.EntityName.Length > 0 && string.Compare(comp.EntityName.Substring(0, 1), "m", StringComparison.InvariantCultureIgnoreCase) > 0)
      str = "drink-component-on-examine-is-half-empty";
    return str;
  }

  protected bool TryDrink(EntityUid user, EntityUid target, DrinkComponent drink, EntityUid item)
  {
    if (!this.HasComp<BodyComponent>(target) || !this._body.TryGetBodyOrganEntityComps<StomachComponent>((Entity<BodyComponent>) target, out List<Entity<StomachComponent, OrganComponent>> _))
      return false;
    if (this._openable.IsClosed(item, new EntityUid?(user), predicted: true))
      return true;
    Solution solution;
    if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) item, drink.Solution, out Entity<SolutionComponent>? _, out solution) || solution.Volume <= 0)
    {
      if (drink.IgnoreEmpty)
        return false;
      this._popup.PopupClient(this.Loc.GetString("drink-component-try-use-drink-is-empty", ("entity", (object) item)), item, new EntityUid?(user));
      return true;
    }
    if (this._food.IsMouthBlocked(target, new EntityUid?(user)) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) item, popup: true))
      return true;
    bool flag = user != target;
    if (flag)
    {
      this._popup.PopupEntity(this.Loc.GetString("drink-component-force-feed", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager))), user, target);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(23, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is forcing ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" to drink ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), nameof (drink), "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution));
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.ForceFeed, LogImpact.High, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(14, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" is drinking ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), nameof (drink), "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution));
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Ingestion, LogImpact.Low, ref local);
    }
    string localizedFlavorsMessage = this._flavorProfile.GetLocalizedFlavorsMessage(user, solution);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, flag ? drink.ForceFeedDelay : drink.Delay, (DoAfterEvent) new ConsumeDoAfterEvent(drink.Solution, localizedFlavorsMessage), new EntityUid?(item), new EntityUid?(target), new EntityUid?(item))
    {
      BreakOnHandChange = false,
      BreakOnMove = flag,
      BreakOnDamage = true,
      MovementThreshold = 0.01f,
      DistanceThreshold = new float?(1f),
      NeedHand = flag || this._hands.IsHolding((Entity<HandsComponent>) user, new EntityUid?(item))
    });
    return true;
  }
}
