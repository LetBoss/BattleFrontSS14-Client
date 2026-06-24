// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.BasicEntityAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class BasicEntityAmmoProviderComponent : 
  AmmoProviderComponent,
  ISerializationGenerated<BasicEntityAmmoProviderComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("proto", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Proto;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("capacity", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? Capacity;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("count", false, 1, false, false, null)]
  [AutoNetworkedField]
  public int? Count;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BasicEntityAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AmmoProviderComponent target1 = (AmmoProviderComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BasicEntityAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<BasicEntityAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Proto == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Proto, ref target2, hookCtx, false, context))
      target2 = this.Proto;
    target.Proto = target2;
    int? target3 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Capacity, ref target3, hookCtx, false, context))
      target3 = this.Capacity;
    target.Capacity = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.Count, ref target4, hookCtx, false, context))
      target4 = this.Count;
    target.Count = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BasicEntityAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref AmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BasicEntityAmmoProviderComponent target1 = (BasicEntityAmmoProviderComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (AmmoProviderComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    BasicEntityAmmoProviderComponent target1 = (BasicEntityAmmoProviderComponent) target;
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
    BasicEntityAmmoProviderComponent target1 = (BasicEntityAmmoProviderComponent) target;
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
  virtual BasicEntityAmmoProviderComponent AmmoProviderComponent.Instantiate()
  {
    return new BasicEntityAmmoProviderComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BasicEntityAmmoProviderComponent_AutoState : IComponentState
  {
    public int? Capacity;
    public int? Count;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BasicEntityAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<BasicEntityAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<BasicEntityAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BasicEntityAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BasicEntityAmmoProviderComponent.BasicEntityAmmoProviderComponent_AutoState()
      {
        Capacity = component.Capacity,
        Count = component.Count
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BasicEntityAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BasicEntityAmmoProviderComponent.BasicEntityAmmoProviderComponent_AutoState current))
        return;
      component.Capacity = current.Capacity;
      component.Count = current.Count;
    }
  }
}
