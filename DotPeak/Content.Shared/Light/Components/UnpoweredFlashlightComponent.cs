// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.Components.UnpoweredFlashlightComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Decals;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class UnpoweredFlashlightComponent : 
  Component,
  ISerializationGenerated<UnpoweredFlashlightComponent>,
  ISerializationGenerated
{
  [DataField("toggleFlashlightSound", false, 1, false, false, null)]
  public SoundSpecifier ToggleSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/flashlight_pda.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LightOn;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleAction = (EntProtoId) "ActionToggleLight";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ProtoId<ColorPalettePrototype> EmaggedColorsPrototype = (ProtoId<ColorPalettePrototype>) "Emagged";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UnpoweredFlashlightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UnpoweredFlashlightComponent) target1;
    if (serialization.TryCustomCopy<UnpoweredFlashlightComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier target2 = (SoundSpecifier) null;
    if (this.ToggleSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ToggleSound, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<SoundSpecifier>(this.ToggleSound, hookCtx, context);
    target.ToggleSound = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.LightOn, ref target3, hookCtx, false, context))
      target3 = this.LightOn;
    target.LightOn = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleAction, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.ToggleAction, hookCtx, context);
    target.ToggleAction = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleActionEntity, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.ToggleActionEntity, hookCtx, context);
    target.ToggleActionEntity = target5;
    ProtoId<ColorPalettePrototype> target6 = new ProtoId<ColorPalettePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ColorPalettePrototype>>(this.EmaggedColorsPrototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<ColorPalettePrototype>>(this.EmaggedColorsPrototype, hookCtx, context);
    target.EmaggedColorsPrototype = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UnpoweredFlashlightComponent target,
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
    UnpoweredFlashlightComponent target1 = (UnpoweredFlashlightComponent) target;
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
    UnpoweredFlashlightComponent target1 = (UnpoweredFlashlightComponent) target;
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
    UnpoweredFlashlightComponent target1 = (UnpoweredFlashlightComponent) target;
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
  virtual UnpoweredFlashlightComponent Component.Instantiate()
  {
    return new UnpoweredFlashlightComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UnpoweredFlashlightComponent_AutoState : IComponentState
  {
    public bool LightOn;
    public NetEntity? ToggleActionEntity;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UnpoweredFlashlightComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UnpoweredFlashlightComponent, ComponentGetState>(new ComponentEventRefHandler<UnpoweredFlashlightComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UnpoweredFlashlightComponent, ComponentHandleState>(new ComponentEventRefHandler<UnpoweredFlashlightComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UnpoweredFlashlightComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UnpoweredFlashlightComponent.UnpoweredFlashlightComponent_AutoState()
      {
        LightOn = component.LightOn,
        ToggleActionEntity = this.GetNetEntity(component.ToggleActionEntity)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UnpoweredFlashlightComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UnpoweredFlashlightComponent.UnpoweredFlashlightComponent_AutoState current))
        return;
      component.LightOn = current.LightOn;
      component.ToggleActionEntity = this.EnsureEntity<UnpoweredFlashlightComponent>(current.ToggleActionEntity, uid);
    }
  }
}
