// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgBipodComponent
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
namespace Content.Shared._PUBG.Loadout;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (PubgBipodSystem), typeof (PubgWeaponModulesSystem)})]
public sealed class PubgBipodComponent : 
  Component,
  ISerializationGenerated<PubgBipodComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Deployed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DeployTime = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DeployedSpreadMultiplier = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DeployedRecoilMultiplier = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DeployedHipfireSpreadMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? DeploySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_activate.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? UndeploySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_deactivate.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgBipodComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgBipodComponent) target1;
    if (serialization.TryCustomCopy<PubgBipodComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deployed, ref target2, hookCtx, false, context))
      target2 = this.Deployed;
    target.Deployed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeployTime, ref target3, hookCtx, false, context))
      target3 = this.DeployTime;
    target.DeployTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeployedSpreadMultiplier, ref target4, hookCtx, false, context))
      target4 = this.DeployedSpreadMultiplier;
    target.DeployedSpreadMultiplier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeployedRecoilMultiplier, ref target5, hookCtx, false, context))
      target5 = this.DeployedRecoilMultiplier;
    target.DeployedRecoilMultiplier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeployedHipfireSpreadMultiplier, ref target6, hookCtx, false, context))
      target6 = this.DeployedHipfireSpreadMultiplier;
    target.DeployedHipfireSpreadMultiplier = target6;
    SoundSpecifier target7 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DeploySound, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<SoundSpecifier>(this.DeploySound, hookCtx, context);
    target.DeploySound = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UndeploySound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.UndeploySound, hookCtx, context);
    target.UndeploySound = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgBipodComponent target,
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
    PubgBipodComponent target1 = (PubgBipodComponent) target;
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
    PubgBipodComponent target1 = (PubgBipodComponent) target;
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
    PubgBipodComponent target1 = (PubgBipodComponent) target;
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
  virtual PubgBipodComponent Component.Instantiate() => new PubgBipodComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgBipodComponent_AutoState : IComponentState
  {
    public bool Deployed;
    public float DeployTime;
    public float DeployedSpreadMultiplier;
    public float DeployedRecoilMultiplier;
    public float DeployedHipfireSpreadMultiplier;
    public SoundSpecifier? DeploySound;
    public SoundSpecifier? UndeploySound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgBipodComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgBipodComponent, ComponentGetState>(new ComponentEventRefHandler<PubgBipodComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgBipodComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgBipodComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgBipodComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgBipodComponent.PubgBipodComponent_AutoState()
      {
        Deployed = component.Deployed,
        DeployTime = component.DeployTime,
        DeployedSpreadMultiplier = component.DeployedSpreadMultiplier,
        DeployedRecoilMultiplier = component.DeployedRecoilMultiplier,
        DeployedHipfireSpreadMultiplier = component.DeployedHipfireSpreadMultiplier,
        DeploySound = component.DeploySound,
        UndeploySound = component.UndeploySound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgBipodComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgBipodComponent.PubgBipodComponent_AutoState current))
        return;
      component.Deployed = current.Deployed;
      component.DeployTime = current.DeployTime;
      component.DeployedSpreadMultiplier = current.DeployedSpreadMultiplier;
      component.DeployedRecoilMultiplier = current.DeployedRecoilMultiplier;
      component.DeployedHipfireSpreadMultiplier = current.DeployedHipfireSpreadMultiplier;
      component.DeploySound = current.DeploySound;
      component.UndeploySound = current.UndeploySound;
    }
  }
}
