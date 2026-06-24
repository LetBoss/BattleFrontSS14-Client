// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.RMCAllowSuitStorageUserWhitelistComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Armor;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Armor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMArmorSystem)})]
public sealed class RMCAllowSuitStorageUserWhitelistComponent : 
  Component,
  ISerializationGenerated<RMCAllowSuitStorageUserWhitelistComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist DefaultWhitelist = new EntityWhitelist();
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId<AllowSuitStorageComponent> AllowedWhitelist;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? User;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCAllowSuitStorageUserWhitelistComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCAllowSuitStorageUserWhitelistComponent) target1;
    if (serialization.TryCustomCopy<RMCAllowSuitStorageUserWhitelistComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist target2 = (EntityWhitelist) null;
    if (this.DefaultWhitelist == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.DefaultWhitelist, ref target2, hookCtx, false, context))
    {
      if (this.DefaultWhitelist == null)
        target2 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.DefaultWhitelist, ref target2, hookCtx, context, true);
    }
    target.DefaultWhitelist = target2;
    EntProtoId<AllowSuitStorageComponent> target3 = new EntProtoId<AllowSuitStorageComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<AllowSuitStorageComponent>>(this.AllowedWhitelist, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId<AllowSuitStorageComponent>>(this.AllowedWhitelist, hookCtx, context);
    target.AllowedWhitelist = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.User, ref target4, hookCtx, false, context))
    {
      if (this.User == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.User, ref target4, hookCtx, context);
    }
    target.User = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCAllowSuitStorageUserWhitelistComponent target,
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
    RMCAllowSuitStorageUserWhitelistComponent target1 = (RMCAllowSuitStorageUserWhitelistComponent) target;
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
    RMCAllowSuitStorageUserWhitelistComponent target1 = (RMCAllowSuitStorageUserWhitelistComponent) target;
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
    RMCAllowSuitStorageUserWhitelistComponent target1 = (RMCAllowSuitStorageUserWhitelistComponent) target;
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
  virtual RMCAllowSuitStorageUserWhitelistComponent Component.Instantiate()
  {
    return new RMCAllowSuitStorageUserWhitelistComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCAllowSuitStorageUserWhitelistComponent_AutoState : IComponentState
  {
    public EntityWhitelist DefaultWhitelist;
    public EntProtoId<AllowSuitStorageComponent> AllowedWhitelist;
    public EntityWhitelist? User;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCAllowSuitStorageUserWhitelistComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCAllowSuitStorageUserWhitelistComponent, ComponentGetState>(new ComponentEventRefHandler<RMCAllowSuitStorageUserWhitelistComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCAllowSuitStorageUserWhitelistComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCAllowSuitStorageUserWhitelistComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCAllowSuitStorageUserWhitelistComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCAllowSuitStorageUserWhitelistComponent.RMCAllowSuitStorageUserWhitelistComponent_AutoState()
      {
        DefaultWhitelist = component.DefaultWhitelist,
        AllowedWhitelist = component.AllowedWhitelist,
        User = component.User
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCAllowSuitStorageUserWhitelistComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCAllowSuitStorageUserWhitelistComponent.RMCAllowSuitStorageUserWhitelistComponent_AutoState current))
        return;
      component.DefaultWhitelist = current.DefaultWhitelist;
      component.AllowedWhitelist = current.AllowedWhitelist;
      component.User = current.User;
    }
  }
}
