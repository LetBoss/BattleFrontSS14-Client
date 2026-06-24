// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Components.RMCActiveFultonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship.Utility.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (RMCFultonSystem)})]
public sealed class RMCActiveFultonComponent : 
  Component,
  ISerializationGenerated<RMCActiveFultonComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan ReturnAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates ReturnTo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? ReturnSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Effects/bamf.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCActiveFultonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCActiveFultonComponent) target1;
    if (serialization.TryCustomCopy<RMCActiveFultonComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ReturnAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ReturnAt, hookCtx, context);
    target.ReturnAt = target2;
    EntityCoordinates target3 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.ReturnTo, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityCoordinates>(this.ReturnTo, hookCtx, context);
    target.ReturnTo = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReturnSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.ReturnSound, hookCtx, context);
    target.ReturnSound = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCActiveFultonComponent target,
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
    RMCActiveFultonComponent target1 = (RMCActiveFultonComponent) target;
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
    RMCActiveFultonComponent target1 = (RMCActiveFultonComponent) target;
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
    RMCActiveFultonComponent target1 = (RMCActiveFultonComponent) target;
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
  virtual RMCActiveFultonComponent Component.Instantiate() => new RMCActiveFultonComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCActiveFultonComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCActiveFultonComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RMCActiveFultonComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RMCActiveFultonComponent component,
      ref EntityUnpausedEvent args)
    {
      component.ReturnAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCActiveFultonComponent_AutoState : IComponentState
  {
    public TimeSpan ReturnAt;
    public NetCoordinates ReturnTo;
    public 
    #nullable enable
    SoundSpecifier? ReturnSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCActiveFultonComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCActiveFultonComponent, ComponentGetState>(new ComponentEventRefHandler<RMCActiveFultonComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCActiveFultonComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCActiveFultonComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCActiveFultonComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCActiveFultonComponent.RMCActiveFultonComponent_AutoState()
      {
        ReturnAt = component.ReturnAt,
        ReturnTo = this.GetNetCoordinates(component.ReturnTo),
        ReturnSound = component.ReturnSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCActiveFultonComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCActiveFultonComponent.RMCActiveFultonComponent_AutoState current))
        return;
      component.ReturnAt = current.ReturnAt;
      component.ReturnTo = this.EnsureCoordinates<RMCActiveFultonComponent>(current.ReturnTo, uid);
      component.ReturnSound = current.ReturnSound;
    }
  }
}
