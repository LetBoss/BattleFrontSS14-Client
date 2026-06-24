// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Weeds.AffectableByWeedsComponent
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
namespace Content.Shared._RMC14.Xenonids.Weeds;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoWeedsSystem)})]
public sealed class AffectableByWeedsComponent : 
  Component,
  ISerializationGenerated<AffectableByWeedsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnXenoWeeds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnFriendlyWeeds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnXenoSlowResin;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnXenoFastResin;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AffectableByWeedsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AffectableByWeedsComponent) target1;
    if (serialization.TryCustomCopy<AffectableByWeedsComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnXenoWeeds, ref target2, hookCtx, false, context))
      target2 = this.OnXenoWeeds;
    target.OnXenoWeeds = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnFriendlyWeeds, ref target3, hookCtx, false, context))
      target3 = this.OnFriendlyWeeds;
    target.OnFriendlyWeeds = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnXenoSlowResin, ref target4, hookCtx, false, context))
      target4 = this.OnXenoSlowResin;
    target.OnXenoSlowResin = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnXenoFastResin, ref target5, hookCtx, false, context))
      target5 = this.OnXenoFastResin;
    target.OnXenoFastResin = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AffectableByWeedsComponent target,
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
    AffectableByWeedsComponent target1 = (AffectableByWeedsComponent) target;
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
    AffectableByWeedsComponent target1 = (AffectableByWeedsComponent) target;
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
    AffectableByWeedsComponent target1 = (AffectableByWeedsComponent) target;
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
  virtual AffectableByWeedsComponent Component.Instantiate() => new AffectableByWeedsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AffectableByWeedsComponent_AutoState : IComponentState
  {
    public bool OnXenoWeeds;
    public bool OnFriendlyWeeds;
    public bool OnXenoSlowResin;
    public bool OnXenoFastResin;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AffectableByWeedsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AffectableByWeedsComponent, ComponentGetState>(new ComponentEventRefHandler<AffectableByWeedsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AffectableByWeedsComponent, ComponentHandleState>(new ComponentEventRefHandler<AffectableByWeedsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AffectableByWeedsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AffectableByWeedsComponent.AffectableByWeedsComponent_AutoState()
      {
        OnXenoWeeds = component.OnXenoWeeds,
        OnFriendlyWeeds = component.OnFriendlyWeeds,
        OnXenoSlowResin = component.OnXenoSlowResin,
        OnXenoFastResin = component.OnXenoFastResin
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AffectableByWeedsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AffectableByWeedsComponent.AffectableByWeedsComponent_AutoState current))
        return;
      component.OnXenoWeeds = current.OnXenoWeeds;
      component.OnFriendlyWeeds = current.OnFriendlyWeeds;
      component.OnXenoSlowResin = current.OnXenoSlowResin;
      component.OnXenoFastResin = current.OnXenoFastResin;
    }
  }
}
