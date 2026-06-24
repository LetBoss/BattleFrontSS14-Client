// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Spawners.SpawnOnInteractComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Spawners;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCSpawnerSystem)})]
public sealed class SpawnOnInteractComponent : 
  Component,
  ISerializationGenerated<SpawnOnInteractComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RequireEvacuation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? EvacuationPopup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? Popup;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SpawnOnInteractComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SpawnOnInteractComponent) target1;
    if (serialization.TryCustomCopy<SpawnOnInteractComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequireEvacuation, ref target3, hookCtx, false, context))
      target3 = this.RequireEvacuation;
    target.RequireEvacuation = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target4, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target4, hookCtx, context);
    }
    target.Blacklist = target4;
    LocId? target5 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.EvacuationPopup, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId?>(this.EvacuationPopup, hookCtx, context);
    target.EvacuationPopup = target5;
    LocId? target6 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.Popup, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId?>(this.Popup, hookCtx, context);
    target.Popup = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SpawnOnInteractComponent target,
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
    SpawnOnInteractComponent target1 = (SpawnOnInteractComponent) target;
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
    SpawnOnInteractComponent target1 = (SpawnOnInteractComponent) target;
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
    SpawnOnInteractComponent target1 = (SpawnOnInteractComponent) target;
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
  virtual SpawnOnInteractComponent Component.Instantiate() => new SpawnOnInteractComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SpawnOnInteractComponent_AutoState : IComponentState
  {
    public EntProtoId Spawn;
    public bool RequireEvacuation;
    public EntityWhitelist? Blacklist;
    public LocId? EvacuationPopup;
    public LocId? Popup;
    public SoundSpecifier? Sound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SpawnOnInteractComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SpawnOnInteractComponent, ComponentGetState>(new ComponentEventRefHandler<SpawnOnInteractComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SpawnOnInteractComponent, ComponentHandleState>(new ComponentEventRefHandler<SpawnOnInteractComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SpawnOnInteractComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SpawnOnInteractComponent.SpawnOnInteractComponent_AutoState()
      {
        Spawn = component.Spawn,
        RequireEvacuation = component.RequireEvacuation,
        Blacklist = component.Blacklist,
        EvacuationPopup = component.EvacuationPopup,
        Popup = component.Popup,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SpawnOnInteractComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SpawnOnInteractComponent.SpawnOnInteractComponent_AutoState current))
        return;
      component.Spawn = current.Spawn;
      component.RequireEvacuation = current.RequireEvacuation;
      component.Blacklist = current.Blacklist;
      component.EvacuationPopup = current.EvacuationPopup;
      component.Popup = current.Popup;
      component.Sound = current.Sound;
    }
  }
}
