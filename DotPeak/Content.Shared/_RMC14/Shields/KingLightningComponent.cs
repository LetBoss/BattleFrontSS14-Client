// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.KingLightningComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Shields;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class KingLightningComponent : 
  Component,
  ISerializationGenerated<KingLightningComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Source;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Lightning = (EntProtoId) "RMCPurpleLightning";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DisappearAt;
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> Trail = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public bool StopUpdating;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref KingLightningComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (KingLightningComponent) target1;
    if (serialization.TryCustomCopy<KingLightningComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Source, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Source, hookCtx, context);
    target.Source = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Lightning, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.Lightning, hookCtx, context);
    target.Lightning = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DisappearAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.DisappearAt, hookCtx, context);
    target.DisappearAt = target4;
    List<EntityUid> target5 = (List<EntityUid>) null;
    if (this.Trail == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Trail, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<EntityUid>>(this.Trail, hookCtx, context);
    target.Trail = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.StopUpdating, ref target6, hookCtx, false, context))
      target6 = this.StopUpdating;
    target.StopUpdating = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref KingLightningComponent target,
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
    KingLightningComponent target1 = (KingLightningComponent) target;
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
    KingLightningComponent target1 = (KingLightningComponent) target;
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
    KingLightningComponent target1 = (KingLightningComponent) target;
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
  virtual KingLightningComponent Component.Instantiate() => new KingLightningComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class KingLightningComponent_AutoState : IComponentState
  {
    public NetEntity Source;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class KingLightningComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<KingLightningComponent, ComponentGetState>(new ComponentEventRefHandler<KingLightningComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<KingLightningComponent, ComponentHandleState>(new ComponentEventRefHandler<KingLightningComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      KingLightningComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new KingLightningComponent.KingLightningComponent_AutoState()
      {
        Source = this.GetNetEntity(component.Source)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      KingLightningComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is KingLightningComponent.KingLightningComponent_AutoState current))
        return;
      component.Source = this.EnsureEntity<KingLightningComponent>(current.Source, uid);
    }
  }
}
