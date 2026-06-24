// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Components.BodyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Prototypes;
using Content.Shared.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.Body.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedBodySystem)})]
public sealed class BodyComponent : 
  Component,
  ISerializationGenerated<BodyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<BodyPrototype>? Prototype;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ContainerSlot RootContainer;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier GibSound = (SoundSpecifier) new SoundCollectionSpecifier("gib", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int RequiredLegs;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> LegEntities = new HashSet<EntityUid>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public string RootPartSlot => ((BaseContainer) this.RootContainer).ID;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BodyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BodyComponent) component;
    if (serialization.TryCustomCopy<BodyComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<BodyPrototype>? nullable = new ProtoId<BodyPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<BodyPrototype>?>(this.Prototype, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<ProtoId<BodyPrototype>?>(this.Prototype, hookCtx, context, false);
    target.Prototype = nullable;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.GibSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GibSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.GibSound, hookCtx, context, false);
    target.GibSound = soundSpecifier;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.RequiredLegs, ref num, hookCtx, false, context))
      num = this.RequiredLegs;
    target.RequiredLegs = num;
    HashSet<EntityUid> entityUidSet = (HashSet<EntityUid>) null;
    if (this.LegEntities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.LegEntities, ref entityUidSet, hookCtx, true, context))
      entityUidSet = serialization.CreateCopy<HashSet<EntityUid>>(this.LegEntities, hookCtx, context, false);
    target.LegEntities = entityUidSet;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BodyComponent target,
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
    BodyComponent target1 = (BodyComponent) target;
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
    BodyComponent target1 = (BodyComponent) target;
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
    BodyComponent target1 = (BodyComponent) target;
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
  virtual BodyComponent Component.Instantiate() => new BodyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BodyComponent_AutoState : IComponentState
  {
    public ProtoId<BodyPrototype>? Prototype;
    public SoundSpecifier GibSound;
    public int RequiredLegs;
    public HashSet<NetEntity> LegEntities;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BodyComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BodyComponent, ComponentGetState>(new ComponentEventRefHandler<BodyComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<BodyComponent, ComponentHandleState>(new ComponentEventRefHandler<BodyComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, BodyComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new BodyComponent.BodyComponent_AutoState()
      {
        Prototype = component.Prototype,
        GibSound = component.GibSound,
        RequiredLegs = component.RequiredLegs,
        LegEntities = this.GetNetEntitySet(component.LegEntities)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BodyComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is BodyComponent.BodyComponent_AutoState current))
        return;
      component.Prototype = current.Prototype;
      component.GibSound = current.GibSound;
      component.RequiredLegs = current.RequiredLegs;
      this.EnsureEntitySet<BodyComponent>(current.LegEntities, uid, component.LegEntities);
    }
  }
}
