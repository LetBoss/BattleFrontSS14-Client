// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Explosion.CMVocalizeTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Content.Shared.Popups;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Explosion;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCExplosionSystem)})]
public sealed class CMVocalizeTriggerComponent : 
  Component,
  ISerializationGenerated<CMVocalizeTriggerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId UserPopup = (LocId) "cm-grenade-primed-user";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId OthersPopup = (LocId) "cm-grenade-primed-others";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public PopupType PopupType = PopupType.MediumCaution;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Sex, SoundSpecifier> Sounds = new Dictionary<Sex, SoundSpecifier>()
  {
    [Sex.Female] = (SoundSpecifier) new SoundCollectionSpecifier("PubgSoundCollectionGrenadeThrow"),
    [Sex.Male] = (SoundSpecifier) new SoundCollectionSpecifier("PubgSoundCollectionGrenadeThrow"),
    [Sex.Unsexed] = (SoundSpecifier) new SoundCollectionSpecifier("PubgSoundCollectionGrenadeThrow")
  };
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId Effect = (EntProtoId) "RMCActiveAlertEffect";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CMVocalizeTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CMVocalizeTriggerComponent) target1;
    if (serialization.TryCustomCopy<CMVocalizeTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    LocId target2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.UserPopup, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<LocId>(this.UserPopup, hookCtx, context);
    target.UserPopup = target2;
    LocId target3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.OthersPopup, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<LocId>(this.OthersPopup, hookCtx, context);
    target.OthersPopup = target3;
    PopupType target4 = PopupType.Small;
    if (!serialization.TryCustomCopy<PopupType>(this.PopupType, ref target4, hookCtx, false, context))
      target4 = this.PopupType;
    target.PopupType = target4;
    Dictionary<Sex, SoundSpecifier> target5 = (Dictionary<Sex, SoundSpecifier>) null;
    if (this.Sounds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Sex, SoundSpecifier>>(this.Sounds, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<Dictionary<Sex, SoundSpecifier>>(this.Sounds, hookCtx, context);
    target.Sounds = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Effect, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.Effect, hookCtx, context);
    target.Effect = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CMVocalizeTriggerComponent target,
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
    CMVocalizeTriggerComponent target1 = (CMVocalizeTriggerComponent) target;
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
    CMVocalizeTriggerComponent target1 = (CMVocalizeTriggerComponent) target;
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
    CMVocalizeTriggerComponent target1 = (CMVocalizeTriggerComponent) target;
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
  virtual CMVocalizeTriggerComponent Component.Instantiate() => new CMVocalizeTriggerComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CMVocalizeTriggerComponent_AutoState : IComponentState
  {
    public LocId UserPopup;
    public LocId OthersPopup;
    public PopupType PopupType;
    public Dictionary<Sex, SoundSpecifier> Sounds;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CMVocalizeTriggerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CMVocalizeTriggerComponent, ComponentGetState>(new ComponentEventRefHandler<CMVocalizeTriggerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CMVocalizeTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<CMVocalizeTriggerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CMVocalizeTriggerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CMVocalizeTriggerComponent.CMVocalizeTriggerComponent_AutoState()
      {
        UserPopup = component.UserPopup,
        OthersPopup = component.OthersPopup,
        PopupType = component.PopupType,
        Sounds = component.Sounds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CMVocalizeTriggerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CMVocalizeTriggerComponent.CMVocalizeTriggerComponent_AutoState current))
        return;
      component.UserPopup = current.UserPopup;
      component.OthersPopup = current.OthersPopup;
      component.PopupType = current.PopupType;
      component.Sounds = current.Sounds == null ? (Dictionary<Sex, SoundSpecifier>) null : new Dictionary<Sex, SoundSpecifier>((IDictionary<Sex, SoundSpecifier>) current.Sounds);
    }
  }
}
