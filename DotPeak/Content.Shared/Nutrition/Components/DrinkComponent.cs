// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.DrinkComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.EntitySystems;
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
namespace Content.Shared.Nutrition.Components;

[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[RegisterComponent]
[Access(new Type[] {typeof (SharedDrinkSystem)})]
public sealed class DrinkComponent : 
  Component,
  ISerializationGenerated<DrinkComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "drink";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier UseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/drink.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TransferAmount = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Delay = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Examinable = true;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreEmpty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ForceFeedDelay = 3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DrinkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DrinkComponent) target1;
    if (serialization.TryCustomCopy<DrinkComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target2, hookCtx, false, context))
      target2 = this.Solution;
    target.Solution = target2;
    SoundSpecifier target3 = (SoundSpecifier) null;
    if (this.UseSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UseSound, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SoundSpecifier>(this.UseSound, hookCtx, context);
    target.UseSound = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context);
    target.TransferAmount = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Delay, ref target5, hookCtx, false, context))
      target5 = this.Delay;
    target.Delay = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Examinable, ref target6, hookCtx, false, context))
      target6 = this.Examinable;
    target.Examinable = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreEmpty, ref target7, hookCtx, false, context))
      target7 = this.IgnoreEmpty;
    target.IgnoreEmpty = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ForceFeedDelay, ref target8, hookCtx, false, context))
      target8 = this.ForceFeedDelay;
    target.ForceFeedDelay = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DrinkComponent target,
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
    DrinkComponent target1 = (DrinkComponent) target;
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
    DrinkComponent target1 = (DrinkComponent) target;
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
    DrinkComponent target1 = (DrinkComponent) target;
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
  virtual DrinkComponent Component.Instantiate() => new DrinkComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DrinkComponent_AutoState : IComponentState
  {
    public SoundSpecifier UseSound;
    public FixedPoint2 TransferAmount;
    public float Delay;
    public bool Examinable;
    public float ForceFeedDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DrinkComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DrinkComponent, ComponentGetState>(new ComponentEventRefHandler<DrinkComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DrinkComponent, ComponentHandleState>(new ComponentEventRefHandler<DrinkComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, DrinkComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DrinkComponent.DrinkComponent_AutoState()
      {
        UseSound = component.UseSound,
        TransferAmount = component.TransferAmount,
        Delay = component.Delay,
        Examinable = component.Examinable,
        ForceFeedDelay = component.ForceFeedDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DrinkComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DrinkComponent.DrinkComponent_AutoState current))
        return;
      component.UseSound = current.UseSound;
      component.TransferAmount = current.TransferAmount;
      component.Delay = current.Delay;
      component.Examinable = current.Examinable;
      component.ForceFeedDelay = current.ForceFeedDelay;
    }
  }
}
