// Decompiled with JetBrains decompiler
// Type: Content.Shared.Emag.Components.EmagComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Emag.Systems;
using Content.Shared.Tag;
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
namespace Content.Shared.Emag.Components;

[Access(new Type[] {typeof (EmagSystem)})]
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EmagComponent : 
  Component,
  ISerializationGenerated<EmagComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<TagPrototype> EmagImmuneTag = (ProtoId<TagPrototype>) "EmagImmune";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EmagType EmagType = EmagType.Interaction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier EmagSound = (SoundSpecifier) new SoundCollectionSpecifier("sparks");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmagComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmagComponent) target1;
    if (serialization.TryCustomCopy<EmagComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<TagPrototype> target2 = new ProtoId<TagPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TagPrototype>>(this.EmagImmuneTag, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<TagPrototype>>(this.EmagImmuneTag, hookCtx, context);
    target.EmagImmuneTag = target2;
    EmagType target3 = EmagType.None;
    if (!serialization.TryCustomCopy<EmagType>(this.EmagType, ref target3, hookCtx, false, context))
      target3 = this.EmagType;
    target.EmagType = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.EmagSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EmagSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.EmagSound, hookCtx, context);
    target.EmagSound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmagComponent target,
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
    EmagComponent target1 = (EmagComponent) target;
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
    EmagComponent target1 = (EmagComponent) target;
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
    EmagComponent target1 = (EmagComponent) target;
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
  virtual EmagComponent Component.Instantiate() => new EmagComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EmagComponent_AutoState : IComponentState
  {
    public ProtoId<TagPrototype> EmagImmuneTag;
    public EmagType EmagType;
    public SoundSpecifier EmagSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EmagComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EmagComponent, ComponentGetState>(new ComponentEventRefHandler<EmagComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EmagComponent, ComponentHandleState>(new ComponentEventRefHandler<EmagComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, EmagComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new EmagComponent.EmagComponent_AutoState()
      {
        EmagImmuneTag = component.EmagImmuneTag,
        EmagType = component.EmagType,
        EmagSound = component.EmagSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EmagComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EmagComponent.EmagComponent_AutoState current))
        return;
      component.EmagImmuneTag = current.EmagImmuneTag;
      component.EmagType = current.EmagType;
      component.EmagSound = current.EmagSound;
    }
  }
}
