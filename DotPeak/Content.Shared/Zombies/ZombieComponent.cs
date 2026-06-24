// Decompiled with JetBrains decompiler
// Type: Content.Shared.Zombies.ZombieComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chat.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Zombies;

[RegisterComponent]
[NetworkedComponent]
public sealed class ZombieComponent : 
  Component,
  ISerializationGenerated<ZombieComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float BaseZombieInfectionChance = 0.75f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MinZombieInfectionChance = 0.05f;
  public DamageSpecifier ResistanceEffectiveness = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Slash",
        (FixedPoint2) 0.5
      },
      {
        "Piercing",
        (FixedPoint2) 0.3
      },
      {
        "Blunt",
        (FixedPoint2) 0.1
      }
    }
  };
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ZombieMovementSpeedDebuff = 0.7f;
  [DataField("skinColor", false, 1, false, false, null)]
  public Color SkinColor = new Color(0.45f, 0.51f, 0.29f, 1f);
  [DataField("eyeColor", false, 1, false, false, null)]
  public Color EyeColor = new Color(0.96f, 0.13f, 0.24f, 1f);
  [DataField("baseLayerExternal", false, 1, false, false, null)]
  public string BaseLayerExternal = "MobHumanoidMarkingMatchSkin";
  [DataField("attackArc", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string AttackAnimation = "WeaponArcBite";
  [DataField("zombieRoleId", false, 1, false, false, typeof (PrototypeIdSerializer<AntagPrototype>))]
  public string ZombieRoleId = "Zombie";
  [DataField("beforeZombifiedCustomBaseLayers", false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> BeforeZombifiedCustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();
  [DataField("beforeZombifiedSkinColor", false, 1, false, false, null)]
  public Color BeforeZombifiedSkinColor;
  [DataField("beforeZombifiedEyeColor", false, 1, false, false, null)]
  public Color BeforeZombifiedEyeColor;
  [DataField("emoteId", false, 1, false, false, typeof (PrototypeIdSerializer<EmoteSoundsPrototype>))]
  public string? EmoteSoundsId = "Zombie";
  public EmoteSoundsPrototype? EmoteSounds;
  [DataField("nextTick", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextTick;
  [DataField("passiveHealing", false, 1, false, false, null)]
  public DamageSpecifier PassiveHealing = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Blunt",
        (FixedPoint2) -0.4
      },
      {
        "Slash",
        (FixedPoint2) -0.2
      },
      {
        "Piercing",
        (FixedPoint2) -0.2
      },
      {
        "Heat",
        (FixedPoint2) -0.02
      },
      {
        "Shock",
        (FixedPoint2) -0.02
      }
    }
  };
  [DataField("passiveHealingCritMultiplier", false, 1, false, false, null)]
  public float PassiveHealingCritMultiplier = 2f;
  [DataField("healingOnBite", false, 1, false, false, null)]
  public DamageSpecifier HealingOnBite = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Blunt",
        (FixedPoint2) -2
      },
      {
        "Slash",
        (FixedPoint2) -2
      },
      {
        "Piercing",
        (FixedPoint2) -2
      }
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier DamageOnBite = new DamageSpecifier()
  {
    DamageDict = new Dictionary<string, FixedPoint2>()
    {
      {
        "Slash",
        (FixedPoint2) 13
      },
      {
        "Piercing",
        (FixedPoint2) 7
      },
      {
        "Structural",
        (FixedPoint2) 10
      }
    }
  };
  [DataField("greetSoundNotification", false, 1, false, false, null)]
  public SoundSpecifier GreetSoundNotification = (SoundSpecifier) new SoundPathSpecifier("/Audio/Ambience/Antag/zombie_start.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier BiteSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/bite.ogg");
  [DataField("beforeZombifiedBloodReagent", false, 1, false, false, null)]
  public string BeforeZombifiedBloodReagent = string.Empty;
  [DataField("newBloodReagent", false, 1, false, false, typeof (PrototypeIdSerializer<ReagentPrototype>))]
  public string NewBloodReagent = "ZombieBlood";

  [DataField("zombieStatusIcon", false, 1, false, false, null)]
  public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = (ProtoId<FactionIconPrototype>) "ZombieFaction";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ZombieComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ZombieComponent) target1;
    if (serialization.TryCustomCopy<ZombieComponent>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SkinColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.SkinColor, hookCtx, context);
    target.SkinColor = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EyeColor, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.EyeColor, hookCtx, context);
    target.EyeColor = target3;
    string target4 = (string) null;
    if (this.BaseLayerExternal == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BaseLayerExternal, ref target4, hookCtx, false, context))
      target4 = this.BaseLayerExternal;
    target.BaseLayerExternal = target4;
    string target5 = (string) null;
    if (this.AttackAnimation == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AttackAnimation, ref target5, hookCtx, false, context))
      target5 = this.AttackAnimation;
    target.AttackAnimation = target5;
    string target6 = (string) null;
    if (this.ZombieRoleId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ZombieRoleId, ref target6, hookCtx, false, context))
      target6 = this.ZombieRoleId;
    target.ZombieRoleId = target6;
    Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> target7 = (Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) null;
    if (this.BeforeZombifiedCustomBaseLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(this.BeforeZombifiedCustomBaseLayers, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(this.BeforeZombifiedCustomBaseLayers, hookCtx, context);
    target.BeforeZombifiedCustomBaseLayers = target7;
    Color target8 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.BeforeZombifiedSkinColor, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Color>(this.BeforeZombifiedSkinColor, hookCtx, context);
    target.BeforeZombifiedSkinColor = target8;
    Color target9 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.BeforeZombifiedEyeColor, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Color>(this.BeforeZombifiedEyeColor, hookCtx, context);
    target.BeforeZombifiedEyeColor = target9;
    string target10 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EmoteSoundsId, ref target10, hookCtx, false, context))
      target10 = this.EmoteSoundsId;
    target.EmoteSoundsId = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextTick, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.NextTick, hookCtx, context);
    target.NextTick = target11;
    ProtoId<FactionIconPrototype> target12 = new ProtoId<FactionIconPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<FactionIconPrototype>>(this.StatusIcon, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<ProtoId<FactionIconPrototype>>(this.StatusIcon, hookCtx, context);
    target.StatusIcon = target12;
    DamageSpecifier target13 = (DamageSpecifier) null;
    if (this.PassiveHealing == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.PassiveHealing, ref target13, hookCtx, false, context))
    {
      if (this.PassiveHealing == null)
        target13 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.PassiveHealing, ref target13, hookCtx, context, true);
    }
    target.PassiveHealing = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PassiveHealingCritMultiplier, ref target14, hookCtx, false, context))
      target14 = this.PassiveHealingCritMultiplier;
    target.PassiveHealingCritMultiplier = target14;
    DamageSpecifier target15 = (DamageSpecifier) null;
    if (this.HealingOnBite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.HealingOnBite, ref target15, hookCtx, false, context))
    {
      if (this.HealingOnBite == null)
        target15 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.HealingOnBite, ref target15, hookCtx, context, true);
    }
    target.HealingOnBite = target15;
    DamageSpecifier target16 = (DamageSpecifier) null;
    if (this.DamageOnBite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageOnBite, ref target16, hookCtx, false, context))
    {
      if (this.DamageOnBite == null)
        target16 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageOnBite, ref target16, hookCtx, context, true);
    }
    target.DamageOnBite = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (this.GreetSoundNotification == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GreetSoundNotification, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.GreetSoundNotification, hookCtx, context);
    target.GreetSoundNotification = target17;
    SoundSpecifier target18 = (SoundSpecifier) null;
    if (this.BiteSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BiteSound, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<SoundSpecifier>(this.BiteSound, hookCtx, context);
    target.BiteSound = target18;
    string target19 = (string) null;
    if (this.BeforeZombifiedBloodReagent == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BeforeZombifiedBloodReagent, ref target19, hookCtx, false, context))
      target19 = this.BeforeZombifiedBloodReagent;
    target.BeforeZombifiedBloodReagent = target19;
    string target20 = (string) null;
    if (this.NewBloodReagent == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NewBloodReagent, ref target20, hookCtx, false, context))
      target20 = this.NewBloodReagent;
    target.NewBloodReagent = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ZombieComponent target,
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
    ZombieComponent target1 = (ZombieComponent) target;
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
    ZombieComponent target1 = (ZombieComponent) target;
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
    ZombieComponent target1 = (ZombieComponent) target;
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
  virtual ZombieComponent Component.Instantiate() => new ZombieComponent();
}
