// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.Stethoscope.StethoscopeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Medical.Stethoscope.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Medical.Stethoscope;

public sealed class StethoscopeSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedContainerSystem _container;
  private const string DamageToListenFor = "Asphyxiation";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StethoscopeComponent, InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>>(new EntityEventRefHandler<StethoscopeComponent, InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>>(this.AddStethoscopeVerb));
    this.SubscribeLocalEvent<StethoscopeComponent, GetItemActionsEvent>(new EntityEventRefHandler<StethoscopeComponent, GetItemActionsEvent>(this.OnGetActions));
    this.SubscribeLocalEvent<StethoscopeComponent, StethoscopeActionEvent>(new EntityEventRefHandler<StethoscopeComponent, StethoscopeActionEvent>(this.OnStethoscopeAction));
    this.SubscribeLocalEvent<StethoscopeComponent, StethoscopeDoAfterEvent>(new EntityEventRefHandler<StethoscopeComponent, StethoscopeDoAfterEvent>(this.OnDoAfter));
  }

  private void OnGetActions(Entity<StethoscopeComponent> ent, ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.ActionEntity, (string) ent.Comp.Action);
  }

  private void OnStethoscopeAction(
    Entity<StethoscopeComponent> ent,
    ref StethoscopeActionEvent args)
  {
    this.StartListening(ent, args.Target);
  }

  private void AddStethoscopeVerb(
    Entity<StethoscopeComponent> ent,
    ref InventoryRelayedEvent<GetVerbsEvent<InnateVerb>> args)
  {
    if (!args.Args.CanInteract || !args.Args.CanAccess || !this.HasComp<MobStateComponent>(args.Args.Target))
      return;
    EntityUid target = args.Args.Target;
    InnateVerb innateVerb1 = new InnateVerb();
    innateVerb1.Act = (Action) (() => this.StartListening(ent, target));
    innateVerb1.Text = this.Loc.GetString("stethoscope-verb");
    innateVerb1.IconEntity = new NetEntity?(this.GetNetEntity((EntityUid) ent));
    innateVerb1.Priority = 2;
    InnateVerb innateVerb2 = innateVerb1;
    args.Args.Verbs.Add(innateVerb2);
  }

  private void StartListening(Entity<StethoscopeComponent> ent, EntityUid target)
  {
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), out container))
      return;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, container.Owner, ent.Comp.Delay, (DoAfterEvent) new StethoscopeDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?(target), new EntityUid?((EntityUid) ent))
    {
      DuplicateCondition = DuplicateConditions.SameEvent,
      BreakOnMove = true,
      Hidden = true,
      BreakOnHandChange = false
    });
  }

  private void OnDoAfter(Entity<StethoscopeComponent> ent, ref StethoscopeDoAfterEvent args)
  {
    EntityUid? target = args.Target;
    if (args.Handled || !target.HasValue || args.Cancelled)
    {
      ent.Comp.LastMeasuredDamage = new FixedPoint2?();
    }
    else
    {
      this.ExamineWithStethoscope(ent, args.Args.User, target.Value);
      args.Repeat = true;
    }
  }

  private void ExamineWithStethoscope(
    Entity<StethoscopeComponent> stethoscope,
    EntityUid user,
    EntityUid target)
  {
    MobStateComponent comp1;
    DamageableComponent comp2;
    FixedPoint2 fixedPoint2;
    if (!this.TryComp<MobStateComponent>(target, out comp1) || !this.TryComp<DamageableComponent>(target, out comp2) || this._mobState.IsDead(target, comp1) || !comp2.Damage.DamageDict.TryGetValue("Asphyxiation", out fixedPoint2))
    {
      this._popup.PopupPredicted(this.Loc.GetString("stethoscope-nothing"), target, new EntityUid?(user));
      stethoscope.Comp.LastMeasuredDamage = new FixedPoint2?();
    }
    else
    {
      string absoluteDamageString = this.GetAbsoluteDamageString(fixedPoint2);
      if (!stethoscope.Comp.LastMeasuredDamage.HasValue)
      {
        this._popup.PopupPredicted(absoluteDamageString, target, new EntityUid?(user));
      }
      else
      {
        string deltaDamageString = this.GetDeltaDamageString(stethoscope.Comp.LastMeasuredDamage.Value, fixedPoint2);
        this._popup.PopupPredicted(this.Loc.GetString("stethoscope-combined-status", ("absolute", (object) absoluteDamageString), ("delta", (object) deltaDamageString)), target, new EntityUid?(user));
      }
      stethoscope.Comp.LastMeasuredDamage = new FixedPoint2?(fixedPoint2);
    }
  }

  private string GetAbsoluteDamageString(FixedPoint2 asphyxDmg)
  {
    int num = (int) asphyxDmg;
    return this.Loc.GetString(num >= 60 ? (num < 80 /*0x50*/ ? "stethoscope-irregular" : "stethoscope-fucked") : (num < 10 ? "stethoscope-normal" : (num < 30 ? "stethoscope-raggedy" : "stethoscope-hyper")));
  }

  private string GetDeltaDamageString(FixedPoint2 lastDamage, FixedPoint2 currentDamage)
  {
    if (lastDamage > currentDamage)
      return this.Loc.GetString("stethoscope-delta-improving");
    return lastDamage < currentDamage ? this.Loc.GetString("stethoscope-delta-worsening") : this.Loc.GetString("stethoscope-delta-steady");
  }
}
