// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Name.XenoNameComponent
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
namespace Content.Shared._RMC14.Xenonids.Name;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedXenoNameSystem)})]
public sealed class XenoNameComponent : 
  Component,
  ISerializationGenerated<XenoNameComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Rank = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Prefix = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Number;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Postfix = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoNameComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoNameComponent) target1;
    if (serialization.TryCustomCopy<XenoNameComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Rank == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Rank, ref target2, hookCtx, false, context))
      target2 = this.Rank;
    target.Rank = target2;
    string target3 = (string) null;
    if (this.Prefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prefix, ref target3, hookCtx, false, context))
      target3 = this.Prefix;
    target.Prefix = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Number, ref target4, hookCtx, false, context))
      target4 = this.Number;
    target.Number = target4;
    string target5 = (string) null;
    if (this.Postfix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Postfix, ref target5, hookCtx, false, context))
      target5 = this.Postfix;
    target.Postfix = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoNameComponent target,
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
    XenoNameComponent target1 = (XenoNameComponent) target;
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
    XenoNameComponent target1 = (XenoNameComponent) target;
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
    XenoNameComponent target1 = (XenoNameComponent) target;
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
  virtual XenoNameComponent Component.Instantiate() => new XenoNameComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoNameComponent_AutoState : IComponentState
  {
    public string Rank;
    public string Prefix;
    public int Number;
    public string Postfix;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoNameComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoNameComponent, ComponentGetState>(new ComponentEventRefHandler<XenoNameComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoNameComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoNameComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, XenoNameComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoNameComponent.XenoNameComponent_AutoState()
      {
        Rank = component.Rank,
        Prefix = component.Prefix,
        Number = component.Number,
        Postfix = component.Postfix
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoNameComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoNameComponent.XenoNameComponent_AutoState current))
        return;
      component.Rank = current.Rank;
      component.Prefix = current.Prefix;
      component.Number = current.Number;
      component.Postfix = current.Postfix;
    }
  }
}
