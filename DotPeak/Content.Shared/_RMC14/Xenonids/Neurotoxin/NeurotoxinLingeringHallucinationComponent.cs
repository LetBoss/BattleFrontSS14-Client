// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Neurotoxin.NeurotoxinLingeringHallucinationComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Neurotoxin;

[RegisterComponent]
[NetworkedComponent]
public sealed class NeurotoxinLingeringHallucinationComponent : 
  Component,
  ISerializationGenerated<NeurotoxinLingeringHallucinationComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> Hallucinations = new List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BoneBreak = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/alien_knockdown.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier XenoClaw = (SoundSpecifier) new SoundCollectionSpecifier("AlienClaw");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier OBTravel = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_orbital_travel.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier MortarTravel = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_travel.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier GauFire = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Dropship/gau.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier RocketFire = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/rocketpod_fire.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier GauHit = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Dropship/gauimpact.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5f)));
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Explosion = (SoundSpecifier) new SoundCollectionSpecifier("CMExplosion");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BigExplosion = (SoundSpecifier) new SoundCollectionSpecifier("RMCExplosionBig");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<EmotePrototype> PainEmote = (ProtoId<EmotePrototype>) "Scream";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NeurotoxinLingeringHallucinationComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NeurotoxinLingeringHallucinationComponent) target1;
    if (serialization.TryCustomCopy<NeurotoxinLingeringHallucinationComponent>(this, ref target, hookCtx, false, context))
      return;
    List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> target2 = (List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>) null;
    if (this.Hallucinations == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>>(this.Hallucinations, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>>(this.Hallucinations, hookCtx, context);
    target.Hallucinations = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.BoneBreak == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BoneBreak, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.BoneBreak, hookCtx, context);
    target.BoneBreak = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.XenoClaw == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.XenoClaw, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.XenoClaw, hookCtx, context);
    target.XenoClaw = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.OBTravel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OBTravel, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.OBTravel, hookCtx, context);
    target.OBTravel = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.MortarTravel == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.MortarTravel, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.MortarTravel, hookCtx, context);
    target.MortarTravel = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.GauFire == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GauFire, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.GauFire, hookCtx, context);
    target.GauFire = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (this.RocketFire == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RocketFire, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.RocketFire, hookCtx, context);
    target.RocketFire = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.GauHit == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GauHit, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.GauHit, hookCtx, context);
    target.GauHit = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.Explosion == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Explosion, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.Explosion, hookCtx, context);
    target.Explosion = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (this.BigExplosion == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BigExplosion, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.BigExplosion, hookCtx, context);
    target.BigExplosion = target11;
    ProtoId<EmotePrototype> target12 = new ProtoId<EmotePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(this.PainEmote, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<ProtoId<EmotePrototype>>(this.PainEmote, hookCtx, context);
    target.PainEmote = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NeurotoxinLingeringHallucinationComponent target,
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
    NeurotoxinLingeringHallucinationComponent target1 = (NeurotoxinLingeringHallucinationComponent) target;
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
    NeurotoxinLingeringHallucinationComponent target1 = (NeurotoxinLingeringHallucinationComponent) target;
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
    NeurotoxinLingeringHallucinationComponent target1 = (NeurotoxinLingeringHallucinationComponent) target;
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
  virtual NeurotoxinLingeringHallucinationComponent Component.Instantiate()
  {
    return new NeurotoxinLingeringHallucinationComponent();
  }
}
