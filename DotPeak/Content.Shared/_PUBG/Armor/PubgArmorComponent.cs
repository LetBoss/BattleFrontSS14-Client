// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Armor.PubgArmorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Armor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class PubgArmorComponent : 
  Component,
  ISerializationGenerated<PubgArmorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Protection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxDurability = 100f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Durability;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ArmorSplit = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DurabilityDamageScale = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? BreakSound;
  [DataField(null, false, 1, false, false, null)]
  public float BreakSoundVolume = 4f;
  [DataField(null, false, 1, false, false, null)]
  public string? BreakPopup;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgArmorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgArmorComponent) target1;
    if (serialization.TryCustomCopy<PubgArmorComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Protection, ref target2, hookCtx, false, context))
      target2 = this.Protection;
    target.Protection = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxDurability, ref target3, hookCtx, false, context))
      target3 = this.MaxDurability;
    target.MaxDurability = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Durability, ref target4, hookCtx, false, context))
      target4 = this.Durability;
    target.Durability = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ArmorSplit, ref target5, hookCtx, false, context))
      target5 = this.ArmorSplit;
    target.ArmorSplit = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DurabilityDamageScale, ref target6, hookCtx, false, context))
      target6 = this.DurabilityDamageScale;
    target.DurabilityDamageScale = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.BreakSound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.BreakSound, hookCtx, context);
    target.BreakSound = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakSoundVolume, ref target8, hookCtx, false, context))
      target8 = this.BreakSoundVolume;
    target.BreakSoundVolume = target8;
    string target9 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BreakPopup, ref target9, hookCtx, false, context))
      target9 = this.BreakPopup;
    target.BreakPopup = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgArmorComponent target,
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
    PubgArmorComponent target1 = (PubgArmorComponent) target;
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
    PubgArmorComponent target1 = (PubgArmorComponent) target;
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
    PubgArmorComponent target1 = (PubgArmorComponent) target;
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
  virtual PubgArmorComponent Component.Instantiate() => new PubgArmorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgArmorComponent_AutoState : IComponentState
  {
    public float Protection;
    public float MaxDurability;
    public float Durability;
    public float ArmorSplit;
    public float DurabilityDamageScale;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgArmorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgArmorComponent, ComponentGetState>(new ComponentEventRefHandler<PubgArmorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgArmorComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgArmorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgArmorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgArmorComponent.PubgArmorComponent_AutoState()
      {
        Protection = component.Protection,
        MaxDurability = component.MaxDurability,
        Durability = component.Durability,
        ArmorSplit = component.ArmorSplit,
        DurabilityDamageScale = component.DurabilityDamageScale
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgArmorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgArmorComponent.PubgArmorComponent_AutoState current))
        return;
      component.Protection = current.Protection;
      component.MaxDurability = current.MaxDurability;
      component.Durability = current.Durability;
      component.ArmorSplit = current.ArmorSplit;
      component.DurabilityDamageScale = current.DurabilityDamageScale;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, PubgArmorComponent>(uid, component, ref args1);
    }
  }
}
