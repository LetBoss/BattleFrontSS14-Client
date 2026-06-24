// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Components.RMCEquipmentDeployerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.AlertLevel;
using Content.Shared.Whitelist;
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
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, true)]
public sealed class RMCEquipmentDeployerComponent : 
  Component,
  ISerializationGenerated<RMCEquipmentDeployerComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? DeployPrototype = (EntProtoId?) "RMCML66DNestMetal";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DeploySlotId = "dropship_deploy";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DropShipWindowButtonText = "MG";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? DeployEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsDeployable;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsDeployed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoDeploy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoUnDeploy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsDeployableByHand;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCAlertLevels AlertLevelRequired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i StarboardForeDeployDirection = new Vector2i(1, 0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i PortForeDeployDirection = new Vector2i(-1, 0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i StarboardWingDeployDirection = new Vector2i(0, -1);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i PortWingDeployDirection = new Vector2i(0, -1);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ForeDeployRotationDegrees = 180f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PortWingDeployRotationDegrees = -90f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float StarboardWingDeployRotationDegrees = 90f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? UtilityDeployedSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? WeaponDeployedSprite;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? ElectronicDeployedSprite;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier DeployAudio = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/hydraulics_1.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier UnDeployAudio = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/hydraulics_2.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCEquipmentDeployerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCEquipmentDeployerComponent) target1;
    if (serialization.TryCustomCopy<RMCEquipmentDeployerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.DeployPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.DeployPrototype, hookCtx, context);
    target.DeployPrototype = target2;
    string target3 = (string) null;
    if (this.DeploySlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DeploySlotId, ref target3, hookCtx, false, context))
      target3 = this.DeploySlotId;
    target.DeploySlotId = target3;
    string target4 = (string) null;
    if (this.DropShipWindowButtonText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DropShipWindowButtonText, ref target4, hookCtx, false, context))
      target4 = this.DropShipWindowButtonText;
    target.DropShipWindowButtonText = target4;
    NetEntity? target5 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.DeployEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<NetEntity?>(this.DeployEntity, hookCtx, context);
    target.DeployEntity = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsDeployable, ref target6, hookCtx, false, context))
      target6 = this.IsDeployable;
    target.IsDeployable = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsDeployed, ref target7, hookCtx, false, context))
      target7 = this.IsDeployed;
    target.IsDeployed = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoDeploy, ref target8, hookCtx, false, context))
      target8 = this.AutoDeploy;
    target.AutoDeploy = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoUnDeploy, ref target9, hookCtx, false, context))
      target9 = this.AutoUnDeploy;
    target.AutoUnDeploy = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsDeployableByHand, ref target10, hookCtx, false, context))
      target10 = this.IsDeployableByHand;
    target.IsDeployableByHand = target10;
    RMCAlertLevels target11 = RMCAlertLevels.Green;
    if (!serialization.TryCustomCopy<RMCAlertLevels>(this.AlertLevelRequired, ref target11, hookCtx, false, context))
      target11 = this.AlertLevelRequired;
    target.AlertLevelRequired = target11;
    EntityWhitelist target12 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target12, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target12 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target12, hookCtx, context);
    }
    target.Blacklist = target12;
    Vector2i target13 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.StarboardForeDeployDirection, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<Vector2i>(this.StarboardForeDeployDirection, hookCtx, context);
    target.StarboardForeDeployDirection = target13;
    Vector2i target14 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.PortForeDeployDirection, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Vector2i>(this.PortForeDeployDirection, hookCtx, context);
    target.PortForeDeployDirection = target14;
    Vector2i target15 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.StarboardWingDeployDirection, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<Vector2i>(this.StarboardWingDeployDirection, hookCtx, context);
    target.StarboardWingDeployDirection = target15;
    Vector2i target16 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.PortWingDeployDirection, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<Vector2i>(this.PortWingDeployDirection, hookCtx, context);
    target.PortWingDeployDirection = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ForeDeployRotationDegrees, ref target17, hookCtx, false, context))
      target17 = this.ForeDeployRotationDegrees;
    target.ForeDeployRotationDegrees = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PortWingDeployRotationDegrees, ref target18, hookCtx, false, context))
      target18 = this.PortWingDeployRotationDegrees;
    target.PortWingDeployRotationDegrees = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StarboardWingDeployRotationDegrees, ref target19, hookCtx, false, context))
      target19 = this.StarboardWingDeployRotationDegrees;
    target.StarboardWingDeployRotationDegrees = target19;
    SpriteSpecifier.Rsi target20 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.UtilityDeployedSprite, ref target20, hookCtx, false, context))
    {
      if (this.UtilityDeployedSprite == null)
        target20 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.UtilityDeployedSprite, ref target20, hookCtx, context);
    }
    target.UtilityDeployedSprite = target20;
    SpriteSpecifier.Rsi target21 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.WeaponDeployedSprite, ref target21, hookCtx, false, context))
    {
      if (this.WeaponDeployedSprite == null)
        target21 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.WeaponDeployedSprite, ref target21, hookCtx, context);
    }
    target.WeaponDeployedSprite = target21;
    SpriteSpecifier.Rsi target22 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.ElectronicDeployedSprite, ref target22, hookCtx, false, context))
    {
      if (this.ElectronicDeployedSprite == null)
        target22 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.ElectronicDeployedSprite, ref target22, hookCtx, context);
    }
    target.ElectronicDeployedSprite = target22;
    SoundSpecifier target23 = (SoundSpecifier) null;
    if (this.DeployAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeployAudio, ref target23, hookCtx, true, context))
      target23 = serialization.CreateCopy<SoundSpecifier>(this.DeployAudio, hookCtx, context);
    target.DeployAudio = target23;
    SoundSpecifier target24 = (SoundSpecifier) null;
    if (this.UnDeployAudio == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnDeployAudio, ref target24, hookCtx, true, context))
      target24 = serialization.CreateCopy<SoundSpecifier>(this.UnDeployAudio, hookCtx, context);
    target.UnDeployAudio = target24;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCEquipmentDeployerComponent target,
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
    RMCEquipmentDeployerComponent target1 = (RMCEquipmentDeployerComponent) target;
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
    RMCEquipmentDeployerComponent target1 = (RMCEquipmentDeployerComponent) target;
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
    RMCEquipmentDeployerComponent target1 = (RMCEquipmentDeployerComponent) target;
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
    RMCEquipmentDeployerComponent target1 = (RMCEquipmentDeployerComponent) target;
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
  virtual RMCEquipmentDeployerComponent Component.Instantiate()
  {
    return new RMCEquipmentDeployerComponent();
  }

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCEquipmentDeployerComponent_AutoState : IComponentState
  {
    public EntProtoId? DeployPrototype;
    public string DeploySlotId;
    public string DropShipWindowButtonText;
    public NetEntity? DeployEntity;
    public bool IsDeployable;
    public bool IsDeployed;
    public bool AutoDeploy;
    public bool AutoUnDeploy;
    public bool IsDeployableByHand;
    public RMCAlertLevels AlertLevelRequired;
    public EntityWhitelist? Blacklist;
    public Vector2i StarboardForeDeployDirection;
    public Vector2i PortForeDeployDirection;
    public Vector2i StarboardWingDeployDirection;
    public Vector2i PortWingDeployDirection;
    public float ForeDeployRotationDegrees;
    public float PortWingDeployRotationDegrees;
    public float StarboardWingDeployRotationDegrees;
    public SpriteSpecifier.Rsi? UtilityDeployedSprite;
    public SpriteSpecifier.Rsi? WeaponDeployedSprite;
    public SpriteSpecifier.Rsi? ElectronicDeployedSprite;

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState ShallowClone()
    {
      return new RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState()
      {
        DeployPrototype = this.DeployPrototype,
        DeploySlotId = this.DeploySlotId,
        DropShipWindowButtonText = this.DropShipWindowButtonText,
        DeployEntity = this.DeployEntity,
        IsDeployable = this.IsDeployable,
        IsDeployed = this.IsDeployed,
        AutoDeploy = this.AutoDeploy,
        AutoUnDeploy = this.AutoUnDeploy,
        IsDeployableByHand = this.IsDeployableByHand,
        AlertLevelRequired = this.AlertLevelRequired,
        Blacklist = this.Blacklist,
        StarboardForeDeployDirection = this.StarboardForeDeployDirection,
        PortForeDeployDirection = this.PortForeDeployDirection,
        StarboardWingDeployDirection = this.StarboardWingDeployDirection,
        PortWingDeployDirection = this.PortWingDeployDirection,
        ForeDeployRotationDegrees = this.ForeDeployRotationDegrees,
        PortWingDeployRotationDegrees = this.PortWingDeployRotationDegrees,
        StarboardWingDeployRotationDegrees = this.StarboardWingDeployRotationDegrees,
        UtilityDeployedSprite = this.UtilityDeployedSprite,
        WeaponDeployedSprite = this.WeaponDeployedSprite,
        ElectronicDeployedSprite = this.ElectronicDeployedSprite
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCEquipmentDeployerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<RMCEquipmentDeployerComponent>("DeployPrototype", "DeploySlotId", "DropShipWindowButtonText", "DeployEntity", "IsDeployable", "IsDeployed", "AutoDeploy", "AutoUnDeploy", "IsDeployableByHand", "AlertLevelRequired", "Blacklist", "StarboardForeDeployDirection", "PortForeDeployDirection", "StarboardWingDeployDirection", "PortWingDeployDirection", "ForeDeployRotationDegrees", "PortWingDeployRotationDegrees", "StarboardWingDeployRotationDegrees", "UtilityDeployedSprite", "WeaponDeployedSprite", "ElectronicDeployedSprite");
      this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, ComponentGetState>(new ComponentEventRefHandler<RMCEquipmentDeployerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCEquipmentDeployerComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCEquipmentDeployerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCEquipmentDeployerComponent component,
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
                  args.State = (IComponentState) new RMCEquipmentDeployerComponent.DeployPrototype_FieldComponentState()
                  {
                    DeployPrototype = component.DeployPrototype
                  };
                  return;
                case 1:
                  args.State = (IComponentState) new RMCEquipmentDeployerComponent.DeploySlotId_FieldComponentState()
                  {
                    DeploySlotId = component.DeploySlotId
                  };
                  return;
                case 2:
                  break;
                case 3:
                  args.State = (IComponentState) new RMCEquipmentDeployerComponent.DropShipWindowButtonText_FieldComponentState()
                  {
                    DropShipWindowButtonText = component.DropShipWindowButtonText
                  };
                  return;
                default:
                  if (modifiedFields == 8U)
                  {
                    args.State = (IComponentState) new RMCEquipmentDeployerComponent.DeployEntity_FieldComponentState()
                    {
                      DeployEntity = component.DeployEntity
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
                args.State = (IComponentState) new RMCEquipmentDeployerComponent.IsDeployed_FieldComponentState()
                {
                  IsDeployed = component.IsDeployed
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.IsDeployable_FieldComponentState()
              {
                IsDeployable = component.IsDeployable
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
                args.State = (IComponentState) new RMCEquipmentDeployerComponent.AutoUnDeploy_FieldComponentState()
                {
                  AutoUnDeploy = component.AutoUnDeploy
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.AutoDeploy_FieldComponentState()
              {
                AutoDeploy = component.AutoDeploy
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
                args.State = (IComponentState) new RMCEquipmentDeployerComponent.Blacklist_FieldComponentState()
                {
                  Blacklist = component.Blacklist
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.AlertLevelRequired_FieldComponentState()
              {
                AlertLevelRequired = component.AlertLevelRequired
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new RMCEquipmentDeployerComponent.IsDeployableByHand_FieldComponentState()
            {
              IsDeployableByHand = component.IsDeployableByHand
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
                args.State = (IComponentState) new RMCEquipmentDeployerComponent.PortForeDeployDirection_FieldComponentState()
                {
                  PortForeDeployDirection = component.PortForeDeployDirection
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.StarboardForeDeployDirection_FieldComponentState()
              {
                StarboardForeDeployDirection = component.StarboardForeDeployDirection
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
                args.State = (IComponentState) new RMCEquipmentDeployerComponent.ForeDeployRotationDegrees_FieldComponentState()
                {
                  ForeDeployRotationDegrees = component.ForeDeployRotationDegrees
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.PortWingDeployDirection_FieldComponentState()
              {
                PortWingDeployDirection = component.PortWingDeployDirection
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new RMCEquipmentDeployerComponent.StarboardWingDeployDirection_FieldComponentState()
            {
              StarboardWingDeployDirection = component.StarboardWingDeployDirection
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
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.StarboardWingDeployRotationDegrees_FieldComponentState()
              {
                StarboardWingDeployRotationDegrees = component.StarboardWingDeployRotationDegrees
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new RMCEquipmentDeployerComponent.PortWingDeployRotationDegrees_FieldComponentState()
            {
              PortWingDeployRotationDegrees = component.PortWingDeployRotationDegrees
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
              args.State = (IComponentState) new RMCEquipmentDeployerComponent.ElectronicDeployedSprite_FieldComponentState()
              {
                ElectronicDeployedSprite = component.ElectronicDeployedSprite
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new RMCEquipmentDeployerComponent.WeaponDeployedSprite_FieldComponentState()
            {
              WeaponDeployedSprite = component.WeaponDeployedSprite
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new RMCEquipmentDeployerComponent.UtilityDeployedSprite_FieldComponentState()
          {
            UtilityDeployedSprite = component.UtilityDeployedSprite
          };
          return;
        }
      }
      args.State = (IComponentState) new RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState()
      {
        DeployPrototype = component.DeployPrototype,
        DeploySlotId = component.DeploySlotId,
        DropShipWindowButtonText = component.DropShipWindowButtonText,
        DeployEntity = component.DeployEntity,
        IsDeployable = component.IsDeployable,
        IsDeployed = component.IsDeployed,
        AutoDeploy = component.AutoDeploy,
        AutoUnDeploy = component.AutoUnDeploy,
        IsDeployableByHand = component.IsDeployableByHand,
        AlertLevelRequired = component.AlertLevelRequired,
        Blacklist = component.Blacklist,
        StarboardForeDeployDirection = component.StarboardForeDeployDirection,
        PortForeDeployDirection = component.PortForeDeployDirection,
        StarboardWingDeployDirection = component.StarboardWingDeployDirection,
        PortWingDeployDirection = component.PortWingDeployDirection,
        ForeDeployRotationDegrees = component.ForeDeployRotationDegrees,
        PortWingDeployRotationDegrees = component.PortWingDeployRotationDegrees,
        StarboardWingDeployRotationDegrees = component.StarboardWingDeployRotationDegrees,
        UtilityDeployedSprite = component.UtilityDeployedSprite,
        WeaponDeployedSprite = component.WeaponDeployedSprite,
        ElectronicDeployedSprite = component.ElectronicDeployedSprite
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCEquipmentDeployerComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case RMCEquipmentDeployerComponent.DeployPrototype_FieldComponentState fieldComponentState1:
          component.DeployPrototype = fieldComponentState1.DeployPrototype;
          break;
        case RMCEquipmentDeployerComponent.DeploySlotId_FieldComponentState fieldComponentState2:
          component.DeploySlotId = fieldComponentState2.DeploySlotId;
          break;
        case RMCEquipmentDeployerComponent.DropShipWindowButtonText_FieldComponentState fieldComponentState3:
          component.DropShipWindowButtonText = fieldComponentState3.DropShipWindowButtonText;
          break;
        case RMCEquipmentDeployerComponent.DeployEntity_FieldComponentState fieldComponentState4:
          component.DeployEntity = fieldComponentState4.DeployEntity;
          break;
        case RMCEquipmentDeployerComponent.IsDeployable_FieldComponentState fieldComponentState5:
          component.IsDeployable = fieldComponentState5.IsDeployable;
          break;
        case RMCEquipmentDeployerComponent.IsDeployed_FieldComponentState fieldComponentState6:
          component.IsDeployed = fieldComponentState6.IsDeployed;
          break;
        case RMCEquipmentDeployerComponent.AutoDeploy_FieldComponentState fieldComponentState7:
          component.AutoDeploy = fieldComponentState7.AutoDeploy;
          break;
        case RMCEquipmentDeployerComponent.AutoUnDeploy_FieldComponentState fieldComponentState8:
          component.AutoUnDeploy = fieldComponentState8.AutoUnDeploy;
          break;
        case RMCEquipmentDeployerComponent.IsDeployableByHand_FieldComponentState fieldComponentState9:
          component.IsDeployableByHand = fieldComponentState9.IsDeployableByHand;
          break;
        case RMCEquipmentDeployerComponent.AlertLevelRequired_FieldComponentState fieldComponentState10:
          component.AlertLevelRequired = fieldComponentState10.AlertLevelRequired;
          break;
        case RMCEquipmentDeployerComponent.Blacklist_FieldComponentState fieldComponentState11:
          component.Blacklist = fieldComponentState11.Blacklist;
          break;
        case RMCEquipmentDeployerComponent.StarboardForeDeployDirection_FieldComponentState fieldComponentState12:
          component.StarboardForeDeployDirection = fieldComponentState12.StarboardForeDeployDirection;
          break;
        case RMCEquipmentDeployerComponent.PortForeDeployDirection_FieldComponentState fieldComponentState13:
          component.PortForeDeployDirection = fieldComponentState13.PortForeDeployDirection;
          break;
        case RMCEquipmentDeployerComponent.StarboardWingDeployDirection_FieldComponentState fieldComponentState14:
          component.StarboardWingDeployDirection = fieldComponentState14.StarboardWingDeployDirection;
          break;
        case RMCEquipmentDeployerComponent.PortWingDeployDirection_FieldComponentState fieldComponentState15:
          component.PortWingDeployDirection = fieldComponentState15.PortWingDeployDirection;
          break;
        case RMCEquipmentDeployerComponent.ForeDeployRotationDegrees_FieldComponentState fieldComponentState16:
          component.ForeDeployRotationDegrees = fieldComponentState16.ForeDeployRotationDegrees;
          break;
        case RMCEquipmentDeployerComponent.PortWingDeployRotationDegrees_FieldComponentState fieldComponentState17:
          component.PortWingDeployRotationDegrees = fieldComponentState17.PortWingDeployRotationDegrees;
          break;
        case RMCEquipmentDeployerComponent.StarboardWingDeployRotationDegrees_FieldComponentState fieldComponentState18:
          component.StarboardWingDeployRotationDegrees = fieldComponentState18.StarboardWingDeployRotationDegrees;
          break;
        case RMCEquipmentDeployerComponent.UtilityDeployedSprite_FieldComponentState fieldComponentState19:
          component.UtilityDeployedSprite = fieldComponentState19.UtilityDeployedSprite;
          break;
        case RMCEquipmentDeployerComponent.WeaponDeployedSprite_FieldComponentState fieldComponentState20:
          component.WeaponDeployedSprite = fieldComponentState20.WeaponDeployedSprite;
          break;
        case RMCEquipmentDeployerComponent.ElectronicDeployedSprite_FieldComponentState fieldComponentState21:
          component.ElectronicDeployedSprite = fieldComponentState21.ElectronicDeployedSprite;
          break;
        case RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState componentAutoState:
          component.DeployPrototype = componentAutoState.DeployPrototype;
          component.DeploySlotId = componentAutoState.DeploySlotId;
          component.DropShipWindowButtonText = componentAutoState.DropShipWindowButtonText;
          component.DeployEntity = componentAutoState.DeployEntity;
          component.IsDeployable = componentAutoState.IsDeployable;
          component.IsDeployed = componentAutoState.IsDeployed;
          component.AutoDeploy = componentAutoState.AutoDeploy;
          component.AutoUnDeploy = componentAutoState.AutoUnDeploy;
          component.IsDeployableByHand = componentAutoState.IsDeployableByHand;
          component.AlertLevelRequired = componentAutoState.AlertLevelRequired;
          component.Blacklist = componentAutoState.Blacklist;
          component.StarboardForeDeployDirection = componentAutoState.StarboardForeDeployDirection;
          component.PortForeDeployDirection = componentAutoState.PortForeDeployDirection;
          component.StarboardWingDeployDirection = componentAutoState.StarboardWingDeployDirection;
          component.PortWingDeployDirection = componentAutoState.PortWingDeployDirection;
          component.ForeDeployRotationDegrees = componentAutoState.ForeDeployRotationDegrees;
          component.PortWingDeployRotationDegrees = componentAutoState.PortWingDeployRotationDegrees;
          component.StarboardWingDeployRotationDegrees = componentAutoState.StarboardWingDeployRotationDegrees;
          component.UtilityDeployedSprite = componentAutoState.UtilityDeployedSprite;
          component.WeaponDeployedSprite = componentAutoState.WeaponDeployedSprite;
          component.ElectronicDeployedSprite = componentAutoState.ElectronicDeployedSprite;
          break;
        default:
          return;
      }
      IComponentState current = args.Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RMCEquipmentDeployerComponent>(uid, component, ref args1);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DeployPrototype_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntProtoId? DeployPrototype;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.DeployPrototype = this.DeployPrototype;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DeploySlotId_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string DeploySlotId;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.DeploySlotId = this.DeploySlotId;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DropShipWindowButtonText_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string DropShipWindowButtonText;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.DropShipWindowButtonText = this.DropShipWindowButtonText;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DeployEntity_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? DeployEntity;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.DeployEntity = this.DeployEntity;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsDeployable_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsDeployable;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.IsDeployable = this.IsDeployable;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsDeployed_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsDeployed;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.IsDeployed = this.IsDeployed;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AutoDeploy_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool AutoDeploy;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.AutoDeploy = this.AutoDeploy;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AutoUnDeploy_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool AutoUnDeploy;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.AutoUnDeploy = this.AutoUnDeploy;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsDeployableByHand_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsDeployableByHand;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.IsDeployableByHand = this.IsDeployableByHand;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AlertLevelRequired_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public RMCAlertLevels AlertLevelRequired;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.AlertLevelRequired = this.AlertLevelRequired;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Blacklist_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntityWhitelist? Blacklist;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.Blacklist = this.Blacklist;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StarboardForeDeployDirection_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Vector2i StarboardForeDeployDirection;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.StarboardForeDeployDirection = this.StarboardForeDeployDirection;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PortForeDeployDirection_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Vector2i PortForeDeployDirection;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.PortForeDeployDirection = this.PortForeDeployDirection;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StarboardWingDeployDirection_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Vector2i StarboardWingDeployDirection;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.StarboardWingDeployDirection = this.StarboardWingDeployDirection;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PortWingDeployDirection_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Vector2i PortWingDeployDirection;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.PortWingDeployDirection = this.PortWingDeployDirection;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ForeDeployRotationDegrees_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float ForeDeployRotationDegrees;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.ForeDeployRotationDegrees = this.ForeDeployRotationDegrees;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PortWingDeployRotationDegrees_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float PortWingDeployRotationDegrees;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.PortWingDeployRotationDegrees = this.PortWingDeployRotationDegrees;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StarboardWingDeployRotationDegrees_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float StarboardWingDeployRotationDegrees;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.StarboardWingDeployRotationDegrees = this.StarboardWingDeployRotationDegrees;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UtilityDeployedSprite_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SpriteSpecifier.Rsi? UtilityDeployedSprite;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.UtilityDeployedSprite = this.UtilityDeployedSprite;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class WeaponDeployedSprite_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SpriteSpecifier.Rsi? WeaponDeployedSprite;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.WeaponDeployedSprite = this.WeaponDeployedSprite;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ElectronicDeployedSprite_FieldComponentState : 
    IComponentDeltaState<RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SpriteSpecifier.Rsi? ElectronicDeployedSprite;

    public void ApplyToFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      fullState.ElectronicDeployedSprite = this.ElectronicDeployedSprite;
    }

    public RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState CreateNewFullState(
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState)
    {
      RMCEquipmentDeployerComponent.RMCEquipmentDeployerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
