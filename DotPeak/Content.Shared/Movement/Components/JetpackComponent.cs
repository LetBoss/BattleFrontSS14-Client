// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.JetpackComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class JetpackComponent : 
  Component,
  ISerializationGenerated<JetpackComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? JetpackUser;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("moleUsage", false, 1, false, false, null)]
  public float MoleUsage = 0.012f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleAction = (EntProtoId) "ActionToggleJetpack";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleActionEntity;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("acceleration", false, 1, false, false, null)]
  public float Acceleration = 1f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("friction", false, 1, false, false, null)]
  public float Friction = 0.25f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("weightlessModifier", false, 1, false, false, null)]
  public float WeightlessModifier = 1.2f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JetpackComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JetpackComponent) target1;
    if (serialization.TryCustomCopy<JetpackComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.JetpackUser, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.JetpackUser, hookCtx, context);
    target.JetpackUser = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MoleUsage, ref target3, hookCtx, false, context))
      target3 = this.MoleUsage;
    target.MoleUsage = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ToggleAction, hookCtx, context);
    target.ToggleAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ToggleActionEntity, hookCtx, context);
    target.ToggleActionEntity = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Acceleration, ref target6, hookCtx, false, context))
      target6 = this.Acceleration;
    target.Acceleration = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Friction, ref target7, hookCtx, false, context))
      target7 = this.Friction;
    target.Friction = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessModifier, ref target8, hookCtx, false, context))
      target8 = this.WeightlessModifier;
    target.WeightlessModifier = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JetpackComponent target,
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
    JetpackComponent target1 = (JetpackComponent) target;
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
    JetpackComponent target1 = (JetpackComponent) target;
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
    JetpackComponent target1 = (JetpackComponent) target;
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
  virtual JetpackComponent Component.Instantiate() => new JetpackComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class JetpackComponent_AutoState : IComponentState
  {
    public NetEntity? JetpackUser;
    public NetEntity? ToggleActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class JetpackComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<JetpackComponent, ComponentGetState>(new ComponentEventRefHandler<JetpackComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<JetpackComponent, ComponentHandleState>(new ComponentEventRefHandler<JetpackComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, JetpackComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new JetpackComponent.JetpackComponent_AutoState()
      {
        JetpackUser = this.GetNetEntity(component.JetpackUser),
        ToggleActionEntity = this.GetNetEntity(component.ToggleActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      JetpackComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is JetpackComponent.JetpackComponent_AutoState current))
        return;
      component.JetpackUser = this.EnsureEntity<JetpackComponent>(current.JetpackUser, uid);
      component.ToggleActionEntity = this.EnsureEntity<JetpackComponent>(current.ToggleActionEntity, uid);
    }
  }
}
