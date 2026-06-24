// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.VendingMachineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.VendingMachines;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
public sealed class VendingMachineComponent : 
  Component,
  ISerializationGenerated<VendingMachineComponent>,
  ISerializationGenerated
{
  [DataField("pack", false, 1, true, false, typeof (PrototypeIdSerializer<VendingMachineInventoryPrototype>))]
  public string PackPrototypeId;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DenyDelay;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan EjectDelay;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, VendingMachineInventoryEntry> Inventory;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, VendingMachineInventoryEntry> EmaggedInventory;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, VendingMachineInventoryEntry> ContrabandInventory;
  [DataField(null, false, 1, false, false, null)]
  public bool Contraband;
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan? EjectEnd;
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan? DenyEnd;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? DispenseOnHitEnd;
  public string? NextItemToEject;
  public bool Broken;
  [DataField(null, false, 1, false, false, null)]
  public bool CanShoot;
  public bool ThrowNextItem;
  [DataField(null, false, 1, false, false, null)]
  public float? DispenseOnHitChance;
  [DataField(null, false, 1, false, false, null)]
  public float? DispenseOnHitThreshold;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan? DispenseOnHitCooldown;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SoundVend;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SoundDeny;
  public float NonLimitedEjectForce;
  public float NonLimitedEjectRange;
  [DataField(null, false, 1, false, false, null)]
  public float InitialStockQuality;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextEmpEject;
  [DataField(null, false, 1, false, false, null)]
  public string? OffState;
  [DataField(null, false, 1, false, false, null)]
  public string? ScreenState;
  [DataField(null, false, 1, false, false, null)]
  public string? NormalState;
  [DataField(null, false, 1, false, false, null)]
  public string? EjectState;
  [DataField(null, false, 1, false, false, null)]
  public string? DenyState;
  [DataField(null, false, 1, false, false, null)]
  public string? BrokenState;
  [DataField("loopDeny", false, 1, false, false, null)]
  public bool LoopDenyAnimation;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Ejecting => this.EjectEnd.HasValue;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Denying => this.DenyEnd.HasValue;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool DispenseOnHitCoolingDown => this.DispenseOnHitEnd.HasValue;

  public VendingMachineComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Machines/machine_vend.ogg");
    soundPathSpecifier.Params = new AudioParams()
    {
      Volume = -4f,
      Variation = new float?(0.15f)
    };
    this.SoundVend = (SoundSpecifier) soundPathSpecifier;
    this.SoundDeny = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg");
    this.NonLimitedEjectForce = 7.5f;
    this.NonLimitedEjectRange = 5f;
    this.InitialStockQuality = 1f;
    this.NextEmpEject = TimeSpan.Zero;
    this.LoopDenyAnimation = true;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VendingMachineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VendingMachineComponent) target1;
    if (serialization.TryCustomCopy<VendingMachineComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.PackPrototypeId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PackPrototypeId, ref target2, hookCtx, false, context))
      target2 = this.PackPrototypeId;
    target.PackPrototypeId = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DenyDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DenyDelay, hookCtx, context);
    target.DenyDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.EjectDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.EjectDelay, hookCtx, context);
    target.EjectDelay = target4;
    Dictionary<string, VendingMachineInventoryEntry> target5 = (Dictionary<string, VendingMachineInventoryEntry>) null;
    if (this.Inventory == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, VendingMachineInventoryEntry>>(this.Inventory, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<string, VendingMachineInventoryEntry>>(this.Inventory, hookCtx, context);
    target.Inventory = target5;
    Dictionary<string, VendingMachineInventoryEntry> target6 = (Dictionary<string, VendingMachineInventoryEntry>) null;
    if (this.EmaggedInventory == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, VendingMachineInventoryEntry>>(this.EmaggedInventory, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<string, VendingMachineInventoryEntry>>(this.EmaggedInventory, hookCtx, context);
    target.EmaggedInventory = target6;
    Dictionary<string, VendingMachineInventoryEntry> target7 = (Dictionary<string, VendingMachineInventoryEntry>) null;
    if (this.ContrabandInventory == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, VendingMachineInventoryEntry>>(this.ContrabandInventory, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<string, VendingMachineInventoryEntry>>(this.ContrabandInventory, hookCtx, context);
    target.ContrabandInventory = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Contraband, ref target8, hookCtx, false, context))
      target8 = this.Contraband;
    target.Contraband = target8;
    TimeSpan? target9 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.EjectEnd, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan?>(this.EjectEnd, hookCtx, context);
    target.EjectEnd = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.DenyEnd, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.DenyEnd, hookCtx, context);
    target.DenyEnd = target10;
    TimeSpan? target11 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.DispenseOnHitEnd, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan?>(this.DispenseOnHitEnd, hookCtx, context);
    target.DispenseOnHitEnd = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanShoot, ref target12, hookCtx, false, context))
      target12 = this.CanShoot;
    target.CanShoot = target12;
    float? target13 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.DispenseOnHitChance, ref target13, hookCtx, false, context))
      target13 = this.DispenseOnHitChance;
    target.DispenseOnHitChance = target13;
    float? target14 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.DispenseOnHitThreshold, ref target14, hookCtx, false, context))
      target14 = this.DispenseOnHitThreshold;
    target.DispenseOnHitThreshold = target14;
    TimeSpan? target15 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.DispenseOnHitCooldown, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan?>(this.DispenseOnHitCooldown, hookCtx, context);
    target.DispenseOnHitCooldown = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (this.SoundVend == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundVend, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.SoundVend, hookCtx, context);
    target.SoundVend = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (this.SoundDeny == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundDeny, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.SoundDeny, hookCtx, context);
    target.SoundDeny = target17;
    float target18 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InitialStockQuality, ref target18, hookCtx, false, context))
      target18 = this.InitialStockQuality;
    target.InitialStockQuality = target18;
    TimeSpan target19 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextEmpEject, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<TimeSpan>(this.NextEmpEject, hookCtx, context);
    target.NextEmpEject = target19;
    string target20 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.OffState, ref target20, hookCtx, false, context))
      target20 = this.OffState;
    target.OffState = target20;
    string target21 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ScreenState, ref target21, hookCtx, false, context))
      target21 = this.ScreenState;
    target.ScreenState = target21;
    string target22 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.NormalState, ref target22, hookCtx, false, context))
      target22 = this.NormalState;
    target.NormalState = target22;
    string target23 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EjectState, ref target23, hookCtx, false, context))
      target23 = this.EjectState;
    target.EjectState = target23;
    string target24 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DenyState, ref target24, hookCtx, false, context))
      target24 = this.DenyState;
    target.DenyState = target24;
    string target25 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BrokenState, ref target25, hookCtx, false, context))
      target25 = this.BrokenState;
    target.BrokenState = target25;
    bool target26 = false;
    if (!serialization.TryCustomCopy<bool>(this.LoopDenyAnimation, ref target26, hookCtx, false, context))
      target26 = this.LoopDenyAnimation;
    target.LoopDenyAnimation = target26;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VendingMachineComponent target,
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
    VendingMachineComponent target1 = (VendingMachineComponent) target;
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
    VendingMachineComponent target1 = (VendingMachineComponent) target;
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
    VendingMachineComponent target1 = (VendingMachineComponent) target;
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
  virtual VendingMachineComponent Component.Instantiate() => new VendingMachineComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VendingMachineComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VendingMachineComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<VendingMachineComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      VendingMachineComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.EjectEnd.HasValue)
        component.EjectEnd = new TimeSpan?(component.EjectEnd.Value + args.PausedTime);
      if (!component.DenyEnd.HasValue)
        return;
      component.DenyEnd = new TimeSpan?(component.DenyEnd.Value + args.PausedTime);
    }
  }
}
