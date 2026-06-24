// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Neurotoxin.NeurotoxinComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.OrbitalCannon;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Neurotoxin;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class NeurotoxinComponent : 
  Component,
  ISerializationGenerated<NeurotoxinComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float NeurotoxinAmount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DepletionPerTick = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StaminaDamagePerTick = 7f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DizzyStrength = TimeSpan.FromSeconds(12L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DizzyStrengthOnStumble = TimeSpan.FromSeconds(55L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastMessage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeBetweenMessages = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AccentTime = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan JitterTime = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StumbleJitterTime = TimeSpan.FromSeconds(25L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastStumbleTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextHallucination;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HallucinationEveryMin = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan HallucinationEveryMax = TimeSpan.FromSeconds(11L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BlurTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BlindTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeafenTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinimumDelayBetweenEvents = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastAccentTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier ToxinDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier OxygenDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier CoughDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DazeLength = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> CoughId = (ProtoId<EmotePrototype>) "Cough";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<EmotePrototype> PainId = (ProtoId<EmotePrototype>) "Scream";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BloodCoughDuration = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextGasInjectionAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextNeuroEffectAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateEvery = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<NeuroHallucinations, float> Hallucinations = new Dictionary<NeuroHallucinations, float>()
  {
    {
      NeuroHallucinations.AlienAttack,
      0.05f
    },
    {
      NeuroHallucinations.OB,
      0.05f
    },
    {
      NeuroHallucinations.Screech,
      0.06f
    },
    {
      NeuroHallucinations.CAS,
      0.08f
    },
    {
      NeuroHallucinations.Mortar,
      0.18f
    },
    {
      NeuroHallucinations.Giggle,
      0.27f
    },
    {
      NeuroHallucinations.Sounds,
      0.31f
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public List<SoundSpecifier> HallucinationRandomSounds = new List<SoundSpecifier>()
  {
    (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_distantroar_3.ogg"),
    (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/xenos_roaring.ogg"),
    (SoundSpecifier) new SoundCollectionSpecifier("XenoRoar"),
    (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Announcements/Marine/notice2.ogg"),
    (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/alien_knockdown.ogg"),
    (SoundSpecifier) new SoundCollectionSpecifier("CMM54CShoot"),
    (SoundSpecifier) new SoundCollectionSpecifier("MetalThud"),
    (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/crowbar.ogg"),
    (SoundSpecifier) new SoundCollectionSpecifier("WindowShatter")
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> GiggleId = (ProtoId<EmotePrototype>) "Laugh";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RainbowDuration = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Screech = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_queen_screech.ogg", new AudioParams?(AudioParams.Default.WithVolume(-7f)));
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ScreechDownTime = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Pounce = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_pounce.ogg");
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan PounceDownTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier OBAlert = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/ob_alert.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier FiremissionStart = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/dropship_sonic_boom.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<OrbitalCannonWarheadComponent>[] WarheadTypes = new EntProtoId<OrbitalCannonWarheadComponent>[3]
  {
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadExplosive",
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadIncendiary",
    (EntProtoId<OrbitalCannonWarheadComponent>) "RMCOrbitalCannonWarheadCluster"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NeurotoxinComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NeurotoxinComponent) target1;
    if (serialization.TryCustomCopy<NeurotoxinComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NeurotoxinAmount, ref target2, hookCtx, false, context))
      target2 = this.NeurotoxinAmount;
    target.NeurotoxinAmount = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DepletionPerTick, ref target3, hookCtx, false, context))
      target3 = this.DepletionPerTick;
    target.DepletionPerTick = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StaminaDamagePerTick, ref target4, hookCtx, false, context))
      target4 = this.StaminaDamagePerTick;
    target.StaminaDamagePerTick = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DizzyStrength, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.DizzyStrength, hookCtx, context);
    target.DizzyStrength = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DizzyStrengthOnStumble, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.DizzyStrengthOnStumble, hookCtx, context);
    target.DizzyStrengthOnStumble = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastMessage, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.LastMessage, hookCtx, context);
    target.LastMessage = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenMessages, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenMessages, hookCtx, context);
    target.TimeBetweenMessages = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AccentTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.AccentTime, hookCtx, context);
    target.AccentTime = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.JitterTime, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.JitterTime, hookCtx, context);
    target.JitterTime = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StumbleJitterTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.StumbleJitterTime, hookCtx, context);
    target.StumbleJitterTime = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastStumbleTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.LastStumbleTime, hookCtx, context);
    target.LastStumbleTime = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextHallucination, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.NextHallucination, hookCtx, context);
    target.NextHallucination = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HallucinationEveryMin, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.HallucinationEveryMin, hookCtx, context);
    target.HallucinationEveryMin = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.HallucinationEveryMax, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.HallucinationEveryMax, hookCtx, context);
    target.HallucinationEveryMax = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BlurTime, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.BlurTime, hookCtx, context);
    target.BlurTime = target16;
    TimeSpan target17 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BlindTime, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<TimeSpan>(this.BlindTime, hookCtx, context);
    target.BlindTime = target17;
    TimeSpan target18 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeafenTime, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<TimeSpan>(this.DeafenTime, hookCtx, context);
    target.DeafenTime = target18;
    TimeSpan target19 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinimumDelayBetweenEvents, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan>(this.MinimumDelayBetweenEvents, hookCtx, context);
    target.MinimumDelayBetweenEvents = target19;
    TimeSpan target20 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAccentTime, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<TimeSpan>(this.LastAccentTime, hookCtx, context);
    target.LastAccentTime = target20;
    DamageSpecifier target21 = (DamageSpecifier) null;
    if (this.ToxinDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ToxinDamage, ref target21, hookCtx, false, context))
    {
      if (this.ToxinDamage == null)
        target21 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ToxinDamage, ref target21, hookCtx, context, true);
    }
    target.ToxinDamage = target21;
    DamageSpecifier target22 = (DamageSpecifier) null;
    if (this.OxygenDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.OxygenDamage, ref target22, hookCtx, false, context))
    {
      if (this.OxygenDamage == null)
        target22 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.OxygenDamage, ref target22, hookCtx, context, true);
    }
    target.OxygenDamage = target22;
    DamageSpecifier target23 = (DamageSpecifier) null;
    if (this.CoughDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CoughDamage, ref target23, hookCtx, false, context))
    {
      if (this.CoughDamage == null)
        target23 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CoughDamage, ref target23, hookCtx, context, true);
    }
    target.CoughDamage = target23;
    TimeSpan target24 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DazeLength, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<TimeSpan>(this.DazeLength, hookCtx, context);
    target.DazeLength = target24;
    ProtoId<EmotePrototype> target25 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.CoughId, ref target25, hookCtx, false, context))
      target25 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.CoughId, hookCtx, context);
    target.CoughId = target25;
    ProtoId<EmotePrototype> target26 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.PainId, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.PainId, hookCtx, context);
    target.PainId = target26;
    TimeSpan target27 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BloodCoughDuration, ref target27, hookCtx, false, context))
      target27 = serialization.CreateCopy<TimeSpan>(this.BloodCoughDuration, hookCtx, context);
    target.BloodCoughDuration = target27;
    TimeSpan target28 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextGasInjectionAt, ref target28, hookCtx, false, context))
      target28 = serialization.CreateCopy<TimeSpan>(this.NextGasInjectionAt, hookCtx, context);
    target.NextGasInjectionAt = target28;
    TimeSpan target29 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextNeuroEffectAt, ref target29, hookCtx, false, context))
      target29 = serialization.CreateCopy<TimeSpan>(this.NextNeuroEffectAt, hookCtx, context);
    target.NextNeuroEffectAt = target29;
    TimeSpan target30 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateEvery, ref target30, hookCtx, false, context))
      target30 = serialization.CreateCopy<TimeSpan>(this.UpdateEvery, hookCtx, context);
    target.UpdateEvery = target30;
    Dictionary<NeuroHallucinations, float> target31 = (Dictionary<NeuroHallucinations, float>) null;
    if (this.Hallucinations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<NeuroHallucinations, float>>(this.Hallucinations, ref target31, hookCtx, true, context))
      target31 = serialization.CreateCopy<Dictionary<NeuroHallucinations, float>>(this.Hallucinations, hookCtx, context);
    target.Hallucinations = target31;
    List<SoundSpecifier> target32 = (List<SoundSpecifier>) null;
    if (this.HallucinationRandomSounds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<SoundSpecifier>>(this.HallucinationRandomSounds, ref target32, hookCtx, true, context))
      target32 = serialization.CreateCopy<List<SoundSpecifier>>(this.HallucinationRandomSounds, hookCtx, context);
    target.HallucinationRandomSounds = target32;
    ProtoId<EmotePrototype> target33 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.GiggleId, ref target33, hookCtx, false, context))
      target33 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.GiggleId, hookCtx, context);
    target.GiggleId = target33;
    TimeSpan target34 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RainbowDuration, ref target34, hookCtx, false, context))
      target34 = serialization.CreateCopy<TimeSpan>(this.RainbowDuration, hookCtx, context);
    target.RainbowDuration = target34;
    SoundSpecifier target35 = (SoundSpecifier) null;
    if (this.Screech == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Screech, ref target35, hookCtx, true, context))
      target35 = serialization.CreateCopy<SoundSpecifier>(this.Screech, hookCtx, context);
    target.Screech = target35;
    TimeSpan target36 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ScreechDownTime, ref target36, hookCtx, false, context))
      target36 = serialization.CreateCopy<TimeSpan>(this.ScreechDownTime, hookCtx, context);
    target.ScreechDownTime = target36;
    SoundSpecifier target37 = (SoundSpecifier) null;
    if (this.Pounce == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Pounce, ref target37, hookCtx, true, context))
      target37 = serialization.CreateCopy<SoundSpecifier>(this.Pounce, hookCtx, context);
    target.Pounce = target37;
    TimeSpan target38 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PounceDownTime, ref target38, hookCtx, false, context))
      target38 = serialization.CreateCopy<TimeSpan>(this.PounceDownTime, hookCtx, context);
    target.PounceDownTime = target38;
    SoundSpecifier target39 = (SoundSpecifier) null;
    if (this.OBAlert == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OBAlert, ref target39, hookCtx, true, context))
      target39 = serialization.CreateCopy<SoundSpecifier>(this.OBAlert, hookCtx, context);
    target.OBAlert = target39;
    SoundSpecifier target40 = (SoundSpecifier) null;
    if (this.FiremissionStart == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FiremissionStart, ref target40, hookCtx, true, context))
      target40 = serialization.CreateCopy<SoundSpecifier>(this.FiremissionStart, hookCtx, context);
    target.FiremissionStart = target40;
    EntProtoId<OrbitalCannonWarheadComponent>[] target41 = (EntProtoId<OrbitalCannonWarheadComponent>[]) null;
    if (this.WarheadTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntProtoId<OrbitalCannonWarheadComponent>[]>(this.WarheadTypes, ref target41, hookCtx, true, context))
      target41 = serialization.CreateCopy<EntProtoId<OrbitalCannonWarheadComponent>[]>(this.WarheadTypes, hookCtx, context);
    target.WarheadTypes = target41;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NeurotoxinComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NeurotoxinComponent target1 = (NeurotoxinComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NeurotoxinComponent target1 = (NeurotoxinComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NeurotoxinComponent target1 = (NeurotoxinComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NeurotoxinComponent Component.Instantiate() => new NeurotoxinComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NeurotoxinComponent_AutoState : IComponentState
  {
    public float NeurotoxinAmount;
    public float DepletionPerTick;
    public float StaminaDamagePerTick;
    public TimeSpan DizzyStrength;
    public TimeSpan DizzyStrengthOnStumble;
    public TimeSpan LastMessage;
    public TimeSpan TimeBetweenMessages;
    public TimeSpan AccentTime;
    public TimeSpan JitterTime;
    public TimeSpan StumbleJitterTime;
    public TimeSpan LastStumbleTime;
    public TimeSpan NextHallucination;
    public TimeSpan HallucinationEveryMin;
    public TimeSpan HallucinationEveryMax;
    public TimeSpan BlurTime;
    public TimeSpan BlindTime;
    public TimeSpan DeafenTime;
    public TimeSpan MinimumDelayBetweenEvents;
    public TimeSpan LastAccentTime;
    public DamageSpecifier ToxinDamage;
    public DamageSpecifier OxygenDamage;
    public DamageSpecifier CoughDamage;
    public TimeSpan DazeLength;
    public ProtoId<EmotePrototype> CoughId;
    public ProtoId<EmotePrototype> PainId;
    public TimeSpan BloodCoughDuration;
    public TimeSpan NextGasInjectionAt;
    public TimeSpan NextNeuroEffectAt;
    public TimeSpan UpdateEvery;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NeurotoxinComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NeurotoxinComponent, ComponentGetState>(new ComponentEventRefHandler<NeurotoxinComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NeurotoxinComponent, ComponentHandleState>(new ComponentEventRefHandler<NeurotoxinComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NeurotoxinComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NeurotoxinComponent.NeurotoxinComponent_AutoState()
      {
        NeurotoxinAmount = component.NeurotoxinAmount,
        DepletionPerTick = component.DepletionPerTick,
        StaminaDamagePerTick = component.StaminaDamagePerTick,
        DizzyStrength = component.DizzyStrength,
        DizzyStrengthOnStumble = component.DizzyStrengthOnStumble,
        LastMessage = component.LastMessage,
        TimeBetweenMessages = component.TimeBetweenMessages,
        AccentTime = component.AccentTime,
        JitterTime = component.JitterTime,
        StumbleJitterTime = component.StumbleJitterTime,
        LastStumbleTime = component.LastStumbleTime,
        NextHallucination = component.NextHallucination,
        HallucinationEveryMin = component.HallucinationEveryMin,
        HallucinationEveryMax = component.HallucinationEveryMax,
        BlurTime = component.BlurTime,
        BlindTime = component.BlindTime,
        DeafenTime = component.DeafenTime,
        MinimumDelayBetweenEvents = component.MinimumDelayBetweenEvents,
        LastAccentTime = component.LastAccentTime,
        ToxinDamage = component.ToxinDamage,
        OxygenDamage = component.OxygenDamage,
        CoughDamage = component.CoughDamage,
        DazeLength = component.DazeLength,
        CoughId = component.CoughId,
        PainId = component.PainId,
        BloodCoughDuration = component.BloodCoughDuration,
        NextGasInjectionAt = component.NextGasInjectionAt,
        NextNeuroEffectAt = component.NextNeuroEffectAt,
        UpdateEvery = component.UpdateEvery
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NeurotoxinComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NeurotoxinComponent.NeurotoxinComponent_AutoState current))
        return;
      component.NeurotoxinAmount = current.NeurotoxinAmount;
      component.DepletionPerTick = current.DepletionPerTick;
      component.StaminaDamagePerTick = current.StaminaDamagePerTick;
      component.DizzyStrength = current.DizzyStrength;
      component.DizzyStrengthOnStumble = current.DizzyStrengthOnStumble;
      component.LastMessage = current.LastMessage;
      component.TimeBetweenMessages = current.TimeBetweenMessages;
      component.AccentTime = current.AccentTime;
      component.JitterTime = current.JitterTime;
      component.StumbleJitterTime = current.StumbleJitterTime;
      component.LastStumbleTime = current.LastStumbleTime;
      component.NextHallucination = current.NextHallucination;
      component.HallucinationEveryMin = current.HallucinationEveryMin;
      component.HallucinationEveryMax = current.HallucinationEveryMax;
      component.BlurTime = current.BlurTime;
      component.BlindTime = current.BlindTime;
      component.DeafenTime = current.DeafenTime;
      component.MinimumDelayBetweenEvents = current.MinimumDelayBetweenEvents;
      component.LastAccentTime = current.LastAccentTime;
      component.ToxinDamage = current.ToxinDamage;
      component.OxygenDamage = current.OxygenDamage;
      component.CoughDamage = current.CoughDamage;
      component.DazeLength = current.DazeLength;
      component.CoughId = current.CoughId;
      component.PainId = current.PainId;
      component.BloodCoughDuration = current.BloodCoughDuration;
      component.NextGasInjectionAt = current.NextGasInjectionAt;
      component.NextNeuroEffectAt = current.NextNeuroEffectAt;
      component.UpdateEvery = current.UpdateEvery;
    }
  }
}
