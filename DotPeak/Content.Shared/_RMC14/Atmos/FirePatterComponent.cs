// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.FirePatterComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCFlammableSystem)})]
public sealed class FirePatterComponent : 
  Component,
  ISerializationGenerated<FirePatterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/thudswoosh.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastPat;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Stacks = -10;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FirePatterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FirePatterComponent) target1;
    if (serialization.TryCustomCopy<FirePatterComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, context);
    }
    target.Blacklist = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastPat, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.LastPat, hookCtx, context);
    target.LastPat = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.Stacks, ref target6, hookCtx, false, context))
      target6 = this.Stacks;
    target.Stacks = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FirePatterComponent target,
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
    FirePatterComponent target1 = (FirePatterComponent) target;
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
    FirePatterComponent target1 = (FirePatterComponent) target;
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
    FirePatterComponent target1 = (FirePatterComponent) target;
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
  virtual FirePatterComponent Component.Instantiate() => new FirePatterComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FirePatterComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FirePatterComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<FirePatterComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      FirePatterComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastPat += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FirePatterComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    SoundSpecifier? Sound;
    public EntityWhitelist? Blacklist;
    public TimeSpan LastPat;
    public TimeSpan Cooldown;
    public int Stacks;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FirePatterComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FirePatterComponent, ComponentGetState>(new ComponentEventRefHandler<FirePatterComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FirePatterComponent, ComponentHandleState>(new ComponentEventRefHandler<FirePatterComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FirePatterComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FirePatterComponent.FirePatterComponent_AutoState()
      {
        Sound = component.Sound,
        Blacklist = component.Blacklist,
        LastPat = component.LastPat,
        Cooldown = component.Cooldown,
        Stacks = component.Stacks
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FirePatterComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FirePatterComponent.FirePatterComponent_AutoState current))
        return;
      component.Sound = current.Sound;
      component.Blacklist = current.Blacklist;
      component.LastPat = current.LastPat;
      component.Cooldown = current.Cooldown;
      component.Stacks = current.Stacks;
    }
  }
}
