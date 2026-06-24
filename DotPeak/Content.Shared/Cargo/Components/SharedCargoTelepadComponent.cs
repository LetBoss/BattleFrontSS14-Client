// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Components.CargoTelepadComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Cargo.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedCargoSystem)})]
public sealed class CargoTelepadComponent : 
  Component,
  ISerializationGenerated<CargoTelepadComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<CargoOrderData> CurrentOrders = new List<CargoOrderData>();
  [DataField("delay", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float Delay = 5f;
  [DataField("accumulator", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float Accumulator;
  [DataField("currentState", false, 1, false, false, null)]
  public CargoTelepadState CurrentState;
  [DataField("teleportSound", false, 1, false, false, null)]
  public SoundSpecifier TeleportSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/phasein.ogg", new AudioParams?());
  [DataField("printerOutput", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string PrinterOutput = "PaperCargoInvoice";
  [DataField("receiverPort", false, 1, false, false, typeof (PrototypeIdSerializer<SinkPortPrototype>))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ReceiverPort = "OrderReceiver";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CargoTelepadComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CargoTelepadComponent) component;
    if (serialization.TryCustomCopy<CargoTelepadComponent>(this, ref target, hookCtx, false, context))
      return;
    List<CargoOrderData> cargoOrderDataList = (List<CargoOrderData>) null;
    if (this.CurrentOrders == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<CargoOrderData>>(this.CurrentOrders, ref cargoOrderDataList, hookCtx, true, context))
      cargoOrderDataList = serialization.CreateCopy<List<CargoOrderData>>(this.CurrentOrders, hookCtx, context, false);
    target.CurrentOrders = cargoOrderDataList;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref num1, hookCtx, false, context))
      num1 = this.Delay;
    target.Delay = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Accumulator, ref num2, hookCtx, false, context))
      num2 = this.Accumulator;
    target.Accumulator = num2;
    CargoTelepadState cargoTelepadState = CargoTelepadState.Unpowered;
    if (!serialization.TryCustomCopy<CargoTelepadState>(this.CurrentState, ref cargoTelepadState, hookCtx, false, context))
      cargoTelepadState = this.CurrentState;
    target.CurrentState = cargoTelepadState;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.TeleportSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TeleportSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.TeleportSound, hookCtx, context, false);
    target.TeleportSound = soundSpecifier;
    string str1 = (string) null;
    if (this.PrinterOutput == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PrinterOutput, ref str1, hookCtx, false, context))
      str1 = this.PrinterOutput;
    target.PrinterOutput = str1;
    string str2 = (string) null;
    if (this.ReceiverPort == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ReceiverPort, ref str2, hookCtx, false, context))
      str2 = this.ReceiverPort;
    target.ReceiverPort = str2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CargoTelepadComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CargoTelepadComponent target1 = (CargoTelepadComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CargoTelepadComponent target1 = (CargoTelepadComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CargoTelepadComponent target1 = (CargoTelepadComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CargoTelepadComponent Component.Instantiate() => new CargoTelepadComponent();
}
