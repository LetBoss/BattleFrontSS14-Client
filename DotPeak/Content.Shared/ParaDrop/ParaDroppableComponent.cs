// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParaDrop.ParaDroppableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.ParaDrop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ParaDroppableComponent : 
  Component,
  ISerializationGenerated<ParaDroppableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DropDuration = 4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int DropScatter = 7;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FallHeight = 16f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier DropSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Items/fulton.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ParachutePrototype = (EntProtoId) "RMCParachuteDeployed";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? LastParaDrop;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ParaDroppableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ParaDroppableComponent) target1;
    if (serialization.TryCustomCopy<ParaDroppableComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DropDuration, ref target2, hookCtx, false, context))
      target2 = this.DropDuration;
    target.DropDuration = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.DropScatter, ref target3, hookCtx, false, context))
      target3 = this.DropScatter;
    target.DropScatter = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FallHeight, ref target4, hookCtx, false, context))
      target4 = this.FallHeight;
    target.FallHeight = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.DropSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DropSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.DropSound, hookCtx, context);
    target.DropSound = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ParachutePrototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.ParachutePrototype, hookCtx, context);
    target.ParachutePrototype = target6;
    TimeSpan? target7 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.LastParaDrop, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan?>(this.LastParaDrop, hookCtx, context);
    target.LastParaDrop = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ParaDroppableComponent target,
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
    ParaDroppableComponent target1 = (ParaDroppableComponent) target;
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
    ParaDroppableComponent target1 = (ParaDroppableComponent) target;
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
    ParaDroppableComponent target1 = (ParaDroppableComponent) target;
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
  virtual ParaDroppableComponent Component.Instantiate() => new ParaDroppableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ParaDroppableComponent_AutoState : IComponentState
  {
    public float DropDuration;
    public int DropScatter;
    public float FallHeight;
    public SoundSpecifier DropSound;
    public EntProtoId ParachutePrototype;
    public TimeSpan? LastParaDrop;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ParaDroppableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ParaDroppableComponent, ComponentGetState>(new ComponentEventRefHandler<ParaDroppableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ParaDroppableComponent, ComponentHandleState>(new ComponentEventRefHandler<ParaDroppableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ParaDroppableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ParaDroppableComponent.ParaDroppableComponent_AutoState()
      {
        DropDuration = component.DropDuration,
        DropScatter = component.DropScatter,
        FallHeight = component.FallHeight,
        DropSound = component.DropSound,
        ParachutePrototype = component.ParachutePrototype,
        LastParaDrop = component.LastParaDrop
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ParaDroppableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ParaDroppableComponent.ParaDroppableComponent_AutoState current))
        return;
      component.DropDuration = current.DropDuration;
      component.DropScatter = current.DropScatter;
      component.FallHeight = current.FallHeight;
      component.DropSound = current.DropSound;
      component.ParachutePrototype = current.ParachutePrototype;
      component.LastParaDrop = current.LastParaDrop;
    }
  }
}
