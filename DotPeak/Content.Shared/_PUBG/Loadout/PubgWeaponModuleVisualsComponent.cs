// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgWeaponModuleVisualsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
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
namespace Content.Shared._PUBG.Loadout;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgWeaponModuleVisualsComponent : 
  Component,
  ISerializationGenerated<PubgWeaponModuleVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OpticLayer = "scope";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string MuzzleLayer = "muzzle";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string MagazineLayer = "magazine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgWeaponModuleVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgWeaponModuleVisualsComponent) target1;
    if (serialization.TryCustomCopy<PubgWeaponModuleVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.OpticLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OpticLayer, ref target2, hookCtx, false, context))
      target2 = this.OpticLayer;
    target.OpticLayer = target2;
    string target3 = (string) null;
    if (this.MuzzleLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MuzzleLayer, ref target3, hookCtx, false, context))
      target3 = this.MuzzleLayer;
    target.MuzzleLayer = target3;
    string target4 = (string) null;
    if (this.MagazineLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MagazineLayer, ref target4, hookCtx, false, context))
      target4 = this.MagazineLayer;
    target.MagazineLayer = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgWeaponModuleVisualsComponent target,
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
    PubgWeaponModuleVisualsComponent target1 = (PubgWeaponModuleVisualsComponent) target;
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
    PubgWeaponModuleVisualsComponent target1 = (PubgWeaponModuleVisualsComponent) target;
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
    PubgWeaponModuleVisualsComponent target1 = (PubgWeaponModuleVisualsComponent) target;
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
  virtual PubgWeaponModuleVisualsComponent Component.Instantiate()
  {
    return new PubgWeaponModuleVisualsComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgWeaponModuleVisualsComponent_AutoState : IComponentState
  {
    public string OpticLayer;
    public string MuzzleLayer;
    public string MagazineLayer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgWeaponModuleVisualsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgWeaponModuleVisualsComponent, ComponentGetState>(new ComponentEventRefHandler<PubgWeaponModuleVisualsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgWeaponModuleVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgWeaponModuleVisualsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgWeaponModuleVisualsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgWeaponModuleVisualsComponent.PubgWeaponModuleVisualsComponent_AutoState()
      {
        OpticLayer = component.OpticLayer,
        MuzzleLayer = component.MuzzleLayer,
        MagazineLayer = component.MagazineLayer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgWeaponModuleVisualsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgWeaponModuleVisualsComponent.PubgWeaponModuleVisualsComponent_AutoState current))
        return;
      component.OpticLayer = current.OpticLayer;
      component.MuzzleLayer = current.MuzzleLayer;
      component.MagazineLayer = current.MagazineLayer;
    }
  }
}
