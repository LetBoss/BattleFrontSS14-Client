// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tackle.TackleComponent
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
namespace Content.Shared._RMC14.Tackle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (TackleSystem)})]
public sealed class TackleComponent : 
  Component,
  ISerializationGenerated<TackleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Min = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Max = 6;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Chance = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunMin = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StunMax = TimeSpan.FromSeconds(3L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TackleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TackleComponent) target1;
    if (serialization.TryCustomCopy<TackleComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Min, ref target2, hookCtx, false, context))
      target2 = this.Min;
    target.Min = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Chance, ref target4, hookCtx, false, context))
      target4 = this.Chance;
    target.Chance = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunMin, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.StunMin, hookCtx, context);
    target.StunMin = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StunMax, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.StunMax, hookCtx, context);
    target.StunMax = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TackleComponent target,
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
    TackleComponent target1 = (TackleComponent) target;
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
    TackleComponent target1 = (TackleComponent) target;
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
    TackleComponent target1 = (TackleComponent) target;
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
  virtual TackleComponent Component.Instantiate() => new TackleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TackleComponent_AutoState : IComponentState
  {
    public int Min;
    public int Max;
    public float Chance;
    public TimeSpan StunMin;
    public TimeSpan StunMax;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TackleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TackleComponent, ComponentGetState>(new ComponentEventRefHandler<TackleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TackleComponent, ComponentHandleState>(new ComponentEventRefHandler<TackleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, TackleComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new TackleComponent.TackleComponent_AutoState()
      {
        Min = component.Min,
        Max = component.Max,
        Chance = component.Chance,
        StunMin = component.StunMin,
        StunMax = component.StunMax
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TackleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TackleComponent.TackleComponent_AutoState current))
        return;
      component.Min = current.Min;
      component.Max = current.Max;
      component.Chance = current.Chance;
      component.StunMin = current.StunMin;
      component.StunMax = current.StunMax;
    }
  }
}
