// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Components.FlatpackComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedFlatpackSystem)})]
public sealed class FlatpackComponent : 
  Component,
  ISerializationGenerated<FlatpackComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> QualityNeeded = ProtoId<ToolQualityPrototype>.op_Implicit("Pulsing");
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntProtoId? Entity;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public SoundSpecifier UnpackSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/unwrap.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, Color> BoardColors = new Dictionary<string, Color>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlatpackComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FlatpackComponent) component;
    if (serialization.TryCustomCopy<FlatpackComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ToolQualityPrototype> protoId = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.QualityNeeded, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.QualityNeeded, hookCtx, context, false);
    target.QualityNeeded = protoId;
    EntProtoId? nullable = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Entity, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntProtoId?>(this.Entity, hookCtx, context, false);
    target.Entity = nullable;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.UnpackSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnpackSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.UnpackSound, hookCtx, context, false);
    target.UnpackSound = soundSpecifier;
    Dictionary<string, Color> dictionary = (Dictionary<string, Color>) null;
    if (this.BoardColors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Color>>(this.BoardColors, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, Color>>(this.BoardColors, hookCtx, context, false);
    target.BoardColors = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlatpackComponent target,
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
    FlatpackComponent target1 = (FlatpackComponent) target;
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
    FlatpackComponent target1 = (FlatpackComponent) target;
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
    FlatpackComponent target1 = (FlatpackComponent) target;
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
  virtual FlatpackComponent Component.Instantiate() => new FlatpackComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FlatpackComponent_AutoState : IComponentState
  {
    public ProtoId<ToolQualityPrototype> QualityNeeded;
    public EntProtoId? Entity;
    public SoundSpecifier UnpackSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FlatpackComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlatpackComponent, ComponentGetState>(new ComponentEventRefHandler<FlatpackComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<FlatpackComponent, ComponentHandleState>(new ComponentEventRefHandler<FlatpackComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, FlatpackComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new FlatpackComponent.FlatpackComponent_AutoState()
      {
        QualityNeeded = component.QualityNeeded,
        Entity = component.Entity,
        UnpackSound = component.UnpackSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FlatpackComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is FlatpackComponent.FlatpackComponent_AutoState current))
        return;
      component.QualityNeeded = current.QualityNeeded;
      component.Entity = current.Entity;
      component.UnpackSound = current.UnpackSound;
    }
  }
}
