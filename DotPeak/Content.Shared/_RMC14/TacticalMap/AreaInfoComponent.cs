// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.AreaInfoComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedTacticalMapSystem), typeof (AreaInfoSystem)})]
public sealed class AreaInfoComponent : 
  Component,
  ISerializationGenerated<AreaInfoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AlertPrototype> Alert = (ProtoId<AlertPrototype>) "AreaInfo";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextUpdateTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UpdateInterval = TimeSpan.FromSeconds(2L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AreaInfoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AreaInfoComponent) target1;
    if (serialization.TryCustomCopy<AreaInfoComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<AlertPrototype> target2 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.Alert, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.Alert, hookCtx, context);
    target.Alert = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdateTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextUpdateTime, hookCtx, context);
    target.NextUpdateTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateInterval, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.UpdateInterval, hookCtx, context);
    target.UpdateInterval = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AreaInfoComponent target,
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
    AreaInfoComponent target1 = (AreaInfoComponent) target;
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
    AreaInfoComponent target1 = (AreaInfoComponent) target;
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
    AreaInfoComponent target1 = (AreaInfoComponent) target;
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
  virtual AreaInfoComponent Component.Instantiate() => new AreaInfoComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AreaInfoComponent_AutoState : IComponentState
  {
    public ProtoId<AlertPrototype> Alert;
    public TimeSpan NextUpdateTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AreaInfoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AreaInfoComponent, ComponentGetState>(new ComponentEventRefHandler<AreaInfoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AreaInfoComponent, ComponentHandleState>(new ComponentEventRefHandler<AreaInfoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, AreaInfoComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new AreaInfoComponent.AreaInfoComponent_AutoState()
      {
        Alert = component.Alert,
        NextUpdateTime = component.NextUpdateTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AreaInfoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AreaInfoComponent.AreaInfoComponent_AutoState current))
        return;
      component.Alert = current.Alert;
      component.NextUpdateTime = current.NextUpdateTime;
    }
  }
}
