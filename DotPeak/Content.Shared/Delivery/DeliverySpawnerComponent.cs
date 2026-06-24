// Decompiled with JetBrains decompiler
// Type: Content.Shared.Delivery.DeliverySpawnerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
namespace Content.Shared.Delivery;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class DeliverySpawnerComponent : 
  Component,
  ISerializationGenerated<DeliverySpawnerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntityTableSelector Table;
  [DataField(null, false, 1, false, false, null)]
  public int MaxContainedDeliveryAmount = 20;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ContainedDeliveryAmount;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SpawnSound = (SoundSpecifier) new SoundCollectionSpecifier("DeliverySpawnSounds", new AudioParams?(((AudioParams) ref AudioParams.Default).WithVolume(-7f)));
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? OpenSound = (SoundSpecifier) new SoundCollectionSpecifier("storageRustle", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeliverySpawnerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeliverySpawnerComponent) component;
    if (serialization.TryCustomCopy<DeliverySpawnerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityTableSelector entityTableSelector = (EntityTableSelector) null;
    if (this.Table == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntityTableSelector>(this.Table, ref entityTableSelector, hookCtx, true, context))
      entityTableSelector = serialization.CreateCopy<EntityTableSelector>(this.Table, hookCtx, context, false);
    target.Table = entityTableSelector;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxContainedDeliveryAmount, ref num1, hookCtx, false, context))
      num1 = this.MaxContainedDeliveryAmount;
    target.MaxContainedDeliveryAmount = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.ContainedDeliveryAmount, ref num2, hookCtx, false, context))
      num2 = this.ContainedDeliveryAmount;
    target.ContainedDeliveryAmount = num2;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpawnSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.SpawnSound, hookCtx, context, false);
    target.SpawnSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context, false);
    target.OpenSound = soundSpecifier2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeliverySpawnerComponent target,
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
    DeliverySpawnerComponent target1 = (DeliverySpawnerComponent) target;
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
    DeliverySpawnerComponent target1 = (DeliverySpawnerComponent) target;
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
    DeliverySpawnerComponent target1 = (DeliverySpawnerComponent) target;
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
  virtual DeliverySpawnerComponent Component.Instantiate() => new DeliverySpawnerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeliverySpawnerComponent_AutoState : IComponentState
  {
    public int ContainedDeliveryAmount;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeliverySpawnerComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliverySpawnerComponent, ComponentGetState>(new ComponentEventRefHandler<DeliverySpawnerComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliverySpawnerComponent, ComponentHandleState>(new ComponentEventRefHandler<DeliverySpawnerComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      DeliverySpawnerComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DeliverySpawnerComponent.DeliverySpawnerComponent_AutoState()
      {
        ContainedDeliveryAmount = component.ContainedDeliveryAmount
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeliverySpawnerComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DeliverySpawnerComponent.DeliverySpawnerComponent_AutoState current))
        return;
      component.ContainedDeliveryAmount = current.ContainedDeliveryAmount;
    }
  }
}
