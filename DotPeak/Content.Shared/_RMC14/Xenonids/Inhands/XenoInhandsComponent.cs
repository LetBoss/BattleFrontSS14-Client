// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Inhands.XenoInhandsComponent
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
namespace Content.Shared._RMC14.Xenonids.Inhands;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoInhandsComponent : 
  Component,
  ISerializationGenerated<XenoInhandsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string Prefix;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Downed = "downed";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Resting = "rest";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Ovi = "ovi";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoInhandsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoInhandsComponent) target1;
    if (serialization.TryCustomCopy<XenoInhandsComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Prefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prefix, ref target2, hookCtx, false, context))
      target2 = this.Prefix;
    target.Prefix = target2;
    string target3 = (string) null;
    if (this.Downed == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Downed, ref target3, hookCtx, false, context))
      target3 = this.Downed;
    target.Downed = target3;
    string target4 = (string) null;
    if (this.Resting == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Resting, ref target4, hookCtx, false, context))
      target4 = this.Resting;
    target.Resting = target4;
    string target5 = (string) null;
    if (this.Ovi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Ovi, ref target5, hookCtx, false, context))
      target5 = this.Ovi;
    target.Ovi = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoInhandsComponent target,
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
    XenoInhandsComponent target1 = (XenoInhandsComponent) target;
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
    XenoInhandsComponent target1 = (XenoInhandsComponent) target;
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
    XenoInhandsComponent target1 = (XenoInhandsComponent) target;
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
  virtual XenoInhandsComponent Component.Instantiate() => new XenoInhandsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoInhandsComponent_AutoState : IComponentState
  {
    public string Prefix;
    public string Downed;
    public string Resting;
    public string Ovi;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoInhandsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoInhandsComponent, ComponentGetState>(new ComponentEventRefHandler<XenoInhandsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoInhandsComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoInhandsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoInhandsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoInhandsComponent.XenoInhandsComponent_AutoState()
      {
        Prefix = component.Prefix,
        Downed = component.Downed,
        Resting = component.Resting,
        Ovi = component.Ovi
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoInhandsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoInhandsComponent.XenoInhandsComponent_AutoState current))
        return;
      component.Prefix = current.Prefix;
      component.Downed = current.Downed;
      component.Resting = current.Resting;
      component.Ovi = current.Ovi;
    }
  }
}
