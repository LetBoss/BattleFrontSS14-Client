// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.EggMorpher.EggMorpherSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.EggMorpher;

public sealed class EggMorpherSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _time;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedXenoParasiteSystem _parasite;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EggMorpherComponent, ExaminedEvent>(new EntityEventRefHandler<EggMorpherComponent, ExaminedEvent>(this.OnExamineEvent));
    this.SubscribeLocalEvent<EggMorpherComponent, InteractHandEvent>(new EntityEventRefHandler<EggMorpherComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<EggMorpherComponent, InteractUsingEvent>(new EntityEventRefHandler<EggMorpherComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<EggMorpherComponent, XenoChangeParasiteReserveMessage>(new EntityEventRefHandler<EggMorpherComponent, XenoChangeParasiteReserveMessage>(this.OnChangeParasiteReserve));
    this.SubscribeLocalEvent<EggMorpherComponent, GetVerbsEvent<ActivationVerb>>(new EntityEventRefHandler<EggMorpherComponent, GetVerbsEvent<ActivationVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<EggMorpherComponent, StepTriggerAttemptEvent>(new EntityEventRefHandler<EggMorpherComponent, StepTriggerAttemptEvent>(this.OnEggMorpherStepAttempt));
    this.SubscribeLocalEvent<EggMorpherComponent, StepTriggeredOffEvent>(new EntityEventRefHandler<EggMorpherComponent, StepTriggeredOffEvent>(this.OnEggMorpherStepTriggered));
  }

  private void OnExamineEvent(Entity<EggMorpherComponent> eggMorpher, ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("EggMorpherComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-xeno-construction-egg-morpher-examine", ("cur_paras", (object) eggMorpher.Comp.CurParasites), ("max_paras", (object) eggMorpher.Comp.MaxParasites)));
  }

  private void OnInteractHand(Entity<EggMorpherComponent> eggMorpher, ref InteractHandEvent args)
  {
    if (this._net.IsClient)
    {
      args.Handled = true;
    }
    else
    {
      EntityUid user = args.User;
      if (this.HasComp<XenoParasiteComponent>(user))
      {
        args.Handled = true;
        if (eggMorpher.Comp.MaxParasites <= eggMorpher.Comp.CurParasites)
        {
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-egg-morpher-already-full"), (EntityUid) eggMorpher, user);
        }
        else
        {
          if (this._mobState.IsDead(user) || this._net.IsClient)
            return;
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-morpher-return-self", ("parasite", (object) user)), (EntityUid) eggMorpher);
          this.QueueDel(new EntityUid?(user));
          ++eggMorpher.Comp.CurParasites;
          this._appearance.SetData((EntityUid) eggMorpher, (Enum) EggmorpherOverlayVisuals.Number, (object) eggMorpher.Comp.CurParasites);
        }
      }
      else if (!this.TryCreateParasiteFromEggMorpher(eggMorpher, out EntityUid? _))
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-egg-morpher-no-parasites"), (EntityUid) eggMorpher, user);
      else
        args.Handled = true;
    }
  }

  private void OnInteractUsing(Entity<EggMorpherComponent> eggMorpher, ref InteractUsingEvent args)
  {
    if (this._net.IsClient)
    {
      args.Handled = true;
    }
    else
    {
      EntityUid user = args.User;
      EntityUid used = args.Used;
      if (!this.HasComp<XenoParasiteComponent>(used))
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-egg-morpher-attempt-insert-non-parasite"), (EntityUid) eggMorpher, user);
      else if (!this.HasComp<ParasiteAIComponent>(used))
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-awake-child", ("parasite", (object) used)), user, user, PopupType.SmallCaution);
      else if (!this._mobState.IsAlive(used))
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-egg-dead-child"), (EntityUid) eggMorpher, user);
      else if (eggMorpher.Comp.MaxParasites <= eggMorpher.Comp.CurParasites)
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-construction-egg-morpher-already-full"), (EntityUid) eggMorpher, user);
      }
      else
      {
        args.Handled = true;
        this.QueueDel(new EntityUid?(used));
        ++eggMorpher.Comp.CurParasites;
        this._appearance.SetData((EntityUid) eggMorpher, (Enum) EggmorpherOverlayVisuals.Number, (object) eggMorpher.Comp.CurParasites);
      }
    }
  }

  private void OnChangeParasiteReserve(
    Entity<EggMorpherComponent> eggMorpher,
    ref XenoChangeParasiteReserveMessage args)
  {
    eggMorpher.Comp.ReservedParasites = args.NewReserve;
  }

  private void OnGetVerbs(
    Entity<EggMorpherComponent> eggMorpher,
    ref GetVerbsEvent<ActivationVerb> args)
  {
    (EntityUid entityUid, EggMorpherComponent comp) = eggMorpher;
    EntityUid user = args.User;
    if (this._hive.FromSameHive((Entity<HiveMemberComponent>) user, (Entity<HiveMemberComponent>) entityUid))
    {
      ActivationVerb activationVerb1 = new ActivationVerb();
      activationVerb1.Text = this.Loc.GetString("xeno-reserve-parasites-verb");
      activationVerb1.Act = (Action) (() => this._ui.OpenUi((Entity<UserInterfaceComponent>) entityUid, (Enum) XenoReserveParasiteChangeUI.Key, new EntityUid?(user)));
      ActivationVerb activationVerb2 = activationVerb1;
      args.Verbs.Add(activationVerb2);
    }
    if (!this.HasComp<ActorComponent>(user) || !this.HasComp<GhostComponent>(user) || comp.CurParasites <= comp.ReservedParasites || comp.CurParasites <= 0)
      return;
    ActivationVerb activationVerb3 = new ActivationVerb();
    activationVerb3.Text = this.Loc.GetString("rmc-xeno-egg-ghost-verb");
    activationVerb3.Act = (Action) (() => this._ui.TryOpenUi((Entity<UserInterfaceComponent>) entityUid, (Enum) XenoParasiteGhostUI.Key, user));
    activationVerb3.Impact = LogImpact.High;
    ActivationVerb activationVerb4 = activationVerb3;
    args.Verbs.Add(activationVerb4);
  }

  private void OnEggMorpherStepAttempt(
    Entity<EggMorpherComponent> eggMorpher,
    ref StepTriggerAttemptEvent args)
  {
    if (!this.CanTrigger(args.Tripper))
      return;
    args.Continue = true;
  }

  private void OnEggMorpherStepTriggered(
    Entity<EggMorpherComponent> eggMorpher,
    ref StepTriggeredOffEvent args)
  {
    this.TryTrigger(eggMorpher, args.Tripper);
  }

  private bool CanTrigger(EntityUid user)
  {
    InfectableComponent comp;
    return this.TryComp<InfectableComponent>(user, out comp) && !comp.BeingInfected && !this._mobState.IsDead(user) && !this.HasComp<VictimInfectedComponent>(user);
  }

  private bool TryTrigger(Entity<EggMorpherComponent> eggMorpher, EntityUid tripper)
  {
    EntityUid? parasite;
    if (!this.CanTrigger(tripper) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) eggMorpher.Owner, (Entity<TransformComponent>) tripper) || !this.TryCreateParasiteFromEggMorpher(eggMorpher, out parasite))
      return false;
    if (parasite.HasValue)
    {
      XenoParasiteComponent parasiteComponent = this.EnsureComp<XenoParasiteComponent>(parasite.Value);
      this._parasite.Infect((Entity<XenoParasiteComponent>) (parasite.Value, parasiteComponent), tripper, force: true);
    }
    return true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    base.Update(frameTime);
    TimeSpan curTime = this._time.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<EggMorpherComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EggMorpherComponent>();
    EntityUid uid;
    EggMorpherComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.GrowMaxParasites > comp1.CurParasites)
      {
        TimeSpan timeSpan1 = this.GetParasiteSpawnCooldown((Entity<EggMorpherComponent>) (uid, comp1)) + curTime;
        TimeSpan? nextSpawnAt = comp1.NextSpawnAt;
        TimeSpan timeSpan2 = curTime;
        if ((nextSpawnAt.HasValue ? (nextSpawnAt.GetValueOrDefault() < timeSpan2 ? 1 : 0) : 0) != 0)
        {
          ++comp1.CurParasites;
          this._appearance.SetData(uid, (Enum) EggmorpherOverlayVisuals.Number, (object) comp1.CurParasites);
          comp1.NextSpawnAt = new TimeSpan?(timeSpan1);
          this.Dirty(uid, (IComponent) comp1);
        }
        else
        {
          TimeSpan timeSpan3 = timeSpan1;
          nextSpawnAt = comp1.NextSpawnAt;
          if ((nextSpawnAt.HasValue ? (timeSpan3 < nextSpawnAt.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          {
            nextSpawnAt = comp1.NextSpawnAt;
            if (nextSpawnAt.HasValue)
              continue;
          }
          comp1.NextSpawnAt = new TimeSpan?(timeSpan1);
        }
      }
    }
  }

  private TimeSpan GetParasiteSpawnCooldown(Entity<EggMorpherComponent> eggMorpher)
  {
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) eggMorpher.Owner);
    if (!hive.HasValue)
      return eggMorpher.Comp.StandardSpawnCooldown;
    EntityUid? currentQueen = hive.GetValueOrDefault().Comp.CurrentQueen;
    return currentQueen.HasValue && this.HasComp<XenoAttachedOvipositorComponent>(currentQueen.GetValueOrDefault()) ? eggMorpher.Comp.OviSpawnCooldown : eggMorpher.Comp.StandardSpawnCooldown;
  }

  public bool TryCreateParasiteFromEggMorpher(
    Entity<EggMorpherComponent> eggMorpher,
    out EntityUid? parasite)
  {
    parasite = new EntityUid?();
    (EntityUid entityUid, EggMorpherComponent comp) = eggMorpher;
    if (comp.CurParasites <= 0)
      return false;
    --comp.CurParasites;
    this._appearance.SetData((EntityUid) eggMorpher, (Enum) EggmorpherOverlayVisuals.Number, (object) eggMorpher.Comp.CurParasites);
    this.Dirty<EggMorpherComponent>(eggMorpher);
    if (this._net.IsClient)
    {
      parasite = new EntityUid?();
      return true;
    }
    parasite = new EntityUid?(this.SpawnAtPosition("CMXenoParasite", entityUid.ToCoordinates()));
    this._hive.SetSameHive((Entity<HiveMemberComponent>) eggMorpher.Owner, (Entity<HiveMemberComponent>) parasite.Value);
    return true;
  }
}
