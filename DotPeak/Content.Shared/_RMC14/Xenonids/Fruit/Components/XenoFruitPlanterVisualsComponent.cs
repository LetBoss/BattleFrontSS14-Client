// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.Components.XenoFruitPlanterVisualsComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fruit.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoFruitPlanterVisualsSystem)})]
public sealed class XenoFruitPlanterVisualsComponent : 
  Component,
  ISerializationGenerated<XenoFruitPlanterVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string Rsi;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string Prefix;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Robust.Shared.Maths.Color? Color;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoFruitPlanterVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoFruitPlanterVisualsComponent) target1;
    if (serialization.TryCustomCopy<XenoFruitPlanterVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Rsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Rsi, ref target2, hookCtx, false, context))
      target2 = this.Rsi;
    target.Rsi = target2;
    string target3 = (string) null;
    if (this.Prefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prefix, ref target3, hookCtx, false, context))
      target3 = this.Prefix;
    target.Prefix = target3;
    Robust.Shared.Maths.Color? target4 = new Robust.Shared.Maths.Color?();
    if (!serialization.TryCustomCopy<Robust.Shared.Maths.Color?>(this.Color, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Robust.Shared.Maths.Color?>(this.Color, hookCtx, context);
    target.Color = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoFruitPlanterVisualsComponent target,
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
    XenoFruitPlanterVisualsComponent target1 = (XenoFruitPlanterVisualsComponent) target;
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
    XenoFruitPlanterVisualsComponent target1 = (XenoFruitPlanterVisualsComponent) target;
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
    XenoFruitPlanterVisualsComponent target1 = (XenoFruitPlanterVisualsComponent) target;
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
  virtual XenoFruitPlanterVisualsComponent Component.Instantiate()
  {
    return new XenoFruitPlanterVisualsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoFruitPlanterVisualsComponent_AutoState : IComponentState
  {
    public string Rsi;
    public string Prefix;
    public Robust.Shared.Maths.Color? Color;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoFruitPlanterVisualsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, ComponentGetState>(new ComponentEventRefHandler<XenoFruitPlanterVisualsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoFruitPlanterVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoFruitPlanterVisualsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoFruitPlanterVisualsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoFruitPlanterVisualsComponent.XenoFruitPlanterVisualsComponent_AutoState()
      {
        Rsi = component.Rsi,
        Prefix = component.Prefix,
        Color = component.Color
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoFruitPlanterVisualsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoFruitPlanterVisualsComponent.XenoFruitPlanterVisualsComponent_AutoState current))
        return;
      component.Rsi = current.Rsi;
      component.Prefix = current.Prefix;
      component.Color = current.Color;
    }
  }
}
