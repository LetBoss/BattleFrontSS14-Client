// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Inventory.CMHolsterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Inventory;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedCMInventorySystem)})]
public sealed class CMHolsterComponent : 
  Component,
  ISerializationGenerated<CMHolsterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> Contents = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastEjectAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? Cooldown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? CooldownPopup;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? InsertSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Guns/gun_pistol_sheathe.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? EjectSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Weapons/Guns/gun_pistol_draw.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMHolsterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMHolsterComponent) target1;
    if (serialization.TryCustomCopy<CMHolsterComponent>(this, ref target, hookCtx, false, context))
      return;
    List<EntityUid> target2 = (List<EntityUid>) null;
    if (this.Contents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Contents, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<EntityUid>>(this.Contents, hookCtx, context);
    target.Contents = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target3, hookCtx, context);
    }
    target.Whitelist = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastEjectAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.LastEjectAt, hookCtx, context);
    target.LastEjectAt = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.Cooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.Cooldown, hookCtx, context);
    target.Cooldown = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CooldownPopup, ref target6, hookCtx, false, context))
      target6 = this.CooldownPopup;
    target.CooldownPopup = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.InsertSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.InsertSound, hookCtx, context);
    target.InsertSound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EjectSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.EjectSound, hookCtx, context);
    target.EjectSound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMHolsterComponent target,
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
    CMHolsterComponent target1 = (CMHolsterComponent) target;
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
    CMHolsterComponent target1 = (CMHolsterComponent) target;
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
    CMHolsterComponent target1 = (CMHolsterComponent) target;
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
  virtual CMHolsterComponent Component.Instantiate() => new CMHolsterComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMHolsterComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMHolsterComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<CMHolsterComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      CMHolsterComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastEjectAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMHolsterComponent_AutoState : IComponentState
  {
    public TimeSpan LastEjectAt;
    public TimeSpan? Cooldown;
    public 
    #nullable enable
    string? CooldownPopup;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMHolsterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMHolsterComponent, ComponentGetState>(new ComponentEventRefHandler<CMHolsterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMHolsterComponent, ComponentHandleState>(new ComponentEventRefHandler<CMHolsterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMHolsterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMHolsterComponent.CMHolsterComponent_AutoState()
      {
        LastEjectAt = component.LastEjectAt,
        Cooldown = component.Cooldown,
        CooldownPopup = component.CooldownPopup
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMHolsterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMHolsterComponent.CMHolsterComponent_AutoState current))
        return;
      component.LastEjectAt = current.LastEjectAt;
      component.Cooldown = current.Cooldown;
      component.CooldownPopup = current.CooldownPopup;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, CMHolsterComponent>(uid, component, ref args1);
    }
  }
}
