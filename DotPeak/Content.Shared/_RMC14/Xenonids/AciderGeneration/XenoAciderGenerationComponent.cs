// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.AciderGeneration.XenoAciderGenerationComponent
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
namespace Content.Shared._RMC14.Xenonids.AciderGeneration;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoAciderGenerationComponent : 
  Component,
  ISerializationGenerated<XenoAciderGenerationComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimeBetweenGeneration = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextIncrease;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int IncreaseAmount = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ExpireDuration = TimeSpan.FromSeconds(6L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? ExpireAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoAciderGenerationComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoAciderGenerationComponent) target1;
    if (serialization.TryCustomCopy<XenoAciderGenerationComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimeBetweenGeneration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.TimeBetweenGeneration, hookCtx, context);
    target.TimeBetweenGeneration = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextIncrease, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextIncrease, hookCtx, context);
    target.NextIncrease = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.IncreaseAmount, ref target4, hookCtx, false, context))
      target4 = this.IncreaseAmount;
    target.IncreaseAmount = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ExpireDuration, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.ExpireDuration, hookCtx, context);
    target.ExpireDuration = target5;
    TimeSpan? target6 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ExpireAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan?>(this.ExpireAt, hookCtx, context);
    target.ExpireAt = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoAciderGenerationComponent target,
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
    XenoAciderGenerationComponent target1 = (XenoAciderGenerationComponent) target;
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
    XenoAciderGenerationComponent target1 = (XenoAciderGenerationComponent) target;
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
    XenoAciderGenerationComponent target1 = (XenoAciderGenerationComponent) target;
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
  virtual XenoAciderGenerationComponent Component.Instantiate()
  {
    return new XenoAciderGenerationComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoAciderGenerationComponent_AutoState : IComponentState
  {
    public TimeSpan TimeBetweenGeneration;
    public TimeSpan NextIncrease;
    public int IncreaseAmount;
    public TimeSpan ExpireDuration;
    public TimeSpan? ExpireAt;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoAciderGenerationComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoAciderGenerationComponent, ComponentGetState>(new ComponentEventRefHandler<XenoAciderGenerationComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoAciderGenerationComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoAciderGenerationComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoAciderGenerationComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoAciderGenerationComponent.XenoAciderGenerationComponent_AutoState()
      {
        TimeBetweenGeneration = component.TimeBetweenGeneration,
        NextIncrease = component.NextIncrease,
        IncreaseAmount = component.IncreaseAmount,
        ExpireDuration = component.ExpireDuration,
        ExpireAt = component.ExpireAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoAciderGenerationComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoAciderGenerationComponent.XenoAciderGenerationComponent_AutoState current))
        return;
      component.TimeBetweenGeneration = current.TimeBetweenGeneration;
      component.NextIncrease = current.NextIncrease;
      component.IncreaseAmount = current.IncreaseAmount;
      component.ExpireDuration = current.ExpireDuration;
      component.ExpireAt = current.ExpireAt;
    }
  }
}
