// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Components.SwapTeleporterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Teleportation.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Teleportation.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SwapTeleporterSystem)})]
public sealed class SwapTeleporterComponent : 
  Component,
  ISerializationGenerated<SwapTeleporterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? LinkedEnt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan? TeleportTime;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan TeleportDelay = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextTeleportUse;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan Cooldown = TimeSpan.FromMinutes(5L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? TeleportSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/flash.ogg");
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist TeleporterWhitelist = new EntityWhitelist();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SwapTeleporterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SwapTeleporterComponent) target1;
    if (serialization.TryCustomCopy<SwapTeleporterComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.LinkedEnt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.LinkedEnt, hookCtx, context);
    target.LinkedEnt = target2;
    TimeSpan? target3 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.TeleportTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan?>(this.TeleportTime, hookCtx, context);
    target.TeleportTime = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TeleportDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.TeleportDelay, hookCtx, context);
    target.TeleportDelay = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextTeleportUse, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextTeleportUse, hookCtx, context);
    target.NextTeleportUse = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TeleportSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.TeleportSound, hookCtx, context);
    target.TeleportSound = target7;
    EntityWhitelist target8 = (EntityWhitelist) null;
    if (this.TeleporterWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.TeleporterWhitelist, ref target8, hookCtx, false, context))
    {
      if (this.TeleporterWhitelist == null)
        target8 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.TeleporterWhitelist, ref target8, hookCtx, context, true);
    }
    target.TeleporterWhitelist = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SwapTeleporterComponent target,
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
    SwapTeleporterComponent target1 = (SwapTeleporterComponent) target;
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
    SwapTeleporterComponent target1 = (SwapTeleporterComponent) target;
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
    SwapTeleporterComponent target1 = (SwapTeleporterComponent) target;
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
  virtual SwapTeleporterComponent Component.Instantiate() => new SwapTeleporterComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SwapTeleporterComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SwapTeleporterComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SwapTeleporterComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SwapTeleporterComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextTeleportUse += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SwapTeleporterComponent_AutoState : IComponentState
  {
    public NetEntity? LinkedEnt;
    public TimeSpan? TeleportTime;
    public TimeSpan NextTeleportUse;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SwapTeleporterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SwapTeleporterComponent, ComponentGetState>(new ComponentEventRefHandler<SwapTeleporterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SwapTeleporterComponent, ComponentHandleState>(new ComponentEventRefHandler<SwapTeleporterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      SwapTeleporterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SwapTeleporterComponent.SwapTeleporterComponent_AutoState()
      {
        LinkedEnt = this.GetNetEntity(component.LinkedEnt),
        TeleportTime = component.TeleportTime,
        NextTeleportUse = component.NextTeleportUse
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SwapTeleporterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SwapTeleporterComponent.SwapTeleporterComponent_AutoState current))
        return;
      component.LinkedEnt = this.EnsureEntity<SwapTeleporterComponent>(current.LinkedEnt, uid);
      component.TeleportTime = current.TeleportTime;
      component.NextTeleportUse = current.NextTeleportUse;
    }
  }
}
