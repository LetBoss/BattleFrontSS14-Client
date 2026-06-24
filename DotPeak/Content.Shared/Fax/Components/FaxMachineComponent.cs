// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fax.Components.FaxMachineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Fax.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class FaxMachineComponent : 
  Component,
  ISerializationGenerated<FaxMachineComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string InsertingState = "inserting";
  [DataField(null, false, 1, true, false, null)]
  public ItemSlot PaperSlot = new ItemSlot();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier PrintSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/printer.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SendSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg");
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float SendTimeoutRemaining;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float SendTimeout = 5f;
  [DataField(null, false, 1, false, false, null)]
  public float InsertingTimeRemaining;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float InsertionTime = 1.88f;
  [DataField(null, false, 1, false, false, null)]
  public float PrintingTimeRemaining;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float PrintingTime = 2.3f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId PrintPaperId = (EntProtoId) "Paper";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId PrintOfficePaperId = (EntProtoId) "PaperOffice";

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("name", false, 1, false, false, null)]
  public string FaxName { get; set; } = "Unknown";

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("destinationAddress", false, 1, false, false, null)]
  public string? DestinationFaxAddress { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool ResponsePings { get; set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool NotifyAdmins { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool ReceiveNukeCodes { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<string, string> KnownFaxes { get; } = new Dictionary<string, string>();

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public Queue<FaxPrintout> PrintingQueue { get; private set; } = new Queue<FaxPrintout>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FaxMachineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FaxMachineComponent) target1;
    if (serialization.TryCustomCopy<FaxMachineComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.FaxName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FaxName, ref target2, hookCtx, false, context))
      target2 = this.FaxName;
    target.FaxName = target2;
    string target3 = (string) null;
    if (this.InsertingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InsertingState, ref target3, hookCtx, false, context))
      target3 = this.InsertingState;
    target.InsertingState = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.DestinationFaxAddress, ref target4, hookCtx, false, context))
      target4 = this.DestinationFaxAddress;
    target.DestinationFaxAddress = target4;
    ItemSlot target5 = (ItemSlot) null;
    if (this.PaperSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ItemSlot>(this.PaperSlot, ref target5, hookCtx, false, context))
    {
      if (this.PaperSlot == null)
        target5 = (ItemSlot) null;
      else
        serialization.CopyTo<ItemSlot>(this.PaperSlot, ref target5, hookCtx, context, true);
    }
    target.PaperSlot = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ResponsePings, ref target6, hookCtx, false, context))
      target6 = this.ResponsePings;
    target.ResponsePings = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.NotifyAdmins, ref target7, hookCtx, false, context))
      target7 = this.NotifyAdmins;
    target.NotifyAdmins = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.ReceiveNukeCodes, ref target8, hookCtx, false, context))
      target8 = this.ReceiveNukeCodes;
    target.ReceiveNukeCodes = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (this.PrintSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.PrintSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.PrintSound, hookCtx, context);
    target.PrintSound = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (this.SendSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SendSound, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.SendSound, hookCtx, context);
    target.SendSound = target10;
    Queue<FaxPrintout> target11 = (Queue<FaxPrintout>) null;
    if (this.PrintingQueue == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Queue<FaxPrintout>>(this.PrintingQueue, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<Queue<FaxPrintout>>(this.PrintingQueue, hookCtx, context);
    target.PrintingQueue = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SendTimeoutRemaining, ref target12, hookCtx, false, context))
      target12 = this.SendTimeoutRemaining;
    target.SendTimeoutRemaining = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SendTimeout, ref target13, hookCtx, false, context))
      target13 = this.SendTimeout;
    target.SendTimeout = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsertingTimeRemaining, ref target14, hookCtx, false, context))
      target14 = this.InsertingTimeRemaining;
    target.InsertingTimeRemaining = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PrintingTimeRemaining, ref target15, hookCtx, false, context))
      target15 = this.PrintingTimeRemaining;
    target.PrintingTimeRemaining = target15;
    EntProtoId target16 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.PrintPaperId, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<EntProtoId>(this.PrintPaperId, hookCtx, context);
    target.PrintPaperId = target16;
    EntProtoId target17 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.PrintOfficePaperId, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<EntProtoId>(this.PrintOfficePaperId, hookCtx, context);
    target.PrintOfficePaperId = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FaxMachineComponent target,
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
    FaxMachineComponent target1 = (FaxMachineComponent) target;
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
    FaxMachineComponent target1 = (FaxMachineComponent) target;
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
    FaxMachineComponent target1 = (FaxMachineComponent) target;
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
  virtual FaxMachineComponent Component.Instantiate() => new FaxMachineComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FaxMachineComponent_AutoState : IComponentState
  {
    public string InsertingState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FaxMachineComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FaxMachineComponent, ComponentGetState>(new ComponentEventRefHandler<FaxMachineComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FaxMachineComponent, ComponentHandleState>(new ComponentEventRefHandler<FaxMachineComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FaxMachineComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FaxMachineComponent.FaxMachineComponent_AutoState()
      {
        InsertingState = component.InsertingState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FaxMachineComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FaxMachineComponent.FaxMachineComponent_AutoState current))
        return;
      component.InsertingState = current.InsertingState;
    }
  }
}
