// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Components.NinjaSuitComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ninja.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.Ninja.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedNinjaSuitSystem)})]
public sealed class NinjaSuitComponent : 
  Component,
  ISerializationGenerated<NinjaSuitComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier RevealSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/chime.ogg");
  [DataField(null, false, 1, false, false, null)]
  public string DisableDelayId = "suit_powers";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId RecallKatanaAction = (EntProtoId) "ActionRecallKatana";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? RecallKatanaActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public float RecallCharge = 3.6f;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId EmpAction = (EntProtoId) "ActionNinjaEmp";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? EmpActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public float EmpCharge = 180f;
  [DataField(null, false, 1, false, false, null)]
  public float EmpRange = 6f;
  [DataField(null, false, 1, false, false, null)]
  public float EmpConsumption = 100000f;
  [DataField(null, false, 1, false, false, null)]
  public float EmpDuration = 60f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NinjaSuitComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NinjaSuitComponent) target1;
    if (serialization.TryCustomCopy<NinjaSuitComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.RevealSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RevealSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.RevealSound, hookCtx, context);
    target.RevealSound = target2;
    string target3 = (string) null;
    if (this.DisableDelayId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DisableDelayId, ref target3, hookCtx, false, context))
      target3 = this.DisableDelayId;
    target.DisableDelayId = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RecallKatanaAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.RecallKatanaAction, hookCtx, context);
    target.RecallKatanaAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.RecallKatanaActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.RecallKatanaActionEntity, hookCtx, context);
    target.RecallKatanaActionEntity = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RecallCharge, ref target6, hookCtx, false, context))
      target6 = this.RecallCharge;
    target.RecallCharge = target6;
    EntProtoId target7 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EmpAction, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<EntProtoId>(this.EmpAction, hookCtx, context);
    target.EmpAction = target7;
    EntityUid? target8 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.EmpActionEntity, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntityUid?>(this.EmpActionEntity, hookCtx, context);
    target.EmpActionEntity = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpCharge, ref target9, hookCtx, false, context))
      target9 = this.EmpCharge;
    target.EmpCharge = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpRange, ref target10, hookCtx, false, context))
      target10 = this.EmpRange;
    target.EmpRange = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpConsumption, ref target11, hookCtx, false, context))
      target11 = this.EmpConsumption;
    target.EmpConsumption = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpDuration, ref target12, hookCtx, false, context))
      target12 = this.EmpDuration;
    target.EmpDuration = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NinjaSuitComponent target,
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
    NinjaSuitComponent target1 = (NinjaSuitComponent) target;
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
    NinjaSuitComponent target1 = (NinjaSuitComponent) target;
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
    NinjaSuitComponent target1 = (NinjaSuitComponent) target;
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
  virtual NinjaSuitComponent Component.Instantiate() => new NinjaSuitComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NinjaSuitComponent_AutoState : IComponentState
  {
    public NetEntity? RecallKatanaActionEntity;
    public NetEntity? EmpActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NinjaSuitComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NinjaSuitComponent, ComponentGetState>(new ComponentEventRefHandler<NinjaSuitComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NinjaSuitComponent, ComponentHandleState>(new ComponentEventRefHandler<NinjaSuitComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NinjaSuitComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NinjaSuitComponent.NinjaSuitComponent_AutoState()
      {
        RecallKatanaActionEntity = this.GetNetEntity(component.RecallKatanaActionEntity),
        EmpActionEntity = this.GetNetEntity(component.EmpActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NinjaSuitComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NinjaSuitComponent.NinjaSuitComponent_AutoState current))
        return;
      component.RecallKatanaActionEntity = this.EnsureEntity<NinjaSuitComponent>(current.RecallKatanaActionEntity, uid);
      component.EmpActionEntity = this.EnsureEntity<NinjaSuitComponent>(current.EmpActionEntity, uid);
    }
  }
}
