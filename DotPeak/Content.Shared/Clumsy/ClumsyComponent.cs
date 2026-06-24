// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clumsy.ClumsyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clumsy;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ClumsyComponent : 
  Component,
  ISerializationGenerated<ClumsyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ClumsySound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/bikehorn.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ClumsyDefaultCheck = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ClumsyDefaultStunTime = TimeSpan.FromSeconds(2.5);
  [DataField(null, false, 1, false, false, null)]
  public SoundCollectionSpecifier TableBonkSound = new SoundCollectionSpecifier("TrayHit", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan GunShootFailStunTime = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? GunShootFailDamage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? CatchingFailDamage;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier GunShootFailSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/bang.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClumsyHypo = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClumsyDefib = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClumsyGuns = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClumsyCatching = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClumsyVaulting = true;
  [DataField(null, false, 1, false, false, null)]
  public LocId HypoFailedMessage = LocId.op_Implicit("clumsy-hypospray-fail-message");
  [DataField(null, false, 1, false, false, null)]
  public LocId GunFailedMessage = LocId.op_Implicit("clumsy-gun-fail-message");
  [DataField(null, false, 1, false, false, null)]
  public LocId CatchingFailedMessageSelf = LocId.op_Implicit("clumsy-catch-fail-message-user");
  [DataField(null, false, 1, false, false, null)]
  public LocId CatchingFailedMessageOthers = LocId.op_Implicit("clumsy-catch-fail-message-others");
  [DataField(null, false, 1, false, false, null)]
  public LocId VaulingFailedMessageSelf = LocId.op_Implicit("clumsy-vaulting-fail-message-user");
  [DataField(null, false, 1, false, false, null)]
  public LocId VaulingFailedMessageOthers = LocId.op_Implicit("clumsy-vaulting-fail-message-others");
  [DataField(null, false, 1, false, false, null)]
  public LocId VaulingFailedMessageForced = LocId.op_Implicit("clumsy-vaulting-fail-forced-message");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClumsyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ClumsyComponent) component;
    if (serialization.TryCustomCopy<ClumsyComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.ClumsySound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ClumsySound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.ClumsySound, hookCtx, context, false);
    target.ClumsySound = soundSpecifier1;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ClumsyDefaultCheck, ref num, hookCtx, false, context))
      num = this.ClumsyDefaultCheck;
    target.ClumsyDefaultCheck = num;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ClumsyDefaultStunTime, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.ClumsyDefaultStunTime, hookCtx, context, false);
    target.ClumsyDefaultStunTime = timeSpan1;
    SoundCollectionSpecifier collectionSpecifier = (SoundCollectionSpecifier) null;
    if (this.TableBonkSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundCollectionSpecifier>(this.TableBonkSound, ref collectionSpecifier, hookCtx, false, context))
    {
      if (this.TableBonkSound == null)
        collectionSpecifier = (SoundCollectionSpecifier) null;
      else
        serialization.CopyTo<SoundCollectionSpecifier>(this.TableBonkSound, ref collectionSpecifier, hookCtx, context, true);
    }
    target.TableBonkSound = collectionSpecifier;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GunShootFailStunTime, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.GunShootFailStunTime, hookCtx, context, false);
    target.GunShootFailStunTime = timeSpan2;
    DamageSpecifier damageSpecifier1 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.GunShootFailDamage, ref damageSpecifier1, hookCtx, false, context))
    {
      if (this.GunShootFailDamage == null)
        damageSpecifier1 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.GunShootFailDamage, ref damageSpecifier1, hookCtx, context, false);
    }
    target.GunShootFailDamage = damageSpecifier1;
    DamageSpecifier damageSpecifier2 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CatchingFailDamage, ref damageSpecifier2, hookCtx, false, context))
    {
      if (this.CatchingFailDamage == null)
        damageSpecifier2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CatchingFailDamage, ref damageSpecifier2, hookCtx, context, false);
    }
    target.CatchingFailDamage = damageSpecifier2;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.GunShootFailSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.GunShootFailSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.GunShootFailSound, hookCtx, context, false);
    target.GunShootFailSound = soundSpecifier2;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClumsyHypo, ref flag1, hookCtx, false, context))
      flag1 = this.ClumsyHypo;
    target.ClumsyHypo = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClumsyDefib, ref flag2, hookCtx, false, context))
      flag2 = this.ClumsyDefib;
    target.ClumsyDefib = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClumsyGuns, ref flag3, hookCtx, false, context))
      flag3 = this.ClumsyGuns;
    target.ClumsyGuns = flag3;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClumsyCatching, ref flag4, hookCtx, false, context))
      flag4 = this.ClumsyCatching;
    target.ClumsyCatching = flag4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClumsyVaulting, ref flag5, hookCtx, false, context))
      flag5 = this.ClumsyVaulting;
    target.ClumsyVaulting = flag5;
    LocId locId1 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.HypoFailedMessage, ref locId1, hookCtx, false, context))
      locId1 = serialization.CreateCopy<LocId>(this.HypoFailedMessage, hookCtx, context, false);
    target.HypoFailedMessage = locId1;
    LocId locId2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.GunFailedMessage, ref locId2, hookCtx, false, context))
      locId2 = serialization.CreateCopy<LocId>(this.GunFailedMessage, hookCtx, context, false);
    target.GunFailedMessage = locId2;
    LocId locId3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CatchingFailedMessageSelf, ref locId3, hookCtx, false, context))
      locId3 = serialization.CreateCopy<LocId>(this.CatchingFailedMessageSelf, hookCtx, context, false);
    target.CatchingFailedMessageSelf = locId3;
    LocId locId4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CatchingFailedMessageOthers, ref locId4, hookCtx, false, context))
      locId4 = serialization.CreateCopy<LocId>(this.CatchingFailedMessageOthers, hookCtx, context, false);
    target.CatchingFailedMessageOthers = locId4;
    LocId locId5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VaulingFailedMessageSelf, ref locId5, hookCtx, false, context))
      locId5 = serialization.CreateCopy<LocId>(this.VaulingFailedMessageSelf, hookCtx, context, false);
    target.VaulingFailedMessageSelf = locId5;
    LocId locId6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VaulingFailedMessageOthers, ref locId6, hookCtx, false, context))
      locId6 = serialization.CreateCopy<LocId>(this.VaulingFailedMessageOthers, hookCtx, context, false);
    target.VaulingFailedMessageOthers = locId6;
    LocId locId7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.VaulingFailedMessageForced, ref locId7, hookCtx, false, context))
      locId7 = serialization.CreateCopy<LocId>(this.VaulingFailedMessageForced, hookCtx, context, false);
    target.VaulingFailedMessageForced = locId7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClumsyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ClumsyComponent target1 = (ClumsyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ClumsyComponent target1 = (ClumsyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ClumsyComponent target1 = (ClumsyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ClumsyComponent Component.Instantiate() => new ClumsyComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ClumsyComponent_AutoState : IComponentState
  {
    public float ClumsyDefaultCheck;
    public TimeSpan ClumsyDefaultStunTime;
    public TimeSpan GunShootFailStunTime;
    public DamageSpecifier? GunShootFailDamage;
    public DamageSpecifier? CatchingFailDamage;
    public bool ClumsyHypo;
    public bool ClumsyDefib;
    public bool ClumsyGuns;
    public bool ClumsyCatching;
    public bool ClumsyVaulting;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ClumsyComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClumsyComponent, ComponentGetState>(new ComponentEventRefHandler<ClumsyComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClumsyComponent, ComponentHandleState>(new ComponentEventRefHandler<ClumsyComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ClumsyComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ClumsyComponent.ClumsyComponent_AutoState()
      {
        ClumsyDefaultCheck = component.ClumsyDefaultCheck,
        ClumsyDefaultStunTime = component.ClumsyDefaultStunTime,
        GunShootFailStunTime = component.GunShootFailStunTime,
        GunShootFailDamage = component.GunShootFailDamage,
        CatchingFailDamage = component.CatchingFailDamage,
        ClumsyHypo = component.ClumsyHypo,
        ClumsyDefib = component.ClumsyDefib,
        ClumsyGuns = component.ClumsyGuns,
        ClumsyCatching = component.ClumsyCatching,
        ClumsyVaulting = component.ClumsyVaulting
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ClumsyComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ClumsyComponent.ClumsyComponent_AutoState current))
        return;
      component.ClumsyDefaultCheck = current.ClumsyDefaultCheck;
      component.ClumsyDefaultStunTime = current.ClumsyDefaultStunTime;
      component.GunShootFailStunTime = current.GunShootFailStunTime;
      component.GunShootFailDamage = current.GunShootFailDamage;
      component.CatchingFailDamage = current.CatchingFailDamage;
      component.ClumsyHypo = current.ClumsyHypo;
      component.ClumsyDefib = current.ClumsyDefib;
      component.ClumsyGuns = current.ClumsyGuns;
      component.ClumsyCatching = current.ClumsyCatching;
      component.ClumsyVaulting = current.ClumsyVaulting;
    }
  }
}
