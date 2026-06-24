// Decompiled with JetBrains decompiler
// Type: Content.Shared.Species.Components.ReformComponent
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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Species.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ReformComponent : 
  Component,
  ISerializationGenerated<ReformComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId ActionPrototype;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActionEntity;
  [DataField(null, false, 1, true, false, null)]
  public float ReformTime;
  [DataField(null, false, 1, false, false, null)]
  public bool StartDelayed = true;
  [DataField(null, false, 1, false, false, null)]
  public bool ShouldStun = true;
  [DataField(null, false, 1, true, false, null)]
  public string PopupText;

  [DataField(null, false, 1, true, false, null)]
  public EntProtoId ReformPrototype { get; private set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReformComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReformComponent) target1;
    if (serialization.TryCustomCopy<ReformComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionPrototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.ActionPrototype, hookCtx, context);
    target.ActionPrototype = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActionEntity, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.ActionEntity, hookCtx, context);
    target.ActionEntity = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReformTime, ref target4, hookCtx, false, context))
      target4 = this.ReformTime;
    target.ReformTime = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartDelayed, ref target5, hookCtx, false, context))
      target5 = this.StartDelayed;
    target.StartDelayed = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShouldStun, ref target6, hookCtx, false, context))
      target6 = this.ShouldStun;
    target.ShouldStun = target6;
    string target7 = (string) null;
    if (this.PopupText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PopupText, ref target7, hookCtx, false, context))
      target7 = this.PopupText;
    target.PopupText = target7;
    EntProtoId target8 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ReformPrototype, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId>(this.ReformPrototype, hookCtx, context);
    target.ReformPrototype = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReformComponent target,
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
    ReformComponent target1 = (ReformComponent) target;
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
    ReformComponent target1 = (ReformComponent) target;
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
    ReformComponent target1 = (ReformComponent) target;
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
  virtual ReformComponent Component.Instantiate() => new ReformComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ReformComponent_AutoState : IComponentState
  {
    public NetEntity? ActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ReformComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ReformComponent, ComponentGetState>(new ComponentEventRefHandler<ReformComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ReformComponent, ComponentHandleState>(new ComponentEventRefHandler<ReformComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ReformComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new ReformComponent.ReformComponent_AutoState()
      {
        ActionEntity = this.GetNetEntity(component.ActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ReformComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ReformComponent.ReformComponent_AutoState current))
        return;
      component.ActionEntity = this.EnsureEntity<ReformComponent>(current.ActionEntity, uid);
    }
  }
}
