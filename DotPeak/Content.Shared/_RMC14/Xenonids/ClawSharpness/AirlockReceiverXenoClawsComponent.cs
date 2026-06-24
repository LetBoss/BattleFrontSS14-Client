// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ClawSharpness.AirlockReceiverXenoClawsComponent
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
public sealed class AirlockReceiverXenoClawsComponent : 
  Component,
  ISerializationGenerated<AirlockReceiverXenoClawsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxHealth = 500f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HitsToDestroyBolted = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int HitsToDestroyWelded = 15;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public XenoClawType MinimumClawStrength = XenoClawType.Sharp;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AirlockReceiverXenoClawsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AirlockReceiverXenoClawsComponent) target1;
    if (serialization.TryCustomCopy<AirlockReceiverXenoClawsComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxHealth, ref target2, hookCtx, false, context))
      target2 = this.MaxHealth;
    target.MaxHealth = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.HitsToDestroyBolted, ref target3, hookCtx, false, context))
      target3 = this.HitsToDestroyBolted;
    target.HitsToDestroyBolted = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.HitsToDestroyWelded, ref target4, hookCtx, false, context))
      target4 = this.HitsToDestroyWelded;
    target.HitsToDestroyWelded = target4;
    XenoClawType target5 = XenoClawType.Normal;
    if (!serialization.TryCustomCopy<XenoClawType>(this.MinimumClawStrength, ref target5, hookCtx, false, context))
      target5 = this.MinimumClawStrength;
    target.MinimumClawStrength = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AirlockReceiverXenoClawsComponent target,
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
    AirlockReceiverXenoClawsComponent target1 = (AirlockReceiverXenoClawsComponent) target;
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
    AirlockReceiverXenoClawsComponent target1 = (AirlockReceiverXenoClawsComponent) target;
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
    AirlockReceiverXenoClawsComponent target1 = (AirlockReceiverXenoClawsComponent) target;
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
  virtual AirlockReceiverXenoClawsComponent Component.Instantiate()
  {
    return new AirlockReceiverXenoClawsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AirlockReceiverXenoClawsComponent_AutoState : IComponentState
  {
    public float MaxHealth;
    public int HitsToDestroyBolted;
    public int HitsToDestroyWelded;
    public XenoClawType MinimumClawStrength;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AirlockReceiverXenoClawsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AirlockReceiverXenoClawsComponent, ComponentGetState>(new ComponentEventRefHandler<AirlockReceiverXenoClawsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AirlockReceiverXenoClawsComponent, ComponentHandleState>(new ComponentEventRefHandler<AirlockReceiverXenoClawsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AirlockReceiverXenoClawsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AirlockReceiverXenoClawsComponent.AirlockReceiverXenoClawsComponent_AutoState()
      {
        MaxHealth = component.MaxHealth,
        HitsToDestroyBolted = component.HitsToDestroyBolted,
        HitsToDestroyWelded = component.HitsToDestroyWelded,
        MinimumClawStrength = component.MinimumClawStrength
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AirlockReceiverXenoClawsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AirlockReceiverXenoClawsComponent.AirlockReceiverXenoClawsComponent_AutoState current))
        return;
      component.MaxHealth = current.MaxHealth;
      component.HitsToDestroyBolted = current.HitsToDestroyBolted;
      component.HitsToDestroyWelded = current.HitsToDestroyWelded;
      component.MinimumClawStrength = current.MinimumClawStrength;
    }
  }
}
