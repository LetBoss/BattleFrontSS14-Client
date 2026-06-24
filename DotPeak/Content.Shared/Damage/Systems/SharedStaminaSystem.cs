// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.SharedStaminaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Armor;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Damage.Systems;

public abstract class SharedStaminaSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private MetaDataSystem _metadata;
  [Dependency]
  private SharedColorFlashEffectSystem _color;
  [Dependency]
  private SharedStunSystem _stunSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IConfigurationManager _config;
  private static readonly TimeSpan StamCritBufferTime = TimeSpan.FromSeconds(3.0);

  public float UniversalStaminaDamageModifier { get; private set; } = 1f;

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeModifier();
    this.InitializeResistance();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaComponent, ComponentStartup>(new ComponentEventHandler<StaminaComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaComponent, ComponentShutdown>(new ComponentEventHandler<StaminaComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<StaminaComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnStamHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaComponent, DisarmedEvent>(new ComponentEventRefHandler<StaminaComponent, DisarmedEvent>((object) this, __methodptr(OnDisarmed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaComponent, RejuvenateEvent>(new ComponentEventHandler<StaminaComponent, RejuvenateEvent>((object) this, __methodptr(OnRejuvenate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaDamageOnEmbedComponent, EmbedEvent>(new ComponentEventRefHandler<StaminaDamageOnEmbedComponent, EmbedEvent>((object) this, __methodptr(OnProjectileEmbed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaDamageOnCollideComponent, ProjectileHitEvent>(new ComponentEventRefHandler<StaminaDamageOnCollideComponent, ProjectileHitEvent>((object) this, __methodptr(OnProjectileHit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaDamageOnCollideComponent, ThrowDoHitEvent>(new ComponentEventHandler<StaminaDamageOnCollideComponent, ThrowDoHitEvent>((object) this, __methodptr(OnThrowHit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaDamageOnHitComponent, MeleeHitEvent>(new ComponentEventHandler<StaminaDamageOnHitComponent, MeleeHitEvent>((object) this, __methodptr(OnMeleeHit)), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._config, CCVars.PlaytestStaminaDamageModifier, (Action<float>) (value => this.UniversalStaminaDamageModifier = value), true);
  }

  private void OnStamHandleState(
    EntityUid uid,
    StaminaComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    if (component.Critical)
    {
      this.EnterStamCrit(uid, component);
    }
    else
    {
      if ((double) component.StaminaDamage > 0.0)
        this.EnsureComp<ActiveStaminaComponent>(uid);
      this.ExitStamCrit(uid, component);
    }
  }

  private void OnShutdown(EntityUid uid, StaminaComponent component, ComponentShutdown args)
  {
    if (this.MetaData(uid).EntityLifeStage < 4)
      this.RemCompDeferred<ActiveStaminaComponent>(uid);
    this._alerts.ClearAlert(uid, component.StaminaAlert);
  }

  private void OnStartup(EntityUid uid, StaminaComponent component, ComponentStartup args)
  {
    this.SetStaminaAlert(uid, component);
  }

  public float GetStaminaDamage(EntityUid uid, StaminaComponent? component = null)
  {
    if (!this.Resolve<StaminaComponent>(uid, ref component, true))
      return 0.0f;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan pauseTime = this._metadata.GetPauseTime(uid, (MetaDataComponent) null);
    return MathF.Max(0.0f, component.StaminaDamage - MathF.Max(0.0f, (float) (curTime - (component.NextUpdate + pauseTime)).TotalSeconds * component.Decay));
  }

  private void OnRejuvenate(EntityUid uid, StaminaComponent component, RejuvenateEvent args)
  {
    if ((double) component.StaminaDamage >= (double) component.CritThreshold)
      this.ExitStamCrit(uid, component);
    component.StaminaDamage = 0.0f;
    this.AdjustSlowdown(Entity<StaminaComponent>.op_Implicit(uid));
    this.RemComp<ActiveStaminaComponent>(uid);
    this.SetStaminaAlert(uid, component);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  private void OnDisarmed(EntityUid uid, StaminaComponent component, ref DisarmedEvent args)
  {
    if (args.Handled || component.Critical)
      return;
    float num = args.PushProbability * component.CritThreshold;
    this.TakeStaminaDamage(uid, num, component, new EntityUid?(args.Source));
    args.PopupPrefix = "disarm-action-shove-";
    args.IsStunned = component.Critical;
    args.Handled = true;
  }

  private void OnMeleeHit(EntityUid uid, StaminaDamageOnHitComponent component, MeleeHitEvent args)
  {
    if (!args.IsHit || !args.HitEntities.Any<EntityUid>() || (double) component.Damage <= 0.0)
      return;
    StaminaDamageOnHitAttemptEvent onHitAttemptEvent = new StaminaDamageOnHitAttemptEvent();
    this.RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>(uid, ref onHitAttemptEvent, false);
    if (onHitAttemptEvent.Cancelled)
      return;
    EntityQuery<StaminaComponent> entityQuery = this.GetEntityQuery<StaminaComponent>();
    List<(EntityUid, StaminaComponent)> hitList = new List<(EntityUid, StaminaComponent)>();
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      StaminaComponent staminaComponent;
      if (entityQuery.TryGetComponent(hitEntity, ref staminaComponent))
        hitList.Add((hitEntity, staminaComponent));
    }
    StaminaMeleeHitEvent staminaMeleeHitEvent = new StaminaMeleeHitEvent(hitList);
    this.RaiseLocalEvent<StaminaMeleeHitEvent>(uid, staminaMeleeHitEvent, false);
    if (staminaMeleeHitEvent.Handled)
      return;
    float num = component.Damage * staminaMeleeHitEvent.Multiplier + staminaMeleeHitEvent.FlatModifier;
    foreach ((EntityUid uid1, StaminaComponent component1) in hitList)
      this.TakeStaminaDamage(uid1, num / (float) hitList.Count, component1, new EntityUid?(args.User), new EntityUid?(args.Weapon), sound: component.Sound);
  }

  private void OnProjectileHit(
    EntityUid uid,
    StaminaDamageOnCollideComponent component,
    ref ProjectileHitEvent args)
  {
    this.OnCollide(uid, component, args.Target);
  }

  private void OnProjectileEmbed(
    EntityUid uid,
    StaminaDamageOnEmbedComponent component,
    ref EmbedEvent args)
  {
    StaminaComponent component1;
    if (!this.TryComp<StaminaComponent>(args.Embedded, ref component1))
      return;
    this.TakeStaminaDamage(args.Embedded, component.Damage, component1, new EntityUid?(uid));
  }

  private void OnThrowHit(
    EntityUid uid,
    StaminaDamageOnCollideComponent component,
    ThrowDoHitEvent args)
  {
    this.OnCollide(uid, component, args.Target);
  }

  private void OnCollide(
    EntityUid uid,
    StaminaDamageOnCollideComponent component,
    EntityUid target)
  {
    if (!this.HasComp<StaminaComponent>(target))
      return;
    StaminaDamageOnHitAttemptEvent onHitAttemptEvent = new StaminaDamageOnHitAttemptEvent();
    this.RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>(uid, ref onHitAttemptEvent, false);
    if (onHitAttemptEvent.Cancelled)
      return;
    EntityUid uid1 = target;
    double damage = (double) component.Damage;
    EntityUid? source = new EntityUid?(uid);
    SoundSpecifier sound1 = component.Sound;
    EntityUid? with = new EntityUid?();
    SoundSpecifier sound2 = sound1;
    this.TakeStaminaDamage(uid1, (float) damage, source: source, with: with, sound: sound2);
  }

  private void SetStaminaAlert(EntityUid uid, StaminaComponent? component = null)
  {
    if (!this.Resolve<StaminaComponent>(uid, ref component, false) || component.Deleted)
      return;
    if (!component.ShowAlert)
    {
      this._alerts.ClearAlert(uid, component.StaminaAlert);
    }
    else
    {
      int levels = ContentHelpers.RoundToLevels((double) MathF.Max(0.0f, component.CritThreshold - component.StaminaDamage), (double) component.CritThreshold, 7);
      this._alerts.ShowAlert(uid, component.StaminaAlert, new short?((short) levels));
    }
  }

  public bool TryTakeStamina(
    EntityUid uid,
    float value,
    StaminaComponent? component = null,
    EntityUid? source = null,
    EntityUid? with = null)
  {
    if (!this.Resolve<StaminaComponent>(uid, ref component, false))
      return true;
    if ((double) component.StaminaDamage + (double) value > (double) component.CritThreshold || component.Critical)
      return false;
    this.TakeStaminaDamage(uid, value, component, source, with, false);
    return true;
  }

  public void TakeStaminaDamage(
    EntityUid uid,
    float value,
    StaminaComponent? component = null,
    EntityUid? source = null,
    EntityUid? with = null,
    bool visual = true,
    SoundSpecifier? sound = null,
    bool ignoreResist = false)
  {
    if (!this.Resolve<StaminaComponent>(uid, ref component, false))
      return;
    BeforeStaminaDamageEvent staminaDamageEvent = new BeforeStaminaDamageEvent(value);
    this.RaiseLocalEvent<BeforeStaminaDamageEvent>(uid, ref staminaDamageEvent, false);
    if (staminaDamageEvent.Cancelled)
      return;
    if (!ignoreResist)
      value = staminaDamageEvent.Value;
    value = this.UniversalStaminaDamageModifier * value;
    if (component.Critical)
      return;
    float staminaDamage = component.StaminaDamage;
    component.StaminaDamage = MathF.Max(0.0f, component.StaminaDamage + value);
    if ((double) staminaDamage < (double) component.StaminaDamage)
    {
      TimeSpan timeSpan = this._timing.CurTime + TimeSpan.FromSeconds((double) component.Cooldown);
      if (component.NextUpdate < timeSpan)
        component.NextUpdate = timeSpan;
    }
    this.AdjustSlowdown(Entity<StaminaComponent>.op_Implicit(uid));
    this.SetStaminaAlert(uid, component);
    if (component.AfterCritical && (double) staminaDamage > (double) component.StaminaDamage && (double) component.StaminaDamage <= 0.0)
      component.AfterCritical = false;
    if (!component.Critical)
    {
      if ((double) component.StaminaDamage >= (double) component.CritThreshold)
        this.EnterStamCrit(uid, component);
    }
    else if ((double) component.StaminaDamage < (double) component.CritThreshold)
      this.ExitStamCrit(uid, component);
    this.EnsureComp<ActiveStaminaComponent>(uid);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    if ((double) value <= 0.0)
      return;
    if (source.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(27, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(source.Value)), "user", "ToPrettyString(source.Value)");
      logStringHandler.AppendLiteral(" caused ");
      logStringHandler.AppendFormatted<float>(value, nameof (value));
      logStringHandler.AppendLiteral(" stamina damage to ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
      ref LogStringHandler local1 = ref logStringHandler;
      string str;
      if (!with.HasValue)
        str = "";
      else
        str = $" using {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(with.Value)):using}";
      local1.AppendFormatted(str);
      ref LogStringHandler local2 = ref logStringHandler;
      adminLogger.Add(LogType.Stamina, ref local2);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(21, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" took ");
      logStringHandler.AppendFormatted<float>(value, nameof (value));
      logStringHandler.AppendLiteral(" stamina damage");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Stamina, ref local);
    }
    if (visual)
    {
      SharedColorFlashEffectSystem color = this._color;
      Color aqua = Color.Aqua;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(uid);
      Filter filter = Filter.Pvs(uid, 2f, (IEntityManager) this.EntityManager, (ISharedPlayerManager) null, (IConfigurationManager) null);
      color.RaiseEffect(aqua, entities, filter);
    }
    if (!this._net.IsServer)
      return;
    this._audio.PlayPvs(sound, uid, new AudioParams?());
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQuery<StaminaComponent> entityQuery = this.GetEntityQuery<StaminaComponent>();
    EntityQueryEnumerator<ActiveStaminaComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveStaminaComponent>();
    TimeSpan curTime = this._timing.CurTime;
    EntityUid uid;
    ActiveStaminaComponent staminaComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref staminaComponent))
    {
      StaminaComponent component;
      if (!entityQuery.TryGetComponent(uid, ref component) || (double) component.StaminaDamage <= 0.0 && !component.Critical)
        this.RemComp<ActiveStaminaComponent>(uid);
      else if (!(component.NextUpdate > curTime))
      {
        if (component.Critical)
          this.ExitStamCrit(uid, component);
        component.NextUpdate += TimeSpan.FromSeconds(1.0);
        this.TakeStaminaDamage(uid, component.AfterCritical ? -component.Decay * component.AfterCritDecayMultiplier : -component.Decay, component);
        this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      }
    }
  }

  private void EnterStamCrit(EntityUid uid, StaminaComponent? component = null)
  {
    if (!this.Resolve<StaminaComponent>(uid, ref component, true) || component.Critical)
      return;
    component.Critical = true;
    component.StaminaDamage = component.CritThreshold;
    this._stunSystem.TryParalyze(uid, component.StunTime, true);
    component.NextUpdate = this._timing.CurTime + component.StunTime + SharedStaminaSystem.StamCritBufferTime;
    this.EnsureComp<ActiveStaminaComponent>(uid);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(21, 1);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "user", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" entered stamina crit");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stamina, LogImpact.Medium, ref local);
  }

  private void ExitStamCrit(EntityUid uid, StaminaComponent? component = null)
  {
    if (!this.Resolve<StaminaComponent>(uid, ref component, true) || !component.Critical)
      return;
    component.Critical = false;
    component.AfterCritical = true;
    component.NextUpdate = this._timing.CurTime;
    this.SetStaminaAlert(uid, component);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(28, 1);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "user", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" recovered from stamina crit");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Stamina, LogImpact.Low, ref local);
  }

  private void AdjustSlowdown(Entity<StaminaComponent?> ent)
  {
    if (!this.Resolve<StaminaComponent>(Entity<StaminaComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    FixedPoint2 key = FixedPoint2.Zero;
    foreach (KeyValuePair<FixedPoint2, float> modifierThreshold in ent.Comp.StunModifierThresholds)
    {
      float num = modifierThreshold.Key.Float();
      if ((double) ent.Comp.StaminaDamage >= (double) num && (FixedPoint2) num > key && key < (FixedPoint2) ent.Comp.CritThreshold)
        key = modifierThreshold.Key;
    }
    this._stunSystem.UpdateStunModifiers(ent, ent.Comp.StunModifierThresholds[key]);
  }

  private void InitializeModifier()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaModifierComponent, ComponentStartup>(new ComponentEventHandler<StaminaModifierComponent, ComponentStartup>((object) this, __methodptr(OnModifierStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaModifierComponent, ComponentShutdown>(new ComponentEventHandler<StaminaModifierComponent, ComponentShutdown>((object) this, __methodptr(OnModifierShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnModifierStartup(
    EntityUid uid,
    StaminaModifierComponent comp,
    ComponentStartup args)
  {
    StaminaComponent staminaComponent;
    if (!this.TryComp<StaminaComponent>(uid, ref staminaComponent))
      return;
    staminaComponent.CritThreshold *= comp.Modifier;
  }

  private void OnModifierShutdown(
    EntityUid uid,
    StaminaModifierComponent comp,
    ComponentShutdown args)
  {
    StaminaComponent staminaComponent;
    if (!this.TryComp<StaminaComponent>(uid, ref staminaComponent))
      return;
    staminaComponent.CritThreshold /= comp.Modifier;
  }

  public void SetModifier(
    EntityUid uid,
    float modifier,
    StaminaComponent? stamina = null,
    StaminaModifierComponent? comp = null)
  {
    if (!this.Resolve<StaminaModifierComponent>(uid, ref comp, true))
      return;
    float modifier1 = comp.Modifier;
    if (modifier1.Equals(modifier))
      return;
    comp.Modifier = modifier;
    this.Dirty(uid, (IComponent) comp, (MetaDataComponent) null);
    if (!this.Resolve<StaminaComponent>(uid, ref stamina, false))
      return;
    stamina.CritThreshold *= modifier / modifier1;
  }

  private void InitializeResistance()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaResistanceComponent, BeforeStaminaDamageEvent>(new EntityEventRefHandler<StaminaResistanceComponent, BeforeStaminaDamageEvent>((object) this, __methodptr(OnGetResistance)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaResistanceComponent, InventoryRelayedEvent<BeforeStaminaDamageEvent>>(new EntityEventRefHandler<StaminaResistanceComponent, InventoryRelayedEvent<BeforeStaminaDamageEvent>>((object) this, __methodptr(RelayedResistance)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StaminaResistanceComponent, ArmorExamineEvent>(new EntityEventRefHandler<StaminaResistanceComponent, ArmorExamineEvent>((object) this, __methodptr(OnArmorExamine)), (Type[]) null, (Type[]) null);
  }

  private void OnGetResistance(
    Entity<StaminaResistanceComponent> ent,
    ref BeforeStaminaDamageEvent args)
  {
    args.Value *= ent.Comp.DamageCoefficient;
  }

  private void RelayedResistance(
    Entity<StaminaResistanceComponent> ent,
    ref InventoryRelayedEvent<BeforeStaminaDamageEvent> args)
  {
    if (!ent.Comp.Worn)
      return;
    this.OnGetResistance(ent, ref args.Args);
  }

  private void OnArmorExamine(Entity<StaminaResistanceComponent> ent, ref ArmorExamineEvent args)
  {
    float num = MathF.Round((float) ((1.0 - (double) ent.Comp.DamageCoefficient) * 100.0), 1);
    if ((double) num == 0.0)
      return;
    args.Msg.PushNewline();
    args.Msg.AddMarkupOrThrow(this.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine), ("value", (object) num)));
  }
}
