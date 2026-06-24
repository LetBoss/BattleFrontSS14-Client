// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Parasite.VictimBurstComponent
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
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Parasite;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoParasiteSystem)})]
public sealed class VictimBurstComponent : 
  Component,
  ISerializationGenerated<VictimBurstComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ResPath RsiPath = new ResPath("/Textures/_RMC14/Effects/burst.rsi");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string BurstState = "bursted_stand";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string BurstingState = "burst_stand";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VictimBurstComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VictimBurstComponent) target1;
    if (serialization.TryCustomCopy<VictimBurstComponent>(this, ref target, hookCtx, false, context))
      return;
    ResPath target2 = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.RsiPath, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ResPath>(this.RsiPath, hookCtx, context);
    target.RsiPath = target2;
    string target3 = (string) null;
    if (this.BurstState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BurstState, ref target3, hookCtx, false, context))
      target3 = this.BurstState;
    target.BurstState = target3;
    string target4 = (string) null;
    if (this.BurstingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BurstingState, ref target4, hookCtx, false, context))
      target4 = this.BurstingState;
    target.BurstingState = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VictimBurstComponent target,
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
    VictimBurstComponent target1 = (VictimBurstComponent) target;
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
    VictimBurstComponent target1 = (VictimBurstComponent) target;
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
    VictimBurstComponent target1 = (VictimBurstComponent) target;
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
  virtual VictimBurstComponent Component.Instantiate() => new VictimBurstComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VictimBurstComponent_AutoState : IComponentState
  {
    public ResPath RsiPath;
    public string BurstState;
    public string BurstingState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VictimBurstComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VictimBurstComponent, ComponentGetState>(new ComponentEventRefHandler<VictimBurstComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VictimBurstComponent, ComponentHandleState>(new ComponentEventRefHandler<VictimBurstComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VictimBurstComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VictimBurstComponent.VictimBurstComponent_AutoState()
      {
        RsiPath = component.RsiPath,
        BurstState = component.BurstState,
        BurstingState = component.BurstingState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VictimBurstComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VictimBurstComponent.VictimBurstComponent_AutoState current))
        return;
      component.RsiPath = current.RsiPath;
      component.BurstState = current.BurstState;
      component.BurstingState = current.BurstingState;
    }
  }
}
