// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.DefibrillatorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Body.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Medical;

[RegisterComponent]
[NetworkedComponent]
public sealed class DefibrillatorComponent : 
  Component,
  ISerializationGenerated<DefibrillatorComponent>,
  ISerializationGenerated
{
  [DataField("zapHeal", false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public DamageSpecifier ZapHeal;
  [DataField("zapDamage", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int ZapDamage = 5;
  [DataField("writheDuration", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan WritheDuration = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public string DelayId = "defib-delay";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ZapDelay = TimeSpan.FromSeconds(5L);
  [DataField("doAfterDuration", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public bool AllowDoAfterMovement = true;
  [DataField(null, false, 1, false, false, null)]
  public bool CanDefibCrit = true;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("zapSound", false, 1, false, false, null)]
  public SoundSpecifier? ZapSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Defib/defib_zap.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("chargeSound", false, 1, false, false, null)]
  public SoundSpecifier? ChargeSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Defib/defib_charge.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("failureSound", false, 1, false, false, null)]
  public SoundSpecifier? FailureSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Defib/defib_failed.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("successSound", false, 1, false, false, null)]
  public SoundSpecifier? SuccessSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Defib/defib_success.ogg");
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("readySound", false, 1, false, false, null)]
  public SoundSpecifier? ReadySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Defib/defib_ready.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? ChargeSoundEntity;
  [DataField("rmcZapHeal", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<(ProtoId<DamageGroupPrototype> Group, int Amount)>? RMCZapDamage;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan SkillMultiplierDuration = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<MetabolismGroupPrototype> MetabolismId = (ProtoId<MetabolismGroupPrototype>) "Medicine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DefibrillatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DefibrillatorComponent) target1;
    if (serialization.TryCustomCopy<DefibrillatorComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.ZapHeal == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.ZapHeal, ref target2, hookCtx, false, context))
    {
      if (this.ZapHeal == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.ZapHeal, ref target2, hookCtx, context, true);
    }
    target.ZapHeal = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.ZapDamage, ref target3, hookCtx, false, context))
      target3 = this.ZapDamage;
    target.ZapDamage = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WritheDuration, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.WritheDuration, hookCtx, context);
    target.WritheDuration = target4;
    string target5 = (string) null;
    if (this.DelayId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DelayId, ref target5, hookCtx, false, context))
      target5 = this.DelayId;
    target.DelayId = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ZapDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.ZapDelay, hookCtx, context);
    target.ZapDelay = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoAfterDuration, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.DoAfterDuration, hookCtx, context);
    target.DoAfterDuration = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.AllowDoAfterMovement, ref target8, hookCtx, false, context))
      target8 = this.AllowDoAfterMovement;
    target.AllowDoAfterMovement = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanDefibCrit, ref target9, hookCtx, false, context))
      target9 = this.CanDefibCrit;
    target.CanDefibCrit = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ZapSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.ZapSound, hookCtx, context);
    target.ZapSound = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ChargeSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.ChargeSound, hookCtx, context);
    target.ChargeSound = target11;
    SoundSpecifier target12 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FailureSound, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<SoundSpecifier>(this.FailureSound, hookCtx, context);
    target.FailureSound = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SuccessSound, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.SuccessSound, hookCtx, context);
    target.SuccessSound = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReadySound, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.ReadySound, hookCtx, context);
    target.ReadySound = target14;
    EntityUid? target15 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ChargeSoundEntity, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntityUid?>(this.ChargeSoundEntity, hookCtx, context);
    target.ChargeSoundEntity = target15;
    List<(ProtoId<DamageGroupPrototype>, int)> target16 = (List<(ProtoId<DamageGroupPrototype>, int)>) null;
    if (!serialization.TryCustomCopy<List<(ProtoId<DamageGroupPrototype>, int)>>(this.RMCZapDamage, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<List<(ProtoId<DamageGroupPrototype>, int)>>(this.RMCZapDamage, hookCtx, context);
    target.RMCZapDamage = target16;
    EntProtoId<SkillDefinitionComponent> target17 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target17;
    TimeSpan target18 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SkillMultiplierDuration, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<TimeSpan>(this.SkillMultiplierDuration, hookCtx, context);
    target.SkillMultiplierDuration = target18;
    ProtoId<MetabolismGroupPrototype> target19 = new ProtoId<MetabolismGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<MetabolismGroupPrototype>>(this.MetabolismId, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<ProtoId<MetabolismGroupPrototype>>(this.MetabolismId, hookCtx, context);
    target.MetabolismId = target19;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DefibrillatorComponent target,
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
    DefibrillatorComponent target1 = (DefibrillatorComponent) target;
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
    DefibrillatorComponent target1 = (DefibrillatorComponent) target;
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
    DefibrillatorComponent target1 = (DefibrillatorComponent) target;
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
  virtual DefibrillatorComponent Component.Instantiate() => new DefibrillatorComponent();
}
