// Decompiled with JetBrains decompiler
// Type: Content.Shared.NameIdentifier.NameIdentifierComponent
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
namespace Content.Shared.NameIdentifier;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class NameIdentifierComponent : 
  Component,
  ISerializationGenerated<NameIdentifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<NameIdentifierGroupPrototype>? Group;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Identifier = -1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FullIdentifier = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NameIdentifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NameIdentifierComponent) target1;
    if (serialization.TryCustomCopy<NameIdentifierComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<NameIdentifierGroupPrototype>? target2 = new ProtoId<NameIdentifierGroupPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<NameIdentifierGroupPrototype>?>(this.Group, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<NameIdentifierGroupPrototype>?>(this.Group, hookCtx, context);
    target.Group = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Identifier, ref target3, hookCtx, false, context))
      target3 = this.Identifier;
    target.Identifier = target3;
    string target4 = (string) null;
    if (this.FullIdentifier == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FullIdentifier, ref target4, hookCtx, false, context))
      target4 = this.FullIdentifier;
    target.FullIdentifier = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NameIdentifierComponent target,
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
    NameIdentifierComponent target1 = (NameIdentifierComponent) target;
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
    NameIdentifierComponent target1 = (NameIdentifierComponent) target;
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
    NameIdentifierComponent target1 = (NameIdentifierComponent) target;
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
  virtual NameIdentifierComponent Component.Instantiate() => new NameIdentifierComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NameIdentifierComponent_AutoState : IComponentState
  {
    public int Identifier;
    public string FullIdentifier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NameIdentifierComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NameIdentifierComponent, ComponentGetState>(new ComponentEventRefHandler<NameIdentifierComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NameIdentifierComponent, ComponentHandleState>(new ComponentEventRefHandler<NameIdentifierComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NameIdentifierComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NameIdentifierComponent.NameIdentifierComponent_AutoState()
      {
        Identifier = component.Identifier,
        FullIdentifier = component.FullIdentifier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NameIdentifierComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NameIdentifierComponent.NameIdentifierComponent_AutoState current))
        return;
      component.Identifier = current.Identifier;
      component.FullIdentifier = current.FullIdentifier;
    }
  }
}
