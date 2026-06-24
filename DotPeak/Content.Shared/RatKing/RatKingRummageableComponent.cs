// Decompiled with JetBrains decompiler
// Type: Content.Shared.RatKing.RatKingRummageableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Random;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.RatKing;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedRatKingSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class RatKingRummageableComponent : 
  Component,
  ISerializationGenerated<RatKingRummageableComponent>,
  ISerializationGenerated
{
  [DataField("looted", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool Looted;
  [DataField("rummageDuration", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float RummageDuration = 3f;
  [DataField("rummageLoot", false, 1, false, false, typeof (PrototypeIdSerializer<WeightedRandomEntityPrototype>))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public string RummageLoot = "RatKingLoot";
  [DataField("sound", false, 1, false, false, null)]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundCollectionSpecifier("storageRustle");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RatKingRummageableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RatKingRummageableComponent) target1;
    if (serialization.TryCustomCopy<RatKingRummageableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Looted, ref target2, hookCtx, false, context))
      target2 = this.Looted;
    target.Looted = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RummageDuration, ref target3, hookCtx, false, context))
      target3 = this.RummageDuration;
    target.RummageDuration = target3;
    string target4 = (string) null;
    if (this.RummageLoot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RummageLoot, ref target4, hookCtx, false, context))
      target4 = this.RummageLoot;
    target.RummageLoot = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RatKingRummageableComponent target,
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
    RatKingRummageableComponent target1 = (RatKingRummageableComponent) target;
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
    RatKingRummageableComponent target1 = (RatKingRummageableComponent) target;
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
    RatKingRummageableComponent target1 = (RatKingRummageableComponent) target;
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
  virtual RatKingRummageableComponent Component.Instantiate() => new RatKingRummageableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RatKingRummageableComponent_AutoState : IComponentState
  {
    public bool Looted;
    public float RummageDuration;
    public string RummageLoot;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RatKingRummageableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RatKingRummageableComponent, ComponentGetState>(new ComponentEventRefHandler<RatKingRummageableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RatKingRummageableComponent, ComponentHandleState>(new ComponentEventRefHandler<RatKingRummageableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RatKingRummageableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RatKingRummageableComponent.RatKingRummageableComponent_AutoState()
      {
        Looted = component.Looted,
        RummageDuration = component.RummageDuration,
        RummageLoot = component.RummageLoot
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RatKingRummageableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RatKingRummageableComponent.RatKingRummageableComponent_AutoState current))
        return;
      component.Looted = current.Looted;
      component.RummageDuration = current.RummageDuration;
      component.RummageLoot = current.RummageLoot;
    }
  }
}
