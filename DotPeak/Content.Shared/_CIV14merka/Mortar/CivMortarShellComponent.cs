// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Mortar.CivMortarShellComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Mortar;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedCivMortarSystem)})]
public sealed class CivMortarShellComponent : 
  Component,
  ISerializationGenerated<CivMortarShellComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LoadDelay = TimeSpan.FromSeconds(1.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TravelDelay = TimeSpan.FromSeconds(4.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ImpactWarningDelay = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ImpactDelay = TimeSpan.FromSeconds(4.5);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CivMortarShellComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CivMortarShellComponent) target1;
    if (serialization.TryCustomCopy<CivMortarShellComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LoadDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.LoadDelay, hookCtx, context);
    target.LoadDelay = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TravelDelay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.TravelDelay, hookCtx, context);
    target.TravelDelay = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ImpactWarningDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.ImpactWarningDelay, hookCtx, context);
    target.ImpactWarningDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ImpactDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ImpactDelay, hookCtx, context);
    target.ImpactDelay = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CivMortarShellComponent target,
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
    CivMortarShellComponent target1 = (CivMortarShellComponent) target;
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
    CivMortarShellComponent target1 = (CivMortarShellComponent) target;
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
    CivMortarShellComponent target1 = (CivMortarShellComponent) target;
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
  virtual CivMortarShellComponent Component.Instantiate() => new CivMortarShellComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CivMortarShellComponent_AutoState : IComponentState
  {
    public TimeSpan LoadDelay;
    public TimeSpan TravelDelay;
    public TimeSpan ImpactWarningDelay;
    public TimeSpan ImpactDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CivMortarShellComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CivMortarShellComponent, ComponentGetState>(new ComponentEventRefHandler<CivMortarShellComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CivMortarShellComponent, ComponentHandleState>(new ComponentEventRefHandler<CivMortarShellComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CivMortarShellComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CivMortarShellComponent.CivMortarShellComponent_AutoState()
      {
        LoadDelay = component.LoadDelay,
        TravelDelay = component.TravelDelay,
        ImpactWarningDelay = component.ImpactWarningDelay,
        ImpactDelay = component.ImpactDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CivMortarShellComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CivMortarShellComponent.CivMortarShellComponent_AutoState current))
        return;
      component.LoadDelay = current.LoadDelay;
      component.TravelDelay = current.TravelDelay;
      component.ImpactWarningDelay = current.ImpactWarningDelay;
      component.ImpactDelay = current.ImpactDelay;
    }
  }
}
