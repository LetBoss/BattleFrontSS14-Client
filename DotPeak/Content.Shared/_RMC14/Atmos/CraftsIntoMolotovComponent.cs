// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.CraftsIntoMolotovComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCFlammableSystem)})]
public sealed class CraftsIntoMolotovComponent : 
  Component,
  ISerializationGenerated<CraftsIntoMolotovComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string SolutionId = "drink";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MinIntensity = (FixedPoint2) 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxIntensity = (FixedPoint2) 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawns = (EntProtoId) "RMCGrenadeMolotov";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CraftsIntoMolotovComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CraftsIntoMolotovComponent) target1;
    if (serialization.TryCustomCopy<CraftsIntoMolotovComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SolutionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionId, ref target2, hookCtx, false, context))
      target2 = this.SolutionId;
    target.SolutionId = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinIntensity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MinIntensity, hookCtx, context);
    target.MinIntensity = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxIntensity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.MaxIntensity, hookCtx, context);
    target.MaxIntensity = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawns, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.Spawns, hookCtx, context);
    target.Spawns = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CraftsIntoMolotovComponent target,
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
    CraftsIntoMolotovComponent target1 = (CraftsIntoMolotovComponent) target;
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
    CraftsIntoMolotovComponent target1 = (CraftsIntoMolotovComponent) target;
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
    CraftsIntoMolotovComponent target1 = (CraftsIntoMolotovComponent) target;
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
  virtual CraftsIntoMolotovComponent Component.Instantiate() => new CraftsIntoMolotovComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CraftsIntoMolotovComponent_AutoState : IComponentState
  {
    public string SolutionId;
    public FixedPoint2 MinIntensity;
    public FixedPoint2 MaxIntensity;
    public TimeSpan Delay;
    public EntProtoId Spawns;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CraftsIntoMolotovComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CraftsIntoMolotovComponent, ComponentGetState>(new ComponentEventRefHandler<CraftsIntoMolotovComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CraftsIntoMolotovComponent, ComponentHandleState>(new ComponentEventRefHandler<CraftsIntoMolotovComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CraftsIntoMolotovComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CraftsIntoMolotovComponent.CraftsIntoMolotovComponent_AutoState()
      {
        SolutionId = component.SolutionId,
        MinIntensity = component.MinIntensity,
        MaxIntensity = component.MaxIntensity,
        Delay = component.Delay,
        Spawns = component.Spawns
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CraftsIntoMolotovComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CraftsIntoMolotovComponent.CraftsIntoMolotovComponent_AutoState current))
        return;
      component.SolutionId = current.SolutionId;
      component.MinIntensity = current.MinIntensity;
      component.MaxIntensity = current.MaxIntensity;
      component.Delay = current.Delay;
      component.Spawns = current.Spawns;
    }
  }
}
