// Decompiled with JetBrains decompiler
// Type: Content.Shared.Physics.JointVisualsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Physics;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class JointVisualsComponent : 
  Component,
  ISerializationGenerated<JointVisualsComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("sprite", false, 1, true, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier Sprite;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("target", false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetEntity? Target;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("offsetA", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetA;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("offsetB", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetB;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JointVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JointVisualsComponent) target1;
    if (serialization.TryCustomCopy<JointVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier target2 = (SpriteSpecifier) null;
    if (this.Sprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Sprite, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SpriteSpecifier>(this.Sprite, hookCtx, context);
    target.Sprite = target2;
    NetEntity? target3 = new NetEntity?();
    if (!serialization.TryCustomCopy<NetEntity?>(this.Target, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<NetEntity?>(this.Target, hookCtx, context);
    target.Target = target3;
    Vector2 target4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetA, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2>(this.OffsetA, hookCtx, context);
    target.OffsetA = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetB, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.OffsetB, hookCtx, context);
    target.OffsetB = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JointVisualsComponent target,
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
    JointVisualsComponent target1 = (JointVisualsComponent) target;
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
    JointVisualsComponent target1 = (JointVisualsComponent) target;
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
    JointVisualsComponent target1 = (JointVisualsComponent) target;
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
  virtual JointVisualsComponent Component.Instantiate() => new JointVisualsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class JointVisualsComponent_AutoState : IComponentState
  {
    public SpriteSpecifier Sprite;
    public NetEntity? Target;
    public Vector2 OffsetA;
    public Vector2 OffsetB;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class JointVisualsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<JointVisualsComponent, ComponentGetState>(new ComponentEventRefHandler<JointVisualsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<JointVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<JointVisualsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      JointVisualsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new JointVisualsComponent.JointVisualsComponent_AutoState()
      {
        Sprite = component.Sprite,
        Target = component.Target,
        OffsetA = component.OffsetA,
        OffsetB = component.OffsetB
      };
    }

    private void OnHandleState(
      EntityUid uid,
      JointVisualsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is JointVisualsComponent.JointVisualsComponent_AutoState current))
        return;
      component.Sprite = current.Sprite;
      component.Target = current.Target;
      component.OffsetA = current.OffsetA;
      component.OffsetB = current.OffsetB;
    }
  }
}
