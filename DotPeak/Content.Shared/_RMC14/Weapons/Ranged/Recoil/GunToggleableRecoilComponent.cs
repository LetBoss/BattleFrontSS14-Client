// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Recoil.GunToggleableRecoilComponent
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
namespace Content.Shared._RMC14.Weapons.Ranged.Recoil;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (GunToggleableRecoilSystem)})]
public sealed class GunToggleableRecoilComponent : 
  Component,
  ISerializationGenerated<GunToggleableRecoilComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BatteryDrain = 1.25f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId ActionId = (EntProtoId) "RMCActionToggleRecoil";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Action;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/click.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunToggleableRecoilComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunToggleableRecoilComponent) target1;
    if (serialization.TryCustomCopy<GunToggleableRecoilComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target2, hookCtx, false, context))
      target2 = this.Active;
    target.Active = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BatteryDrain, ref target3, hookCtx, false, context))
      target3 = this.BatteryDrain;
    target.BatteryDrain = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ActionId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ActionId, hookCtx, context);
    target.ActionId = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Action, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Action, hookCtx, context);
    target.Action = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunToggleableRecoilComponent target,
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
    GunToggleableRecoilComponent target1 = (GunToggleableRecoilComponent) target;
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
    GunToggleableRecoilComponent target1 = (GunToggleableRecoilComponent) target;
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
    GunToggleableRecoilComponent target1 = (GunToggleableRecoilComponent) target;
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
  virtual GunToggleableRecoilComponent Component.Instantiate()
  {
    return new GunToggleableRecoilComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunToggleableRecoilComponent_AutoState : IComponentState
  {
    public bool Active;
    public float BatteryDrain;
    public EntProtoId ActionId;
    public NetEntity? Action;
    public SoundSpecifier ToggleSound;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunToggleableRecoilComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunToggleableRecoilComponent, ComponentGetState>(new ComponentEventRefHandler<GunToggleableRecoilComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunToggleableRecoilComponent, ComponentHandleState>(new ComponentEventRefHandler<GunToggleableRecoilComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunToggleableRecoilComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunToggleableRecoilComponent.GunToggleableRecoilComponent_AutoState()
      {
        Active = component.Active,
        BatteryDrain = component.BatteryDrain,
        ActionId = component.ActionId,
        Action = this.GetNetEntity(component.Action),
        ToggleSound = component.ToggleSound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunToggleableRecoilComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunToggleableRecoilComponent.GunToggleableRecoilComponent_AutoState current))
        return;
      component.Active = current.Active;
      component.BatteryDrain = current.BatteryDrain;
      component.ActionId = current.ActionId;
      component.Action = this.EnsureEntity<GunToggleableRecoilComponent>(current.Action, uid);
      component.ToggleSound = current.ToggleSound;
    }
  }
}
