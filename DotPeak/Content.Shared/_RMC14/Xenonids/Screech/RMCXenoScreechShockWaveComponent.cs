// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Screech.RMCXenoScreechShockWaveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Screech;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoScreechSystem)})]
public sealed class RMCXenoScreechShockWaveComponent : 
  Component,
  ISerializationGenerated<RMCXenoScreechShockWaveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float WaveSpeed = 15.3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float WaveStrength = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float DownScale = 1.5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCXenoScreechShockWaveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCXenoScreechShockWaveComponent) target1;
    if (serialization.TryCustomCopy<RMCXenoScreechShockWaveComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WaveSpeed, ref target2, hookCtx, false, context))
      target2 = this.WaveSpeed;
    target.WaveSpeed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WaveStrength, ref target3, hookCtx, false, context))
      target3 = this.WaveStrength;
    target.WaveStrength = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DownScale, ref target4, hookCtx, false, context))
      target4 = this.DownScale;
    target.DownScale = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCXenoScreechShockWaveComponent target,
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
    RMCXenoScreechShockWaveComponent target1 = (RMCXenoScreechShockWaveComponent) target;
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
    RMCXenoScreechShockWaveComponent target1 = (RMCXenoScreechShockWaveComponent) target;
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
    RMCXenoScreechShockWaveComponent target1 = (RMCXenoScreechShockWaveComponent) target;
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
  virtual RMCXenoScreechShockWaveComponent Component.Instantiate()
  {
    return new RMCXenoScreechShockWaveComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCXenoScreechShockWaveComponent_AutoState : IComponentState
  {
    public float WaveSpeed;
    public float WaveStrength;
    public float DownScale;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCXenoScreechShockWaveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCXenoScreechShockWaveComponent, ComponentGetState>(new ComponentEventRefHandler<RMCXenoScreechShockWaveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCXenoScreechShockWaveComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCXenoScreechShockWaveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCXenoScreechShockWaveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCXenoScreechShockWaveComponent.RMCXenoScreechShockWaveComponent_AutoState()
      {
        WaveSpeed = component.WaveSpeed,
        WaveStrength = component.WaveStrength,
        DownScale = component.DownScale
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCXenoScreechShockWaveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCXenoScreechShockWaveComponent.RMCXenoScreechShockWaveComponent_AutoState current))
        return;
      component.WaveSpeed = current.WaveSpeed;
      component.WaveStrength = current.WaveStrength;
      component.DownScale = current.DownScale;
    }
  }
}
