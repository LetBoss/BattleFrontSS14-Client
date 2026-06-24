// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Clothing.ClothingRequireEquippedComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Clothing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCClothingSystem)})]
public sealed class ClothingRequireEquippedComponent : 
  Component,
  ISerializationGenerated<ClothingRequireEquippedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DenyReason = "rmc-wear-smart-gun-required";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoUnequip;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClothingRequireEquippedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ClothingRequireEquippedComponent) target1;
    if (serialization.TryCustomCopy<ClothingRequireEquippedComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target2, hookCtx, context);
    }
    target.Whitelist = target2;
    string target3 = (string) null;
    if (this.DenyReason == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DenyReason, ref target3, hookCtx, false, context))
      target3 = this.DenyReason;
    target.DenyReason = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoUnequip, ref target4, hookCtx, false, context))
      target4 = this.AutoUnequip;
    target.AutoUnequip = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClothingRequireEquippedComponent target,
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
    ClothingRequireEquippedComponent target1 = (ClothingRequireEquippedComponent) target;
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
    ClothingRequireEquippedComponent target1 = (ClothingRequireEquippedComponent) target;
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
    ClothingRequireEquippedComponent target1 = (ClothingRequireEquippedComponent) target;
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
  virtual ClothingRequireEquippedComponent Component.Instantiate()
  {
    return new ClothingRequireEquippedComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ClothingRequireEquippedComponent_AutoState : IComponentState
  {
    public EntityWhitelist? Whitelist;
    public string DenyReason;
    public bool AutoUnequip;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ClothingRequireEquippedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ClothingRequireEquippedComponent, ComponentGetState>(new ComponentEventRefHandler<ClothingRequireEquippedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ClothingRequireEquippedComponent, ComponentHandleState>(new ComponentEventRefHandler<ClothingRequireEquippedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ClothingRequireEquippedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ClothingRequireEquippedComponent.ClothingRequireEquippedComponent_AutoState()
      {
        Whitelist = component.Whitelist,
        DenyReason = component.DenyReason,
        AutoUnequip = component.AutoUnequip
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ClothingRequireEquippedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ClothingRequireEquippedComponent.ClothingRequireEquippedComponent_AutoState current))
        return;
      component.Whitelist = current.Whitelist;
      component.DenyReason = current.DenyReason;
      component.AutoUnequip = current.AutoUnequip;
    }
  }
}
