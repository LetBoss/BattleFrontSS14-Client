// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.MeleeWeaponComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Melee;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
public sealed class MeleeWeaponComponent : 
  Component,
  ISerializationGenerated<MeleeWeaponComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  public bool WidePrimary;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AltDisarm;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Hidden;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextAttack;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ResetOnHandSelected;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AttackRate;
  [AutoNetworkedField]
  public bool Attacking;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoAttack;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ResistanceBypass;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BluntStaminaDamageFactor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 ClickDamageModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle Angle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Animation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId WideAnimation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle WideAnimationRotation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SwingLeft;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundHit", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? HitSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MustBeEquippedToUse;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundSwing", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier SwingSound { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundNoDamage", false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier NoDamageSound { get; set; }

  public MeleeWeaponComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Weapons/punchmiss.ogg");
    soundPathSpecifier.Params = AudioParams.Default.WithVolume(-3f).WithVariation(new float?(0.025f));
    // ISSUE: reference to a compiler-generated field
    this.\u003CSwingSound\u003Ek__BackingField = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: reference to a compiler-generated field
    this.\u003CNoDamageSound\u003Ek__BackingField = (SoundSpecifier) new SoundCollectionSpecifier("WeakHit");
    // ISSUE: reference to a compiler-generated field
    this.\u003CLastFieldUpdate\u003Ek__BackingField = GameTick.Zero;
    // ISSUE: reference to a compiler-generated field
    this.\u003CLastModifiedFields\u003Ek__BackingField = Array.Empty<GameTick>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MeleeWeaponComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MeleeWeaponComponent) target1;
    if (serialization.TryCustomCopy<MeleeWeaponComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.WidePrimary, ref target2, hookCtx, false, context))
      target2 = this.WidePrimary;
    target.WidePrimary = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.AltDisarm, ref target3, hookCtx, false, context))
      target3 = this.AltDisarm;
    target.AltDisarm = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Hidden, ref target4, hookCtx, false, context))
      target4 = this.Hidden;
    target.Hidden = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextAttack, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextAttack, hookCtx, context);
    target.NextAttack = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResetOnHandSelected, ref target6, hookCtx, false, context))
      target6 = this.ResetOnHandSelected;
    target.ResetOnHandSelected = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AttackRate, ref target7, hookCtx, false, context))
      target7 = this.AttackRate;
    target.AttackRate = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoAttack, ref target8, hookCtx, false, context))
      target8 = this.AutoAttack;
    target.AutoAttack = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResistanceBypass, ref target9, hookCtx, false, context))
      target9 = this.ResistanceBypass;
    target.ResistanceBypass = target9;
    DamageSpecifier target10 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target10, hookCtx, false, context))
    {
      if (this.Damage == null)
        target10 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target10, hookCtx, context, true);
    }
    target.Damage = target10;
    FixedPoint2 target11 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BluntStaminaDamageFactor, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<FixedPoint2>(this.BluntStaminaDamageFactor, hookCtx, context);
    target.BluntStaminaDamageFactor = target11;
    FixedPoint2 target12 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.ClickDamageModifier, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<FixedPoint2>(this.ClickDamageModifier, hookCtx, context);
    target.ClickDamageModifier = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target13, hookCtx, false, context))
      target13 = this.Range;
    target.Range = target13;
    Angle target14 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Angle, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Angle>(this.Angle, hookCtx, context);
    target.Angle = target14;
    EntProtoId target15 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Animation, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId>(this.Animation, hookCtx, context);
    target.Animation = target15;
    EntProtoId target16 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.WideAnimation, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId>(this.WideAnimation, hookCtx, context);
    target.WideAnimation = target16;
    Angle target17 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.WideAnimationRotation, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<Angle>(this.WideAnimationRotation, hookCtx, context);
    target.WideAnimationRotation = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.SwingLeft, ref target18, hookCtx, false, context))
      target18 = this.SwingLeft;
    target.SwingLeft = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (this.SwingSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SwingSound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.SwingSound, hookCtx, context);
    target.SwingSound = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.HitSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.HitSound, hookCtx, context);
    target.HitSound = target20;
    SoundSpecifier target21 = (SoundSpecifier) null;
    if (this.NoDamageSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.NoDamageSound, ref target21, hookCtx, true, context))
      target21 = serialization.CreateCopy<SoundSpecifier>(this.NoDamageSound, hookCtx, context);
    target.NoDamageSound = target21;
    bool target22 = false;
    if (!serialization.TryCustomCopy<bool>(this.MustBeEquippedToUse, ref target22, hookCtx, false, context))
      target22 = this.MustBeEquippedToUse;
    target.MustBeEquippedToUse = target22;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MeleeWeaponComponent target,
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
    MeleeWeaponComponent target1 = (MeleeWeaponComponent) target;
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
    MeleeWeaponComponent target1 = (MeleeWeaponComponent) target;
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
    MeleeWeaponComponent target1 = (MeleeWeaponComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MeleeWeaponComponent target1 = (MeleeWeaponComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MeleeWeaponComponent Component.Instantiate() => new MeleeWeaponComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; }

  public GameTick[] LastModifiedFields { get; set; }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MeleeWeaponComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MeleeWeaponComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<MeleeWeaponComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      MeleeWeaponComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextAttack += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MeleeWeaponComponent_AutoState : IComponentState
  {
    public bool AltDisarm;
    public bool Hidden;
    public TimeSpan NextAttack;
    public bool ResetOnHandSelected;
    public float AttackRate;
    public bool Attacking;
    public bool AutoAttack;
    public bool ResistanceBypass;
    public 
    #nullable enable
    DamageSpecifier Damage;
    public FixedPoint2 BluntStaminaDamageFactor;
    public FixedPoint2 ClickDamageModifier;
    public float Range;
    public Angle Angle;
    public EntProtoId Animation;
    public EntProtoId WideAnimation;
    public Angle WideAnimationRotation;
    public bool SwingLeft;
    public SoundSpecifier SwingSound;
    public SoundSpecifier? HitSound;
    public SoundSpecifier NoDamageSound;
    public bool MustBeEquippedToUse;

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState ShallowClone()
    {
      return new MeleeWeaponComponent.MeleeWeaponComponent_AutoState()
      {
        AltDisarm = this.AltDisarm,
        Hidden = this.Hidden,
        NextAttack = this.NextAttack,
        ResetOnHandSelected = this.ResetOnHandSelected,
        AttackRate = this.AttackRate,
        Attacking = this.Attacking,
        AutoAttack = this.AutoAttack,
        ResistanceBypass = this.ResistanceBypass,
        Damage = this.Damage,
        BluntStaminaDamageFactor = this.BluntStaminaDamageFactor,
        ClickDamageModifier = this.ClickDamageModifier,
        Range = this.Range,
        Angle = this.Angle,
        Animation = this.Animation,
        WideAnimation = this.WideAnimation,
        WideAnimationRotation = this.WideAnimationRotation,
        SwingLeft = this.SwingLeft,
        SwingSound = this.SwingSound,
        HitSound = this.HitSound,
        NoDamageSound = this.NoDamageSound,
        MustBeEquippedToUse = this.MustBeEquippedToUse
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MeleeWeaponComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<MeleeWeaponComponent>("AltDisarm", "Hidden", "NextAttack", "ResetOnHandSelected", "AttackRate", "Attacking", "AutoAttack", "ResistanceBypass", "Damage", "BluntStaminaDamageFactor", "ClickDamageModifier", "Range", "Angle", "Animation", "WideAnimation", "WideAnimationRotation", "SwingLeft", "SwingSound", "HitSound", "NoDamageSound", "MustBeEquippedToUse");
      this.SubscribeLocalEvent<MeleeWeaponComponent, ComponentGetState>(new ComponentEventRefHandler<MeleeWeaponComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MeleeWeaponComponent, ComponentHandleState>(new ComponentEventRefHandler<MeleeWeaponComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MeleeWeaponComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        uint modifiedFields = this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick);
        if (modifiedFields <= 1024U /*0x0400*/)
        {
          if (modifiedFields <= 32U /*0x20*/)
          {
            if (modifiedFields <= 8U)
            {
              switch ((int) modifiedFields - 1)
              {
                case 0:
                  args.State = (IComponentState) new MeleeWeaponComponent.AltDisarm_FieldComponentState()
                  {
                    AltDisarm = component.AltDisarm
                  };
                  return;
                case 1:
                  args.State = (IComponentState) new MeleeWeaponComponent.Hidden_FieldComponentState()
                  {
                    Hidden = component.Hidden
                  };
                  return;
                case 2:
                  break;
                case 3:
                  args.State = (IComponentState) new MeleeWeaponComponent.NextAttack_FieldComponentState()
                  {
                    NextAttack = component.NextAttack
                  };
                  return;
                default:
                  if (modifiedFields == 8U)
                  {
                    args.State = (IComponentState) new MeleeWeaponComponent.ResetOnHandSelected_FieldComponentState()
                    {
                      ResetOnHandSelected = component.ResetOnHandSelected
                    };
                    return;
                  }
                  break;
              }
            }
            else if (modifiedFields != 16U /*0x10*/)
            {
              if (modifiedFields == 32U /*0x20*/)
              {
                args.State = (IComponentState) new MeleeWeaponComponent.Attacking_FieldComponentState()
                {
                  Attacking = component.Attacking
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new MeleeWeaponComponent.AttackRate_FieldComponentState()
              {
                AttackRate = component.AttackRate
              };
              return;
            }
          }
          else if (modifiedFields <= 128U /*0x80*/)
          {
            if (modifiedFields != 64U /*0x40*/)
            {
              if (modifiedFields == 128U /*0x80*/)
              {
                args.State = (IComponentState) new MeleeWeaponComponent.ResistanceBypass_FieldComponentState()
                {
                  ResistanceBypass = component.ResistanceBypass
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new MeleeWeaponComponent.AutoAttack_FieldComponentState()
              {
                AutoAttack = component.AutoAttack
              };
              return;
            }
          }
          else if (modifiedFields != 256U /*0x0100*/)
          {
            if (modifiedFields != 512U /*0x0200*/)
            {
              if (modifiedFields == 1024U /*0x0400*/)
              {
                args.State = (IComponentState) new MeleeWeaponComponent.ClickDamageModifier_FieldComponentState()
                {
                  ClickDamageModifier = component.ClickDamageModifier
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new MeleeWeaponComponent.BluntStaminaDamageFactor_FieldComponentState()
              {
                BluntStaminaDamageFactor = component.BluntStaminaDamageFactor
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new MeleeWeaponComponent.Damage_FieldComponentState()
            {
              Damage = component.Damage
            };
            return;
          }
        }
        else if (modifiedFields <= 32768U /*0x8000*/)
        {
          if (modifiedFields <= 4096U /*0x1000*/)
          {
            if (modifiedFields != 2048U /*0x0800*/)
            {
              if (modifiedFields == 4096U /*0x1000*/)
              {
                args.State = (IComponentState) new MeleeWeaponComponent.Angle_FieldComponentState()
                {
                  Angle = component.Angle
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new MeleeWeaponComponent.Range_FieldComponentState()
              {
                Range = component.Range
              };
              return;
            }
          }
          else if (modifiedFields != 8192U /*0x2000*/)
          {
            if (modifiedFields != 16384U /*0x4000*/)
            {
              if (modifiedFields == 32768U /*0x8000*/)
              {
                args.State = (IComponentState) new MeleeWeaponComponent.WideAnimationRotation_FieldComponentState()
                {
                  WideAnimationRotation = component.WideAnimationRotation
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new MeleeWeaponComponent.WideAnimation_FieldComponentState()
              {
                WideAnimation = component.WideAnimation
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new MeleeWeaponComponent.Animation_FieldComponentState()
            {
              Animation = component.Animation
            };
            return;
          }
        }
        else if (modifiedFields <= 131072U /*0x020000*/)
        {
          if (modifiedFields != 65536U /*0x010000*/)
          {
            if (modifiedFields == 131072U /*0x020000*/)
            {
              args.State = (IComponentState) new MeleeWeaponComponent.SwingSound_FieldComponentState()
              {
                SwingSound = component.SwingSound
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new MeleeWeaponComponent.SwingLeft_FieldComponentState()
            {
              SwingLeft = component.SwingLeft
            };
            return;
          }
        }
        else if (modifiedFields != 262144U /*0x040000*/)
        {
          if (modifiedFields != 524288U /*0x080000*/)
          {
            if (modifiedFields == 1048576U /*0x100000*/)
            {
              args.State = (IComponentState) new MeleeWeaponComponent.MustBeEquippedToUse_FieldComponentState()
              {
                MustBeEquippedToUse = component.MustBeEquippedToUse
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new MeleeWeaponComponent.NoDamageSound_FieldComponentState()
            {
              NoDamageSound = component.NoDamageSound
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new MeleeWeaponComponent.HitSound_FieldComponentState()
          {
            HitSound = component.HitSound
          };
          return;
        }
      }
      args.State = (IComponentState) new MeleeWeaponComponent.MeleeWeaponComponent_AutoState()
      {
        AltDisarm = component.AltDisarm,
        Hidden = component.Hidden,
        NextAttack = component.NextAttack,
        ResetOnHandSelected = component.ResetOnHandSelected,
        AttackRate = component.AttackRate,
        Attacking = component.Attacking,
        AutoAttack = component.AutoAttack,
        ResistanceBypass = component.ResistanceBypass,
        Damage = component.Damage,
        BluntStaminaDamageFactor = component.BluntStaminaDamageFactor,
        ClickDamageModifier = component.ClickDamageModifier,
        Range = component.Range,
        Angle = component.Angle,
        Animation = component.Animation,
        WideAnimation = component.WideAnimation,
        WideAnimationRotation = component.WideAnimationRotation,
        SwingLeft = component.SwingLeft,
        SwingSound = component.SwingSound,
        HitSound = component.HitSound,
        NoDamageSound = component.NoDamageSound,
        MustBeEquippedToUse = component.MustBeEquippedToUse
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MeleeWeaponComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case MeleeWeaponComponent.AltDisarm_FieldComponentState fieldComponentState1:
          component.AltDisarm = fieldComponentState1.AltDisarm;
          break;
        case MeleeWeaponComponent.Hidden_FieldComponentState fieldComponentState2:
          component.Hidden = fieldComponentState2.Hidden;
          break;
        case MeleeWeaponComponent.NextAttack_FieldComponentState fieldComponentState3:
          component.NextAttack = fieldComponentState3.NextAttack;
          break;
        case MeleeWeaponComponent.ResetOnHandSelected_FieldComponentState fieldComponentState4:
          component.ResetOnHandSelected = fieldComponentState4.ResetOnHandSelected;
          break;
        case MeleeWeaponComponent.AttackRate_FieldComponentState fieldComponentState5:
          component.AttackRate = fieldComponentState5.AttackRate;
          break;
        case MeleeWeaponComponent.Attacking_FieldComponentState fieldComponentState6:
          component.Attacking = fieldComponentState6.Attacking;
          break;
        case MeleeWeaponComponent.AutoAttack_FieldComponentState fieldComponentState7:
          component.AutoAttack = fieldComponentState7.AutoAttack;
          break;
        case MeleeWeaponComponent.ResistanceBypass_FieldComponentState fieldComponentState8:
          component.ResistanceBypass = fieldComponentState8.ResistanceBypass;
          break;
        case MeleeWeaponComponent.Damage_FieldComponentState fieldComponentState9:
          component.Damage = fieldComponentState9.Damage;
          break;
        case MeleeWeaponComponent.BluntStaminaDamageFactor_FieldComponentState fieldComponentState10:
          component.BluntStaminaDamageFactor = fieldComponentState10.BluntStaminaDamageFactor;
          break;
        case MeleeWeaponComponent.ClickDamageModifier_FieldComponentState fieldComponentState11:
          component.ClickDamageModifier = fieldComponentState11.ClickDamageModifier;
          break;
        case MeleeWeaponComponent.Range_FieldComponentState fieldComponentState12:
          component.Range = fieldComponentState12.Range;
          break;
        case MeleeWeaponComponent.Angle_FieldComponentState fieldComponentState13:
          component.Angle = fieldComponentState13.Angle;
          break;
        case MeleeWeaponComponent.Animation_FieldComponentState fieldComponentState14:
          component.Animation = fieldComponentState14.Animation;
          break;
        case MeleeWeaponComponent.WideAnimation_FieldComponentState fieldComponentState15:
          component.WideAnimation = fieldComponentState15.WideAnimation;
          break;
        case MeleeWeaponComponent.WideAnimationRotation_FieldComponentState fieldComponentState16:
          component.WideAnimationRotation = fieldComponentState16.WideAnimationRotation;
          break;
        case MeleeWeaponComponent.SwingLeft_FieldComponentState fieldComponentState17:
          component.SwingLeft = fieldComponentState17.SwingLeft;
          break;
        case MeleeWeaponComponent.SwingSound_FieldComponentState fieldComponentState18:
          component.SwingSound = fieldComponentState18.SwingSound;
          break;
        case MeleeWeaponComponent.HitSound_FieldComponentState fieldComponentState19:
          component.HitSound = fieldComponentState19.HitSound;
          break;
        case MeleeWeaponComponent.NoDamageSound_FieldComponentState fieldComponentState20:
          component.NoDamageSound = fieldComponentState20.NoDamageSound;
          break;
        case MeleeWeaponComponent.MustBeEquippedToUse_FieldComponentState fieldComponentState21:
          component.MustBeEquippedToUse = fieldComponentState21.MustBeEquippedToUse;
          break;
        case MeleeWeaponComponent.MeleeWeaponComponent_AutoState componentAutoState:
          component.AltDisarm = componentAutoState.AltDisarm;
          component.Hidden = componentAutoState.Hidden;
          component.NextAttack = componentAutoState.NextAttack;
          component.ResetOnHandSelected = componentAutoState.ResetOnHandSelected;
          component.AttackRate = componentAutoState.AttackRate;
          component.Attacking = componentAutoState.Attacking;
          component.AutoAttack = componentAutoState.AutoAttack;
          component.ResistanceBypass = componentAutoState.ResistanceBypass;
          component.Damage = componentAutoState.Damage;
          component.BluntStaminaDamageFactor = componentAutoState.BluntStaminaDamageFactor;
          component.ClickDamageModifier = componentAutoState.ClickDamageModifier;
          component.Range = componentAutoState.Range;
          component.Angle = componentAutoState.Angle;
          component.Animation = componentAutoState.Animation;
          component.WideAnimation = componentAutoState.WideAnimation;
          component.WideAnimationRotation = componentAutoState.WideAnimationRotation;
          component.SwingLeft = componentAutoState.SwingLeft;
          component.SwingSound = componentAutoState.SwingSound;
          component.HitSound = componentAutoState.HitSound;
          component.NoDamageSound = componentAutoState.NoDamageSound;
          component.MustBeEquippedToUse = componentAutoState.MustBeEquippedToUse;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AltDisarm_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool AltDisarm;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.AltDisarm = this.AltDisarm;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Hidden_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Hidden;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.Hidden = this.Hidden;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextAttack_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextAttack;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.NextAttack = this.NextAttack;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ResetOnHandSelected_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool ResetOnHandSelected;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.ResetOnHandSelected = this.ResetOnHandSelected;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AttackRate_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float AttackRate;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.AttackRate = this.AttackRate;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Attacking_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Attacking;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.Attacking = this.Attacking;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AutoAttack_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool AutoAttack;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.AutoAttack = this.AutoAttack;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ResistanceBypass_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool ResistanceBypass;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.ResistanceBypass = this.ResistanceBypass;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Damage_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public DamageSpecifier Damage;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.Damage = this.Damage;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BluntStaminaDamageFactor_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 BluntStaminaDamageFactor;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.BluntStaminaDamageFactor = this.BluntStaminaDamageFactor;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ClickDamageModifier_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 ClickDamageModifier;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.ClickDamageModifier = this.ClickDamageModifier;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Range_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Range;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.Range = this.Range;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Angle_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle Angle;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.Angle = this.Angle;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Animation_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntProtoId Animation;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.Animation = this.Animation;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class WideAnimation_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntProtoId WideAnimation;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.WideAnimation = this.WideAnimation;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class WideAnimationRotation_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle WideAnimationRotation;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.WideAnimationRotation = this.WideAnimationRotation;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SwingLeft_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool SwingLeft;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.SwingLeft = this.SwingLeft;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SwingSound_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier SwingSound;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.SwingSound = this.SwingSound;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class HitSound_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier? HitSound;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.HitSound = this.HitSound;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NoDamageSound_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier NoDamageSound;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.NoDamageSound = this.NoDamageSound;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MustBeEquippedToUse_FieldComponentState : 
    IComponentDeltaState<MeleeWeaponComponent.MeleeWeaponComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool MustBeEquippedToUse;

    public void ApplyToFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      fullState.MustBeEquippedToUse = this.MustBeEquippedToUse;
    }

    public MeleeWeaponComponent.MeleeWeaponComponent_AutoState CreateNewFullState(
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState)
    {
      MeleeWeaponComponent.MeleeWeaponComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
