// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.CartridgeAmmoComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
public sealed class CartridgeAmmoComponent : 
  AmmoComponent,
  ISerializationGenerated<CartridgeAmmoComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("proto", false, 1, true, false, null)]
  public EntProtoId Prototype;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Spent;
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteOnSpawn;
  [DataField("soundEject", false, 1, false, false, null)]
  public SoundSpecifier? EjectSound = (SoundSpecifier) new SoundCollectionSpecifier("CasingEject");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundInsert;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CartridgeAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AmmoComponent target1 = (AmmoComponent) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CartridgeAmmoComponent) target1;
    if (serialization.TryCustomCopy<CartridgeAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Spent, ref target3, hookCtx, false, context))
      target3 = this.Spent;
    target.Spent = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnSpawn, ref target4, hookCtx, false, context))
      target4 = this.DeleteOnSpawn;
    target.DeleteOnSpawn = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EjectSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.EjectSound, hookCtx, context);
    target.EjectSound = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundInsert, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.SoundInsert, hookCtx, context);
    target.SoundInsert = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CartridgeAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref AmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CartridgeAmmoComponent target1 = (CartridgeAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (AmmoComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CartridgeAmmoComponent target1 = (CartridgeAmmoComponent) target;
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
    CartridgeAmmoComponent target1 = (CartridgeAmmoComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CartridgeAmmoComponent target1 = (CartridgeAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CartridgeAmmoComponent AmmoComponent.Instantiate() => new CartridgeAmmoComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CartridgeAmmoComponent_AutoState : IComponentState
  {
    public bool Spent;

    public CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState ShallowClone()
    {
      return new CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState()
      {
        Spent = this.Spent
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CartridgeAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<CartridgeAmmoComponent>("Spent");
      this.SubscribeLocalEvent<CartridgeAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<CartridgeAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CartridgeAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<CartridgeAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CartridgeAmmoComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick && this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick) == 1U)
        args.State = (IComponentState) new CartridgeAmmoComponent.Spent_FieldComponentState()
        {
          Spent = component.Spent
        };
      else
        args.State = (IComponentState) new CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState()
        {
          Spent = component.Spent
        };
    }

    private void OnHandleState(
      EntityUid uid,
      CartridgeAmmoComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case CartridgeAmmoComponent.Spent_FieldComponentState fieldComponentState:
          component.Spent = fieldComponentState.Spent;
          break;
        case CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState componentAutoState:
          component.Spent = componentAutoState.Spent;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Spent_FieldComponentState : 
    IComponentDeltaState<CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Spent;

    public void ApplyToFullState(
      CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState fullState)
    {
      fullState.Spent = this.Spent;
    }

    public CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState CreateNewFullState(
      CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState fullState)
    {
      CartridgeAmmoComponent.CartridgeAmmoComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
