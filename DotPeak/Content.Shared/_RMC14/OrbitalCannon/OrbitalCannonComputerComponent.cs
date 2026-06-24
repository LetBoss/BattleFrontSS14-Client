// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonComputerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.OrbitalCannon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (OrbitalCannonSystem)})]
public sealed class OrbitalCannonComputerComponent : 
  Component,
  ISerializationGenerated<OrbitalCannonComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Warhead;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Fuel;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<WarheadFuelRequirement> FuelRequirements = new List<WarheadFuelRequirement>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public OrbitalCannonStatus Status;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref OrbitalCannonComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (OrbitalCannonComputerComponent) target1;
    if (serialization.TryCustomCopy<OrbitalCannonComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Warhead, ref target2, hookCtx, false, context))
      target2 = this.Warhead;
    target.Warhead = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Fuel, ref target3, hookCtx, false, context))
      target3 = this.Fuel;
    target.Fuel = target3;
    List<WarheadFuelRequirement> target4 = (List<WarheadFuelRequirement>) null;
    if (this.FuelRequirements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<WarheadFuelRequirement>>(this.FuelRequirements, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<WarheadFuelRequirement>>(this.FuelRequirements, hookCtx, context);
    target.FuelRequirements = target4;
    OrbitalCannonStatus target5 = OrbitalCannonStatus.Unloaded;
    if (!serialization.TryCustomCopy<OrbitalCannonStatus>(this.Status, ref target5, hookCtx, false, context))
      target5 = this.Status;
    target.Status = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref OrbitalCannonComputerComponent target,
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
    OrbitalCannonComputerComponent target1 = (OrbitalCannonComputerComponent) target;
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
    OrbitalCannonComputerComponent target1 = (OrbitalCannonComputerComponent) target;
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
    OrbitalCannonComputerComponent target1 = (OrbitalCannonComputerComponent) target;
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
  virtual OrbitalCannonComputerComponent Component.Instantiate()
  {
    return new OrbitalCannonComputerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class OrbitalCannonComputerComponent_AutoState : IComponentState
  {
    public string? Warhead;
    public int Fuel;
    public List<WarheadFuelRequirement> FuelRequirements;
    public OrbitalCannonStatus Status;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class OrbitalCannonComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<OrbitalCannonComputerComponent, ComponentGetState>(new ComponentEventRefHandler<OrbitalCannonComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<OrbitalCannonComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<OrbitalCannonComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      OrbitalCannonComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new OrbitalCannonComputerComponent.OrbitalCannonComputerComponent_AutoState()
      {
        Warhead = component.Warhead,
        Fuel = component.Fuel,
        FuelRequirements = component.FuelRequirements,
        Status = component.Status
      };
    }

    private void OnHandleState(
      EntityUid uid,
      OrbitalCannonComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is OrbitalCannonComputerComponent.OrbitalCannonComputerComponent_AutoState current))
        return;
      component.Warhead = current.Warhead;
      component.Fuel = current.Fuel;
      component.FuelRequirements = current.FuelRequirements == null ? (List<WarheadFuelRequirement>) null : new List<WarheadFuelRequirement>((IEnumerable<WarheadFuelRequirement>) current.FuelRequirements);
      component.Status = current.Status;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, OrbitalCannonComputerComponent>(uid, component, ref args1);
    }
  }
}
