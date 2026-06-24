// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Components.AttachableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableHolderSystem)})]
public sealed class AttachableComponent : 
  Component,
  ISerializationGenerated<AttachableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AttachDoAfter = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? AttachSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_add.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6.5f)));
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DetachSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_remove.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5.5f)));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AttachableComponent) target1;
    if (serialization.TryCustomCopy<AttachableComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AttachDoAfter, ref target2, hookCtx, false, context))
      target2 = this.AttachDoAfter;
    target.AttachDoAfter = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AttachSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.AttachSound, hookCtx, context);
    target.AttachSound = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DetachSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.DetachSound, hookCtx, context);
    target.DetachSound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableComponent target,
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
    AttachableComponent target1 = (AttachableComponent) target;
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
    AttachableComponent target1 = (AttachableComponent) target;
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
    AttachableComponent target1 = (AttachableComponent) target;
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
  virtual AttachableComponent Component.Instantiate() => new AttachableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AttachableComponent_AutoState : IComponentState
  {
    public float AttachDoAfter;
    public SoundSpecifier? AttachSound;
    public SoundSpecifier? DetachSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AttachableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AttachableComponent, ComponentGetState>(new ComponentEventRefHandler<AttachableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AttachableComponent, ComponentHandleState>(new ComponentEventRefHandler<AttachableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AttachableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AttachableComponent.AttachableComponent_AutoState()
      {
        AttachDoAfter = component.AttachDoAfter,
        AttachSound = component.AttachSound,
        DetachSound = component.DetachSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AttachableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AttachableComponent.AttachableComponent_AutoState current))
        return;
      component.AttachDoAfter = current.AttachDoAfter;
      component.AttachSound = current.AttachSound;
      component.DetachSound = current.DetachSound;
    }
  }
}
