// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.Components.RMCExplosionShockWaveComponent
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
namespace Content.Shared._RMC14.Explosion.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCExplosionSystem)})]
public sealed class RMCExplosionShockWaveComponent : 
  Component,
  ISerializationGenerated<RMCExplosionShockWaveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float FalloffPower = 40f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Sharpness = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float Width = 0.8f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCExplosionShockWaveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCExplosionShockWaveComponent) target1;
    if (serialization.TryCustomCopy<RMCExplosionShockWaveComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FalloffPower, ref target2, hookCtx, false, context))
      target2 = this.FalloffPower;
    target.FalloffPower = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Sharpness, ref target3, hookCtx, false, context))
      target3 = this.Sharpness;
    target.Sharpness = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Width, ref target4, hookCtx, false, context))
      target4 = this.Width;
    target.Width = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCExplosionShockWaveComponent target,
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
    RMCExplosionShockWaveComponent target1 = (RMCExplosionShockWaveComponent) target;
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
    RMCExplosionShockWaveComponent target1 = (RMCExplosionShockWaveComponent) target;
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
    RMCExplosionShockWaveComponent target1 = (RMCExplosionShockWaveComponent) target;
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
  virtual RMCExplosionShockWaveComponent Component.Instantiate()
  {
    return new RMCExplosionShockWaveComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCExplosionShockWaveComponent_AutoState : IComponentState
  {
    public float FalloffPower;
    public float Sharpness;
    public float Width;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCExplosionShockWaveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCExplosionShockWaveComponent, ComponentGetState>(new ComponentEventRefHandler<RMCExplosionShockWaveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCExplosionShockWaveComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCExplosionShockWaveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCExplosionShockWaveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCExplosionShockWaveComponent.RMCExplosionShockWaveComponent_AutoState()
      {
        FalloffPower = component.FalloffPower,
        Sharpness = component.Sharpness,
        Width = component.Width
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCExplosionShockWaveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCExplosionShockWaveComponent.RMCExplosionShockWaveComponent_AutoState current))
        return;
      component.FalloffPower = current.FalloffPower;
      component.Sharpness = current.Sharpness;
      component.Width = current.Width;
    }
  }
}
