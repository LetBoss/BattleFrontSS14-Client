// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Ammo.Components.PubgAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared._PUBG.Ammo.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class PubgAmmoProviderComponent : 
  Component,
  ISerializationGenerated<PubgAmmoProviderComponent>,
  ISerializationGenerated
{
  private static readonly Regex AmmoTagRegex = new Regex("Ammo(.+?)Pubg", RegexOptions.Compiled);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public int CurrentAmmo = 30;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public int MaxAmmo = 30;
  [DataField(null, false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string AmmoPrototype = "CartridgeRifle762Pubg";
  [DataField(null, false, 1, false, false, null)]
  public string AmmoTag = "Ammo762Pubg";
  [DataField(null, false, 1, false, false, null)]
  public string? AmmoType;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ReloadTime = 2.5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int ReloadAmount;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ReloadSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsReloading;
  [Robust.Shared.ViewVariables.ViewVariables]
  public DoAfterId? ActiveReloadDoAfter;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float UnloadTime = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UnloadSound;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsUnloading;
  [Robust.Shared.ViewVariables.ViewVariables]
  public DoAfterId? ActiveUnloadDoAfter;

  public string GetAmmoTypeDisplay()
  {
    if (!string.IsNullOrEmpty(this.AmmoType))
      return this.AmmoType;
    Match match = PubgAmmoProviderComponent.AmmoTagRegex.Match(this.AmmoTag);
    return match.Success ? match.Groups[1].Value.Replace("-", ".").Replace("mm", "") : "";
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<PubgAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.CurrentAmmo, ref target2, hookCtx, false, context))
      target2 = this.CurrentAmmo;
    target.CurrentAmmo = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxAmmo, ref target3, hookCtx, false, context))
      target3 = this.MaxAmmo;
    target.MaxAmmo = target3;
    string target4 = (string) null;
    if (this.AmmoPrototype == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AmmoPrototype, ref target4, hookCtx, false, context))
      target4 = this.AmmoPrototype;
    target.AmmoPrototype = target4;
    string target5 = (string) null;
    if (this.AmmoTag == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AmmoTag, ref target5, hookCtx, false, context))
      target5 = this.AmmoTag;
    target.AmmoTag = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.AmmoType, ref target6, hookCtx, false, context))
      target6 = this.AmmoType;
    target.AmmoType = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReloadTime, ref target7, hookCtx, false, context))
      target7 = this.ReloadTime;
    target.ReloadTime = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.ReloadAmount, ref target8, hookCtx, false, context))
      target8 = this.ReloadAmount;
    target.ReloadAmount = target8;
    SoundSpecifier target9 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ReloadSound, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<SoundSpecifier>(this.ReloadSound, hookCtx, context);
    target.ReloadSound = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UnloadTime, ref target10, hookCtx, false, context))
      target10 = this.UnloadTime;
    target.UnloadTime = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnloadSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.UnloadSound, hookCtx, context);
    target.UnloadSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgAmmoProviderComponent target,
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
    PubgAmmoProviderComponent target1 = (PubgAmmoProviderComponent) target;
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
    PubgAmmoProviderComponent target1 = (PubgAmmoProviderComponent) target;
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
    PubgAmmoProviderComponent target1 = (PubgAmmoProviderComponent) target;
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
  virtual PubgAmmoProviderComponent Component.Instantiate() => new PubgAmmoProviderComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PubgAmmoProviderComponent_AutoState : IComponentState
  {
    public int CurrentAmmo;
    public int MaxAmmo;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PubgAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PubgAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<PubgAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PubgAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<PubgAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PubgAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PubgAmmoProviderComponent.PubgAmmoProviderComponent_AutoState()
      {
        CurrentAmmo = component.CurrentAmmo,
        MaxAmmo = component.MaxAmmo
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PubgAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PubgAmmoProviderComponent.PubgAmmoProviderComponent_AutoState current))
        return;
      component.CurrentAmmo = current.CurrentAmmo;
      component.MaxAmmo = current.MaxAmmo;
    }
  }
}
