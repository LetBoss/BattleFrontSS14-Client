// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.FoodSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FoodSystem : EntitySystem
{
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private FlavorProfileSystem _flavorProfile;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private OpenableSystem _openable;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private ReactiveSystem _reaction;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private StomachSystem _stomach;
  [Dependency]
  private UtensilSystem _utensil;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  public const float MaxFeedDistance = 1.5f;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FoodComponent, UseInHandEvent>(new EntityEventRefHandler<FoodComponent, UseInHandEvent>(this.OnUseFoodInHand), after: new Type[2]
    {
      typeof (OpenableSystem),
      typeof (InventorySystem)
    });
    this.SubscribeLocalEvent<FoodComponent, AfterInteractEvent>(new EntityEventRefHandler<FoodComponent, AfterInteractEvent>(this.OnFeedFood));
    this.SubscribeLocalEvent<FoodComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<FoodComponent, GetVerbsEvent<AlternativeVerb>>(this.AddEatVerb));
    this.SubscribeLocalEvent<FoodComponent, ConsumeDoAfterEvent>(new EntityEventRefHandler<FoodComponent, ConsumeDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<InventoryComponent, IngestionAttemptEvent>(new EntityEventRefHandler<InventoryComponent, IngestionAttemptEvent>(this.OnInventoryIngestAttempt));
  }

  private void OnUseFoodInHand(Entity<FoodComponent> entity, ref UseInHandEvent ev)
  {
    if (ev.Handled)
      return;
    (_, ev.Handled) = this.TryFeed(ev.User, ev.User, (EntityUid) entity, entity.Comp);
  }

  private void OnFeedFood(Entity<FoodComponent> entity, ref AfterInteractEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue || !args.CanReach)
      return;
    EntityUid user = args.User;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    EntityUid food = (EntityUid) entity;
    FoodComponent comp = entity.Comp;
    (_, args.Handled) = this.TryFeed(user, target2, food, comp);
  }

  public (bool Success, bool Handled) TryFeed(
    EntityUid user,
    EntityUid target,
    EntityUid food,
    FoodComponent foodComp)
  {
    if (food == user || this._mobState.IsAlive(food) && foodComp.RequireDead)
      return (false, false);
    BodyComponent comp1;
    if (!this.TryComp<BodyComponent>(target, out comp1))
      return (false, false);
    if (this.HasComp<UnremoveableComponent>(food))
      return (false, false);
    if (this._openable.IsClosed(food, new EntityUid?(user), predicted: true))
      return (false, true);
    Solution solution;
    if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) food, foodComp.Solution, out Entity<SolutionComponent>? _, out solution))
      return (false, false);
    List<Entity<StomachComponent, OrganComponent>> comps;
    if (!this._body.TryGetBodyOrganEntityComps<StomachComponent>((Entity<BodyComponent>) (target, comp1), out comps))
      return (false, false);
    if (!this.IsDigestibleBy(food, foodComp, comps))
      return (false, false);
    if (!this.TryGetRequiredUtensils(user, foodComp, out List<EntityUid> _))
      return (false, false);
    StorageComponent comp2;
    if (this.TryComp<StorageComponent>(food, out comp2) && comp2.Container.ContainedEntities.Any<EntityUid>())
    {
      this._popup.PopupClient(this.Loc.GetString("food-has-used-storage", (nameof (food), (object) food)), user, new EntityUid?(user));
      return (false, true);
    }
    ItemSlotsComponent comp3;
    if (this.TryComp<ItemSlotsComponent>(food, out comp3) && comp3.Slots.Any<KeyValuePair<string, ItemSlot>>((Func<KeyValuePair<string, ItemSlot>, bool>) (slot => slot.Value.HasItem)))
    {
      this._popup.PopupClient(this.Loc.GetString("food-has-used-storage", (nameof (food), (object) food)), user, new EntityUid?(user));
      return (false, true);
    }
    string localizedFlavorsMessage = this._flavorProfile.GetLocalizedFlavorsMessage(food, user, solution);
    if (this.GetUsesRemaining(food, foodComp) <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("food-system-try-use-food-is-empty", ("entity", (object) food)), user, new EntityUid?(user));
      this.DeleteAndSpawnTrash(foodComp, food, user);
      return (false, true);
    }
    if (this.IsMouthBlocked(target, new EntityUid?(user)))
      return (false, true);
    if (!this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) food, popup: true))
      return (false, true);
    if (!this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) target, popup: true))
      return (false, true);
    if (!this._transform.GetMapCoordinates(user).InRange(this._transform.GetMapCoordinates(target), 1.5f))
    {
      this._popup.PopupClient(this.Loc.GetString("interaction-system-user-interaction-cannot-reach"), user, new EntityUid?(user));
      return (false, true);
    }
    bool flag = user != target;
    if (flag)
    {
      this._popup.PopupEntity(this.Loc.GetString("food-system-force-feed", (nameof (user), (object) Identity.Entity(user, (IEntityManager) this.EntityManager))), user, target);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(21, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is forcing ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" to eat ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) food), nameof (food), "ToPrettyString(food)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution));
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.ForceFeed, LogImpact.Medium, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(12, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" is eating ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) food), nameof (food), "ToPrettyString(food)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution));
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Ingestion, LogImpact.Low, ref local);
    }
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, flag ? foodComp.ForceFeedDelay : foodComp.Delay, (DoAfterEvent) new ConsumeDoAfterEvent(foodComp.Solution, localizedFlavorsMessage), new EntityUid?(food), new EntityUid?(target), new EntityUid?(food))
    {
      BreakOnHandChange = false,
      BreakOnMove = flag,
      BreakOnDamage = true,
      MovementThreshold = 0.3f,
      DistanceThreshold = new float?(1.5f),
      NeedHand = flag || this._hands.IsHolding((Entity<HandsComponent>) user, new EntityUid?(food))
    });
    return (true, true);
  }

  private void OnDoAfter(Entity<FoodComponent> entity, ref ConsumeDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || entity.Comp.Deleted)
      return;
    EntityUid? nullable1 = args.Target;
    if (!nullable1.HasValue)
      return;
    nullable1 = args.Target;
    BodyComponent comp1;
    if (!this.TryComp<BodyComponent>(nullable1.Value, out comp1))
      return;
    SharedBodySystem body = this._body;
    nullable1 = args.Target;
    Entity<BodyComponent> entity1 = (Entity<BodyComponent>) (nullable1.Value, comp1);
    List<Entity<StomachComponent, OrganComponent>> entityList;
    ref List<Entity<StomachComponent, OrganComponent>> local1 = ref entityList;
    if (!body.TryGetBodyOrganEntityComps<StomachComponent>(entity1, out local1))
      return;
    nullable1 = args.Used;
    if (!nullable1.HasValue)
      return;
    SharedSolutionContainerSystem solutionContainer = this._solutionContainer;
    nullable1 = args.Used;
    Entity<SolutionContainerManagerComponent> container = (Entity<SolutionContainerManagerComponent>) nullable1.Value;
    string solution1 = args.Solution;
    Entity<SolutionComponent>? nullable2;
    ref Entity<SolutionComponent>? local2 = ref nullable2;
    Solution solution2;
    ref Solution local3 = ref solution2;
    List<EntityUid> utensils;
    if (!solutionContainer.TryGetSolution(container, solution1, out local2, out local3) || !this.TryGetRequiredUtensils(args.User, entity.Comp, out utensils))
      return;
    nullable1 = args.Target;
    EntityUid uid1 = nullable1.Value;
    nullable1 = new EntityUid?();
    EntityUid? popupUid = nullable1;
    if (this.IsMouthBlocked(uid1, popupUid))
      return;
    SharedInteractionSystem interaction = this._interaction;
    Entity<TransformComponent> user1 = (Entity<TransformComponent>) args.User;
    nullable1 = args.Target;
    Entity<TransformComponent> other = (Entity<TransformComponent>) nullable1.Value;
    nullable1 = new EntityUid?();
    EntityUid? user2 = nullable1;
    if (!interaction.InRangeUnobstructed(user1, other, user: user2))
      return;
    EntityUid user3 = args.User;
    nullable1 = args.Target;
    bool flag = !nullable1.HasValue || user3 != nullable1.GetValueOrDefault();
    args.Handled = true;
    FixedPoint2 quantity = entity.Comp.TransferAmount.HasValue ? FixedPoint2.Min(entity.Comp.TransferAmount.Value, solution2.Volume) : solution2.Volume;
    Solution solution3 = this._solutionContainer.SplitSolution(nullable2.Value, quantity);
    FixedPoint2 fixedPoint2 = FixedPoint2.Zero;
    Entity<StomachComponent>? nullable3 = new Entity<StomachComponent>?();
    foreach (Entity<StomachComponent, OrganComponent> entity2 in entityList)
    {
      EntityUid owner = entity2.Owner;
      Solution solution4;
      if (this._stomach.CanTransferSolution(owner, solution3, entity2.Comp1) && this._solutionContainer.ResolveSolution((Entity<SolutionContainerManagerComponent>) owner, "stomach", ref entity2.Comp1.Solution, out solution4) && !(solution4.AvailableVolume <= fixedPoint2))
      {
        nullable3 = new Entity<StomachComponent>?((Entity<StomachComponent>) entity2);
        fixedPoint2 = solution4.AvailableVolume;
      }
    }
    if (!nullable3.HasValue)
    {
      this._solutionContainer.TryAddSolution(nullable2.Value, solution3);
      this._popup.PopupClient(flag ? this.Loc.GetString("food-system-you-cannot-eat-any-more-other", ("target", (object) args.Target.Value)) : this.Loc.GetString("food-system-you-cannot-eat-any-more"), args.Target.Value, new EntityUid?(args.User));
    }
    else
    {
      this._reaction.DoEntityReaction(args.Target.Value, solution2, ReactionMethod.Ingestion);
      StomachSystem stomach = this._stomach;
      EntityUid owner = nullable3.Value.Owner;
      Solution solution5 = solution3;
      Entity<StomachComponent>? nullable4 = nullable3;
      StomachComponent valueOrDefault = nullable4.HasValue ? (StomachComponent) nullable4.GetValueOrDefault() : (StomachComponent) null;
      stomach.TryTransferSolution(owner, solution5, valueOrDefault);
      string flavorMessage = args.FlavorMessage;
      if (flag)
      {
        EntityUid entityUid = Identity.Entity(args.Target.Value, (IEntityManager) this.EntityManager);
        this._popup.PopupEntity(this.Loc.GetString("food-system-force-feed-success", ("user", (object) Identity.Entity(args.User, (IEntityManager) this.EntityManager)), ("flavors", (object) flavorMessage)), entity.Owner, entity.Owner);
        this._popup.PopupClient(this.Loc.GetString("food-system-force-feed-success-user", ("target", (object) entityUid)), args.User, new EntityUid?(args.User));
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 3);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity.Owner), "user", "ToPrettyString(entity.Owner)");
        logStringHandler.AppendLiteral(" forced ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "target", "ToPrettyString(args.User)");
        logStringHandler.AppendLiteral(" to eat ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity.Owner), "food", "ToPrettyString(entity.Owner)");
        ref LogStringHandler local4 = ref logStringHandler;
        adminLogger.Add(LogType.ForceFeed, LogImpact.Medium, ref local4);
      }
      else
      {
        this._popup.PopupClient(this.Loc.GetString((string) entity.Comp.EatMessage, ("food", (object) entity.Owner), ("flavors", (object) flavorMessage)), args.User, new EntityUid?(args.User));
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(5, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "target", "ToPrettyString(args.User)");
        logStringHandler.AppendLiteral(" ate ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity.Owner), "food", "ToPrettyString(entity.Owner)");
        ref LogStringHandler local5 = ref logStringHandler;
        adminLogger.Add(LogType.Ingestion, LogImpact.Low, ref local5);
      }
      this._audio.PlayPredicted(entity.Comp.UseSound, args.Target.Value, new EntityUid?(args.User), new AudioParams?(AudioParams.Default.WithVolume(-1f).WithVariation(new float?(0.2f))));
      foreach (EntityUid uid2 in utensils)
        this._utensil.TryBreak(uid2, args.User);
      args.Repeat = !flag;
      StackComponent comp2;
      if (this.TryComp<StackComponent>((EntityUid) entity, out comp2))
      {
        if (comp2.Count > 1)
        {
          this._stack.SetCount(entity.Owner, comp2.Count - 1);
          this._solutionContainer.TryAddSolution(nullable2.Value, solution3);
          return;
        }
      }
      else if (this.GetUsesRemaining(entity.Owner, entity.Comp) > 0)
        return;
      args.Repeat = false;
      this.DeleteAndSpawnTrash(entity.Comp, entity.Owner, args.User);
    }
  }

  public void DeleteAndSpawnTrash(FoodComponent component, EntityUid food, EntityUid user)
  {
    BeforeFullyEatenEvent args1 = new BeforeFullyEatenEvent()
    {
      User = user
    };
    this.RaiseLocalEvent<BeforeFullyEatenEvent>(food, args1);
    if (args1.Cancelled)
      return;
    DestructionAttemptEvent args2 = new DestructionAttemptEvent();
    this.RaiseLocalEvent<DestructionAttemptEvent>(food, args2);
    if (args2.Cancelled)
      return;
    AfterFullyEatenEvent args3 = new AfterFullyEatenEvent(user);
    this.RaiseLocalEvent<AfterFullyEatenEvent>(food, ref args3);
    DestructionEventArgs args4 = new DestructionEventArgs();
    this.RaiseLocalEvent<DestructionEventArgs>(food, args4);
    if (component.Trash.Count == 0)
    {
      this.PredictedQueueDel(food);
    }
    else
    {
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(food);
      List<EntProtoId> trash = component.Trash;
      bool flag = this._hands.IsHolding((Entity<HandsComponent>) user, new EntityUid?(food), out string _);
      this.PredictedDel((Entity<MetaDataComponent, TransformComponent>) food);
      foreach (EntProtoId protoName in trash)
      {
        EntityUid entity = this.EntityManager.PredictedSpawn((string) protoName, mapCoordinates, rotation: new Angle());
        if (flag)
          this._hands.TryPickupAnyHand(user, entity);
      }
    }
  }

  private void AddEatVerb(Entity<FoodComponent> entity, ref GetVerbsEvent<AlternativeVerb> ev)
  {
    BodyComponent comp;
    List<Entity<StomachComponent, OrganComponent>> comps;
    if (entity.Owner == ev.User || !ev.CanInteract || !ev.CanAccess || !this.TryComp<BodyComponent>(ev.User, out comp) || !this._body.TryGetBodyOrganEntityComps<StomachComponent>((Entity<BodyComponent>) (ev.User, comp), out comps) || this._mobState.IsAlive((EntityUid) entity) && entity.Comp.RequireDead || !this.IsDigestibleBy((EntityUid) entity, entity.Comp, comps))
      return;
    EntityUid user = ev.User;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryFeed(user, user, (EntityUid) entity, entity.Comp));
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png"));
    alternativeVerb1.Text = this.Loc.GetString("food-system-verb-eat");
    alternativeVerb1.Priority = -1;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    ev.Verbs.Add(alternativeVerb2);
  }

  public bool IsDigestibleBy(EntityUid uid, EntityUid food, FoodComponent? foodComp = null)
  {
    List<Entity<StomachComponent, OrganComponent>> comps;
    return this.Resolve<FoodComponent>(food, ref foodComp, false) && this._body.TryGetBodyOrganEntityComps<StomachComponent>((Entity<BodyComponent>) uid, out comps) && this.IsDigestibleBy(food, foodComp, comps);
  }

  private bool IsDigestibleBy(
    EntityUid food,
    FoodComponent component,
    List<Entity<StomachComponent, OrganComponent>> stomachs)
  {
    bool flag = true;
    if (stomachs.Count < component.RequiredStomachs)
      return false;
    foreach (Entity<StomachComponent, OrganComponent> stomach in stomachs)
    {
      if (stomach.Comp1.SpecialDigestible != null)
      {
        if (this._whitelistSystem.IsWhitelistPass(stomach.Comp1.SpecialDigestible, food))
          return true;
        if (stomach.Comp1.IsSpecialDigestibleExclusive)
          return false;
      }
    }
    return !component.RequiresSpecialDigestion && flag;
  }

  private bool TryGetRequiredUtensils(
    EntityUid user,
    FoodComponent component,
    out List<EntityUid> utensils,
    HandsComponent? hands = null)
  {
    utensils = new List<EntityUid>();
    if (component.Utensil == UtensilType.None || !this.Resolve<HandsComponent>(user, ref hands, false))
      return true;
    UtensilType utensilType = UtensilType.None;
    foreach (EntityUid uid in this._hands.EnumerateHeld((Entity<HandsComponent>) (user, hands)))
    {
      UtensilComponent comp;
      if (this.TryComp<UtensilComponent>(uid, out comp) && (comp.Types & component.Utensil) != UtensilType.None && (utensilType & comp.Types) != comp.Types)
      {
        utensilType |= comp.Types;
        utensils.Add(uid);
      }
    }
    if (!component.UtensilRequired || (utensilType & component.Utensil) == component.Utensil)
      return true;
    this._popup.PopupClient(this.Loc.GetString("food-you-need-to-hold-utensil", ("utensil", (object) (component.Utensil ^ utensilType))), user, new EntityUid?(user));
    return false;
  }

  private void OnInventoryIngestAttempt(
    Entity<InventoryComponent> entity,
    ref IngestionAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid? entityUid1;
    IngestionBlockerComponent comp;
    if (this._inventory.TryGetSlotEntity(entity.Owner, "mask", out entityUid1) && this.TryComp<IngestionBlockerComponent>(entityUid1, out comp) && comp.Enabled)
    {
      args.Blocker = entityUid1;
      args.Cancel();
    }
    else
    {
      EntityUid? entityUid2;
      if (!this._inventory.TryGetSlotEntity(entity.Owner, "head", out entityUid2) || !this.TryComp<IngestionBlockerComponent>(entityUid2, out comp) || !comp.Enabled)
        return;
      args.Blocker = entityUid2;
      args.Cancel();
    }
  }

  public bool IsMouthBlocked(EntityUid uid, EntityUid? popupUid = null)
  {
    IngestionAttemptEvent args = new IngestionAttemptEvent();
    this.RaiseLocalEvent<IngestionAttemptEvent>(uid, args);
    if (args.Cancelled && args.Blocker.HasValue && popupUid.HasValue)
      this._popup.PopupClient(this.Loc.GetString("food-system-remove-mask", ("entity", (object) args.Blocker.Value)), uid, new EntityUid?(popupUid.Value));
    return args.Cancelled;
  }

  public int GetUsesRemaining(EntityUid uid, FoodComponent? comp = null)
  {
    Solution solution;
    if (!this.Resolve<FoodComponent>(uid, ref comp) || !this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) uid, comp.Solution, out Entity<SolutionComponent>? _, out solution) || solution.Volume == 0)
      return 0;
    return !comp.TransferAmount.HasValue ? 1 : Math.Max(1, (int) Math.Ceiling((double) (solution.Volume / comp.TransferAmount.Value).Float()));
  }
}
