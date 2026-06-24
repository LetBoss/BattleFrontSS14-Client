// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Components.PortalComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Teleportation.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PortalComponent : 
  Component,
  ISerializationGenerated<PortalComponent>,
  ISerializationGenerated
{
  [DataField("arrivalSound", false, 1, false, false, null)]
  public SoundSpecifier ArrivalSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");
  [DataField("departureSound", false, 1, false, false, null)]
  public SoundSpecifier DepartureSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/teleport_departure.ogg");
  [DataField("maxRandomRadius", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MaxRandomRadius = 7f;
  [DataField("canTeleportToOtherMaps", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool CanTeleportToOtherMaps;
  [DataField("maxTeleportRadius", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float? MaxTeleportRadius;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool RandomTeleport = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PortalComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PortalComponent) target1;
    if (serialization.TryCustomCopy<PortalComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.ArrivalSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ArrivalSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.ArrivalSound, hookCtx, context);
    target.ArrivalSound = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.DepartureSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DepartureSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.DepartureSound, hookCtx, context);
    target.DepartureSound = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRandomRadius, ref target4, hookCtx, false, context))
      target4 = this.MaxRandomRadius;
    target.MaxRandomRadius = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanTeleportToOtherMaps, ref target5, hookCtx, false, context))
      target5 = this.CanTeleportToOtherMaps;
    target.CanTeleportToOtherMaps = target5;
    float? target6 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.MaxTeleportRadius, ref target6, hookCtx, false, context))
      target6 = this.MaxTeleportRadius;
    target.MaxTeleportRadius = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.RandomTeleport, ref target7, hookCtx, false, context))
      target7 = this.RandomTeleport;
    target.RandomTeleport = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PortalComponent target,
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
    PortalComponent target1 = (PortalComponent) target;
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
    PortalComponent target1 = (PortalComponent) target;
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
    PortalComponent target1 = (PortalComponent) target;
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
  virtual PortalComponent Component.Instantiate() => new PortalComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PortalComponent_AutoState : IComponentState
  {
    public bool RandomTeleport;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PortalComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PortalComponent, ComponentGetState>(new ComponentEventRefHandler<PortalComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PortalComponent, ComponentHandleState>(new ComponentEventRefHandler<PortalComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, PortalComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new PortalComponent.PortalComponent_AutoState()
      {
        RandomTeleport = component.RandomTeleport
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PortalComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PortalComponent.PortalComponent_AutoState current))
        return;
      component.RandomTeleport = current.RandomTeleport;
    }
  }
}
