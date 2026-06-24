// Decompiled with JetBrains decompiler
// Type: Content.Shared.Flash.SharedFlashSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Flash.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Light;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Content.Shared.Traits.Assorted;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Flash;

public abstract class SharedFlashSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private StatusEffectsSystem _statusEffectsSystem;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private UseDelaySystem _useDelay;
  private Robust.Shared.GameObjects.EntityQuery<StatusEffectsComponent> _statusEffectsQuery;
  private Robust.Shared.GameObjects.EntityQuery<DamagedByFlashingComponent> _damagedByFlashingQuery;
  private HashSet<EntityUid> _entSet = new HashSet<EntityUid>();
  private static readonly ProtoId<TagPrototype> TrashTag = (ProtoId<TagPrototype>) "Trash";
  public ProtoId<StatusEffectPrototype> FlashedKey = (ProtoId<StatusEffectPrototype>) "Flashed";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FlashComponent, MeleeHitEvent>(new EntityEventRefHandler<FlashComponent, MeleeHitEvent>(this.OnFlashMeleeHit));
    this.SubscribeLocalEvent<FlashComponent, UseInHandEvent>(new EntityEventRefHandler<FlashComponent, UseInHandEvent>(this.OnFlashUseInHand));
    this.SubscribeLocalEvent<FlashComponent, LightToggleEvent>(new EntityEventRefHandler<FlashComponent, LightToggleEvent>(this.OnLightToggle));
    this.SubscribeLocalEvent<PermanentBlindnessComponent, FlashAttemptEvent>(new EntityEventRefHandler<PermanentBlindnessComponent, FlashAttemptEvent>(this.OnPermanentBlindnessFlashAttempt));
    this.SubscribeLocalEvent<TemporaryBlindnessComponent, FlashAttemptEvent>(new EntityEventRefHandler<TemporaryBlindnessComponent, FlashAttemptEvent>(this.OnTemporaryBlindnessFlashAttempt));
    this.Subs.SubscribeWithRelay<FlashImmunityComponent, FlashAttemptEvent>(new EntityEventRefHandler<FlashImmunityComponent, FlashAttemptEvent>(this.OnFlashImmunityFlashAttempt), held: false);
    this.SubscribeLocalEvent<FlashImmunityComponent, ExaminedEvent>(new EntityEventRefHandler<FlashImmunityComponent, ExaminedEvent>(this.OnExamine));
    this._statusEffectsQuery = this.GetEntityQuery<StatusEffectsComponent>();
    this._damagedByFlashingQuery = this.GetEntityQuery<DamagedByFlashingComponent>();
  }

  private void OnFlashMeleeHit(Entity<FlashComponent> ent, ref MeleeHitEvent args)
  {
    if (!ent.Comp.FlashOnMelee || !args.IsHit || !args.HitEntities.Any<EntityUid>() || !this.UseFlash(ent, new EntityUid?(args.User)))
      return;
    args.Handled = true;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
      this.Flash(hitEntity, new EntityUid?(args.User), new EntityUid?(ent.Owner), ent.Comp.MeleeDuration, ent.Comp.SlowTo, melee: true, stunDuration: ent.Comp.MeleeStunDuration);
  }

  private void OnFlashUseInHand(Entity<FlashComponent> ent, ref UseInHandEvent args)
  {
    if (!ent.Comp.FlashOnUse || args.Handled || !this.UseFlash(ent, new EntityUid?(args.User)))
      return;
    args.Handled = true;
    this.FlashArea(ent.Owner, new EntityUid?(args.User), ent.Comp.Range, ent.Comp.AoeFlashDuration, ent.Comp.SlowTo, true, ent.Comp.Probability);
  }

  private void OnLightToggle(Entity<FlashComponent> ent, ref LightToggleEvent args)
  {
    if (!args.IsOn || !this.UseFlash(ent, new EntityUid?()))
      return;
    this.FlashArea(ent.Owner, new EntityUid?(), ent.Comp.Range, ent.Comp.AoeFlashDuration, ent.Comp.SlowTo, true, ent.Comp.Probability);
  }

  private bool UseFlash(Entity<FlashComponent> ent, EntityUid? user)
  {
    LimitedChargesComponent comp;
    if (this._useDelay.IsDelayed((Entity<UseDelayComponent>) ent.Owner) || this.TryComp<LimitedChargesComponent>(ent.Owner, out comp) && this._sharedCharges.IsEmpty((Entity<LimitedChargesComponent>) (ent.Owner, comp)))
      return false;
    this._sharedCharges.TryUseCharge((Entity<LimitedChargesComponent>) (ent.Owner, comp));
    this._audio.PlayPredicted(ent.Comp.Sound, ent.Owner, user);
    ActiveFlashComponent activeFlashComponent = this.EnsureComp<ActiveFlashComponent>(ent.Owner);
    activeFlashComponent.ActiveUntil = this._timing.CurTime + ent.Comp.FlashingTime;
    this.Dirty(ent.Owner, (IComponent) activeFlashComponent);
    this._appearance.SetData(ent.Owner, (Enum) FlashVisuals.Flashing, (object) true);
    if (this._sharedCharges.IsEmpty((Entity<LimitedChargesComponent>) (ent.Owner, comp)))
    {
      this._appearance.SetData(ent.Owner, (Enum) FlashVisuals.Burnt, (object) true);
      this._tag.AddTag(ent.Owner, SharedFlashSystem.TrashTag);
      this._popup.PopupClient(this.Loc.GetString("flash-component-becomes-empty"), user);
    }
    return true;
  }

  public void Flash(
    EntityUid target,
    EntityUid? user,
    EntityUid? used,
    TimeSpan flashDuration,
    float slowTo,
    bool displayPopup = true,
    bool melee = false,
    TimeSpan? stunDuration = null)
  {
    FlashAttemptEvent args1 = new FlashAttemptEvent(target, user, used);
    this.RaiseLocalEvent<FlashAttemptEvent>(target, ref args1, true);
    if (args1.Cancelled || !this._statusEffectsSystem.TryAddStatusEffect<FlashedComponent>(target, (string) this.FlashedKey, flashDuration, true))
      return;
    if (stunDuration.HasValue)
      this._stun.TryParalyze(target, stunDuration.Value, true);
    else
      this._stun.TrySlowdown(target, flashDuration, true, slowTo, slowTo);
    if (displayPopup && user.HasValue)
    {
      EntityUid entityUid = target;
      EntityUid? nullable = user;
      if ((nullable.HasValue ? (entityUid != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0 && this.Exists(user.Value))
      {
        SharedPopupSystem popup = this._popup;
        ILocalizationManager loc = this.Loc;
        EntityUid uid1 = user.Value;
        EntityManager entityManager = this.EntityManager;
        nullable = new EntityUid?();
        EntityUid? viewer = nullable;
        (string, object) valueTuple = (nameof (user), (object) Identity.Entity(uid1, (IEntityManager) entityManager, viewer));
        string message = loc.GetString("flash-component-user-blinds-you", valueTuple);
        EntityUid uid2 = target;
        EntityUid recipient = target;
        popup.PopupEntity(message, uid2, recipient);
      }
    }
    AfterFlashedEvent args2 = new AfterFlashedEvent(target, user, used, melee);
    this.RaiseLocalEvent<AfterFlashedEvent>(target, ref args2);
    if (user.HasValue)
      this.RaiseLocalEvent<AfterFlashedEvent>(user.Value, ref args2);
    if (!used.HasValue)
      return;
    this.RaiseLocalEvent<AfterFlashedEvent>(used.Value, ref args2);
  }

  public void FlashArea(
    EntityUid source,
    EntityUid? user,
    float range,
    TimeSpan flashDuration,
    float slowTo = 0.8f,
    bool displayPopup = false,
    float probability = 1f,
    SoundSpecifier? sound = null)
  {
    TransformComponent xform = this.Transform(source);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(xform);
    this._entSet.Clear();
    this._entityLookup.GetEntitiesInRange(xform.Coordinates, range, this._entSet);
    foreach (EntityUid ent in this._entSet)
    {
      if (new Random((int) this._timing.CurTick.Value + this.GetNetEntity(ent).Id).NextDouble() < (double) probability && (this._statusEffectsQuery.HasComponent(ent) || this._damagedByFlashingQuery.HasComponent(ent)) && this._examine.InRangeUnOccluded(ent, mapCoordinates, range, (SharedInteractionSystem.Ignored) (e => this._damagedByFlashingQuery.HasComponent(e))))
        this.Flash(ent, user, new EntityUid?(source), flashDuration, slowTo, displayPopup);
    }
    this._audio.PlayPredicted(sound, source, user, new AudioParams?(AudioParams.Default.WithVolume(1f).WithMaxDistance(3f)));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveFlashComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveFlashComponent>();
    EntityUid uid;
    ActiveFlashComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.ActiveUntil < curTime)
      {
        this._appearance.SetData(uid, (Enum) FlashVisuals.Flashing, (object) false);
        this.RemCompDeferred<ActiveFlashComponent>(uid);
      }
    }
  }

  private void OnPermanentBlindnessFlashAttempt(
    Entity<PermanentBlindnessComponent> ent,
    ref FlashAttemptEvent args)
  {
    if (ent.Comp.Blindness != 0)
      return;
    args.Cancelled = true;
  }

  private void OnTemporaryBlindnessFlashAttempt(
    Entity<TemporaryBlindnessComponent> ent,
    ref FlashAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnFlashImmunityFlashAttempt(
    Entity<FlashImmunityComponent> ent,
    ref FlashAttemptEvent args)
  {
    if (!ent.Comp.Enabled)
      return;
    args.Cancelled = true;
  }

  private void OnExamine(Entity<FlashImmunityComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("flash-protection"));
  }

  public virtual bool Flash(
    EntityUid target,
    EntityUid? user,
    EntityUid? used,
    float flashDuration,
    float slowTo = 0.8f,
    bool displayPopup = true,
    bool melee = false,
    TimeSpan? stunDuration = null)
  {
    return false;
  }
}
