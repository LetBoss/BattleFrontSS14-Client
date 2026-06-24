// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.AegisCrate.AegisCrateComponent
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.AegisCrate;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class AegisCrateComponent : 
  Component,
  ISerializationGenerated<AegisCrateComponent>,
  ISerializationGenerated
{
  [DataField("openSound", false, 1, false, false, null)]
  public SoundSpecifier? OpenSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Structures/secure_box_opening/secure_box_opening.ogg");
  [DataField("closeSound", false, 1, false, false, null)]
  public SoundSpecifier? CloseSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId OB = (EntProtoId) "RMCOrbitalCannonWarheadAegis";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? OpenAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Spawned;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public AegisCrateState State { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AegisCrateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AegisCrateComponent) target1;
    if (serialization.TryCustomCopy<AegisCrateComponent>(this, ref target, hookCtx, false, context))
      return;
    AegisCrateState target2 = AegisCrateState.Closed;
    if (!serialization.TryCustomCopy<AegisCrateState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context);
    target.OpenSound = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.CloseSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.CloseSound, hookCtx, context);
    target.CloseSound = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.OB, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.OB, hookCtx, context);
    target.OB = target5;
    TimeSpan? target6 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.OpenAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan?>(this.OpenAt, hookCtx, context);
    target.OpenAt = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Spawned, ref target7, hookCtx, false, context))
      target7 = this.Spawned;
    target.Spawned = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AegisCrateComponent target,
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
    AegisCrateComponent target1 = (AegisCrateComponent) target;
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
    AegisCrateComponent target1 = (AegisCrateComponent) target;
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
    AegisCrateComponent target1 = (AegisCrateComponent) target;
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
  virtual AegisCrateComponent Component.Instantiate() => new AegisCrateComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AegisCrateComponent_AutoState : IComponentState
  {
    public AegisCrateState State;
    public EntProtoId OB;
    public TimeSpan? OpenAt;
    public bool Spawned;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AegisCrateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AegisCrateComponent, ComponentGetState>(new ComponentEventRefHandler<AegisCrateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AegisCrateComponent, ComponentHandleState>(new ComponentEventRefHandler<AegisCrateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AegisCrateComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AegisCrateComponent.AegisCrateComponent_AutoState()
      {
        State = component.State,
        OB = component.OB,
        OpenAt = component.OpenAt,
        Spawned = component.Spawned
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AegisCrateComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AegisCrateComponent.AegisCrateComponent_AutoState current))
        return;
      component.State = current.State;
      component.OB = current.OB;
      component.OpenAt = current.OpenAt;
      component.Spawned = current.Spawned;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, AegisCrateComponent>(uid, component, ref args1);
    }
  }
}
