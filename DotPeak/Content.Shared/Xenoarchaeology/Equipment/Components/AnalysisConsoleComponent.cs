// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Equipment.Components.AnalysisConsoleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Equipment.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class AnalysisConsoleComponent : 
  Component,
  ISerializationGenerated<AnalysisConsoleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? AnalyzerEntity;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ScanFinishedSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ExtractSound;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SourcePortPrototype> LinkingPort;

  public AnalysisConsoleComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Effects/radpulse11.ogg");
    soundPathSpecifier.Params = new AudioParams()
    {
      Volume = 4f
    };
    this.ExtractSound = (SoundSpecifier) soundPathSpecifier;
    this.LinkingPort = (ProtoId<SourcePortPrototype>) "ArtifactAnalyzerSender";
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AnalysisConsoleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AnalysisConsoleComponent) target1;
    if (serialization.TryCustomCopy<AnalysisConsoleComponent>(this, ref target, hookCtx, false, context))
      return;
    NetEntity? target2 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.AnalyzerEntity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<NetEntity?>(this.AnalyzerEntity, hookCtx, context);
    target.AnalyzerEntity = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ScanFinishedSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.ScanFinishedSound, hookCtx, context);
    target.ScanFinishedSound = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ExtractSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.ExtractSound, hookCtx, context);
    target.ExtractSound = target4;
    ProtoId<SourcePortPrototype> target5 = new ProtoId<SourcePortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SourcePortPrototype>>(this.LinkingPort, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<SourcePortPrototype>>(this.LinkingPort, hookCtx, context);
    target.LinkingPort = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AnalysisConsoleComponent target,
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
    AnalysisConsoleComponent target1 = (AnalysisConsoleComponent) target;
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
    AnalysisConsoleComponent target1 = (AnalysisConsoleComponent) target;
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
    AnalysisConsoleComponent target1 = (AnalysisConsoleComponent) target;
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
  virtual AnalysisConsoleComponent Component.Instantiate() => new AnalysisConsoleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AnalysisConsoleComponent_AutoState : IComponentState
  {
    public NetEntity? AnalyzerEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AnalysisConsoleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AnalysisConsoleComponent, ComponentGetState>(new ComponentEventRefHandler<AnalysisConsoleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AnalysisConsoleComponent, ComponentHandleState>(new ComponentEventRefHandler<AnalysisConsoleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AnalysisConsoleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AnalysisConsoleComponent.AnalysisConsoleComponent_AutoState()
      {
        AnalyzerEntity = component.AnalyzerEntity
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AnalysisConsoleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AnalysisConsoleComponent.AnalysisConsoleComponent_AutoState current))
        return;
      component.AnalyzerEntity = current.AnalyzerEntity;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, AnalysisConsoleComponent>(uid, component, ref args1);
    }
  }
}
