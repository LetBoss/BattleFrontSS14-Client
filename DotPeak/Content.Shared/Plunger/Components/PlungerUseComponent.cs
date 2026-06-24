// Decompiled with JetBrains decompiler
// Type: Content.Shared.Plunger.Components.PlungerUseComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
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
namespace Content.Shared.Plunger.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PlungerUseComponent : 
  Component,
  ISerializationGenerated<PlungerUseComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Plunged;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NeedsPlunger;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<WeightedRandomEntityPrototype> PlungerLoot = (ProtoId<WeightedRandomEntityPrototype>) nameof (PlungerLoot);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/Fluids/glug.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlungerUseComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlungerUseComponent) target1;
    if (serialization.TryCustomCopy<PlungerUseComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Plunged, ref target2, hookCtx, false, context))
      target2 = this.Plunged;
    target.Plunged = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedsPlunger, ref target3, hookCtx, false, context))
      target3 = this.NeedsPlunger;
    target.NeedsPlunger = target3;
    ProtoId<WeightedRandomEntityPrototype> target4 = new ProtoId<WeightedRandomEntityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<WeightedRandomEntityPrototype>>(this.PlungerLoot, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<WeightedRandomEntityPrototype>>(this.PlungerLoot, hookCtx, context);
    target.PlungerLoot = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.Sound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlungerUseComponent target,
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
    PlungerUseComponent target1 = (PlungerUseComponent) target;
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
    PlungerUseComponent target1 = (PlungerUseComponent) target;
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
    PlungerUseComponent target1 = (PlungerUseComponent) target;
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
  virtual PlungerUseComponent Component.Instantiate() => new PlungerUseComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PlungerUseComponent_AutoState : IComponentState
  {
    public bool Plunged;
    public bool NeedsPlunger;
    public ProtoId<WeightedRandomEntityPrototype> PlungerLoot;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PlungerUseComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PlungerUseComponent, ComponentGetState>(new ComponentEventRefHandler<PlungerUseComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PlungerUseComponent, ComponentHandleState>(new ComponentEventRefHandler<PlungerUseComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PlungerUseComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PlungerUseComponent.PlungerUseComponent_AutoState()
      {
        Plunged = component.Plunged,
        NeedsPlunger = component.NeedsPlunger,
        PlungerLoot = component.PlungerLoot
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PlungerUseComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PlungerUseComponent.PlungerUseComponent_AutoState current))
        return;
      component.Plunged = current.Plunged;
      component.NeedsPlunger = current.NeedsPlunger;
      component.PlungerLoot = current.PlungerLoot;
    }
  }
}
