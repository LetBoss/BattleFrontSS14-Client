// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Fabricator.DropshipFabricatorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Fabricator;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (DropshipFabricatorSystem)})]
public sealed class DropshipFabricatorComponent : 
  Component,
  ISerializationGenerated<DropshipFabricatorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Account;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Points;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<DropshipFabricatorPrintableComponent>? Printing;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan PrintAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i PrintOffset = new Vector2i(1, 0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier RecycleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/fax.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipFabricatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipFabricatorComponent) target1;
    if (serialization.TryCustomCopy<DropshipFabricatorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Account, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Account, hookCtx, context);
    target.Account = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Points, ref target3, hookCtx, false, context))
      target3 = this.Points;
    target.Points = target3;
    EntProtoId<DropshipFabricatorPrintableComponent>? target4 = new EntProtoId<DropshipFabricatorPrintableComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<DropshipFabricatorPrintableComponent>?>(this.Printing, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<DropshipFabricatorPrintableComponent>?>(this.Printing, hookCtx, context);
    target.Printing = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.PrintAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.PrintAt, hookCtx, context);
    target.PrintAt = target5;
    Vector2i target6 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.PrintOffset, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2i>(this.PrintOffset, hookCtx, context);
    target.PrintOffset = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (this.RecycleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RecycleSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.RecycleSound, hookCtx, context);
    target.RecycleSound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipFabricatorComponent target,
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
    DropshipFabricatorComponent target1 = (DropshipFabricatorComponent) target;
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
    DropshipFabricatorComponent target1 = (DropshipFabricatorComponent) target;
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
    DropshipFabricatorComponent target1 = (DropshipFabricatorComponent) target;
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
  virtual DropshipFabricatorComponent Component.Instantiate() => new DropshipFabricatorComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipFabricatorComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipFabricatorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DropshipFabricatorComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DropshipFabricatorComponent component,
      ref EntityUnpausedEvent args)
    {
      component.PrintAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipFabricatorComponent_AutoState : IComponentState
  {
    public int Points;
    public EntProtoId<
    #nullable enable
    DropshipFabricatorPrintableComponent>? Printing;
    public TimeSpan PrintAt;
    public Vector2i PrintOffset;
    public SoundSpecifier RecycleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipFabricatorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipFabricatorComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipFabricatorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipFabricatorComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipFabricatorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipFabricatorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipFabricatorComponent.DropshipFabricatorComponent_AutoState()
      {
        Points = component.Points,
        Printing = component.Printing,
        PrintAt = component.PrintAt,
        PrintOffset = component.PrintOffset,
        RecycleSound = component.RecycleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipFabricatorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipFabricatorComponent.DropshipFabricatorComponent_AutoState current))
        return;
      component.Points = current.Points;
      component.Printing = current.Printing;
      component.PrintAt = current.PrintAt;
      component.PrintOffset = current.PrintOffset;
      component.RecycleSound = current.RecycleSound;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, DropshipFabricatorComponent>(uid, component, ref args1);
    }
  }
}
