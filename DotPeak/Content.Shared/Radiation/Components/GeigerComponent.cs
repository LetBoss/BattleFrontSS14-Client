// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Components.GeigerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radiation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Radiation.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedGeigerSystem)})]
public sealed class GeigerComponent : 
  Component,
  ISerializationGenerated<GeigerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool AttachedToSuit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsEnabled;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool ShowExamine;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  public bool ShowControl;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<GeigerDangerLevel, SoundSpecifier> Sounds = new Dictionary<GeigerDangerLevel, SoundSpecifier>()
  {
    {
      GeigerDangerLevel.Low,
      (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Geiger/low.ogg")
    },
    {
      GeigerDangerLevel.Med,
      (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Geiger/med.ogg")
    },
    {
      GeigerDangerLevel.High,
      (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Geiger/high.ogg")
    },
    {
      GeigerDangerLevel.Extreme,
      (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/Geiger/ext.ogg")
    }
  };
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [AutoNetworkedField]
  public float CurrentRadiation;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [AutoNetworkedField]
  public GeigerDangerLevel DangerLevel;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [AutoNetworkedField]
  public EntityUid? User;
  [Access(new Type[] {typeof (SharedGeigerSystem)}, Other = AccessPermissions.ReadWrite)]
  public bool UiUpdateNeeded;
  public EntityUid? Stream;
  [DataField(null, false, 1, false, false, null)]
  public bool BroadcastAudio;
  [DataField(null, false, 1, false, false, null)]
  public float BroadcastRange = 4f;
  [DataField(null, false, 1, false, false, null)]
  public float Volume = -4f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GeigerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GeigerComponent) target1;
    if (serialization.TryCustomCopy<GeigerComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.AttachedToSuit, ref target2, hookCtx, false, context))
      target2 = this.AttachedToSuit;
    target.AttachedToSuit = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsEnabled, ref target3, hookCtx, false, context))
      target3 = this.IsEnabled;
    target.IsEnabled = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowExamine, ref target4, hookCtx, false, context))
      target4 = this.ShowExamine;
    target.ShowExamine = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowControl, ref target5, hookCtx, false, context))
      target5 = this.ShowControl;
    target.ShowControl = target5;
    Dictionary<GeigerDangerLevel, SoundSpecifier> target6 = (Dictionary<GeigerDangerLevel, SoundSpecifier>) null;
    if (this.Sounds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<GeigerDangerLevel, SoundSpecifier>>(this.Sounds, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<GeigerDangerLevel, SoundSpecifier>>(this.Sounds, hookCtx, context);
    target.Sounds = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.BroadcastAudio, ref target7, hookCtx, false, context))
      target7 = this.BroadcastAudio;
    target.BroadcastAudio = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BroadcastRange, ref target8, hookCtx, false, context))
      target8 = this.BroadcastRange;
    target.BroadcastRange = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Volume, ref target9, hookCtx, false, context))
      target9 = this.Volume;
    target.Volume = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GeigerComponent target,
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
    GeigerComponent target1 = (GeigerComponent) target;
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
    GeigerComponent target1 = (GeigerComponent) target;
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
    GeigerComponent target1 = (GeigerComponent) target;
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
  virtual GeigerComponent Component.Instantiate() => new GeigerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GeigerComponent_AutoState : IComponentState
  {
    public bool IsEnabled;
    public float CurrentRadiation;
    public GeigerDangerLevel DangerLevel;
    public NetEntity? User;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GeigerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GeigerComponent, ComponentGetState>(new ComponentEventRefHandler<GeigerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GeigerComponent, ComponentHandleState>(new ComponentEventRefHandler<GeigerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, GeigerComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new GeigerComponent.GeigerComponent_AutoState()
      {
        IsEnabled = component.IsEnabled,
        CurrentRadiation = component.CurrentRadiation,
        DangerLevel = component.DangerLevel,
        User = this.GetNetEntity(component.User)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GeigerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GeigerComponent.GeigerComponent_AutoState current))
        return;
      component.IsEnabled = current.IsEnabled;
      component.CurrentRadiation = current.CurrentRadiation;
      component.DangerLevel = current.DangerLevel;
      component.User = this.EnsureEntity<GeigerComponent>(current.User, uid);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, GeigerComponent>(uid, component, ref args1);
    }
  }
}
