// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mind.Components.MindContainerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Mind.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedMindSystem)})]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MindContainerComponent : 
  Component,
  ISerializationGenerated<MindContainerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Mind { get; set; }

  [MemberNotNullWhen(true, "Mind")]
  public bool HasMind
  {
    [MemberNotNullWhen(true, "Mind")] get => this.Mind.HasValue;
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("showExamineInfo", false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowExamineInfo { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("ghostOnShutdown", false, 1, false, false, null)]
  public bool GhostOnShutdown { get; set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MindContainerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MindContainerComponent) target1;
    if (serialization.TryCustomCopy<MindContainerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Mind, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Mind, hookCtx, context);
    target.Mind = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowExamineInfo, ref target3, hookCtx, false, context))
      target3 = this.ShowExamineInfo;
    target.ShowExamineInfo = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.GhostOnShutdown, ref target4, hookCtx, false, context))
      target4 = this.GhostOnShutdown;
    target.GhostOnShutdown = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MindContainerComponent target,
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
    MindContainerComponent target1 = (MindContainerComponent) target;
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
    MindContainerComponent target1 = (MindContainerComponent) target;
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
    MindContainerComponent target1 = (MindContainerComponent) target;
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
  virtual MindContainerComponent Component.Instantiate() => new MindContainerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MindContainerComponent_AutoState : IComponentState
  {
    public NetEntity? Mind;
    public bool ShowExamineInfo;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MindContainerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MindContainerComponent, ComponentGetState>(new ComponentEventRefHandler<MindContainerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MindContainerComponent, ComponentHandleState>(new ComponentEventRefHandler<MindContainerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MindContainerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MindContainerComponent.MindContainerComponent_AutoState()
      {
        Mind = this.GetNetEntity(component.Mind),
        ShowExamineInfo = component.ShowExamineInfo
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MindContainerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MindContainerComponent.MindContainerComponent_AutoState current))
        return;
      component.Mind = this.EnsureEntity<MindContainerComponent>(current.Mind, uid);
      component.ShowExamineInfo = current.ShowExamineInfo;
    }
  }
}
