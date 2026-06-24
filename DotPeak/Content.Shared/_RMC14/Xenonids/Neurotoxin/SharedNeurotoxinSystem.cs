// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Neurotoxin.SharedNeurotoxinSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.OrbitalCannon;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stamina;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.ActionBlocker;
using Content.Shared.Chat;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Drugs;
using Content.Shared.Drunk;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Random.Helpers;
using Content.Shared.Rejuvenate;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Neurotoxin;

public abstract class SharedNeurotoxinSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private RMCStaminaSystem _stamina;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private SharedSlurredSystem _slurred;
  [Dependency]
  private SharedStutteringSystem _stutter;
  [Dependency]
  private RMCDazedSystem _daze;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedDeafnessSystem _deafness;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedCMChatSystem _rmcChat;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private IPrototypeManager _proto;
  private readonly HashSet<Entity<MarineComponent>> _marines = new HashSet<Entity<MarineComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NeurotoxinComponent, RejuvenateEvent>(new EntityEventRefHandler<NeurotoxinComponent, RejuvenateEvent>(this.OnRejuvenate));
    this.SubscribeLocalEvent<NeurotoxinInjectorComponent, ProjectileHitEvent>(new EntityEventRefHandler<NeurotoxinInjectorComponent, ProjectileHitEvent>(this.OnProjectileHit));
  }

  private void OnRejuvenate(Entity<NeurotoxinComponent> ent, ref RejuvenateEvent args)
  {
    this.RemCompDeferred<NeurotoxinComponent>((EntityUid) ent);
  }

  private void OnProjectileHit(Entity<NeurotoxinInjectorComponent> ent, ref ProjectileHitEvent args)
  {
    if (!this.HasComp<MarineComponent>(args.Target) || !ent.Comp.AffectsDead && this._mobState.IsDead(args.Target) || !ent.Comp.AffectsInfectedNested && this.HasComp<XenoNestedComponent>(args.Target) && this.HasComp<VictimInfectedComponent>(args.Target))
      return;
    TimeSpan curTime = this._timing.CurTime;
    NeurotoxinComponent comp;
    if (!this.EnsureComp<NeurotoxinComponent>(args.Target, out comp))
    {
      comp.LastMessage = curTime;
      comp.LastAccentTime = curTime;
      comp.LastStumbleTime = curTime;
    }
    this._statusEffects.TryAddStatusEffect<RMCBlindedComponent>(args.Target, "Blinded", comp.BlurTime * 6.0, true);
    this._daze.TryDaze((EntityUid) ent, ent.Comp.DazeTime, true, stutter: true);
    comp.NeurotoxinAmount += ent.Comp.NeuroPerSecond;
    comp.ToxinDamage = ent.Comp.ToxinDamage;
    comp.OxygenDamage = ent.Comp.OxygenDamage;
    comp.CoughDamage = ent.Comp.CoughDamage;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<NeurotoxinInjectorComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<NeurotoxinInjectorComponent>();
    EntityUid uid1;
    NeurotoxinInjectorComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (comp1_1.InjectInContact)
      {
        this._marines.Clear();
        this._entityLookup.GetEntitiesInRange<MarineComponent>(uid1.ToCoordinates(), 0.5f, this._marines);
        foreach (Entity<MarineComponent> marine in this._marines)
        {
          if ((comp1_1.AffectsDead || !this._mobState.IsDead((EntityUid) marine)) && (comp1_1.AffectsInfectedNested || !this.HasComp<XenoNestedComponent>((EntityUid) marine) || !this.HasComp<VictimInfectedComponent>((EntityUid) marine)))
          {
            NeurotoxinComponent comp;
            if (!this.EnsureComp<NeurotoxinComponent>((EntityUid) marine, out comp))
            {
              comp.LastMessage = curTime;
              comp.LastAccentTime = curTime;
              comp.LastStumbleTime = curTime;
              comp.NextGasInjectionAt = curTime;
              comp.NextNeuroEffectAt = curTime;
            }
            if (!(curTime < comp.NextGasInjectionAt))
            {
              this._statusEffects.TryAddStatusEffect<RMCBlindedComponent>((EntityUid) marine, "Blinded", comp.BlurTime * 12.0, true);
              this._daze.TryDaze((EntityUid) marine, comp1_1.DazeTime, true, stutter: true);
              comp.NeurotoxinAmount += comp1_1.NeuroPerSecond;
              comp.ToxinDamage = comp1_1.ToxinDamage;
              comp.OxygenDamage = comp1_1.OxygenDamage;
              comp.CoughDamage = comp1_1.CoughDamage;
              comp.NextGasInjectionAt = curTime + comp1_1.TimeBetweenGasInjects;
            }
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<NeurotoxinComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<NeurotoxinComponent>();
    EntityUid uid2;
    NeurotoxinComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(curTime < comp1_2.NextNeuroEffectAt))
      {
        comp1_2.NeurotoxinAmount -= comp1_2.DepletionPerTick;
        comp1_2.NextNeuroEffectAt = curTime + comp1_2.UpdateEvery;
        if ((double) comp1_2.NeurotoxinAmount <= 0.0 || this.HasComp<SynthComponent>(uid2))
          this.RemCompDeferred<NeurotoxinComponent>(uid2);
        else if (!this._mobState.IsDead(uid2))
        {
          this._stamina.DoStaminaDamage((Entity<RMCStaminaComponent>) uid2, (double) comp1_2.StaminaDamagePerTick, false);
          this._statusEffects.TryAddStatusEffect<DrunkComponent>(uid2, "Drunk", comp1_2.DizzyStrength, true);
          float coughChance;
          float stumbleChance;
          this.NeurotoxinNonStackingEffects(uid2, comp1_2, curTime, out coughChance, out stumbleChance);
          this.NeurotoxinStackingEffects(uid2, comp1_2, curTime);
          NeurotoxinEmoteEvent neurotoxinEmoteEvent;
          if (this._random.Prob(stumbleChance) && curTime - comp1_2.LastStumbleTime >= comp1_2.MinimumDelayBetweenEvents)
          {
            comp1_2.LastStumbleTime = curTime;
            if (this._blocker.CanMove(uid2))
            {
              this._rmcPulling.TryStopPullsOn(uid2);
              this._physics.SetLinearVelocity(uid2, Vector2.Zero);
              this._physics.SetAngularVelocity(uid2, 0.0f);
              ThrowingSystem throwing = this._throwing;
              EntityUid uid3 = uid2;
              Angle angle = this._random.NextAngle();
              Vector2 direction = Vector2Helpers.Normalized(((Angle) ref angle).ToVec()) / 10f;
              EntityUid? user = new EntityUid?();
              float? friction = new float?();
              throwing.TryThrow(uid3, direction, user: user, friction: friction, compensateFriction: true, animated: false, playSound: false, doSpin: false);
            }
            this._popup.PopupEntity(this.Loc.GetString("rmc-stumble-others", ("victim", (object) uid2)), uid2, Filter.PvsExcept(uid2), true, PopupType.SmallCaution);
            this._popup.PopupEntity(this.Loc.GetString("rmc-stumble"), uid2, uid2, PopupType.MediumCaution);
            this._daze.TryDaze(uid2, comp1_2.DazeLength * 5.0, true, stutter: true);
            this._jitter.DoJitter(uid2, comp1_2.StumbleJitterTime, true);
            this._statusEffects.TryAddStatusEffect<DrunkComponent>(uid2, "Drunk", comp1_2.DizzyStrengthOnStumble, true);
            neurotoxinEmoteEvent = new NeurotoxinEmoteEvent();
            neurotoxinEmoteEvent.Emote = comp1_2.PainId;
            NeurotoxinEmoteEvent args = neurotoxinEmoteEvent;
            this.RaiseLocalEvent<NeurotoxinEmoteEvent>(uid2, args);
          }
          if (this._random.Prob(coughChance))
          {
            this._slow.TrySlowdown(uid2, comp1_2.BloodCoughDuration);
            this._damage.TryChangeDamage(new EntityUid?(uid2), comp1_2.CoughDamage);
            this._popup.PopupEntity(this.Loc.GetString("rmc-bloodcough"), uid2, uid2, PopupType.MediumCaution);
            neurotoxinEmoteEvent = new NeurotoxinEmoteEvent();
            neurotoxinEmoteEvent.Emote = comp1_2.CoughId;
            NeurotoxinEmoteEvent args = neurotoxinEmoteEvent;
            this.RaiseLocalEvent<NeurotoxinEmoteEvent>(uid2, args);
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<NeurotoxinLingeringHallucinationComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<NeurotoxinLingeringHallucinationComponent>();
    EntityUid uid4;
    NeurotoxinLingeringHallucinationComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid4, out comp1_3))
    {
      if (comp1_3.Hallucinations.Count == 0)
      {
        this.RemCompDeferred<NeurotoxinLingeringHallucinationComponent>(uid4);
      }
      else
      {
        List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> toRemove = new List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>();
        List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> collection = new List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>();
        foreach ((NeuroHallucinations, int, TimeSpan, EntityCoordinates?) hallucination in comp1_3.Hallucinations)
        {
          if (!(hallucination.Item3 > curTime))
          {
            (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)? nullable = this.ProcessHallucination(uid4, comp1_3, hallucination);
            toRemove.Add(hallucination);
            if (nullable.HasValue)
              collection.Add(nullable.Value);
          }
        }
        comp1_3.Hallucinations.RemoveAll((Predicate<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>) (a => toRemove.Contains(a)));
        comp1_3.Hallucinations.AddRange((IEnumerable<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>) collection);
      }
    }
  }

  private void NeurotoxinNonStackingEffects(
    EntityUid victim,
    NeurotoxinComponent neurotoxin,
    TimeSpan time,
    out float coughChance,
    out float stumbleChance)
  {
    string messageId = "rmc-neuro-tired";
    PopupType type = PopupType.Small;
    coughChance = 0.0f;
    stumbleChance = 0.0f;
    if ((double) neurotoxin.NeurotoxinAmount > 9.0)
    {
      if ((double) neurotoxin.NeurotoxinAmount <= 14.0)
      {
        messageId = "rmc-neuro-numb";
        type = PopupType.SmallCaution;
        coughChance = 0.1f;
      }
      else if ((double) neurotoxin.NeurotoxinAmount <= 19.0)
      {
        if (this._random.Next(4) == 0)
        {
          messageId = "rmc-neuro-where";
          type = PopupType.Large;
        }
        else
        {
          messageId = RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) new List<string>()
          {
            "rmc-neuro-very-numb",
            "rmc-neuro-erratic",
            "rmc-neuro-panic"
          });
          type = PopupType.MediumCaution;
        }
        coughChance = 0.1f;
        stumbleChance = 0.05f;
      }
      else if ((double) neurotoxin.NeurotoxinAmount <= 24.0)
      {
        messageId = "rmc-neuro-sting";
        type = PopupType.MediumCaution;
        coughChance = 0.25f;
        stumbleChance = 0.25f;
      }
      else
      {
        switch (this._random.Next(7))
        {
          case 0:
            messageId = "rmc-neuro-what";
            type = PopupType.Large;
            break;
          case 1:
            messageId = "rmc-neuro-hearing";
            type = PopupType.MediumCaution;
            break;
          default:
            messageId = RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) new List<string>()
            {
              "rmc-neuro-pain",
              "rmc-neuro-agh",
              "rmc-neuro-so-numb",
              "rmc-neuro-limbs",
              "rmc-neuro-think"
            });
            type = PopupType.LargeCaution;
            break;
        }
        coughChance = 0.25f;
        stumbleChance = 0.25f;
      }
    }
    if (!(time - neurotoxin.LastMessage >= neurotoxin.TimeBetweenMessages))
      return;
    neurotoxin.LastMessage = time;
    this._popup.PopupEntity(this.Loc.GetString(messageId), victim, victim, type);
  }

  private void NeurotoxinStackingEffects(
    EntityUid victim,
    NeurotoxinComponent neurotoxin,
    TimeSpan currTime)
  {
    if ((double) neurotoxin.NeurotoxinAmount >= 10.0)
    {
      this._statusEffects.TryAddStatusEffect<RMCBlindedComponent>(victim, "Blinded", neurotoxin.BlurTime, true);
      if (currTime - neurotoxin.LastAccentTime >= neurotoxin.MinimumDelayBetweenEvents)
      {
        neurotoxin.LastAccentTime = currTime;
        if (this._random.Prob(0.5f))
          this._slurred.DoSlur(victim, neurotoxin.AccentTime);
        else
          this._stutter.DoStutter(victim, neurotoxin.AccentTime, true);
      }
    }
    if ((double) neurotoxin.NeurotoxinAmount >= 15.0)
    {
      this._jitter.DoJitter(victim, neurotoxin.JitterTime, true);
      if (currTime >= neurotoxin.NextHallucination)
      {
        neurotoxin.NextHallucination = currTime + this._random.Next(neurotoxin.HallucinationEveryMin, neurotoxin.HallucinationEveryMax);
        this.DoNeuroHallucination(victim, neurotoxin);
      }
    }
    if ((double) neurotoxin.NeurotoxinAmount >= 20.0)
      this._statusEffects.TryAddStatusEffect<TemporaryBlindnessComponent>(victim, "TemporaryBlindness", neurotoxin.BlindTime, true);
    if ((double) neurotoxin.NeurotoxinAmount >= 27.0)
    {
      this._daze.TryDaze(victim, neurotoxin.DazeLength, true, stutter: true);
      this._damage.TryChangeDamage(new EntityUid?(victim), neurotoxin.ToxinDamage);
      this._deafness.TryDeafen(victim, neurotoxin.DeafenTime, true, ignoreProtection: true);
    }
    if ((double) neurotoxin.NeurotoxinAmount < 50.0)
      return;
    this._damage.TryChangeDamage(new EntityUid?(victim), neurotoxin.OxygenDamage);
  }

  private void DoNeuroHallucination(EntityUid victim, NeurotoxinComponent neurotoxin)
  {
    switch (this._random.Pick<NeuroHallucinations>(neurotoxin.Hallucinations))
    {
      case NeuroHallucinations.AlienAttack:
        this._audio.PlayStatic(neurotoxin.Pounce, victim, victim.ToCoordinates());
        this._stun.TryParalyze(victim, neurotoxin.PounceDownTime, true);
        this.EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.AlienAttack, 0, this._timing.CurTime + TimeSpan.FromSeconds(1L), new EntityCoordinates?()));
        break;
      case NeuroHallucinations.OB:
        ICommonSession session;
        if (this._player.TryGetSessionByEntity(victim, out session))
        {
          string str1 = $"[bold][font size=24][color=red]\n[font size=16][color=red]Orbital bombardment launch command detected![/color][/font]\n[/color][/font][/bold]";
          this._rmcChat.ChatMessageToOne(ChatChannel.Radio, str1, str1, new EntityUid(), false, session.Channel, recordReplay: true);
          EntityPrototype areaPrototype;
          EntityPrototype prototype;
          if (this._area.TryGetArea(victim.ToCoordinates(), out Entity<AreaComponent>? _, out areaPrototype) && this._proto.TryIndex((EntProtoId) RandomExtensions.Pick<EntProtoId<OrbitalCannonWarheadComponent>>(this._random, (IReadOnlyList<EntProtoId<OrbitalCannonWarheadComponent>>) neurotoxin.WarheadTypes), out prototype))
          {
            string str2 = $"[color=red]Launch command informs {prototype.Name}. Estimated impact area: {areaPrototype.Name}[/color]";
            this._rmcChat.ChatMessageToOne(ChatChannel.Radio, str2, str2, new EntityUid(), false, session.Channel, recordReplay: true);
          }
        }
        this._audio.PlayGlobal(neurotoxin.OBAlert, victim);
        this.EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.OB, 0, this._timing.CurTime + TimeSpan.FromSeconds(2L), new EntityCoordinates?()));
        break;
      case NeuroHallucinations.Screech:
        this._audio.PlayStatic(neurotoxin.Screech, victim, this.HallucinationSoundOffset(victim, 3f));
        this._stun.TryParalyze(victim, neurotoxin.ScreechDownTime, true);
        break;
      case NeuroHallucinations.CAS:
        EntityCoordinates coordinates = this.HallucinationSoundOffset(victim, 7f);
        this._audio.PlayStatic(neurotoxin.FiremissionStart, victim, coordinates);
        this.EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.CAS, 0, this._timing.CurTime + TimeSpan.FromSeconds(3.5), new EntityCoordinates?(coordinates)));
        break;
      case NeuroHallucinations.Mortar:
        EntityCoordinates coords = this.HallucinationSoundOffset(victim, 7f);
        this.FakeWarning(coords, victim, (LocId) "rmc-mortar-shell-impact-warning", (LocId) "rmc-mortar-shell-impact-warning-above");
        this.EnsureComp<NeurotoxinLingeringHallucinationComponent>(victim).Hallucinations.Add((NeuroHallucinations.Mortar, 0, this._timing.CurTime + TimeSpan.FromSeconds(1L), new EntityCoordinates?(coords)));
        break;
      case NeuroHallucinations.Giggle:
        NeurotoxinEmoteEvent args = new NeurotoxinEmoteEvent()
        {
          Emote = neurotoxin.GiggleId
        };
        this.RaiseLocalEvent<NeurotoxinEmoteEvent>(victim, args);
        this._statusEffects.TryAddStatusEffect<SeeingRainbowsStatusEffectComponent>(victim, "StatusEffectSeeingRainbow", neurotoxin.RainbowDuration, true);
        break;
      case NeuroHallucinations.Sounds:
        this._audio.PlayStatic(RandomExtensions.Pick<SoundSpecifier>(this._random, (IReadOnlyList<SoundSpecifier>) neurotoxin.HallucinationRandomSounds), victim, this.HallucinationSoundOffset(victim, 7f));
        break;
    }
  }

  private (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)? ProcessHallucination(
    EntityUid victim,
    NeurotoxinLingeringHallucinationComponent lingering,
    (NeuroHallucinations, int, TimeSpan, EntityCoordinates?) hallucination)
  {
    switch (hallucination.Item1)
    {
      case NeuroHallucinations.AlienAttack:
        if (hallucination.Item2 == 0)
        {
          this._audio.PlayStatic(lingering.XenoClaw, victim, victim.ToCoordinates());
          this._audio.PlayStatic(lingering.BoneBreak, victim, victim.ToCoordinates());
          hallucination.Item2 = 1;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 < 3)
        {
          this._audio.PlayStatic(lingering.XenoClaw, victim, victim.ToCoordinates());
          ++hallucination.Item2;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        this._audio.PlayStatic(lingering.BoneBreak, victim, victim.ToCoordinates());
        NeurotoxinEmoteEvent args1 = new NeurotoxinEmoteEvent()
        {
          Emote = lingering.PainEmote
        };
        this.RaiseLocalEvent<NeurotoxinEmoteEvent>(victim, args1);
        break;
      case NeuroHallucinations.OB:
        this._audio.PlayStatic(lingering.OBTravel, victim, this.HallucinationSoundOffset(victim, 7f));
        break;
      case NeuroHallucinations.CAS:
        if (hallucination.Item2 == 0)
        {
          this.FakeWarning(hallucination.Item4 ?? victim.ToCoordinates(), victim, (LocId) "rmc-dropship-firemission-warning", (LocId) "rmc-dropship-firemission-warning-above");
          hallucination.Item2 = 1;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 1)
        {
          this._audio.PlayStatic(lingering.RocketFire, victim, this.HallucinationSoundOffset(hallucination.Item4 ?? victim.ToCoordinates(), 7f));
          hallucination.Item2 = 2;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 2)
        {
          this._audio.PlayStatic(lingering.GauFire, victim, this.HallucinationSoundOffset(hallucination.Item4 ?? victim.ToCoordinates(), 7f));
          hallucination.Item2 = 3;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 3)
        {
          SharedAudioSystem audio1 = this._audio;
          SoundSpecifier rocketFire = lingering.RocketFire;
          EntityUid recipient1 = victim;
          EntityCoordinates? nullable = hallucination.Item4;
          EntityCoordinates coordinates1 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams1 = new AudioParams?();
          audio1.PlayStatic(rocketFire, recipient1, coordinates1, audioParams1);
          SharedAudioSystem audio2 = this._audio;
          SoundSpecifier gauHit1 = lingering.GauHit;
          EntityUid recipient2 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates2 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams2 = new AudioParams?();
          audio2.PlayStatic(gauHit1, recipient2, coordinates2, audioParams2);
          SharedAudioSystem audio3 = this._audio;
          SoundSpecifier gauHit2 = lingering.GauHit;
          EntityUid recipient3 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates3 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams3 = new AudioParams?();
          audio3.PlayStatic(gauHit2, recipient3, coordinates3, audioParams3);
          hallucination.Item2 = 4;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(1L);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 4)
        {
          SharedAudioSystem audio4 = this._audio;
          SoundSpecifier explosion = lingering.Explosion;
          EntityUid recipient4 = victim;
          EntityCoordinates? nullable = hallucination.Item4;
          EntityCoordinates coordinates4 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams4 = new AudioParams?();
          audio4.PlayStatic(explosion, recipient4, coordinates4, audioParams4);
          SharedAudioSystem audio5 = this._audio;
          SoundSpecifier gauHit = lingering.GauHit;
          EntityUid recipient5 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates5 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams5 = new AudioParams?();
          audio5.PlayStatic(gauHit, recipient5, coordinates5, audioParams5);
          hallucination.Item2 = 5;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(1L);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 5)
        {
          SharedAudioSystem audio6 = this._audio;
          SoundSpecifier rocketFire = lingering.RocketFire;
          EntityUid recipient6 = victim;
          EntityCoordinates? nullable = hallucination.Item4;
          EntityCoordinates coordinates6 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams6 = new AudioParams?();
          audio6.PlayStatic(rocketFire, recipient6, coordinates6, audioParams6);
          SharedAudioSystem audio7 = this._audio;
          SoundSpecifier gauHit = lingering.GauHit;
          EntityUid recipient7 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates7 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams7 = new AudioParams?();
          audio7.PlayStatic(gauHit, recipient7, coordinates7, audioParams7);
          hallucination.Item2 = 6;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(1L);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 6)
        {
          SharedAudioSystem audio8 = this._audio;
          SoundSpecifier explosion = lingering.Explosion;
          EntityUid recipient8 = victim;
          EntityCoordinates? nullable = hallucination.Item4;
          EntityCoordinates coordinates8 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams8 = new AudioParams?();
          audio8.PlayStatic(explosion, recipient8, coordinates8, audioParams8);
          SharedAudioSystem audio9 = this._audio;
          SoundSpecifier gauHit3 = lingering.GauHit;
          EntityUid recipient9 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates9 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams9 = new AudioParams?();
          audio9.PlayStatic(gauHit3, recipient9, coordinates9, audioParams9);
          SharedAudioSystem audio10 = this._audio;
          SoundSpecifier gauHit4 = lingering.GauHit;
          EntityUid recipient10 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates10 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams10 = new AudioParams?();
          audio10.PlayStatic(gauHit4, recipient10, coordinates10, audioParams10);
          hallucination.Item2 = 7;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 7)
        {
          SharedAudioSystem audio11 = this._audio;
          SoundSpecifier bigExplosion = lingering.BigExplosion;
          EntityUid recipient11 = victim;
          EntityCoordinates? nullable = hallucination.Item4;
          EntityCoordinates coordinates11 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams11 = new AudioParams?();
          audio11.PlayStatic(bigExplosion, recipient11, coordinates11, audioParams11);
          SharedAudioSystem audio12 = this._audio;
          SoundSpecifier gauHit = lingering.GauHit;
          EntityUid recipient12 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates12 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams12 = new AudioParams?();
          audio12.PlayStatic(gauHit, recipient12, coordinates12, audioParams12);
          hallucination.Item2 = 8;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 8)
        {
          this._audio.PlayStatic(lingering.RocketFire, victim, this.HallucinationSoundOffset(hallucination.Item4 ?? victim.ToCoordinates(), 7f));
          hallucination.Item2 = 9;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 9)
        {
          SharedAudioSystem audio13 = this._audio;
          SoundSpecifier gauHit = lingering.GauHit;
          EntityUid recipient13 = victim;
          EntityCoordinates? nullable = hallucination.Item4;
          EntityCoordinates coordinates13 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams13 = new AudioParams?();
          audio13.PlayStatic(gauHit, recipient13, coordinates13, audioParams13);
          SharedAudioSystem audio14 = this._audio;
          SoundSpecifier explosion = lingering.Explosion;
          EntityUid recipient14 = victim;
          nullable = hallucination.Item4;
          EntityCoordinates coordinates14 = this.HallucinationSoundOffset(nullable ?? victim.ToCoordinates(), 7f);
          AudioParams? audioParams14 = new AudioParams?();
          audio14.PlayStatic(explosion, recipient14, coordinates14, audioParams14);
          hallucination.Item2 = 10;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        if (hallucination.Item2 == 10)
        {
          this._audio.PlayStatic(lingering.GauHit, victim, this.HallucinationSoundOffset(hallucination.Item4 ?? victim.ToCoordinates(), 7f));
          hallucination.Item2 = 11;
          hallucination.Item3 = this._timing.CurTime + TimeSpan.FromSeconds(0.5);
          return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?(hallucination);
        }
        SharedAudioSystem audio15 = this._audio;
        SoundSpecifier explosion1 = lingering.Explosion;
        EntityUid recipient15 = victim;
        EntityCoordinates? nullable1 = hallucination.Item4;
        EntityCoordinates coordinates15 = this.HallucinationSoundOffset(nullable1 ?? victim.ToCoordinates(), 7f);
        AudioParams? audioParams15 = new AudioParams?();
        audio15.PlayStatic(explosion1, recipient15, coordinates15, audioParams15);
        SharedAudioSystem audio16 = this._audio;
        SoundSpecifier gauHit5 = lingering.GauHit;
        EntityUid recipient16 = victim;
        nullable1 = hallucination.Item4;
        EntityCoordinates coordinates16 = this.HallucinationSoundOffset(nullable1 ?? victim.ToCoordinates(), 7f);
        AudioParams? audioParams16 = new AudioParams?();
        audio16.PlayStatic(gauHit5, recipient16, coordinates16, audioParams16);
        NeurotoxinEmoteEvent args2 = new NeurotoxinEmoteEvent()
        {
          Emote = lingering.PainEmote
        };
        this.RaiseLocalEvent<NeurotoxinEmoteEvent>(victim, args2);
        break;
      case NeuroHallucinations.Mortar:
        this._audio.PlayStatic(lingering.MortarTravel, victim, hallucination.Item4 ?? victim.ToCoordinates());
        break;
    }
    return new (NeuroHallucinations, int, TimeSpan, EntityCoordinates?)?();
  }

  private EntityCoordinates HallucinationSoundOffset(EntityUid victim, float maxDistance)
  {
    Vector2 position = new Vector2(this._random.NextFloat(-maxDistance, maxDistance + 0.01f), this._random.NextFloat(-maxDistance, maxDistance + 0.01f));
    return this.Transform(victim).Coordinates.Offset(position);
  }

  private EntityCoordinates HallucinationSoundOffset(EntityCoordinates coords, float maxDistance)
  {
    Vector2 position = new Vector2(this._random.NextFloat(-maxDistance, maxDistance + 0.01f), this._random.NextFloat(-maxDistance, maxDistance + 0.01f));
    return coords.Offset(position);
  }

  private void FakeWarning(
    EntityCoordinates coords,
    EntityUid player,
    LocId directionWarning,
    LocId aboveWarning)
  {
    Vector2 vector2 = this._transform.GetMapCoordinates(player).Position - this._transform.ToMapCoordinates(coords).Position;
    double num = (double) vector2.Length();
    string upperInvariant = DirectionExtensions.GetDir(vector2).ToString().ToUpperInvariant();
    string message = num < 1.0 ? this.Loc.GetString((string) aboveWarning) : this.Loc.GetString((string) directionWarning, ("direction", (object) upperInvariant));
    this._popup.PopupEntity(message, player, player, PopupType.LargeCaution);
    ICommonSession session;
    if (!this._player.TryGetSessionByEntity(player, out session))
      return;
    string str = $"[bold][font size=24][color=red]\n{message}\n[/color][/font][/bold]";
    this._rmcChat.ChatMessageToOne(ChatChannel.Radio, str, str, new EntityUid(), false, session.Channel, recordReplay: true);
  }
}
