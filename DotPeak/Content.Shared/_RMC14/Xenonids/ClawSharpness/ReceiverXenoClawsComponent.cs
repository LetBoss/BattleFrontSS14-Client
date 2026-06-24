// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ClawSharpness.ReceiverXenoClawsComponent
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
namespace Content.Shared._RMC14.Xenonids.ClawSharpness;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoClawsSystem)})]
public sealed class ReceiverXenoClawsComponent : 
  Component,
  ISerializationGenerated<ReceiverXenoClawsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxHealth = 100f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HitsToDestroy = 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoClawType MinimumClawStrength = XenoClawType.Sharp;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? MinimumXenoTier;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReceiverXenoClawsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReceiverXenoClawsComponent) target1;
    if (serialization.TryCustomCopy<ReceiverXenoClawsComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxHealth, ref target2, hookCtx, false, context))
      target2 = this.MaxHealth;
    target.MaxHealth = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.HitsToDestroy, ref target3, hookCtx, false, context))
      target3 = this.HitsToDestroy;
    target.HitsToDestroy = target3;
    XenoClawType target4 = XenoClawType.Normal;
    if (!serialization.TryCustomCopy<XenoClawType>(this.MinimumClawStrength, ref target4, hookCtx, false, context))
      target4 = this.MinimumClawStrength;
    target.MinimumClawStrength = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MinimumXenoTier, ref target5, hookCtx, false, context))
      target5 = this.MinimumXenoTier;
    target.MinimumXenoTier = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReceiverXenoClawsComponent target,
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
    ReceiverXenoClawsComponent target1 = (ReceiverXenoClawsComponent) target;
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
    ReceiverXenoClawsComponent target1 = (ReceiverXenoClawsComponent) target;
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
    ReceiverXenoClawsComponent target1 = (ReceiverXenoClawsComponent) target;
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
  virtual ReceiverXenoClawsComponent Component.Instantiate() => new ReceiverXenoClawsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ReceiverXenoClawsComponent_AutoState : IComponentState
  {
    public float MaxHealth;
    public int HitsToDestroy;
    public XenoClawType MinimumClawStrength;
    public int? MinimumXenoTier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ReceiverXenoClawsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ReceiverXenoClawsComponent, ComponentGetState>(new ComponentEventRefHandler<ReceiverXenoClawsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ReceiverXenoClawsComponent, ComponentHandleState>(new ComponentEventRefHandler<ReceiverXenoClawsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ReceiverXenoClawsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ReceiverXenoClawsComponent.ReceiverXenoClawsComponent_AutoState()
      {
        MaxHealth = component.MaxHealth,
        HitsToDestroy = component.HitsToDestroy,
        MinimumClawStrength = component.MinimumClawStrength,
        MinimumXenoTier = component.MinimumXenoTier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ReceiverXenoClawsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ReceiverXenoClawsComponent.ReceiverXenoClawsComponent_AutoState current))
        return;
      component.MaxHealth = current.MaxHealth;
      component.HitsToDestroy = current.HitsToDestroy;
      component.MinimumClawStrength = current.MinimumClawStrength;
      component.MinimumXenoTier = current.MinimumXenoTier;
    }
  }
}
