// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tools.Components.ToolOpenableComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ToolOpenableComponent : 
  Component,
  ISerializationGenerated<ToolOpenableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsOpen;
  [DataField(null, false, 1, false, false, null)]
  public float OpenTime = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float CloseTime = 1f;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype>? OpenToolQualityNeeded;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype>? CloseToolQualityNeeded;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasVerbs = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool VerbOnly;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? Name;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ToolOpenableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ToolOpenableComponent) target1;
    if (serialization.TryCustomCopy<ToolOpenableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsOpen, ref target2, hookCtx, false, context))
      target2 = this.IsOpen;
    target.IsOpen = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OpenTime, ref target3, hookCtx, false, context))
      target3 = this.OpenTime;
    target.OpenTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CloseTime, ref target4, hookCtx, false, context))
      target4 = this.CloseTime;
    target.CloseTime = target4;
    ProtoId<ToolQualityPrototype>? target5 = new ProtoId<ToolQualityPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>?>(this.OpenToolQualityNeeded, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>?>(this.OpenToolQualityNeeded, hookCtx, context);
    target.OpenToolQualityNeeded = target5;
    ProtoId<ToolQualityPrototype>? target6 = new ProtoId<ToolQualityPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>?>(this.CloseToolQualityNeeded, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>?>(this.CloseToolQualityNeeded, hookCtx, context);
    target.CloseToolQualityNeeded = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasVerbs, ref target7, hookCtx, false, context))
      target7 = this.HasVerbs;
    target.HasVerbs = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.VerbOnly, ref target8, hookCtx, false, context))
      target8 = this.VerbOnly;
    target.VerbOnly = target8;
    string target9 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Name, ref target9, hookCtx, false, context))
      target9 = this.Name;
    target.Name = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ToolOpenableComponent target,
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
    ToolOpenableComponent target1 = (ToolOpenableComponent) target;
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
    ToolOpenableComponent target1 = (ToolOpenableComponent) target;
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
    ToolOpenableComponent target1 = (ToolOpenableComponent) target;
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
  virtual ToolOpenableComponent Component.Instantiate() => new ToolOpenableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ToolOpenableComponent_AutoState : IComponentState
  {
    public bool IsOpen;
    public bool HasVerbs;
    public bool VerbOnly;
    public string? Name;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ToolOpenableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ToolOpenableComponent, ComponentGetState>(new ComponentEventRefHandler<ToolOpenableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ToolOpenableComponent, ComponentHandleState>(new ComponentEventRefHandler<ToolOpenableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ToolOpenableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ToolOpenableComponent.ToolOpenableComponent_AutoState()
      {
        IsOpen = component.IsOpen,
        HasVerbs = component.HasVerbs,
        VerbOnly = component.VerbOnly,
        Name = component.Name
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ToolOpenableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ToolOpenableComponent.ToolOpenableComponent_AutoState current))
        return;
      component.IsOpen = current.IsOpen;
      component.HasVerbs = current.HasVerbs;
      component.VerbOnly = current.VerbOnly;
      component.Name = current.Name;
    }
  }
}
