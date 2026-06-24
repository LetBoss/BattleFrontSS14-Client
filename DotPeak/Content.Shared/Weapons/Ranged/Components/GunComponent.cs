// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.GunComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedGunSystem), typeof (RMCSelectiveFireSystem)})]
public sealed class GunComponent : 
  Component,
  ISerializationGenerated<GunComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundGunshot = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/smg.ogg");
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public SoundSpecifier? SoundGunshotModified;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundEmpty = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Empty/empty.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundMode = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/selector.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CameraRecoilScalar = 1f;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float CameraRecoilScalarModified = 1f;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LastFire = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle CurrentAngle;
  [DataField(null, false, 1, false, false, null)]
  public Angle AngleIncrease = Angle.FromDegrees(0.5);
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Angle AngleIncreaseModified;
  [DataField(null, false, 1, false, false, null)]
  public Angle AngleDecay = Angle.FromDegrees(4.0);
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Angle AngleDecayModified;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle MaxAngle = Angle.FromDegrees(2.0);
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Angle MaxAngleModified;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle MinAngle = Angle.FromDegrees(1.0);
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Angle MinAngleModified;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseKey = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityCoordinates? ShootCoordinates;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShotsPerBurst = 3;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int ShotsPerBurstModified = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BurstCooldown = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BurstFireRate = 8f;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool BurstActivated;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int BurstShotsCount;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public int ShotCounter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FireRate = 8f;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float FireRateModified;
  [DataField(null, false, 1, false, false, null)]
  public bool ResetOnHandSelected = true;
  [DataField(null, false, 1, false, false, null)]
  public float ProjectileSpeed = 62f;
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ProjectileSpeedModified;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextFire = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SelectiveFire AvailableModes = SelectiveFire.SemiAuto;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SelectiveFire SelectedMode = SelectiveFire.SemiAuto;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowExamineText = true;
  [DataField(null, false, 1, false, false, null)]
  public bool ClumsyProof;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 DefaultDirection = new Vector2(0.0f, -1f);
  [DataField(null, false, 1, false, false, null)]
  public bool MeleeCooldownOnShoot = true;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 ShootOriginOffset = Vector2.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunComponent) target1;
    if (serialization.TryCustomCopy<GunComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundGunshot, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.SoundGunshot, hookCtx, context);
    target.SoundGunshot = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundEmpty, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.SoundEmpty, hookCtx, context);
    target.SoundEmpty = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundMode, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.SoundMode, hookCtx, context);
    target.SoundMode = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CameraRecoilScalar, ref target5, hookCtx, false, context))
      target5 = this.CameraRecoilScalar;
    target.CameraRecoilScalar = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastFire, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.LastFire, hookCtx, context);
    target.LastFire = target6;
    Angle target7 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.CurrentAngle, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Angle>(this.CurrentAngle, hookCtx, context);
    target.CurrentAngle = target7;
    Angle target8 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleIncrease, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Angle>(this.AngleIncrease, hookCtx, context);
    target.AngleIncrease = target8;
    Angle target9 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleDecay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Angle>(this.AngleDecay, hookCtx, context);
    target.AngleDecay = target9;
    Angle target10 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MaxAngle, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Angle>(this.MaxAngle, hookCtx, context);
    target.MaxAngle = target10;
    Angle target11 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.MinAngle, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<Angle>(this.MinAngle, hookCtx, context);
    target.MinAngle = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseKey, ref target12, hookCtx, false, context))
      target12 = this.UseKey;
    target.UseKey = target12;
    int target13 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShotsPerBurst, ref target13, hookCtx, false, context))
      target13 = this.ShotsPerBurst;
    target.ShotsPerBurst = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BurstCooldown, ref target14, hookCtx, false, context))
      target14 = this.BurstCooldown;
    target.BurstCooldown = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BurstFireRate, ref target15, hookCtx, false, context))
      target15 = this.BurstFireRate;
    target.BurstFireRate = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireRate, ref target16, hookCtx, false, context))
      target16 = this.FireRate;
    target.FireRate = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResetOnHandSelected, ref target17, hookCtx, false, context))
      target17 = this.ResetOnHandSelected;
    target.ResetOnHandSelected = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ProjectileSpeed, ref target18, hookCtx, false, context))
      target18 = this.ProjectileSpeed;
    target.ProjectileSpeed = target18;
    TimeSpan target19 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextFire, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan>(this.NextFire, hookCtx, context);
    target.NextFire = target19;
    SelectiveFire target20 = SelectiveFire.Invalid;
    if (!serialization.TryCustomCopy<SelectiveFire>(this.AvailableModes, ref target20, hookCtx, false, context))
      target20 = this.AvailableModes;
    target.AvailableModes = target20;
    SelectiveFire target21 = SelectiveFire.Invalid;
    if (!serialization.TryCustomCopy<SelectiveFire>(this.SelectedMode, ref target21, hookCtx, false, context))
      target21 = this.SelectedMode;
    target.SelectedMode = target21;
    bool target22 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowExamineText, ref target22, hookCtx, false, context))
      target22 = this.ShowExamineText;
    target.ShowExamineText = target22;
    bool target23 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClumsyProof, ref target23, hookCtx, false, context))
      target23 = this.ClumsyProof;
    target.ClumsyProof = target23;
    Vector2 target24 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.DefaultDirection, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<Vector2>(this.DefaultDirection, hookCtx, context);
    target.DefaultDirection = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.MeleeCooldownOnShoot, ref target25, hookCtx, false, context))
      target25 = this.MeleeCooldownOnShoot;
    target.MeleeCooldownOnShoot = target25;
    Vector2 target26 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.ShootOriginOffset, ref target26, hookCtx, false, context))
      target26 = serialization.CreateCopy<Vector2>(this.ShootOriginOffset, hookCtx, context);
    target.ShootOriginOffset = target26;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunComponent target,
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
    GunComponent target1 = (GunComponent) target;
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
    GunComponent target1 = (GunComponent) target;
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
    GunComponent target1 = (GunComponent) target;
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
    GunComponent target1 = (GunComponent) target;
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
  virtual GunComponent Component.Instantiate() => new GunComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<GunComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      GunComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextFire += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    SoundSpecifier? SoundGunshotModified;
    public float CameraRecoilScalar;
    public float CameraRecoilScalarModified;
    public Angle CurrentAngle;
    public Angle AngleIncreaseModified;
    public Angle AngleDecayModified;
    public Angle MaxAngle;
    public Angle MaxAngleModified;
    public Angle MinAngle;
    public Angle MinAngleModified;
    public bool UseKey;
    public int ShotsPerBurst;
    public int ShotsPerBurstModified;
    public float BurstCooldown;
    public float BurstFireRate;
    public bool BurstActivated;
    public int BurstShotsCount;
    public int ShotCounter;
    public float FireRate;
    public float FireRateModified;
    public float ProjectileSpeedModified;
    public TimeSpan NextFire;
    public SelectiveFire AvailableModes;
    public SelectiveFire SelectedMode;

    public GunComponent.GunComponent_AutoState ShallowClone()
    {
      return new GunComponent.GunComponent_AutoState()
      {
        SoundGunshotModified = this.SoundGunshotModified,
        CameraRecoilScalar = this.CameraRecoilScalar,
        CameraRecoilScalarModified = this.CameraRecoilScalarModified,
        CurrentAngle = this.CurrentAngle,
        AngleIncreaseModified = this.AngleIncreaseModified,
        AngleDecayModified = this.AngleDecayModified,
        MaxAngle = this.MaxAngle,
        MaxAngleModified = this.MaxAngleModified,
        MinAngle = this.MinAngle,
        MinAngleModified = this.MinAngleModified,
        UseKey = this.UseKey,
        ShotsPerBurst = this.ShotsPerBurst,
        ShotsPerBurstModified = this.ShotsPerBurstModified,
        BurstCooldown = this.BurstCooldown,
        BurstFireRate = this.BurstFireRate,
        BurstActivated = this.BurstActivated,
        BurstShotsCount = this.BurstShotsCount,
        ShotCounter = this.ShotCounter,
        FireRate = this.FireRate,
        FireRateModified = this.FireRateModified,
        ProjectileSpeedModified = this.ProjectileSpeedModified,
        NextFire = this.NextFire,
        AvailableModes = this.AvailableModes,
        SelectedMode = this.SelectedMode
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<GunComponent>("SoundGunshotModified", "CameraRecoilScalar", "CameraRecoilScalarModified", "CurrentAngle", "AngleIncreaseModified", "AngleDecayModified", "MaxAngle", "MaxAngleModified", "MinAngle", "MinAngleModified", "UseKey", "ShotsPerBurst", "ShotsPerBurstModified", "BurstCooldown", "BurstFireRate", "BurstActivated", "BurstShotsCount", "ShotCounter", "FireRate", "FireRateModified", "ProjectileSpeedModified", "NextFire", "AvailableModes", "SelectedMode");
      this.SubscribeLocalEvent<GunComponent, ComponentGetState>(new ComponentEventRefHandler<GunComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunComponent, ComponentHandleState>(new ComponentEventRefHandler<GunComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, GunComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        uint modifiedFields = this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick);
        if (modifiedFields <= 4096U /*0x1000*/)
        {
          if (modifiedFields <= 64U /*0x40*/)
          {
            if (modifiedFields <= 8U)
            {
              switch ((int) modifiedFields - 1)
              {
                case 0:
                  args.State = (IComponentState) new GunComponent.SoundGunshotModified_FieldComponentState()
                  {
                    SoundGunshotModified = component.SoundGunshotModified
                  };
                  return;
                case 1:
                  args.State = (IComponentState) new GunComponent.CameraRecoilScalar_FieldComponentState()
                  {
                    CameraRecoilScalar = component.CameraRecoilScalar
                  };
                  return;
                case 2:
                  break;
                case 3:
                  args.State = (IComponentState) new GunComponent.CameraRecoilScalarModified_FieldComponentState()
                  {
                    CameraRecoilScalarModified = component.CameraRecoilScalarModified
                  };
                  return;
                default:
                  if (modifiedFields == 8U)
                  {
                    args.State = (IComponentState) new GunComponent.CurrentAngle_FieldComponentState()
                    {
                      CurrentAngle = component.CurrentAngle
                    };
                    return;
                  }
                  break;
              }
            }
            else if (modifiedFields != 16U /*0x10*/)
            {
              if (modifiedFields != 32U /*0x20*/)
              {
                if (modifiedFields == 64U /*0x40*/)
                {
                  args.State = (IComponentState) new GunComponent.MaxAngle_FieldComponentState()
                  {
                    MaxAngle = component.MaxAngle
                  };
                  return;
                }
              }
              else
              {
                args.State = (IComponentState) new GunComponent.AngleDecayModified_FieldComponentState()
                {
                  AngleDecayModified = component.AngleDecayModified
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new GunComponent.AngleIncreaseModified_FieldComponentState()
              {
                AngleIncreaseModified = component.AngleIncreaseModified
              };
              return;
            }
          }
          else if (modifiedFields <= 512U /*0x0200*/)
          {
            if (modifiedFields != 128U /*0x80*/)
            {
              if (modifiedFields != 256U /*0x0100*/)
              {
                if (modifiedFields == 512U /*0x0200*/)
                {
                  args.State = (IComponentState) new GunComponent.MinAngleModified_FieldComponentState()
                  {
                    MinAngleModified = component.MinAngleModified
                  };
                  return;
                }
              }
              else
              {
                args.State = (IComponentState) new GunComponent.MinAngle_FieldComponentState()
                {
                  MinAngle = component.MinAngle
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new GunComponent.MaxAngleModified_FieldComponentState()
              {
                MaxAngleModified = component.MaxAngleModified
              };
              return;
            }
          }
          else if (modifiedFields != 1024U /*0x0400*/)
          {
            if (modifiedFields != 2048U /*0x0800*/)
            {
              if (modifiedFields == 4096U /*0x1000*/)
              {
                args.State = (IComponentState) new GunComponent.ShotsPerBurstModified_FieldComponentState()
                {
                  ShotsPerBurstModified = component.ShotsPerBurstModified
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new GunComponent.ShotsPerBurst_FieldComponentState()
              {
                ShotsPerBurst = component.ShotsPerBurst
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new GunComponent.UseKey_FieldComponentState()
            {
              UseKey = component.UseKey
            };
            return;
          }
        }
        else if (modifiedFields <= 131072U /*0x020000*/)
        {
          if (modifiedFields <= 16384U /*0x4000*/)
          {
            if (modifiedFields != 8192U /*0x2000*/)
            {
              if (modifiedFields == 16384U /*0x4000*/)
              {
                args.State = (IComponentState) new GunComponent.BurstFireRate_FieldComponentState()
                {
                  BurstFireRate = component.BurstFireRate
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new GunComponent.BurstCooldown_FieldComponentState()
              {
                BurstCooldown = component.BurstCooldown
              };
              return;
            }
          }
          else if (modifiedFields != 32768U /*0x8000*/)
          {
            if (modifiedFields != 65536U /*0x010000*/)
            {
              if (modifiedFields == 131072U /*0x020000*/)
              {
                args.State = (IComponentState) new GunComponent.ShotCounter_FieldComponentState()
                {
                  ShotCounter = component.ShotCounter
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new GunComponent.BurstShotsCount_FieldComponentState()
              {
                BurstShotsCount = component.BurstShotsCount
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new GunComponent.BurstActivated_FieldComponentState()
            {
              BurstActivated = component.BurstActivated
            };
            return;
          }
        }
        else if (modifiedFields <= 1048576U /*0x100000*/)
        {
          if (modifiedFields != 262144U /*0x040000*/)
          {
            if (modifiedFields != 524288U /*0x080000*/)
            {
              if (modifiedFields == 1048576U /*0x100000*/)
              {
                args.State = (IComponentState) new GunComponent.ProjectileSpeedModified_FieldComponentState()
                {
                  ProjectileSpeedModified = component.ProjectileSpeedModified
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new GunComponent.FireRateModified_FieldComponentState()
              {
                FireRateModified = component.FireRateModified
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new GunComponent.FireRate_FieldComponentState()
            {
              FireRate = component.FireRate
            };
            return;
          }
        }
        else if (modifiedFields != 2097152U /*0x200000*/)
        {
          if (modifiedFields != 4194304U /*0x400000*/)
          {
            if (modifiedFields == 8388608U /*0x800000*/)
            {
              args.State = (IComponentState) new GunComponent.SelectedMode_FieldComponentState()
              {
                SelectedMode = component.SelectedMode
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new GunComponent.AvailableModes_FieldComponentState()
            {
              AvailableModes = component.AvailableModes
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new GunComponent.NextFire_FieldComponentState()
          {
            NextFire = component.NextFire
          };
          return;
        }
      }
      args.State = (IComponentState) new GunComponent.GunComponent_AutoState()
      {
        SoundGunshotModified = component.SoundGunshotModified,
        CameraRecoilScalar = component.CameraRecoilScalar,
        CameraRecoilScalarModified = component.CameraRecoilScalarModified,
        CurrentAngle = component.CurrentAngle,
        AngleIncreaseModified = component.AngleIncreaseModified,
        AngleDecayModified = component.AngleDecayModified,
        MaxAngle = component.MaxAngle,
        MaxAngleModified = component.MaxAngleModified,
        MinAngle = component.MinAngle,
        MinAngleModified = component.MinAngleModified,
        UseKey = component.UseKey,
        ShotsPerBurst = component.ShotsPerBurst,
        ShotsPerBurstModified = component.ShotsPerBurstModified,
        BurstCooldown = component.BurstCooldown,
        BurstFireRate = component.BurstFireRate,
        BurstActivated = component.BurstActivated,
        BurstShotsCount = component.BurstShotsCount,
        ShotCounter = component.ShotCounter,
        FireRate = component.FireRate,
        FireRateModified = component.FireRateModified,
        ProjectileSpeedModified = component.ProjectileSpeedModified,
        NextFire = component.NextFire,
        AvailableModes = component.AvailableModes,
        SelectedMode = component.SelectedMode
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case GunComponent.SoundGunshotModified_FieldComponentState fieldComponentState1:
          component.SoundGunshotModified = fieldComponentState1.SoundGunshotModified;
          break;
        case GunComponent.CameraRecoilScalar_FieldComponentState fieldComponentState2:
          component.CameraRecoilScalar = fieldComponentState2.CameraRecoilScalar;
          break;
        case GunComponent.CameraRecoilScalarModified_FieldComponentState fieldComponentState3:
          component.CameraRecoilScalarModified = fieldComponentState3.CameraRecoilScalarModified;
          break;
        case GunComponent.CurrentAngle_FieldComponentState fieldComponentState4:
          component.CurrentAngle = fieldComponentState4.CurrentAngle;
          break;
        case GunComponent.AngleIncreaseModified_FieldComponentState fieldComponentState5:
          component.AngleIncreaseModified = fieldComponentState5.AngleIncreaseModified;
          break;
        case GunComponent.AngleDecayModified_FieldComponentState fieldComponentState6:
          component.AngleDecayModified = fieldComponentState6.AngleDecayModified;
          break;
        case GunComponent.MaxAngle_FieldComponentState fieldComponentState7:
          component.MaxAngle = fieldComponentState7.MaxAngle;
          break;
        case GunComponent.MaxAngleModified_FieldComponentState fieldComponentState8:
          component.MaxAngleModified = fieldComponentState8.MaxAngleModified;
          break;
        case GunComponent.MinAngle_FieldComponentState fieldComponentState9:
          component.MinAngle = fieldComponentState9.MinAngle;
          break;
        case GunComponent.MinAngleModified_FieldComponentState fieldComponentState10:
          component.MinAngleModified = fieldComponentState10.MinAngleModified;
          break;
        case GunComponent.UseKey_FieldComponentState fieldComponentState11:
          component.UseKey = fieldComponentState11.UseKey;
          break;
        case GunComponent.ShotsPerBurst_FieldComponentState fieldComponentState12:
          component.ShotsPerBurst = fieldComponentState12.ShotsPerBurst;
          break;
        case GunComponent.ShotsPerBurstModified_FieldComponentState fieldComponentState13:
          component.ShotsPerBurstModified = fieldComponentState13.ShotsPerBurstModified;
          break;
        case GunComponent.BurstCooldown_FieldComponentState fieldComponentState14:
          component.BurstCooldown = fieldComponentState14.BurstCooldown;
          break;
        case GunComponent.BurstFireRate_FieldComponentState fieldComponentState15:
          component.BurstFireRate = fieldComponentState15.BurstFireRate;
          break;
        case GunComponent.BurstActivated_FieldComponentState fieldComponentState16:
          component.BurstActivated = fieldComponentState16.BurstActivated;
          break;
        case GunComponent.BurstShotsCount_FieldComponentState fieldComponentState17:
          component.BurstShotsCount = fieldComponentState17.BurstShotsCount;
          break;
        case GunComponent.ShotCounter_FieldComponentState fieldComponentState18:
          component.ShotCounter = fieldComponentState18.ShotCounter;
          break;
        case GunComponent.FireRate_FieldComponentState fieldComponentState19:
          component.FireRate = fieldComponentState19.FireRate;
          break;
        case GunComponent.FireRateModified_FieldComponentState fieldComponentState20:
          component.FireRateModified = fieldComponentState20.FireRateModified;
          break;
        case GunComponent.ProjectileSpeedModified_FieldComponentState fieldComponentState21:
          component.ProjectileSpeedModified = fieldComponentState21.ProjectileSpeedModified;
          break;
        case GunComponent.NextFire_FieldComponentState fieldComponentState22:
          component.NextFire = fieldComponentState22.NextFire;
          break;
        case GunComponent.AvailableModes_FieldComponentState fieldComponentState23:
          component.AvailableModes = fieldComponentState23.AvailableModes;
          break;
        case GunComponent.SelectedMode_FieldComponentState fieldComponentState24:
          component.SelectedMode = fieldComponentState24.SelectedMode;
          break;
        case GunComponent.GunComponent_AutoState componentAutoState:
          component.SoundGunshotModified = componentAutoState.SoundGunshotModified;
          component.CameraRecoilScalar = componentAutoState.CameraRecoilScalar;
          component.CameraRecoilScalarModified = componentAutoState.CameraRecoilScalarModified;
          component.CurrentAngle = componentAutoState.CurrentAngle;
          component.AngleIncreaseModified = componentAutoState.AngleIncreaseModified;
          component.AngleDecayModified = componentAutoState.AngleDecayModified;
          component.MaxAngle = componentAutoState.MaxAngle;
          component.MaxAngleModified = componentAutoState.MaxAngleModified;
          component.MinAngle = componentAutoState.MinAngle;
          component.MinAngleModified = componentAutoState.MinAngleModified;
          component.UseKey = componentAutoState.UseKey;
          component.ShotsPerBurst = componentAutoState.ShotsPerBurst;
          component.ShotsPerBurstModified = componentAutoState.ShotsPerBurstModified;
          component.BurstCooldown = componentAutoState.BurstCooldown;
          component.BurstFireRate = componentAutoState.BurstFireRate;
          component.BurstActivated = componentAutoState.BurstActivated;
          component.BurstShotsCount = componentAutoState.BurstShotsCount;
          component.ShotCounter = componentAutoState.ShotCounter;
          component.FireRate = componentAutoState.FireRate;
          component.FireRateModified = componentAutoState.FireRateModified;
          component.ProjectileSpeedModified = componentAutoState.ProjectileSpeedModified;
          component.NextFire = componentAutoState.NextFire;
          component.AvailableModes = componentAutoState.AvailableModes;
          component.SelectedMode = componentAutoState.SelectedMode;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SoundGunshotModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier? SoundGunshotModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.SoundGunshotModified = this.SoundGunshotModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CameraRecoilScalar_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float CameraRecoilScalar;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.CameraRecoilScalar = this.CameraRecoilScalar;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CameraRecoilScalarModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float CameraRecoilScalarModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.CameraRecoilScalarModified = this.CameraRecoilScalarModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CurrentAngle_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle CurrentAngle;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.CurrentAngle = this.CurrentAngle;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AngleIncreaseModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle AngleIncreaseModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.AngleIncreaseModified = this.AngleIncreaseModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AngleDecayModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle AngleDecayModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.AngleDecayModified = this.AngleDecayModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MaxAngle_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle MaxAngle;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.MaxAngle = this.MaxAngle;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MaxAngleModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle MaxAngleModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.MaxAngleModified = this.MaxAngleModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MinAngle_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle MinAngle;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.MinAngle = this.MinAngle;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MinAngleModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Angle MinAngleModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.MinAngleModified = this.MinAngleModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UseKey_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool UseKey;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.UseKey = this.UseKey;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ShotsPerBurst_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int ShotsPerBurst;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.ShotsPerBurst = this.ShotsPerBurst;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ShotsPerBurstModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int ShotsPerBurstModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.ShotsPerBurstModified = this.ShotsPerBurstModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BurstCooldown_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BurstCooldown;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.BurstCooldown = this.BurstCooldown;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BurstFireRate_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BurstFireRate;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.BurstFireRate = this.BurstFireRate;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BurstActivated_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool BurstActivated;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.BurstActivated = this.BurstActivated;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BurstShotsCount_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int BurstShotsCount;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.BurstShotsCount = this.BurstShotsCount;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ShotCounter_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int ShotCounter;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.ShotCounter = this.ShotCounter;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FireRate_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float FireRate;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.FireRate = this.FireRate;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FireRateModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float FireRateModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.FireRateModified = this.FireRateModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ProjectileSpeedModified_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float ProjectileSpeedModified;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.ProjectileSpeedModified = this.ProjectileSpeedModified;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextFire_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextFire;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.NextFire = this.NextFire;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AvailableModes_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SelectiveFire AvailableModes;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.AvailableModes = this.AvailableModes;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SelectedMode_FieldComponentState : 
    IComponentDeltaState<GunComponent.GunComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SelectiveFire SelectedMode;

    public void ApplyToFullState(GunComponent.GunComponent_AutoState fullState)
    {
      fullState.SelectedMode = this.SelectedMode;
    }

    public GunComponent.GunComponent_AutoState CreateNewFullState(
      GunComponent.GunComponent_AutoState fullState)
    {
      GunComponent.GunComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
