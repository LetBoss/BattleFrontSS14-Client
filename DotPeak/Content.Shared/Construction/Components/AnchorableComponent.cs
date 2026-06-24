// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Components.AnchorableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.EntitySystems;
using Content.Shared.Tools;
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
namespace Content.Shared.Construction.Components;

[RegisterComponent]
[Access(new Type[] {typeof (AnchorableSystem)})]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class AnchorableComponent : 
  Component,
  ISerializationGenerated<AnchorableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AnchorableFlags Flags = AnchorableFlags.Anchorable | AnchorableFlags.Unanchorable;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public float Delay = 1f;

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> Tool { get; private set; } = ProtoId<ToolQualityPrototype>.op_Implicit("Anchoring");

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Snap { get; private set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AnchorableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AnchorableComponent) component;
    if (serialization.TryCustomCopy<AnchorableComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ToolQualityPrototype> protoId = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.Tool, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.Tool, hookCtx, context, false);
    target.Tool = protoId;
    AnchorableFlags anchorableFlags = AnchorableFlags.None;
    if (!serialization.TryCustomCopy<AnchorableFlags>(this.Flags, ref anchorableFlags, hookCtx, false, context))
      anchorableFlags = this.Flags;
    target.Flags = anchorableFlags;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Snap, ref flag, hookCtx, false, context))
      flag = this.Snap;
    target.Snap = flag;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref num, hookCtx, false, context))
      num = this.Delay;
    target.Delay = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AnchorableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AnchorableComponent target1 = (AnchorableComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AnchorableComponent target1 = (AnchorableComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AnchorableComponent target1 = (AnchorableComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AnchorableComponent Component.Instantiate() => new AnchorableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AnchorableComponent_AutoState : IComponentState
  {
    public AnchorableFlags Flags;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AnchorableComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnchorableComponent, ComponentGetState>(new ComponentEventRefHandler<AnchorableComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnchorableComponent, ComponentHandleState>(new ComponentEventRefHandler<AnchorableComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      AnchorableComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new AnchorableComponent.AnchorableComponent_AutoState()
      {
        Flags = component.Flags
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AnchorableComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is AnchorableComponent.AnchorableComponent_AutoState current))
        return;
      component.Flags = current.Flags;
    }
  }
}
