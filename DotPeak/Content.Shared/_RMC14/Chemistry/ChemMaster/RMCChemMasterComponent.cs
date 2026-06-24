// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.ChemMaster.RMCChemMasterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.ChemMaster;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedRMCChemMasterSystem)})]
public sealed class RMCChemMasterComponent : 
  Component,
  ISerializationGenerated<RMCChemMasterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 BottleSize = FixedPoint2.New(60);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string BufferSolutionId = "buffer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string BeakerSlot = "beakerSlot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string PillBottleContainer = "rmc_pill_bottle";
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist PillBottleWhitelist = new EntityWhitelist();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxPillBottles = 8;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> SelectedBottles = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxLabelLength = 64 /*0x40*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi PillRsi = new SpriteSpecifier.Rsi(new ResPath("_RMC14/Objects/Chemistry/pills.rsi"), "pill1");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ResPath PillCanisterRsi = new ResPath("_RMC14/Objects/Chemistry/pill_canister.rsi");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2[] TransferSettings = new FixedPoint2[5]
  {
    (FixedPoint2) 1,
    (FixedPoint2) 5,
    (FixedPoint2) 10,
    (FixedPoint2) 30,
    (FixedPoint2) 60
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCChemMasterBufferMode BufferTransferMode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PillTypes = 22;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PillCanisterTypes = 12;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PillAmount = 16 /*0x10*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxPillAmount = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public uint SelectedType = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float LinkRange = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId PillProto = (EntProtoId) "CMPill";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? PillBottleInsertSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagIn/revolver_magin.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? PillBottleEjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/MagOut/revolver_magout.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCChemMasterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCChemMasterComponent) target1;
    if (serialization.TryCustomCopy<RMCChemMasterComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.BottleSize, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.BottleSize, hookCtx, context);
    target.BottleSize = target2;
    string target3 = (string) null;
    if (this.BufferSolutionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BufferSolutionId, ref target3, hookCtx, false, context))
      target3 = this.BufferSolutionId;
    target.BufferSolutionId = target3;
    string target4 = (string) null;
    if (this.BeakerSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BeakerSlot, ref target4, hookCtx, false, context))
      target4 = this.BeakerSlot;
    target.BeakerSlot = target4;
    string target5 = (string) null;
    if (this.PillBottleContainer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PillBottleContainer, ref target5, hookCtx, false, context))
      target5 = this.PillBottleContainer;
    target.PillBottleContainer = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (this.PillBottleWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.PillBottleWhitelist, ref target6, hookCtx, false, context))
    {
      if (this.PillBottleWhitelist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.PillBottleWhitelist, ref target6, hookCtx, context, true);
    }
    target.PillBottleWhitelist = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPillBottles, ref target7, hookCtx, false, context))
      target7 = this.MaxPillBottles;
    target.MaxPillBottles = target7;
    HashSet<EntityUid> target8 = (HashSet<EntityUid>) null;
    if (this.SelectedBottles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.SelectedBottles, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<HashSet<EntityUid>>(this.SelectedBottles, hookCtx, context);
    target.SelectedBottles = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxLabelLength, ref target9, hookCtx, false, context))
      target9 = this.MaxLabelLength;
    target.MaxLabelLength = target9;
    SpriteSpecifier.Rsi target10 = (SpriteSpecifier.Rsi) null;
    if (this.PillRsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.PillRsi, ref target10, hookCtx, false, context))
    {
      if (this.PillRsi == null)
        target10 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.PillRsi, ref target10, hookCtx, context, true);
    }
    target.PillRsi = target10;
    ResPath target11 = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.PillCanisterRsi, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<ResPath>(this.PillCanisterRsi, hookCtx, context);
    target.PillCanisterRsi = target11;
    FixedPoint2[] target12 = (FixedPoint2[]) null;
    if (this.TransferSettings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FixedPoint2[]>(this.TransferSettings, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<FixedPoint2[]>(this.TransferSettings, hookCtx, context);
    target.TransferSettings = target12;
    RMCChemMasterBufferMode target13 = RMCChemMasterBufferMode.ToBeaker;
    if (!serialization.TryCustomCopy<RMCChemMasterBufferMode>(this.BufferTransferMode, ref target13, hookCtx, false, context))
      target13 = this.BufferTransferMode;
    target.BufferTransferMode = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.PillTypes, ref target14, hookCtx, false, context))
      target14 = this.PillTypes;
    target.PillTypes = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.PillCanisterTypes, ref target15, hookCtx, false, context))
      target15 = this.PillCanisterTypes;
    target.PillCanisterTypes = target15;
    int target16 = 0;
    if (!serialization.TryCustomCopy<int>(this.PillAmount, ref target16, hookCtx, false, context))
      target16 = this.PillAmount;
    target.PillAmount = target16;
    int target17 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPillAmount, ref target17, hookCtx, false, context))
      target17 = this.MaxPillAmount;
    target.MaxPillAmount = target17;
    uint target18 = 0;
    if (!serialization.TryCustomCopy<uint>(this.SelectedType, ref target18, hookCtx, false, context))
      target18 = this.SelectedType;
    target.SelectedType = target18;
    float target19 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LinkRange, ref target19, hookCtx, false, context))
      target19 = this.LinkRange;
    target.LinkRange = target19;
    EntProtoId target20 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.PillProto, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<EntProtoId>(this.PillProto, hookCtx, context);
    target.PillProto = target20;
    SoundSpecifier target21 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PillBottleInsertSound, ref target21, hookCtx, true, context))
      target21 = serialization.CreateCopy<SoundSpecifier>(this.PillBottleInsertSound, hookCtx, context);
    target.PillBottleInsertSound = target21;
    SoundSpecifier target22 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PillBottleEjectSound, ref target22, hookCtx, true, context))
      target22 = serialization.CreateCopy<SoundSpecifier>(this.PillBottleEjectSound, hookCtx, context);
    target.PillBottleEjectSound = target22;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCChemMasterComponent target,
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
    RMCChemMasterComponent target1 = (RMCChemMasterComponent) target;
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
    RMCChemMasterComponent target1 = (RMCChemMasterComponent) target;
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
    RMCChemMasterComponent target1 = (RMCChemMasterComponent) target;
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
  virtual RMCChemMasterComponent Component.Instantiate() => new RMCChemMasterComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCChemMasterComponent_AutoState : IComponentState
  {
    public FixedPoint2 BottleSize;
    public string BufferSolutionId;
    public string BeakerSlot;
    public string PillBottleContainer;
    public EntityWhitelist PillBottleWhitelist;
    public int MaxPillBottles;
    public HashSet<NetEntity> SelectedBottles;
    public int MaxLabelLength;
    public SpriteSpecifier.Rsi PillRsi;
    public ResPath PillCanisterRsi;
    public FixedPoint2[] TransferSettings;
    public RMCChemMasterBufferMode BufferTransferMode;
    public int PillTypes;
    public int PillCanisterTypes;
    public int PillAmount;
    public int MaxPillAmount;
    public uint SelectedType;
    public float LinkRange;
    public EntProtoId PillProto;
    public SoundSpecifier? PillBottleInsertSound;
    public SoundSpecifier? PillBottleEjectSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCChemMasterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCChemMasterComponent, ComponentGetState>(new ComponentEventRefHandler<RMCChemMasterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCChemMasterComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCChemMasterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCChemMasterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCChemMasterComponent.RMCChemMasterComponent_AutoState()
      {
        BottleSize = component.BottleSize,
        BufferSolutionId = component.BufferSolutionId,
        BeakerSlot = component.BeakerSlot,
        PillBottleContainer = component.PillBottleContainer,
        PillBottleWhitelist = component.PillBottleWhitelist,
        MaxPillBottles = component.MaxPillBottles,
        SelectedBottles = this.GetNetEntitySet(component.SelectedBottles),
        MaxLabelLength = component.MaxLabelLength,
        PillRsi = component.PillRsi,
        PillCanisterRsi = component.PillCanisterRsi,
        TransferSettings = component.TransferSettings,
        BufferTransferMode = component.BufferTransferMode,
        PillTypes = component.PillTypes,
        PillCanisterTypes = component.PillCanisterTypes,
        PillAmount = component.PillAmount,
        MaxPillAmount = component.MaxPillAmount,
        SelectedType = component.SelectedType,
        LinkRange = component.LinkRange,
        PillProto = component.PillProto,
        PillBottleInsertSound = component.PillBottleInsertSound,
        PillBottleEjectSound = component.PillBottleEjectSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCChemMasterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCChemMasterComponent.RMCChemMasterComponent_AutoState current))
        return;
      component.BottleSize = current.BottleSize;
      component.BufferSolutionId = current.BufferSolutionId;
      component.BeakerSlot = current.BeakerSlot;
      component.PillBottleContainer = current.PillBottleContainer;
      component.PillBottleWhitelist = current.PillBottleWhitelist;
      component.MaxPillBottles = current.MaxPillBottles;
      this.EnsureEntitySet<RMCChemMasterComponent>(current.SelectedBottles, uid, component.SelectedBottles);
      component.MaxLabelLength = current.MaxLabelLength;
      component.PillRsi = current.PillRsi;
      component.PillCanisterRsi = current.PillCanisterRsi;
      component.TransferSettings = current.TransferSettings;
      component.BufferTransferMode = current.BufferTransferMode;
      component.PillTypes = current.PillTypes;
      component.PillCanisterTypes = current.PillCanisterTypes;
      component.PillAmount = current.PillAmount;
      component.MaxPillAmount = current.MaxPillAmount;
      component.SelectedType = current.SelectedType;
      component.LinkRange = current.LinkRange;
      component.PillProto = current.PillProto;
      component.PillBottleInsertSound = current.PillBottleInsertSound;
      component.PillBottleEjectSound = current.PillBottleEjectSound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RMCChemMasterComponent>(uid, component, ref args1);
    }
  }
}
