// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Medicine.PubgEnergyDrinkComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Medicine;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgEnergyDrinkComponent : 
  Component,
  ISerializationGenerated<PubgEnergyDrinkComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float EnergyAmount = 30f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TemporaryHealthAmount = 50f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float UseDelay = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier DrinkSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/drink.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? EmptyPrototype;
  [DataField("breakOnMove", false, 1, false, false, null)]
  public bool BreakOnMove = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgEnergyDrinkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgEnergyDrinkComponent) target1;
    if (serialization.TryCustomCopy<PubgEnergyDrinkComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyAmount, ref target2, hookCtx, false, context))
      target2 = this.EnergyAmount;
    target.EnergyAmount = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TemporaryHealthAmount, ref target3, hookCtx, false, context))
      target3 = this.TemporaryHealthAmount;
    target.TemporaryHealthAmount = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UseDelay, ref target4, hookCtx, false, context))
      target4 = this.UseDelay;
    target.UseDelay = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.DrinkSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DrinkSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.DrinkSound, hookCtx, context);
    target.DrinkSound = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.EmptyPrototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.EmptyPrototype, hookCtx, context);
    target.EmptyPrototype = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnMove, ref target7, hookCtx, false, context))
      target7 = this.BreakOnMove;
    target.BreakOnMove = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgEnergyDrinkComponent target,
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
    PubgEnergyDrinkComponent target1 = (PubgEnergyDrinkComponent) target;
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
    PubgEnergyDrinkComponent target1 = (PubgEnergyDrinkComponent) target;
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
    PubgEnergyDrinkComponent target1 = (PubgEnergyDrinkComponent) target;
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
  virtual PubgEnergyDrinkComponent Component.Instantiate() => new PubgEnergyDrinkComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgEnergyDrinkComponent_AutoState : IComponentState
  {
    public float EnergyAmount;
    public float TemporaryHealthAmount;
    public float UseDelay;
    public SoundSpecifier DrinkSound;
    public EntProtoId? EmptyPrototype;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgEnergyDrinkComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgEnergyDrinkComponent, ComponentGetState>(new ComponentEventRefHandler<PubgEnergyDrinkComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgEnergyDrinkComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgEnergyDrinkComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgEnergyDrinkComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgEnergyDrinkComponent.PubgEnergyDrinkComponent_AutoState()
      {
        EnergyAmount = component.EnergyAmount,
        TemporaryHealthAmount = component.TemporaryHealthAmount,
        UseDelay = component.UseDelay,
        DrinkSound = component.DrinkSound,
        EmptyPrototype = component.EmptyPrototype
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgEnergyDrinkComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgEnergyDrinkComponent.PubgEnergyDrinkComponent_AutoState current))
        return;
      component.EnergyAmount = current.EnergyAmount;
      component.TemporaryHealthAmount = current.TemporaryHealthAmount;
      component.UseDelay = current.UseDelay;
      component.DrinkSound = current.DrinkSound;
      component.EmptyPrototype = current.EmptyPrototype;
    }
  }
}
