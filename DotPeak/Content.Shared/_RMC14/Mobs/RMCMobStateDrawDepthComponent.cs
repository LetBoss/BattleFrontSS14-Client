// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mobs.RMCMobStateDrawDepthComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Mobs;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMMobStateSystem)})]
public sealed class RMCMobStateDrawDepthComponent : 
  Component,
  ISerializationGenerated<RMCMobStateDrawDepthComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Content.Shared.DrawDepth.DrawDepth Default = Content.Shared.DrawDepth.DrawDepth.Mobs;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth> DrawDepths = new Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth>()
  {
    [MobState.Dead] = Content.Shared.DrawDepth.DrawDepth.DeadMobs
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCMobStateDrawDepthComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCMobStateDrawDepthComponent) target1;
    if (serialization.TryCustomCopy<RMCMobStateDrawDepthComponent>(this, ref target, hookCtx, false, context))
      return;
    Content.Shared.DrawDepth.DrawDepth target2 = Content.Shared.DrawDepth.DrawDepth.Objects;
    if (!serialization.TryCustomCopy<Content.Shared.DrawDepth.DrawDepth>(this.Default, ref target2, hookCtx, false, context))
      target2 = this.Default;
    target.Default = target2;
    Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth> target3 = (Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth>) null;
    if (this.DrawDepths == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth>>(this.DrawDepths, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth>>(this.DrawDepths, hookCtx, context);
    target.DrawDepths = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCMobStateDrawDepthComponent target,
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
    RMCMobStateDrawDepthComponent target1 = (RMCMobStateDrawDepthComponent) target;
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
    RMCMobStateDrawDepthComponent target1 = (RMCMobStateDrawDepthComponent) target;
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
    RMCMobStateDrawDepthComponent target1 = (RMCMobStateDrawDepthComponent) target;
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
  virtual RMCMobStateDrawDepthComponent Component.Instantiate()
  {
    return new RMCMobStateDrawDepthComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCMobStateDrawDepthComponent_AutoState : IComponentState
  {
    public Content.Shared.DrawDepth.DrawDepth Default;
    public Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth> DrawDepths;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCMobStateDrawDepthComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCMobStateDrawDepthComponent, ComponentGetState>(new ComponentEventRefHandler<RMCMobStateDrawDepthComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCMobStateDrawDepthComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCMobStateDrawDepthComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCMobStateDrawDepthComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCMobStateDrawDepthComponent.RMCMobStateDrawDepthComponent_AutoState()
      {
        Default = component.Default,
        DrawDepths = component.DrawDepths
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCMobStateDrawDepthComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCMobStateDrawDepthComponent.RMCMobStateDrawDepthComponent_AutoState current))
        return;
      component.Default = current.Default;
      component.DrawDepths = current.DrawDepths == null ? (Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth>) null : new Dictionary<MobState, Content.Shared.DrawDepth.DrawDepth>((IDictionary<MobState, Content.Shared.DrawDepth.DrawDepth>) current.DrawDepths);
    }
  }
}
