// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.PilotedClothingComponent
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
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PilotedClothingComponent : 
  Component,
  ISerializationGenerated<PilotedClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? PilotWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public bool RelayMovement = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Pilot;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Wearer;

  public bool IsActive => this.Pilot.HasValue && this.Wearer.HasValue;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PilotedClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PilotedClothingComponent) component;
    if (serialization.TryCustomCopy<PilotedClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist entityWhitelist = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.PilotWhitelist, ref entityWhitelist, hookCtx, false, context))
    {
      if (this.PilotWhitelist == null)
        entityWhitelist = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.PilotWhitelist, ref entityWhitelist, hookCtx, context, false);
    }
    target.PilotWhitelist = entityWhitelist;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.RelayMovement, ref flag, hookCtx, false, context))
      flag = this.RelayMovement;
    target.RelayMovement = flag;
    EntityUid? nullable1 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Pilot, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntityUid?>(this.Pilot, hookCtx, context, false);
    target.Pilot = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Wearer, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.Wearer, hookCtx, context, false);
    target.Wearer = nullable2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PilotedClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PilotedClothingComponent target1 = (PilotedClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PilotedClothingComponent target1 = (PilotedClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PilotedClothingComponent target1 = (PilotedClothingComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PilotedClothingComponent Component.Instantiate() => new PilotedClothingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PilotedClothingComponent_AutoState : IComponentState
  {
    public NetEntity? Pilot;
    public NetEntity? Wearer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PilotedClothingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<PilotedClothingComponent, ComponentGetState>(new ComponentEventRefHandler<PilotedClothingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<PilotedClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<PilotedClothingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      PilotedClothingComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new PilotedClothingComponent.PilotedClothingComponent_AutoState()
      {
        Pilot = this.GetNetEntity(component.Pilot, (MetaDataComponent) null),
        Wearer = this.GetNetEntity(component.Wearer, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PilotedClothingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is PilotedClothingComponent.PilotedClothingComponent_AutoState current))
        return;
      component.Pilot = this.EnsureEntity<PilotedClothingComponent>(current.Pilot, uid);
      component.Wearer = this.EnsureEntity<PilotedClothingComponent>(current.Wearer, uid);
    }
  }
}
