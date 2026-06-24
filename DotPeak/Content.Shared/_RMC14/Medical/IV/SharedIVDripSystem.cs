// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.IV.SharedIVDripSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Medical.IV;

public abstract class SharedIVDripSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly HashSet<EntityUid> _packsToUpdate = new HashSet<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<BloodPackComponent> _bloodPackQuery;

  public override void Initialize()
  {
    this._bloodPackQuery = this.GetEntityQuery<BloodPackComponent>();
    this.SubscribeLocalEvent<IVDripComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<IVDripComponent, EntInsertedIntoContainerMessage>(this.OnIVDripEntInserted));
    this.SubscribeLocalEvent<IVDripComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<IVDripComponent, EntRemovedFromContainerMessage>(this.OnIVDripEntRemoved));
    this.SubscribeLocalEvent<IVDripComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<IVDripComponent, AfterAutoHandleStateEvent>(this.OnIVDripAfterHandleState));
    this.SubscribeLocalEvent<IVDripComponent, CanDragEvent>(new EntityEventRefHandler<IVDripComponent, CanDragEvent>(this.OnIVDripCanDrag));
    this.SubscribeLocalEvent<IVDripComponent, CanDropDraggedEvent>(new EntityEventRefHandler<IVDripComponent, CanDropDraggedEvent>(this.OnIVDripCanDropDragged));
    this.SubscribeLocalEvent<IVDripComponent, DragDropDraggedEvent>(new EntityEventRefHandler<IVDripComponent, DragDropDraggedEvent>(this.OnIVDripDragDropDragged));
    this.SubscribeLocalEvent<IVDripComponent, InteractHandEvent>(new EntityEventRefHandler<IVDripComponent, InteractHandEvent>(this.OnIVInteractHand));
    this.SubscribeLocalEvent<IVDripComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<IVDripComponent, GetVerbsEvent<InteractionVerb>>(this.OnIVVerbs));
    this.SubscribeLocalEvent<IVDripComponent, ExaminedEvent>(new EntityEventRefHandler<IVDripComponent, ExaminedEvent>(this.OnIVExamine));
    this.SubscribeLocalEvent<IVDripTargetComponent, CanDropTargetEvent>(new EntityEventRefHandler<IVDripTargetComponent, CanDropTargetEvent>(this.OnIVTargetCanDropTarget));
    this.SubscribeLocalEvent<BloodPackComponent, MapInitEvent>(new EntityEventRefHandler<BloodPackComponent, MapInitEvent>(this.OnBloodPackMapInit));
    this.SubscribeLocalEvent<BloodPackComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<BloodPackComponent, AfterAutoHandleStateEvent>(this.OnBloodPackAfterState));
    this.SubscribeLocalEvent<BloodPackComponent, SolutionContainerChangedEvent>(new EntityEventRefHandler<BloodPackComponent, SolutionContainerChangedEvent>(this.OnBloodPackSolutionChanged));
    this.SubscribeLocalEvent<BloodPackComponent, AfterInteractEvent>(new EntityEventRefHandler<BloodPackComponent, AfterInteractEvent>(this.OnBloodPackAfterInteract));
    this.SubscribeLocalEvent<BloodPackComponent, AttachBloodPackDoAfterEvent>(new EntityEventRefHandler<BloodPackComponent, AttachBloodPackDoAfterEvent>(this.OnBloodPackAttachDoAfter));
    this.SubscribeLocalEvent<BloodPackComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<BloodPackComponent, GotUnequippedHandEvent>(this.OnBloodPackUnequippedHand));
    this.SubscribeLocalEvent<BloodPackComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<BloodPackComponent, GetVerbsEvent<InteractionVerb>>(this.OnBloodPackVerbs));
    this.SubscribeLocalEvent<BloodPackComponent, ExaminedEvent>(new EntityEventRefHandler<BloodPackComponent, ExaminedEvent>(this.OnBloodPackExamine));
  }

  private void OnIVDripEntInserted(
    Entity<IVDripComponent> iv,
    ref EntInsertedIntoContainerMessage args)
  {
    this.UpdateIVVisuals(iv);
  }

  private void OnIVDripEntRemoved(
    Entity<IVDripComponent> iv,
    ref EntRemovedFromContainerMessage args)
  {
    this.UpdateIVVisuals(iv);
  }

  private void OnIVDripAfterHandleState(
    Entity<IVDripComponent> iv,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateIVAppearance(iv);
  }

  private void OnIVDripCanDrag(Entity<IVDripComponent> iv, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnIVDripCanDropDragged(Entity<IVDripComponent> iv, ref CanDropDraggedEvent args)
  {
    if (!this.HasComp<IVDripTargetComponent>(args.Target) || !this.InRange((EntityUid) iv, args.Target, (float) iv.Comp.Range))
      return;
    args.Handled = true;
    args.CanDrop = true;
  }

  private void OnIVTargetCanDropTarget(
    Entity<IVDripTargetComponent> marine,
    ref CanDropTargetEvent args)
  {
    EntityUid dragged = args.Dragged;
    IVDripComponent comp;
    if (!this.TryComp<IVDripComponent>(dragged, out comp) || !this.InRange(dragged, (EntityUid) marine, (float) comp.Range))
      return;
    args.Handled = true;
    args.CanDrop = true;
  }

  private void OnIVDripDragDropDragged(Entity<IVDripComponent> iv, ref DragDropDraggedEvent args)
  {
    if (args.Handled)
      return;
    if (!iv.Comp.AttachedTo.HasValue)
      this.AttachIV(iv, args.User, args.Target);
    else
      this.DetachIV(iv, new EntityUid?(args.User), false, true);
  }

  private void OnIVInteractHand(Entity<IVDripComponent> iv, ref InteractHandEvent args)
  {
    this.DetachIV(iv, new EntityUid?(args.User), false, true);
  }

  private void OnIVVerbs(Entity<IVDripComponent> iv, ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Act = (Action) (() => this.ToggleInject(iv, user));
    interactionVerb.Text = this.Loc.GetString("cm-iv-verb-toggle-inject");
    verbs.Add(interactionVerb);
  }

  private void OnIVExamine(Entity<IVDripComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("IVDripComponent"))
    {
      string messageId = ent.Comp.Injecting ? "cm-iv-examine-injecting" : "cm-iv-examine-drawing";
      args.PushMarkup(this.Loc.GetString(messageId, ("iv", (object) ent.Owner)));
      string markup1 = this.Loc.GetString("cm-iv-examine-chemicals-none");
      BaseContainer container;
      if (this._containers.TryGetContainer((EntityUid) ent, ent.Comp.Slot, out container))
      {
        EntityUid entityUid = container.ContainedEntities.FirstOrDefault<EntityUid>();
        BloodPackComponent comp;
        Solution solution;
        if (entityUid.Valid && this.TryComp<BloodPackComponent>(entityUid, out comp) && this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) entityUid, comp.Solution, out Entity<SolutionComponent>? _, out solution))
          markup1 = this.Loc.GetString("cm-iv-examine-chemicals", ("attached", (object) entityUid), ("units", (object) solution.Volume.Int()));
      }
      args.PushMarkup(markup1);
      EntityUid? attachedTo = ent.Comp.AttachedTo;
      string markup2 = !attachedTo.HasValue ? this.Loc.GetString("cm-iv-examine-attached-none") : this.Loc.GetString("cm-iv-examine-attached", ("attached", (object) attachedTo.GetValueOrDefault()));
      args.PushMarkup(markup2);
    }
  }

  private void OnBloodPackMapInit(Entity<BloodPackComponent> pack, ref MapInitEvent args)
  {
    this._packsToUpdate.Add((EntityUid) pack);
  }

  private void OnBloodPackAfterState(
    Entity<BloodPackComponent> pack,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdatePackVisuals(pack);
  }

  private void OnBloodPackSolutionChanged(
    Entity<BloodPackComponent> pack,
    ref SolutionContainerChangedEvent args)
  {
    this.UpdatePackVisuals(pack);
  }

  private void OnBloodPackAfterInteract(
    Entity<BloodPackComponent> pack,
    ref AfterInteractEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!this.InRange((EntityUid) pack, valueOrDefault, (float) pack.Comp.Range) || !this.HasComp<IVDripTargetComponent>(valueOrDefault))
      return;
    args.Handled = true;
    EntityUid user = args.User;
    if (pack.Comp.AttachedTo.HasValue)
      this.DetachPack((Entity<BloodPackComponent>) ((EntityUid) pack, (BloodPackComponent) pack), new EntityUid?(user), false, true);
    else if (!this._skills.HasAllSkills((Entity<SkillsComponent>) user, pack.Comp.SkillRequired))
      this._popup.PopupClient(this.Loc.GetString("cm-iv-attach-no-skill"), user, new EntityUid?(user));
    else if (user == valueOrDefault)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-blood-pack-cannot-self"), user, new EntityUid?(user));
    }
    else
    {
      TimeSpan attachDelay = pack.Comp.AttachDelay;
      if (attachDelay > TimeSpan.Zero)
        this._popup.PopupPredicted(this.Loc.GetString("cm-blood-pack-poke-self", (nameof (pack), (object) pack.Owner), ("target", (object) valueOrDefault)), this.Loc.GetString("cm-blood-pack-poke-others", ("user", (object) user), (nameof (pack), (object) pack.Owner), ("target", (object) valueOrDefault)), valueOrDefault, new EntityUid?(user));
      AttachBloodPackDoAfterEvent @event = new AttachBloodPackDoAfterEvent();
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, attachDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) pack), new EntityUid?(valueOrDefault), new EntityUid?((EntityUid) pack))
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        BreakOnHandChange = true,
        BlockDuplicate = true,
        DuplicateCondition = DuplicateConditions.SameEvent,
        TargetEffect = (EntProtoId?) "RMCEffectHealBusy"
      });
    }
  }

  private void OnBloodPackAttachDoAfter(
    Entity<BloodPackComponent> pack,
    ref AttachBloodPackDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this.AttachPack(pack, args.User, valueOrDefault);
  }

  private void OnBloodPackUnequippedHand(
    Entity<BloodPackComponent> pack,
    ref GotUnequippedHandEvent args)
  {
    this.DetachPack((Entity<BloodPackComponent>) ((EntityUid) pack, (BloodPackComponent) pack), new EntityUid?(args.User), true, true);
  }

  private void OnBloodPackVerbs(
    Entity<BloodPackComponent> pack,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Act = (Action) (() => this.ToggleInject(pack, user));
    interactionVerb.Text = this.Loc.GetString("cm-iv-verb-toggle-inject");
    verbs.Add(interactionVerb);
  }

  private void OnBloodPackExamine(Entity<BloodPackComponent> pack, ref ExaminedEvent args)
  {
    using (args.PushGroup("BloodPackComponent"))
    {
      string messageId = pack.Comp.Injecting ? "cm-iv-examine-injecting" : "cm-iv-examine-drawing";
      args.PushMarkup(this.Loc.GetString(messageId, ("iv", (object) pack.Owner)));
      EntityUid? attachedTo = pack.Comp.AttachedTo;
      string markup = !attachedTo.HasValue ? this.Loc.GetString("cm-iv-examine-attached-none") : this.Loc.GetString("cm-iv-examine-attached", ("attached", (object) attachedTo.GetValueOrDefault()));
      args.PushMarkup(markup);
      Solution solution;
      if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) pack.Owner, pack.Comp.Solution, out Entity<SolutionComponent>? _, out solution))
        return;
      args.PushMarkup(this.Loc.GetString("cm-blood-pack-contains", ("units", (object) solution.Volume.Int())));
    }
  }

  protected bool InRange(EntityUid iv, EntityUid to, float range)
  {
    return this._transform.GetMapCoordinates(iv).InRange(this._transform.GetMapCoordinates(to), range);
  }

  private void AttachIV(Entity<IVDripComponent> iv, EntityUid user, EntityUid to)
  {
    if (!this.InRange((EntityUid) iv, to, (float) iv.Comp.Range))
      return;
    if (!this._skills.HasAllSkills((Entity<SkillsComponent>) user, iv.Comp.SkillRequired))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-iv-attach-no-skill"), user, new EntityUid?(user));
    }
    else
    {
      iv.Comp.AttachedTo = new EntityUid?(to);
      this.Dirty<IVDripComponent>(iv);
      this.AttachFeedback((EntityUid) iv, user, to, iv.Comp.Injecting);
    }
  }

  protected void DetachIV(Entity<IVDripComponent> iv, EntityUid? user, bool rip, bool predict)
  {
    EntityUid? attachedTo = iv.Comp.AttachedTo;
    if (!attachedTo.HasValue)
      return;
    EntityUid valueOrDefault = attachedTo.GetValueOrDefault();
    if (user.HasValue && !this._skills.HasAllSkills((Entity<SkillsComponent>) user.Value, iv.Comp.SkillRequired))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-iv-detach-no-skill"), user.Value, new EntityUid?(user.Value));
    }
    else
    {
      iv.Comp.AttachedTo = new EntityUid?();
      this.Dirty<IVDripComponent>(iv);
      if (rip)
        this.DoRip(iv.Comp.RipDamage, valueOrDefault, user, iv.Comp.RipEmote, predict);
      else
        this.DoDetachFeedback((EntityUid) iv, valueOrDefault, user, predict);
    }
  }

  private void AttachPack(Entity<BloodPackComponent> pack, EntityUid user, EntityUid to)
  {
    if (!this.InRange((EntityUid) pack, to, (float) pack.Comp.Range))
      return;
    if (!this._skills.HasAllSkills((Entity<SkillsComponent>) user, pack.Comp.SkillRequired))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-iv-attach-no-skill"), user, new EntityUid?(user));
    }
    else
    {
      pack.Comp.AttachedTo = new EntityUid?(to);
      this.Dirty<BloodPackComponent>(pack);
      this.AttachFeedback((EntityUid) pack, user, to, pack.Comp.Injecting);
    }
  }

  protected void DetachPack(
    Entity<BloodPackComponent> pack,
    EntityUid? user,
    bool rip,
    bool predict)
  {
    EntityUid? attachedTo = pack.Comp.AttachedTo;
    if (!attachedTo.HasValue)
      return;
    EntityUid valueOrDefault = attachedTo.GetValueOrDefault();
    if (user.HasValue && !this._skills.HasAllSkills((Entity<SkillsComponent>) user.Value, pack.Comp.SkillRequired))
    {
      this._popup.PopupClient(this.Loc.GetString("cm-iv-detach-no-skill"), user.Value, new EntityUid?(user.Value));
    }
    else
    {
      pack.Comp.AttachedTo = new EntityUid?();
      this.Dirty<BloodPackComponent>(pack);
      if (rip)
        this.DoRip(pack.Comp.RipDamage, valueOrDefault, user, pack.Comp.RipEmote, predict);
      else
        this.DoDetachFeedback((EntityUid) pack, valueOrDefault, user, predict);
    }
  }

  private void ToggleInject(Entity<IVDripComponent> iv, EntityUid user)
  {
    this.ToggleInject((EntityUid) iv, ref iv.Comp.Injecting, user);
    this.Dirty<IVDripComponent>(iv);
  }

  private void ToggleInject(Entity<BloodPackComponent> pack, EntityUid user)
  {
    this.ToggleInject((EntityUid) pack, ref pack.Comp.Injecting, user);
    this.Dirty<BloodPackComponent>(pack);
  }

  private void ToggleInject(EntityUid iv, ref bool injecting, EntityUid user)
  {
    injecting = !injecting;
    this._popup.PopupClient(injecting ? this.Loc.GetString("cm-iv-now-injecting") : this.Loc.GetString("cm-iv-now-taking"), iv, new EntityUid?(user));
  }

  protected void UpdatePackVisuals(Entity<BloodPackComponent> pack)
  {
    Solution solution;
    if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) pack.Owner, pack.Comp.Solution, out Entity<SolutionComponent>? _, out solution))
    {
      this.UpdatePackAppearance(pack);
    }
    else
    {
      BaseContainer container;
      IVDripComponent comp;
      if (this._containers.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) pack, (TransformComponent) null), out container) && this.TryComp<IVDripComponent>(container.Owner, out comp))
      {
        comp.FillColor = solution.GetColor(this._prototype);
        comp.FillPercentage = (int) (solution.Volume / solution.MaxVolume * 100);
        this.Dirty(container.Owner, (IComponent) comp);
        this.UpdateIVAppearance((Entity<IVDripComponent>) (container.Owner, comp));
      }
      this.UpdatePackAppearance(pack);
    }
  }

  protected void UpdateIVVisuals(Entity<IVDripComponent> iv)
  {
    if (this._net.IsClient)
    {
      this.UpdateIVAppearance(iv);
    }
    else
    {
      BaseContainer container;
      if (!this._containers.TryGetContainer((EntityUid) iv, iv.Comp.Slot, out container))
        return;
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
      {
        BloodPackComponent comp;
        Solution solution;
        if (this.TryComp<BloodPackComponent>(containedEntity, out comp) && this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) containedEntity, comp.Solution, out Entity<SolutionComponent>? _, out solution))
        {
          iv.Comp.FillColor = solution.GetColor(this._prototype);
          iv.Comp.FillPercentage = (int) (solution.Volume / solution.MaxVolume * 100);
          this.Dirty<IVDripComponent>(iv);
          this.UpdateIVAppearance(iv);
          return;
        }
      }
      iv.Comp.FillColor = Color.White;
      iv.Comp.FillPercentage = 0;
      this.Dirty<IVDripComponent>(iv);
      this.UpdateIVAppearance(iv);
    }
  }

  protected virtual void UpdateIVAppearance(Entity<IVDripComponent> iv)
  {
  }

  protected virtual void UpdatePackAppearance(Entity<BloodPackComponent> pack)
  {
    if (this._net.IsClient)
      return;
    Entity<SolutionComponent>? entity;
    if (this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) pack.Owner, pack.Comp.Solution, out entity))
    {
      Solution solution = entity.Value.Comp.Solution;
      pack.Comp.FillPercentage = solution.Volume / solution.MaxVolume;
      pack.Comp.FillColor = solution.GetColor(this._prototype);
    }
    else
    {
      pack.Comp.FillPercentage = FixedPoint2.Zero;
      pack.Comp.FillColor = Color.Transparent;
    }
    this.Dirty<BloodPackComponent>(pack);
  }

  protected virtual void DoRip(
    DamageSpecifier? damage,
    EntityUid attached,
    EntityUid? user,
    ProtoId<EmotePrototype> ripEmote,
    bool predict)
  {
    if (damage != null)
      this._damageable.TryChangeDamage(new EntityUid?(attached), damage, true);
    if (!this._timing.IsFirstTimePredicted)
      return;
    string message = this.Loc.GetString("cm-iv-rip", ("target", (object) attached));
    if (predict)
    {
      this._popup.PopupClient(message, attached, user);
      Filter filter = !user.HasValue ? Filter.Pvs(attached) : Filter.PvsExcept(user.Value);
      this._popup.PopupEntity(message, attached, filter, true);
    }
    else
      this._popup.PopupEntity(message, attached);
  }

  private void AttachFeedback(EntityUid iv, EntityUid user, EntityUid to, bool injecting)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    string messageId1 = "cm-iv-attach-self-drawing";
    string messageId2 = "cm-iv-attach-others-drawing";
    if (injecting)
    {
      messageId1 = "cm-iv-attach-self-injecting";
      messageId2 = "cm-iv-attach-others-injecting";
    }
    this._popup.PopupClient(this.Loc.GetString(messageId1, (nameof (iv), (object) iv), ("target", (object) to)), to, new EntityUid?(user));
    Filter filter = Filter.PvsExcept(user);
    this._popup.PopupEntity(this.Loc.GetString(messageId2, (nameof (iv), (object) iv), (nameof (user), (object) user), ("target", (object) to)), to, filter, true);
  }

  private void DoDetachFeedback(EntityUid iv, EntityUid attached, EntityUid? user, bool predict)
  {
    string message = this.Loc.GetString("cm-iv-detach-self", (nameof (iv), (object) iv), ("target", (object) attached));
    if (predict)
      this._popup.PopupClient(message, attached, user);
    else
      this._popup.PopupEntity(message, attached);
    if (!user.HasValue)
      return;
    Filter filter = Filter.PvsExcept(user.Value);
    this._popup.PopupEntity(this.Loc.GetString("cm-iv-detach-others", (nameof (iv), (object) iv), (nameof (user), (object) user), ("target", (object) attached)), attached, filter, true);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    foreach (EntityUid uid in this._packsToUpdate)
    {
      BloodPackComponent component;
      if (this._bloodPackQuery.TryComp(uid, out component))
        this.UpdatePackVisuals((Entity<BloodPackComponent>) (uid, component));
    }
    this._packsToUpdate.Clear();
  }
}
