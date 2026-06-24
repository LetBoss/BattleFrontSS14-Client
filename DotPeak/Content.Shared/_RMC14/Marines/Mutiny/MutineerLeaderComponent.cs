// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Mutiny.MutineerLeaderComponent
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
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Marines.Mutiny;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MutineerLeaderComponent : 
  Component,
  ISerializationGenerated<MutineerLeaderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Interface/cm_job_icons.rsi"), "hudmutineerleader");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId RecruitAction = (EntProtoId) "ActionMutineerRecruit";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? RecruitActionEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MutineerLeaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MutineerLeaderComponent) target1;
    if (serialization.TryCustomCopy<MutineerLeaderComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target2 = (SpriteSpecifier) null;
    if (this.Icon == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context);
    target.Icon = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RecruitAction, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.RecruitAction, hookCtx, context);
    target.RecruitAction = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.RecruitActionEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.RecruitActionEntity, hookCtx, context);
    target.RecruitActionEntity = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MutineerLeaderComponent target,
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
    MutineerLeaderComponent target1 = (MutineerLeaderComponent) target;
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
    MutineerLeaderComponent target1 = (MutineerLeaderComponent) target;
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
    MutineerLeaderComponent target1 = (MutineerLeaderComponent) target;
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
  virtual MutineerLeaderComponent Component.Instantiate() => new MutineerLeaderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MutineerLeaderComponent_AutoState : IComponentState
  {
    public SpriteSpecifier Icon;
    public EntProtoId RecruitAction;
    public NetEntity? RecruitActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MutineerLeaderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MutineerLeaderComponent, ComponentGetState>(new ComponentEventRefHandler<MutineerLeaderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MutineerLeaderComponent, ComponentHandleState>(new ComponentEventRefHandler<MutineerLeaderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MutineerLeaderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MutineerLeaderComponent.MutineerLeaderComponent_AutoState()
      {
        Icon = component.Icon,
        RecruitAction = component.RecruitAction,
        RecruitActionEntity = this.GetNetEntity(component.RecruitActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MutineerLeaderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MutineerLeaderComponent.MutineerLeaderComponent_AutoState current))
        return;
      component.Icon = current.Icon;
      component.RecruitAction = current.RecruitAction;
      component.RecruitActionEntity = this.EnsureEntity<MutineerLeaderComponent>(current.RecruitActionEntity, uid);
    }
  }
}
