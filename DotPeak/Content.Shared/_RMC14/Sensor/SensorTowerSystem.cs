// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sensor.SensorTowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Sensor;

public sealed class SensorTowerSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedToolSystem _tool;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TacticalMapIncludeXenosEvent>(new EntityEventRefHandler<TacticalMapIncludeXenosEvent>(this.OnTacticalMapIncludeXenos));
    this.SubscribeLocalEvent<SensorTowerComponent, MapInitEvent>(new EntityEventRefHandler<SensorTowerComponent, MapInitEvent>(this.OnSensorTowerMapInit));
    this.SubscribeLocalEvent<SensorTowerComponent, InteractUsingEvent>(new EntityEventRefHandler<SensorTowerComponent, InteractUsingEvent>(this.OnSensorTowerInteractUsing));
    this.SubscribeLocalEvent<SensorTowerComponent, InteractHandEvent>(new EntityEventRefHandler<SensorTowerComponent, InteractHandEvent>(this.OnSensorTowerInteractHand));
    this.SubscribeLocalEvent<SensorTowerComponent, ExaminedEvent>(new EntityEventRefHandler<SensorTowerComponent, ExaminedEvent>(this.OnSensorTowerExamined));
    this.SubscribeLocalEvent<SensorTowerComponent, SensorTowerRepairDoAfterEvent>(new EntityEventRefHandler<SensorTowerComponent, SensorTowerRepairDoAfterEvent>(this.OnSensorTowerRepairDoAfter));
    this.SubscribeLocalEvent<SensorTowerComponent, SensorTowerDestroyDoAfterEvent>(new EntityEventRefHandler<SensorTowerComponent, SensorTowerDestroyDoAfterEvent>(this.OnSensorTowerDestroyDoAfter));
  }

  private void OnTacticalMapIncludeXenos(ref TacticalMapIncludeXenosEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SensorTowerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SensorTowerComponent>();
    SensorTowerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out comp1))
    {
      if (comp1.State == SensorTowerState.On)
      {
        ev.Include = true;
        break;
      }
    }
  }

  private void OnSensorTowerMapInit(Entity<SensorTowerComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnSensorTowerInteractUsing(
    Entity<SensorTowerComponent> ent,
    ref InteractUsingEvent args)
  {
    EntityUid user = args.User;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) user, ent.Comp.Skill, ent.Comp.SkillLevel))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-skills-no-training", ("target", (object) ent)), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      EntityUid used = args.Used;
      RMCDeviceBreakerComponent comp;
      if (this.TryComp<RMCDeviceBreakerComponent>(args.Used, out comp) && ent.Comp.State != SensorTowerState.Weld)
      {
        DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp.DoAfterTime, (DoAfterEvent) new RMCDeviceBreakerDoAfterEvent(), new EntityUid?(args.Used), new EntityUid?(args.Target), new EntityUid?(args.Used))
        {
          BreakOnMove = true,
          RequireCanInteract = true,
          BreakOnHandChange = true,
          DuplicateCondition = DuplicateConditions.SameTool
        };
        args.Handled = true;
        this._doAfter.TryStartDoAfter(args1);
      }
      else
      {
        ProtoId<ToolQualityPrototype> protoId;
        switch (ent.Comp.State)
        {
          case SensorTowerState.Weld:
            protoId = ent.Comp.WeldingQuality;
            break;
          case SensorTowerState.Wire:
            protoId = ent.Comp.CuttingQuality;
            break;
          case SensorTowerState.Wrench:
            protoId = ent.Comp.WrenchQuality;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        ProtoId<ToolQualityPrototype> quality = protoId;
        args.Handled = true;
        if (!this._tool.HasQuality(used, (string) quality))
          return;
        this.TryRepair(ent, user, used, ent.Comp.State);
      }
    }
  }

  private void OnSensorTowerInteractHand(
    Entity<SensorTowerComponent> ent,
    ref InteractHandEvent args)
  {
    EntityUid user = args.User;
    if (this.HasComp<XenoComponent>(user))
    {
      if (!this.HasComp<HandsComponent>(user))
        return;
      this.Destroy(ent, user);
    }
    else if (!this._skills.HasSkill((Entity<SkillsComponent>) user, ent.Comp.Skill, ent.Comp.SkillLevel))
    {
      this._popup.PopupClient("You have no clue how this thing works...", (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      ref SensorTowerState local = ref ent.Comp.State;
      string message;
      switch (local)
      {
        case SensorTowerState.Weld:
          message = "Use a blowtorch, then wirecutters, then wrench to repair it.";
          break;
        case SensorTowerState.Wire:
          message = "Use some wirecutters, then wrench to repair it.";
          break;
        case SensorTowerState.Wrench:
          message = "Use a wrench to repair it.";
          break;
        case SensorTowerState.Off:
          message = $"The {this.Name((EntityUid) ent)} lights up.";
          break;
        case SensorTowerState.On:
          message = $"The {this.Name((EntityUid) ent)} goes dark.";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      this._popup.PopupClient(message, (EntityUid) ent, new EntityUid?(user), PopupType.Medium);
      if (local < SensorTowerState.Off)
        return;
      if (local == SensorTowerState.Off)
        local = SensorTowerState.On;
      else if (local == SensorTowerState.On)
        local = SensorTowerState.Off;
      this.Dirty<SensorTowerComponent>(ent);
      this.UpdateAppearance(ent);
    }
  }

  private void OnSensorTowerExamined(Entity<SensorTowerComponent> ent, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("SensorTowerComponent"))
    {
      string str1;
      switch (ent.Comp.State)
      {
        case SensorTowerState.Weld:
          str1 = "This one is heavily damaged. Use a blowtorch, wirecutters, then a wrench to repair it.";
          break;
        case SensorTowerState.Wire:
          str1 = "This one is heavily damaged. Use wirecutters, then a wrench to repair it.";
          break;
        case SensorTowerState.Wrench:
          str1 = "This one is heavily damaged. Use a wrench to repair it.";
          break;
        case SensorTowerState.Off:
          str1 = "It looks like it is offline.";
          break;
        case SensorTowerState.On:
          str1 = "It looks like it is online.";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      string text = str1;
      args.PushText(text);
      if (ent.Comp.State >= SensorTowerState.Off)
        return;
      string str2;
      switch (ent.Comp.State)
      {
        case SensorTowerState.Weld:
          str2 = "a [color=cyan]Welder[/color]";
          break;
        case SensorTowerState.Wire:
          str2 = "[color=cyan]Wirecutters[/color]";
          break;
        case SensorTowerState.Wrench:
          str2 = "a [color=cyan]Wrench[/color]";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      string str3 = str2;
      args.PushMarkup($"Use {str3} to repair it!");
    }
  }

  private void OnSensorTowerRepairDoAfter(
    Entity<SensorTowerComponent> ent,
    ref SensorTowerRepairDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    if (ent.Comp.State != args.State)
      return;
    SensorTowerComponent comp = ent.Comp;
    SensorTowerState sensorTowerState;
    switch (args.State)
    {
      case SensorTowerState.Weld:
        sensorTowerState = SensorTowerState.Wire;
        break;
      case SensorTowerState.Wire:
        sensorTowerState = SensorTowerState.Wrench;
        break;
      case SensorTowerState.Wrench:
        sensorTowerState = SensorTowerState.Off;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    comp.State = sensorTowerState;
    this.Dirty<SensorTowerComponent>(ent);
    this.UpdateAppearance(ent);
  }

  private void OnSensorTowerDestroyDoAfter(
    Entity<SensorTowerComponent> ent,
    ref SensorTowerDestroyDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    ent.Comp.State = SensorTowerState.Weld;
    this.Dirty<SensorTowerComponent>(ent);
    this.UpdateAppearance(ent);
  }

  public void SensorTowerIncrementalDestroy(Entity<SensorTowerComponent> ent)
  {
    SensorTowerComponent comp = ent.Comp;
    SensorTowerState sensorTowerState;
    switch (ent.Comp.State)
    {
      case SensorTowerState.Wire:
        sensorTowerState = SensorTowerState.Weld;
        break;
      case SensorTowerState.Wrench:
        sensorTowerState = SensorTowerState.Wire;
        break;
      case SensorTowerState.Off:
        sensorTowerState = SensorTowerState.Wrench;
        break;
      case SensorTowerState.On:
        sensorTowerState = SensorTowerState.Wrench;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    comp.State = sensorTowerState;
    this.Dirty<SensorTowerComponent>(ent);
    this.UpdateAppearance(ent);
  }

  private void TryRepair(
    Entity<SensorTowerComponent> tower,
    EntityUid user,
    EntityUid used,
    SensorTowerState state)
  {
    ProtoId<ToolQualityPrototype> protoId;
    switch (state)
    {
      case SensorTowerState.Weld:
        protoId = tower.Comp.WeldingQuality;
        break;
      case SensorTowerState.Wire:
        protoId = tower.Comp.CuttingQuality;
        break;
      case SensorTowerState.Wrench:
        protoId = tower.Comp.WrenchQuality;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (state), (object) state, (string) null);
    }
    ProtoId<ToolQualityPrototype> toolQualityNeeded = protoId;
    TimeSpan timeSpan1;
    switch (state)
    {
      case SensorTowerState.Weld:
        timeSpan1 = tower.Comp.WeldingDelay;
        break;
      case SensorTowerState.Wire:
        timeSpan1 = tower.Comp.CuttingDelay;
        break;
      case SensorTowerState.Wrench:
        timeSpan1 = tower.Comp.WrenchDelay;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (state), (object) state, (string) null);
    }
    TimeSpan timeSpan2 = timeSpan1;
    this._tool.UseTool(used, user, new EntityUid?((EntityUid) tower), (float) timeSpan2.TotalSeconds, (string) toolQualityNeeded, (DoAfterEvent) new SensorTowerRepairDoAfterEvent(state), tower.Comp.WeldingCost);
  }

  private void UpdateAppearance(Entity<SensorTowerComponent> tower)
  {
    this._appearance.SetData((EntityUid) tower, (Enum) SensorTowerLayers.Layer, (object) tower.Comp.State);
  }

  private void Destroy(Entity<SensorTowerComponent> tower, EntityUid user)
  {
    if (tower.Comp.State == SensorTowerState.Weld)
    {
      this._popup.PopupClient("We stare at the experimental sensor tower cluelessly.", user, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      SensorTowerDestroyDoAfterEvent @event = new SensorTowerDestroyDoAfterEvent();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, tower.Comp.DestroyDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) tower), new EntityUid?((EntityUid) tower), new EntityUid?(user))
      {
        ForceVisible = true
      }))
        return;
      this._popup.PopupClient($"You start wrenching apart the {this.Name((EntityUid) tower)}'s panels and reaching inside it!", (EntityUid) tower, new EntityUid?(user), PopupType.Medium);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<SensorTowerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SensorTowerComponent>();
    EntityUid uid;
    SensorTowerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.State == SensorTowerState.On && !(curTime < comp1.NextBreakAt))
      {
        if (!this._random.Prob(comp1.BreakChance))
        {
          comp1.NextBreakAt = curTime + comp1.BreakEvery;
          this.Dirty(uid, (IComponent) comp1);
        }
        else
        {
          if (this._random.Prob(0.75f))
          {
            this._popup.PopupEntity($"The {this.Name(uid)} beeps wildly and sprays random pieces everywhere! Use a wrench to repair it.", uid, uid, PopupType.LargeCaution);
            comp1.State = SensorTowerState.Wrench;
            this.Dirty(uid, (IComponent) comp1);
          }
          else
          {
            this._popup.PopupEntity($"The {this.Name(uid)} beeps wildly and a fuse blows! Use wirecutters, then a wrench to repair it.", uid, uid, PopupType.LargeCaution);
            comp1.State = SensorTowerState.Wire;
            this.Dirty(uid, (IComponent) comp1);
          }
          this.UpdateAppearance((Entity<SensorTowerComponent>) (uid, comp1));
        }
      }
    }
  }
}
