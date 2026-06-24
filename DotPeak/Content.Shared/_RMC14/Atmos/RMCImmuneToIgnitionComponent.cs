// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.RMCImmuneToIgnitionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCImmuneToIgnitionComponent : 
  Component,
  ISerializationGenerated<RMCImmuneToIgnitionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? BypassWhitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int IntensityResistance = 80 /*0x50*/;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ImmuneToDirectHits = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCImmuneToIgnitionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCImmuneToIgnitionComponent) target1;
    if (serialization.TryCustomCopy<RMCImmuneToIgnitionComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.BypassWhitelist, ref target2, hookCtx, false, context))
    {
      if (this.BypassWhitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.BypassWhitelist, ref target2, hookCtx, context);
    }
    target.BypassWhitelist = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.IntensityResistance, ref target3, hookCtx, false, context))
      target3 = this.IntensityResistance;
    target.IntensityResistance = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ImmuneToDirectHits, ref target4, hookCtx, false, context))
      target4 = this.ImmuneToDirectHits;
    target.ImmuneToDirectHits = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCImmuneToIgnitionComponent target,
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
    RMCImmuneToIgnitionComponent target1 = (RMCImmuneToIgnitionComponent) target;
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
    RMCImmuneToIgnitionComponent target1 = (RMCImmuneToIgnitionComponent) target;
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
    RMCImmuneToIgnitionComponent target1 = (RMCImmuneToIgnitionComponent) target;
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
  virtual RMCImmuneToIgnitionComponent Component.Instantiate()
  {
    return new RMCImmuneToIgnitionComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCImmuneToIgnitionComponent_AutoState : IComponentState
  {
    public EntityWhitelist? BypassWhitelist;
    public int IntensityResistance;
    public bool ImmuneToDirectHits;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCImmuneToIgnitionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCImmuneToIgnitionComponent, ComponentGetState>(new ComponentEventRefHandler<RMCImmuneToIgnitionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCImmuneToIgnitionComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCImmuneToIgnitionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCImmuneToIgnitionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCImmuneToIgnitionComponent.RMCImmuneToIgnitionComponent_AutoState()
      {
        BypassWhitelist = component.BypassWhitelist,
        IntensityResistance = component.IntensityResistance,
        ImmuneToDirectHits = component.ImmuneToDirectHits
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCImmuneToIgnitionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCImmuneToIgnitionComponent.RMCImmuneToIgnitionComponent_AutoState current))
        return;
      component.BypassWhitelist = current.BypassWhitelist;
      component.IntensityResistance = current.IntensityResistance;
      component.ImmuneToDirectHits = current.ImmuneToDirectHits;
    }
  }
}
