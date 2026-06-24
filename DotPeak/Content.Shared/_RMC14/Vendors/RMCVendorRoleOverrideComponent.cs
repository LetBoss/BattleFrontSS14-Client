// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.RMCVendorRoleOverrideComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCVendorRoleOverrideComponent : 
  Component,
  ISerializationGenerated<RMCVendorRoleOverrideComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId? GiveSquadRoleName;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsAppendSquadRoleName;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier.Rsi? GiveIcon;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVendorRoleOverrideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVendorRoleOverrideComponent) target1;
    if (serialization.TryCustomCopy<RMCVendorRoleOverrideComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId? target2 = new LocId?();
    if (!serialization.TryCustomCopy<LocId?>(this.GiveSquadRoleName, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId?>(this.GiveSquadRoleName, hookCtx, context);
    target.GiveSquadRoleName = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAppendSquadRoleName, ref target3, hookCtx, false, context))
      target3 = this.IsAppendSquadRoleName;
    target.IsAppendSquadRoleName = target3;
    SpriteSpecifier.Rsi target4 = (SpriteSpecifier.Rsi) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier.Rsi>(this.GiveIcon, ref target4, hookCtx, false, context))
    {
      if (this.GiveIcon == null)
        target4 = (SpriteSpecifier.Rsi) null;
      else
        serialization.CopyTo<SpriteSpecifier.Rsi>(this.GiveIcon, ref target4, hookCtx, context);
    }
    target.GiveIcon = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVendorRoleOverrideComponent target,
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
    RMCVendorRoleOverrideComponent target1 = (RMCVendorRoleOverrideComponent) target;
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
    RMCVendorRoleOverrideComponent target1 = (RMCVendorRoleOverrideComponent) target;
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
    RMCVendorRoleOverrideComponent target1 = (RMCVendorRoleOverrideComponent) target;
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
  virtual RMCVendorRoleOverrideComponent Component.Instantiate()
  {
    return new RMCVendorRoleOverrideComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCVendorRoleOverrideComponent_AutoState : IComponentState
  {
    public LocId? GiveSquadRoleName;
    public bool IsAppendSquadRoleName;
    public SpriteSpecifier.Rsi? GiveIcon;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCVendorRoleOverrideComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCVendorRoleOverrideComponent, ComponentGetState>(new ComponentEventRefHandler<RMCVendorRoleOverrideComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCVendorRoleOverrideComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCVendorRoleOverrideComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCVendorRoleOverrideComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCVendorRoleOverrideComponent.RMCVendorRoleOverrideComponent_AutoState()
      {
        GiveSquadRoleName = component.GiveSquadRoleName,
        IsAppendSquadRoleName = component.IsAppendSquadRoleName,
        GiveIcon = component.GiveIcon
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCVendorRoleOverrideComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCVendorRoleOverrideComponent.RMCVendorRoleOverrideComponent_AutoState current))
        return;
      component.GiveSquadRoleName = current.GiveSquadRoleName;
      component.IsAppendSquadRoleName = current.IsAppendSquadRoleName;
      component.GiveIcon = current.GiveIcon;
    }
  }
}
