// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.SecretStashComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Item;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Storage.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SecretStashSystem)})]
public sealed class SecretStashComponent : 
  Component,
  ISerializationGenerated<SecretStashComponent>,
  ISerializationGenerated
{
  [DataField("maxItemSize", false, 1, false, false, null)]
  public ProtoId<ItemSizePrototype> MaxItemSize = (ProtoId<ItemSizePrototype>) "Small";
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? TryInsertItemSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? TryRemoveItemSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasVerbs = true;
  [DataField(null, false, 1, false, false, null)]
  public string? SecretStashName;
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? DamageEatenItemInside;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ContainerSlot ItemContainer;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SecretStashComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SecretStashComponent) target1;
    if (serialization.TryCustomCopy<SecretStashComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ItemSizePrototype> target2 = new ProtoId<ItemSizePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>>(this.MaxItemSize, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ItemSizePrototype>>(this.MaxItemSize, hookCtx, context);
    target.MaxItemSize = target2;
    EntityWhitelist target3 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, false, context))
    {
      if (this.Blacklist == null)
        target3 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Blacklist, ref target3, hookCtx, context);
    }
    target.Blacklist = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TryInsertItemSound, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.TryInsertItemSound, hookCtx, context);
    target.TryInsertItemSound = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.TryRemoveItemSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.TryRemoveItemSound, hookCtx, context);
    target.TryRemoveItemSound = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasVerbs, ref target6, hookCtx, false, context))
      target6 = this.HasVerbs;
    target.HasVerbs = target6;
    string target7 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SecretStashName, ref target7, hookCtx, false, context))
      target7 = this.SecretStashName;
    target.SecretStashName = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.DamageEatenItemInside, ref target8, hookCtx, false, context))
    {
      if (this.DamageEatenItemInside == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.DamageEatenItemInside, ref target8, hookCtx, context);
    }
    target.DamageEatenItemInside = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SecretStashComponent target,
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
    SecretStashComponent target1 = (SecretStashComponent) target;
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
    SecretStashComponent target1 = (SecretStashComponent) target;
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
    SecretStashComponent target1 = (SecretStashComponent) target;
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
  virtual SecretStashComponent Component.Instantiate() => new SecretStashComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SecretStashComponent_AutoState : IComponentState
  {
    public bool HasVerbs;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SecretStashComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SecretStashComponent, ComponentGetState>(new ComponentEventRefHandler<SecretStashComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SecretStashComponent, ComponentHandleState>(new ComponentEventRefHandler<SecretStashComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SecretStashComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SecretStashComponent.SecretStashComponent_AutoState()
      {
        HasVerbs = component.HasVerbs
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SecretStashComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SecretStashComponent.SecretStashComponent_AutoState current))
        return;
      component.HasVerbs = current.HasVerbs;
    }
  }
}
