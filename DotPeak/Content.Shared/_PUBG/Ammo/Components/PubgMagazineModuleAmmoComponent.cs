// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Ammo.Components.PubgMagazineModuleAmmoComponent
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
namespace Content.Shared._PUBG.Ammo.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgMagazineModuleAmmoComponent : 
  Component,
  ISerializationGenerated<PubgMagazineModuleAmmoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CurrentAmmo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Capacity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AmmoTag = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AmmoStackPrototype = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgMagazineModuleAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgMagazineModuleAmmoComponent) target1;
    if (serialization.TryCustomCopy<PubgMagazineModuleAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentAmmo, ref target2, hookCtx, false, context))
      target2 = this.CurrentAmmo;
    target.CurrentAmmo = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Capacity, ref target3, hookCtx, false, context))
      target3 = this.Capacity;
    target.Capacity = target3;
    string target4 = (string) null;
    if (this.AmmoTag == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AmmoTag, ref target4, hookCtx, false, context))
      target4 = this.AmmoTag;
    target.AmmoTag = target4;
    string target5 = (string) null;
    if (this.AmmoStackPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AmmoStackPrototype, ref target5, hookCtx, false, context))
      target5 = this.AmmoStackPrototype;
    target.AmmoStackPrototype = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgMagazineModuleAmmoComponent target,
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
    PubgMagazineModuleAmmoComponent target1 = (PubgMagazineModuleAmmoComponent) target;
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
    PubgMagazineModuleAmmoComponent target1 = (PubgMagazineModuleAmmoComponent) target;
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
    PubgMagazineModuleAmmoComponent target1 = (PubgMagazineModuleAmmoComponent) target;
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
  virtual PubgMagazineModuleAmmoComponent Component.Instantiate()
  {
    return new PubgMagazineModuleAmmoComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgMagazineModuleAmmoComponent_AutoState : IComponentState
  {
    public int CurrentAmmo;
    public int Capacity;
    public string AmmoTag;
    public string AmmoStackPrototype;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgMagazineModuleAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgMagazineModuleAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<PubgMagazineModuleAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgMagazineModuleAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgMagazineModuleAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgMagazineModuleAmmoComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgMagazineModuleAmmoComponent.PubgMagazineModuleAmmoComponent_AutoState()
      {
        CurrentAmmo = component.CurrentAmmo,
        Capacity = component.Capacity,
        AmmoTag = component.AmmoTag,
        AmmoStackPrototype = component.AmmoStackPrototype
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgMagazineModuleAmmoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgMagazineModuleAmmoComponent.PubgMagazineModuleAmmoComponent_AutoState current))
        return;
      component.CurrentAmmo = current.CurrentAmmo;
      component.Capacity = current.Capacity;
      component.AmmoTag = current.AmmoTag;
      component.AmmoStackPrototype = current.AmmoStackPrototype;
    }
  }
}
