using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Examine;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Flash.Components;
using Content.Shared.IdentityManagement;
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
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

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

	private EntityQuery<StatusEffectsComponent> _statusEffectsQuery;

	private EntityQuery<DamagedByFlashingComponent> _damagedByFlashingQuery;

	private HashSet<EntityUid> _entSet = new HashSet<EntityUid>();

	private static readonly ProtoId<TagPrototype> TrashTag = ProtoId<TagPrototype>.op_Implicit("Trash");

	public ProtoId<StatusEffectPrototype> FlashedKey = ProtoId<StatusEffectPrototype>.op_Implicit("Flashed");

	public override void Initialize()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FlashComponent, MeleeHitEvent>((EntityEventRefHandler<FlashComponent, MeleeHitEvent>)OnFlashMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlashComponent, UseInHandEvent>((EntityEventRefHandler<FlashComponent, UseInHandEvent>)OnFlashUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FlashComponent, LightToggleEvent>((EntityEventRefHandler<FlashComponent, LightToggleEvent>)OnLightToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PermanentBlindnessComponent, FlashAttemptEvent>((EntityEventRefHandler<PermanentBlindnessComponent, FlashAttemptEvent>)OnPermanentBlindnessFlashAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TemporaryBlindnessComponent, FlashAttemptEvent>((EntityEventRefHandler<TemporaryBlindnessComponent, FlashAttemptEvent>)OnTemporaryBlindnessFlashAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.SubscribeWithRelay<FlashImmunityComponent, FlashAttemptEvent>((EntityEventRefHandler<FlashImmunityComponent, FlashAttemptEvent>)OnFlashImmunityFlashAttempt, true, true, false);
		((EntitySystem)this).SubscribeLocalEvent<FlashImmunityComponent, ExaminedEvent>((EntityEventRefHandler<FlashImmunityComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		_statusEffectsQuery = ((EntitySystem)this).GetEntityQuery<StatusEffectsComponent>();
		_damagedByFlashingQuery = ((EntitySystem)this).GetEntityQuery<DamagedByFlashingComponent>();
	}

	private void OnFlashMeleeHit(Entity<FlashComponent> ent, ref MeleeHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.FlashOnMelee || !args.IsHit || !args.HitEntities.Any() || !UseFlash(ent, args.User))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		foreach (EntityUid target in args.HitEntities)
		{
			Flash(target, args.User, ent.Owner, ent.Comp.MeleeDuration, ent.Comp.SlowTo, displayPopup: true, melee: true, ent.Comp.MeleeStunDuration);
		}
	}

	private void OnFlashUseInHand(Entity<FlashComponent> ent, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.FlashOnUse && !((HandledEntityEventArgs)args).Handled && UseFlash(ent, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
			FlashArea(ent.Owner, args.User, ent.Comp.Range, ent.Comp.AoeFlashDuration, ent.Comp.SlowTo, displayPopup: true, ent.Comp.Probability);
		}
	}

	private void OnLightToggle(Entity<FlashComponent> ent, ref LightToggleEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsOn && UseFlash(ent, null))
		{
			FlashArea(ent.Owner, null, ent.Comp.Range, ent.Comp.AoeFlashDuration, ent.Comp.SlowTo, displayPopup: true, ent.Comp.Probability);
		}
	}

	private bool UseFlash(Entity<FlashComponent> ent, EntityUid? user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (_useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit(ent.Owner)))
		{
			return false;
		}
		LimitedChargesComponent charges = default(LimitedChargesComponent);
		if (((EntitySystem)this).TryComp<LimitedChargesComponent>(ent.Owner, ref charges) && _sharedCharges.IsEmpty(Entity<LimitedChargesComponent>.op_Implicit((ent.Owner, charges))))
		{
			return false;
		}
		_sharedCharges.TryUseCharge(Entity<LimitedChargesComponent>.op_Implicit((ent.Owner, charges)));
		_audio.PlayPredicted(ent.Comp.Sound, ent.Owner, user, (AudioParams?)null);
		ActiveFlashComponent active = ((EntitySystem)this).EnsureComp<ActiveFlashComponent>(ent.Owner);
		active.ActiveUntil = _timing.CurTime + ent.Comp.FlashingTime;
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)active, (MetaDataComponent)null);
		_appearance.SetData(ent.Owner, (Enum)FlashVisuals.Flashing, (object)true, (AppearanceComponent)null);
		if (_sharedCharges.IsEmpty(Entity<LimitedChargesComponent>.op_Implicit((ent.Owner, charges))))
		{
			_appearance.SetData(ent.Owner, (Enum)FlashVisuals.Burnt, (object)true, (AppearanceComponent)null);
			_tag.AddTag(ent.Owner, TrashTag);
			_popup.PopupClient(base.Loc.GetString("flash-component-becomes-empty"), user);
		}
		return true;
	}

	public void Flash(EntityUid target, EntityUid? user, EntityUid? used, TimeSpan flashDuration, float slowTo, bool displayPopup = true, bool melee = false, TimeSpan? stunDuration = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		FlashAttemptEvent attempt = new FlashAttemptEvent(target, user, used);
		((EntitySystem)this).RaiseLocalEvent<FlashAttemptEvent>(target, ref attempt, true);
		if (attempt.Cancelled || !_statusEffectsSystem.TryAddStatusEffect<FlashedComponent>(target, ProtoId<StatusEffectPrototype>.op_Implicit(FlashedKey), flashDuration, true, (StatusEffectsComponent?)null, false))
		{
			return;
		}
		if (stunDuration.HasValue)
		{
			_stun.TryParalyze(target, stunDuration.Value, refresh: true);
		}
		else
		{
			_stun.TrySlowdown(target, flashDuration, refresh: true, slowTo, slowTo);
		}
		if (displayPopup && user.HasValue)
		{
			EntityUid? val = user;
			if ((!val.HasValue || target != val.GetValueOrDefault()) && ((EntitySystem)this).Exists(user.Value))
			{
				_popup.PopupEntity(base.Loc.GetString("flash-component-user-blinds-you", (ValueTuple<string, object>)("user", Identity.Entity(user.Value, (IEntityManager)(object)base.EntityManager))), target, target);
			}
		}
		AfterFlashedEvent ev = new AfterFlashedEvent(target, user, used, melee);
		((EntitySystem)this).RaiseLocalEvent<AfterFlashedEvent>(target, ref ev, false);
		if (user.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<AfterFlashedEvent>(user.Value, ref ev, false);
		}
		if (used.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<AfterFlashedEvent>(used.Value, ref ev, false);
		}
	}

	public void FlashArea(EntityUid source, EntityUid? user, float range, TimeSpan flashDuration, float slowTo = 0.8f, bool displayPopup = false, float probability = 1f, SoundSpecifier? sound = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = ((EntitySystem)this).Transform(source);
		MapCoordinates mapPosition = _transform.GetMapCoordinates(transform);
		_entSet.Clear();
		_entityLookup.GetEntitiesInRange(transform.Coordinates, range, _entSet, (LookupFlags)110);
		foreach (EntityUid entity in _entSet)
		{
			if (!(new System.Random((int)_timing.CurTick.Value + ((EntitySystem)this).GetNetEntity(entity, (MetaDataComponent)null).Id).NextDouble() >= (double)probability) && (_statusEffectsQuery.HasComponent(entity) || _damagedByFlashingQuery.HasComponent(entity)) && _examine.InRangeUnOccluded(entity, mapPosition, range, (EntityUid e) => _damagedByFlashingQuery.HasComponent(e)))
			{
				Flash(entity, user, source, flashDuration, slowTo, displayPopup);
			}
		}
		SharedAudioSystem audio = _audio;
		AudioParams val = ((AudioParams)(ref AudioParams.Default)).WithVolume(1f);
		audio.PlayPredicted(sound, source, user, (AudioParams?)((AudioParams)(ref val)).WithMaxDistance(3f));
	}

	public override void Update(float frameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		TimeSpan curTime = _timing.CurTime;
		EntityQueryEnumerator<ActiveFlashComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveFlashComponent>();
		EntityUid uid = default(EntityUid);
		ActiveFlashComponent active = default(ActiveFlashComponent);
		while (query.MoveNext(ref uid, ref active))
		{
			if (active.ActiveUntil < curTime)
			{
				_appearance.SetData(uid, (Enum)FlashVisuals.Flashing, (object)false, (AppearanceComponent)null);
				((EntitySystem)this).RemCompDeferred<ActiveFlashComponent>(uid);
			}
		}
	}

	private void OnPermanentBlindnessFlashAttempt(Entity<PermanentBlindnessComponent> ent, ref FlashAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Blindness == 0)
		{
			args.Cancelled = true;
		}
	}

	private void OnTemporaryBlindnessFlashAttempt(Entity<TemporaryBlindnessComponent> ent, ref FlashAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnFlashImmunityFlashAttempt(Entity<FlashImmunityComponent> ent, ref FlashAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Enabled)
		{
			args.Cancelled = true;
		}
	}

	private void OnExamine(Entity<FlashImmunityComponent> ent, ref ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("flash-protection"));
	}

	public virtual bool Flash(EntityUid target, EntityUid? user, EntityUid? used, float flashDuration, float slowTo = 0.8f, bool displayPopup = true, bool melee = false, TimeSpan? stunDuration = null)
	{
		return false;
	}
}
