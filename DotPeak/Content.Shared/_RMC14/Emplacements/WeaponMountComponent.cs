// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emplacements.WeaponMountComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
using Content.Shared.Tools;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Emplacements;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, true)]
[Access(new Type[] {typeof (SharedWeaponMountSystem)})]
public sealed class WeaponMountComponent : 
  Component,
  ISerializationGenerated<WeaponMountComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? MountableWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? FixedWeaponPrototype;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? MountedEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsWeaponSecured;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsWeaponLocked;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan AssembleDelay = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DisassembleDelay = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> RotationTool = (ProtoId<ToolQualityPrototype>) "Anchoring";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> DismantlingTool = (ProtoId<ToolQualityPrototype>) "Prying";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string WeaponSlotId = "weapon";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype> MountSize = (ProtoId<ItemSizePrototype>) "Normal";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype> MountedWeaponSize = (ProtoId<ItemSizePrototype>) "Huge";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MountExclusionAreaSize = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BarricadeExclusionAreaSize;
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string? DismountAction = "RMCActionDismount";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? DismountActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanRotateWithoutTool;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MountOnDeploy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Broken;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BusyHands;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? BusyPopup;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UndeploySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/screwdriver.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RotateSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/ratchet.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DetachSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/crowbar.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SecureSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/deconstruct.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DeploySound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WeaponMountComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WeaponMountComponent) target1;
    if (serialization.TryCustomCopy<WeaponMountComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.MountableWhitelist, ref target2, hookCtx, false, context))
    {
      if (this.MountableWhitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.MountableWhitelist, ref target2, hookCtx, context);
    }
    target.MountableWhitelist = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.FixedWeaponPrototype, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.FixedWeaponPrototype, hookCtx, context);
    target.FixedWeaponPrototype = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MountedEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.MountedEntity, hookCtx, context);
    target.MountedEntity = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.User, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.User, hookCtx, context);
    target.User = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsWeaponSecured, ref target6, hookCtx, false, context))
      target6 = this.IsWeaponSecured;
    target.IsWeaponSecured = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsWeaponLocked, ref target7, hookCtx, false, context))
      target7 = this.IsWeaponLocked;
    target.IsWeaponLocked = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AssembleDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.AssembleDelay, hookCtx, context);
    target.AssembleDelay = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DisassembleDelay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.DisassembleDelay, hookCtx, context);
    target.DisassembleDelay = target9;
    ProtoId<ToolQualityPrototype> target10 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.RotationTool, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.RotationTool, hookCtx, context);
    target.RotationTool = target10;
    ProtoId<ToolQualityPrototype> target11 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.DismantlingTool, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.DismantlingTool, hookCtx, context);
    target.DismantlingTool = target11;
    string target12 = (string) null;
    if (this.WeaponSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.WeaponSlotId, ref target12, hookCtx, false, context))
      target12 = this.WeaponSlotId;
    target.WeaponSlotId = target12;
    ProtoId<ItemSizePrototype> target13 = new ProtoId<ItemSizePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(this.MountSize, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(this.MountSize, hookCtx, context);
    target.MountSize = target13;
    ProtoId<ItemSizePrototype> target14 = new ProtoId<ItemSizePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(this.MountedWeaponSize, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(this.MountedWeaponSize, hookCtx, context);
    target.MountedWeaponSize = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.MountExclusionAreaSize, ref target15, hookCtx, false, context))
      target15 = this.MountExclusionAreaSize;
    target.MountExclusionAreaSize = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.BarricadeExclusionAreaSize, ref target16, hookCtx, false, context))
      target16 = this.BarricadeExclusionAreaSize;
    target.BarricadeExclusionAreaSize = target16;
    string target17 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DismountAction, ref target17, hookCtx, false, context))
      target17 = this.DismountAction;
    target.DismountAction = target17;
    EntityUid? target18 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.DismountActionEntity, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntityUid?>(this.DismountActionEntity, hookCtx, context);
    target.DismountActionEntity = target18;
    bool target19 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanRotateWithoutTool, ref target19, hookCtx, false, context))
      target19 = this.CanRotateWithoutTool;
    target.CanRotateWithoutTool = target19;
    bool target20 = false;
    if (!serialization.TryCustomCopy<bool>(this.MountOnDeploy, ref target20, hookCtx, false, context))
      target20 = this.MountOnDeploy;
    target.MountOnDeploy = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.Broken, ref target21, hookCtx, false, context))
      target21 = this.Broken;
    target.Broken = target21;
    int target22 = 0;
    if (!serialization.TryCustomCopy<int>(this.BusyHands, ref target22, hookCtx, false, context))
      target22 = this.BusyHands;
    target.BusyHands = target22;
    string target23 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BusyPopup, ref target23, hookCtx, false, context))
      target23 = this.BusyPopup;
    target.BusyPopup = target23;
    SoundSpecifier target24 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UndeploySound, ref target24, hookCtx, true, context))
      target24 = serialization.CreateCopy<SoundSpecifier>(this.UndeploySound, hookCtx, context);
    target.UndeploySound = target24;
    SoundSpecifier target25 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RotateSound, ref target25, hookCtx, true, context))
      target25 = serialization.CreateCopy<SoundSpecifier>(this.RotateSound, hookCtx, context);
    target.RotateSound = target25;
    SoundSpecifier target26 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DetachSound, ref target26, hookCtx, true, context))
      target26 = serialization.CreateCopy<SoundSpecifier>(this.DetachSound, hookCtx, context);
    target.DetachSound = target26;
    SoundSpecifier target27 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SecureSound, ref target27, hookCtx, true, context))
      target27 = serialization.CreateCopy<SoundSpecifier>(this.SecureSound, hookCtx, context);
    target.SecureSound = target27;
    SoundSpecifier target28 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeploySound, ref target28, hookCtx, true, context))
      target28 = serialization.CreateCopy<SoundSpecifier>(this.DeploySound, hookCtx, context);
    target.DeploySound = target28;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WeaponMountComponent target,
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
    WeaponMountComponent target1 = (WeaponMountComponent) target;
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
    WeaponMountComponent target1 = (WeaponMountComponent) target;
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
    WeaponMountComponent target1 = (WeaponMountComponent) target;
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
    WeaponMountComponent target1 = (WeaponMountComponent) target;
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
  virtual WeaponMountComponent Component.Instantiate() => new WeaponMountComponent();

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
  public sealed class WeaponMountComponent_AutoState : IComponentState
  {
    public EntityWhitelist? MountableWhitelist;
    public EntProtoId? FixedWeaponPrototype;
    public NetEntity? MountedEntity;
    public NetEntity? User;
    public bool IsWeaponSecured;
    public bool IsWeaponLocked;
    public TimeSpan AssembleDelay;
    public TimeSpan DisassembleDelay;
    public ProtoId<ToolQualityPrototype> RotationTool;
    public ProtoId<ToolQualityPrototype> DismantlingTool;
    public string WeaponSlotId;
    public ProtoId<ItemSizePrototype> MountSize;
    public ProtoId<ItemSizePrototype> MountedWeaponSize;
    public int MountExclusionAreaSize;
    public int BarricadeExclusionAreaSize;
    public NetEntity? DismountActionEntity;
    public bool CanRotateWithoutTool;
    public bool MountOnDeploy;
    public bool Broken;
    public int BusyHands;
    public string? BusyPopup;

    public WeaponMountComponent.WeaponMountComponent_AutoState ShallowClone()
    {
      return new WeaponMountComponent.WeaponMountComponent_AutoState()
      {
        MountableWhitelist = this.MountableWhitelist,
        FixedWeaponPrototype = this.FixedWeaponPrototype,
        MountedEntity = this.MountedEntity,
        User = this.User,
        IsWeaponSecured = this.IsWeaponSecured,
        IsWeaponLocked = this.IsWeaponLocked,
        AssembleDelay = this.AssembleDelay,
        DisassembleDelay = this.DisassembleDelay,
        RotationTool = this.RotationTool,
        DismantlingTool = this.DismantlingTool,
        WeaponSlotId = this.WeaponSlotId,
        MountSize = this.MountSize,
        MountedWeaponSize = this.MountedWeaponSize,
        MountExclusionAreaSize = this.MountExclusionAreaSize,
        BarricadeExclusionAreaSize = this.BarricadeExclusionAreaSize,
        DismountActionEntity = this.DismountActionEntity,
        CanRotateWithoutTool = this.CanRotateWithoutTool,
        MountOnDeploy = this.MountOnDeploy,
        Broken = this.Broken,
        BusyHands = this.BusyHands,
        BusyPopup = this.BusyPopup
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WeaponMountComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<WeaponMountComponent>("MountableWhitelist", "FixedWeaponPrototype", "MountedEntity", "User", "IsWeaponSecured", "IsWeaponLocked", "AssembleDelay", "DisassembleDelay", "RotationTool", "DismantlingTool", "WeaponSlotId", "MountSize", "MountedWeaponSize", "MountExclusionAreaSize", "BarricadeExclusionAreaSize", "DismountActionEntity", "CanRotateWithoutTool", "MountOnDeploy", "Broken", "BusyHands", "BusyPopup");
      this.SubscribeLocalEvent<WeaponMountComponent, ComponentGetState>(new ComponentEventRefHandler<WeaponMountComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WeaponMountComponent, ComponentHandleState>(new ComponentEventRefHandler<WeaponMountComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WeaponMountComponent component,
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
                  args.State = (IComponentState) new WeaponMountComponent.MountableWhitelist_FieldComponentState()
                  {
                    MountableWhitelist = component.MountableWhitelist
                  };
                  return;
                case 1:
                  args.State = (IComponentState) new WeaponMountComponent.FixedWeaponPrototype_FieldComponentState()
                  {
                    FixedWeaponPrototype = component.FixedWeaponPrototype
                  };
                  return;
                case 2:
                  break;
                case 3:
                  args.State = (IComponentState) new WeaponMountComponent.MountedEntity_FieldComponentState()
                  {
                    MountedEntity = this.GetNetEntity(component.MountedEntity)
                  };
                  return;
                default:
                  if (modifiedFields == 8U)
                  {
                    args.State = (IComponentState) new WeaponMountComponent.User_FieldComponentState()
                    {
                      User = this.GetNetEntity(component.User)
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
                args.State = (IComponentState) new WeaponMountComponent.IsWeaponLocked_FieldComponentState()
                {
                  IsWeaponLocked = component.IsWeaponLocked
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new WeaponMountComponent.IsWeaponSecured_FieldComponentState()
              {
                IsWeaponSecured = component.IsWeaponSecured
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
                args.State = (IComponentState) new WeaponMountComponent.DisassembleDelay_FieldComponentState()
                {
                  DisassembleDelay = component.DisassembleDelay
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new WeaponMountComponent.AssembleDelay_FieldComponentState()
              {
                AssembleDelay = component.AssembleDelay
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
                args.State = (IComponentState) new WeaponMountComponent.WeaponSlotId_FieldComponentState()
                {
                  WeaponSlotId = component.WeaponSlotId
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new WeaponMountComponent.DismantlingTool_FieldComponentState()
              {
                DismantlingTool = component.DismantlingTool
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new WeaponMountComponent.RotationTool_FieldComponentState()
            {
              RotationTool = component.RotationTool
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
                args.State = (IComponentState) new WeaponMountComponent.MountedWeaponSize_FieldComponentState()
                {
                  MountedWeaponSize = component.MountedWeaponSize
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new WeaponMountComponent.MountSize_FieldComponentState()
              {
                MountSize = component.MountSize
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
                args.State = (IComponentState) new WeaponMountComponent.DismountActionEntity_FieldComponentState()
                {
                  DismountActionEntity = this.GetNetEntity(component.DismountActionEntity)
                };
                return;
              }
            }
            else
            {
              args.State = (IComponentState) new WeaponMountComponent.BarricadeExclusionAreaSize_FieldComponentState()
              {
                BarricadeExclusionAreaSize = component.BarricadeExclusionAreaSize
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new WeaponMountComponent.MountExclusionAreaSize_FieldComponentState()
            {
              MountExclusionAreaSize = component.MountExclusionAreaSize
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
              args.State = (IComponentState) new WeaponMountComponent.MountOnDeploy_FieldComponentState()
              {
                MountOnDeploy = component.MountOnDeploy
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new WeaponMountComponent.CanRotateWithoutTool_FieldComponentState()
            {
              CanRotateWithoutTool = component.CanRotateWithoutTool
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
              args.State = (IComponentState) new WeaponMountComponent.BusyPopup_FieldComponentState()
              {
                BusyPopup = component.BusyPopup
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new WeaponMountComponent.BusyHands_FieldComponentState()
            {
              BusyHands = component.BusyHands
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new WeaponMountComponent.Broken_FieldComponentState()
          {
            Broken = component.Broken
          };
          return;
        }
      }
      args.State = (IComponentState) new WeaponMountComponent.WeaponMountComponent_AutoState()
      {
        MountableWhitelist = component.MountableWhitelist,
        FixedWeaponPrototype = component.FixedWeaponPrototype,
        MountedEntity = this.GetNetEntity(component.MountedEntity),
        User = this.GetNetEntity(component.User),
        IsWeaponSecured = component.IsWeaponSecured,
        IsWeaponLocked = component.IsWeaponLocked,
        AssembleDelay = component.AssembleDelay,
        DisassembleDelay = component.DisassembleDelay,
        RotationTool = component.RotationTool,
        DismantlingTool = component.DismantlingTool,
        WeaponSlotId = component.WeaponSlotId,
        MountSize = component.MountSize,
        MountedWeaponSize = component.MountedWeaponSize,
        MountExclusionAreaSize = component.MountExclusionAreaSize,
        BarricadeExclusionAreaSize = component.BarricadeExclusionAreaSize,
        DismountActionEntity = this.GetNetEntity(component.DismountActionEntity),
        CanRotateWithoutTool = component.CanRotateWithoutTool,
        MountOnDeploy = component.MountOnDeploy,
        Broken = component.Broken,
        BusyHands = component.BusyHands,
        BusyPopup = component.BusyPopup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WeaponMountComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case WeaponMountComponent.MountableWhitelist_FieldComponentState fieldComponentState1:
          component.MountableWhitelist = fieldComponentState1.MountableWhitelist;
          break;
        case WeaponMountComponent.FixedWeaponPrototype_FieldComponentState fieldComponentState2:
          component.FixedWeaponPrototype = fieldComponentState2.FixedWeaponPrototype;
          break;
        case WeaponMountComponent.MountedEntity_FieldComponentState fieldComponentState3:
          component.MountedEntity = this.EnsureEntity<WeaponMountComponent>(fieldComponentState3.MountedEntity, uid);
          break;
        case WeaponMountComponent.User_FieldComponentState fieldComponentState4:
          component.User = this.EnsureEntity<WeaponMountComponent>(fieldComponentState4.User, uid);
          break;
        case WeaponMountComponent.IsWeaponSecured_FieldComponentState fieldComponentState5:
          component.IsWeaponSecured = fieldComponentState5.IsWeaponSecured;
          break;
        case WeaponMountComponent.IsWeaponLocked_FieldComponentState fieldComponentState6:
          component.IsWeaponLocked = fieldComponentState6.IsWeaponLocked;
          break;
        case WeaponMountComponent.AssembleDelay_FieldComponentState fieldComponentState7:
          component.AssembleDelay = fieldComponentState7.AssembleDelay;
          break;
        case WeaponMountComponent.DisassembleDelay_FieldComponentState fieldComponentState8:
          component.DisassembleDelay = fieldComponentState8.DisassembleDelay;
          break;
        case WeaponMountComponent.RotationTool_FieldComponentState fieldComponentState9:
          component.RotationTool = fieldComponentState9.RotationTool;
          break;
        case WeaponMountComponent.DismantlingTool_FieldComponentState fieldComponentState10:
          component.DismantlingTool = fieldComponentState10.DismantlingTool;
          break;
        case WeaponMountComponent.WeaponSlotId_FieldComponentState fieldComponentState11:
          component.WeaponSlotId = fieldComponentState11.WeaponSlotId;
          break;
        case WeaponMountComponent.MountSize_FieldComponentState fieldComponentState12:
          component.MountSize = fieldComponentState12.MountSize;
          break;
        case WeaponMountComponent.MountedWeaponSize_FieldComponentState fieldComponentState13:
          component.MountedWeaponSize = fieldComponentState13.MountedWeaponSize;
          break;
        case WeaponMountComponent.MountExclusionAreaSize_FieldComponentState fieldComponentState14:
          component.MountExclusionAreaSize = fieldComponentState14.MountExclusionAreaSize;
          break;
        case WeaponMountComponent.BarricadeExclusionAreaSize_FieldComponentState fieldComponentState15:
          component.BarricadeExclusionAreaSize = fieldComponentState15.BarricadeExclusionAreaSize;
          break;
        case WeaponMountComponent.DismountActionEntity_FieldComponentState fieldComponentState16:
          component.DismountActionEntity = this.EnsureEntity<WeaponMountComponent>(fieldComponentState16.DismountActionEntity, uid);
          break;
        case WeaponMountComponent.CanRotateWithoutTool_FieldComponentState fieldComponentState17:
          component.CanRotateWithoutTool = fieldComponentState17.CanRotateWithoutTool;
          break;
        case WeaponMountComponent.MountOnDeploy_FieldComponentState fieldComponentState18:
          component.MountOnDeploy = fieldComponentState18.MountOnDeploy;
          break;
        case WeaponMountComponent.Broken_FieldComponentState fieldComponentState19:
          component.Broken = fieldComponentState19.Broken;
          break;
        case WeaponMountComponent.BusyHands_FieldComponentState fieldComponentState20:
          component.BusyHands = fieldComponentState20.BusyHands;
          break;
        case WeaponMountComponent.BusyPopup_FieldComponentState fieldComponentState21:
          component.BusyPopup = fieldComponentState21.BusyPopup;
          break;
        case WeaponMountComponent.WeaponMountComponent_AutoState componentAutoState:
          component.MountableWhitelist = componentAutoState.MountableWhitelist;
          component.FixedWeaponPrototype = componentAutoState.FixedWeaponPrototype;
          component.MountedEntity = this.EnsureEntity<WeaponMountComponent>(componentAutoState.MountedEntity, uid);
          component.User = this.EnsureEntity<WeaponMountComponent>(componentAutoState.User, uid);
          component.IsWeaponSecured = componentAutoState.IsWeaponSecured;
          component.IsWeaponLocked = componentAutoState.IsWeaponLocked;
          component.AssembleDelay = componentAutoState.AssembleDelay;
          component.DisassembleDelay = componentAutoState.DisassembleDelay;
          component.RotationTool = componentAutoState.RotationTool;
          component.DismantlingTool = componentAutoState.DismantlingTool;
          component.WeaponSlotId = componentAutoState.WeaponSlotId;
          component.MountSize = componentAutoState.MountSize;
          component.MountedWeaponSize = componentAutoState.MountedWeaponSize;
          component.MountExclusionAreaSize = componentAutoState.MountExclusionAreaSize;
          component.BarricadeExclusionAreaSize = componentAutoState.BarricadeExclusionAreaSize;
          component.DismountActionEntity = this.EnsureEntity<WeaponMountComponent>(componentAutoState.DismountActionEntity, uid);
          component.CanRotateWithoutTool = componentAutoState.CanRotateWithoutTool;
          component.MountOnDeploy = componentAutoState.MountOnDeploy;
          component.Broken = componentAutoState.Broken;
          component.BusyHands = componentAutoState.BusyHands;
          component.BusyPopup = componentAutoState.BusyPopup;
          break;
        default:
          return;
      }
      IComponentState current = args.Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, WeaponMountComponent>(uid, component, ref args1);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MountableWhitelist_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntityWhitelist? MountableWhitelist;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.MountableWhitelist = this.MountableWhitelist;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FixedWeaponPrototype_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public EntProtoId? FixedWeaponPrototype;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.FixedWeaponPrototype = this.FixedWeaponPrototype;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MountedEntity_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? MountedEntity;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.MountedEntity = this.MountedEntity;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class User_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? User;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.User = this.User;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsWeaponSecured_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsWeaponSecured;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.IsWeaponSecured = this.IsWeaponSecured;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsWeaponLocked_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsWeaponLocked;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.IsWeaponLocked = this.IsWeaponLocked;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AssembleDelay_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan AssembleDelay;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.AssembleDelay = this.AssembleDelay;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DisassembleDelay_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan DisassembleDelay;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.DisassembleDelay = this.DisassembleDelay;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RotationTool_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<ToolQualityPrototype> RotationTool;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.RotationTool = this.RotationTool;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DismantlingTool_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<ToolQualityPrototype> DismantlingTool;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.DismantlingTool = this.DismantlingTool;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class WeaponSlotId_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string WeaponSlotId;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.WeaponSlotId = this.WeaponSlotId;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MountSize_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<ItemSizePrototype> MountSize;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.MountSize = this.MountSize;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MountedWeaponSize_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<ItemSizePrototype> MountedWeaponSize;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.MountedWeaponSize = this.MountedWeaponSize;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MountExclusionAreaSize_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int MountExclusionAreaSize;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.MountExclusionAreaSize = this.MountExclusionAreaSize;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BarricadeExclusionAreaSize_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int BarricadeExclusionAreaSize;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.BarricadeExclusionAreaSize = this.BarricadeExclusionAreaSize;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DismountActionEntity_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? DismountActionEntity;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.DismountActionEntity = this.DismountActionEntity;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CanRotateWithoutTool_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool CanRotateWithoutTool;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.CanRotateWithoutTool = this.CanRotateWithoutTool;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MountOnDeploy_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool MountOnDeploy;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.MountOnDeploy = this.MountOnDeploy;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Broken_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Broken;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.Broken = this.Broken;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BusyHands_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int BusyHands;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.BusyHands = this.BusyHands;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BusyPopup_FieldComponentState : 
    IComponentDeltaState<WeaponMountComponent.WeaponMountComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string? BusyPopup;

    public void ApplyToFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      fullState.BusyPopup = this.BusyPopup;
    }

    public WeaponMountComponent.WeaponMountComponent_AutoState CreateNewFullState(
      WeaponMountComponent.WeaponMountComponent_AutoState fullState)
    {
      WeaponMountComponent.WeaponMountComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
