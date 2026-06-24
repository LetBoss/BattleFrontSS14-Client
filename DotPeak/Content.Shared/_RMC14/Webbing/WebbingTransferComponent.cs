// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Webbing.WebbingTransferComponent
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
namespace Content.Shared._RMC14.Webbing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedWebbingSystem)})]
public sealed class WebbingTransferComponent : 
  Component,
  ISerializationGenerated<WebbingTransferComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Clothing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public WebbingTransferComponent.TransferType Transfer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Defer = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WebbingTransferComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WebbingTransferComponent) target1;
    if (serialization.TryCustomCopy<WebbingTransferComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Clothing, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Clothing, hookCtx, context);
    target.Clothing = target2;
    WebbingTransferComponent.TransferType target3 = WebbingTransferComponent.TransferType.ToClothing;
    if (!serialization.TryCustomCopy<WebbingTransferComponent.TransferType>(this.Transfer, ref target3, hookCtx, false, context))
      target3 = this.Transfer;
    target.Transfer = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Defer, ref target4, hookCtx, false, context))
      target4 = this.Defer;
    target.Defer = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WebbingTransferComponent target,
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
    WebbingTransferComponent target1 = (WebbingTransferComponent) target;
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
    WebbingTransferComponent target1 = (WebbingTransferComponent) target;
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
    WebbingTransferComponent target1 = (WebbingTransferComponent) target;
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
  virtual WebbingTransferComponent Component.Instantiate() => new WebbingTransferComponent();

  public enum TransferType
  {
    ToClothing,
    ToWebbing,
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WebbingTransferComponent_AutoState : IComponentState
  {
    public NetEntity? Clothing;
    public WebbingTransferComponent.TransferType Transfer;
    public bool Defer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WebbingTransferComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WebbingTransferComponent, ComponentGetState>(new ComponentEventRefHandler<WebbingTransferComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WebbingTransferComponent, ComponentHandleState>(new ComponentEventRefHandler<WebbingTransferComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WebbingTransferComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WebbingTransferComponent.WebbingTransferComponent_AutoState()
      {
        Clothing = this.GetNetEntity(component.Clothing),
        Transfer = component.Transfer,
        Defer = component.Defer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WebbingTransferComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WebbingTransferComponent.WebbingTransferComponent_AutoState current))
        return;
      component.Clothing = this.EnsureEntity<WebbingTransferComponent>(current.Clothing, uid);
      component.Transfer = current.Transfer;
      component.Defer = current.Defer;
    }
  }
}
