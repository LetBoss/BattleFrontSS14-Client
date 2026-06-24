// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Cryostorage.CryostorageComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Bed.Cryostorage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class CryostorageComponent : 
  Component,
  ISerializationGenerated<CryostorageComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ContainerId = "storage";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan NoMindGracePeriod = TimeSpan.FromSeconds(30.0);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan GracePeriod = TimeSpan.FromMinutes(5.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntityUid> StoredPlayers = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RemoveSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/teleport_departure.ogg", new AudioParams?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CryostorageComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (CryostorageComponent) component;
    if (serialization.TryCustomCopy<CryostorageComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref str, hookCtx, false, context))
      str = this.ContainerId;
    target.ContainerId = str;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NoMindGracePeriod, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NoMindGracePeriod, hookCtx, context, false);
    target.NoMindGracePeriod = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.GracePeriod, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.GracePeriod, hookCtx, context, false);
    target.GracePeriod = timeSpan2;
    List<EntityUid> entityUidList = (List<EntityUid>) null;
    if (this.StoredPlayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.StoredPlayers, ref entityUidList, hookCtx, true, context))
      entityUidList = serialization.CreateCopy<List<EntityUid>>(this.StoredPlayers, hookCtx, context, false);
    target.StoredPlayers = entityUidList;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RemoveSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.RemoveSound, hookCtx, context, false);
    target.RemoveSound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CryostorageComponent target,
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
    CryostorageComponent target1 = (CryostorageComponent) target;
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
    CryostorageComponent target1 = (CryostorageComponent) target;
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
    CryostorageComponent target1 = (CryostorageComponent) target;
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
  virtual CryostorageComponent Component.Instantiate() => new CryostorageComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CryostorageComponent_AutoState : IComponentState
  {
    public TimeSpan NoMindGracePeriod;
    public TimeSpan GracePeriod;
    public List<NetEntity> StoredPlayers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CryostorageComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CryostorageComponent, ComponentGetState>(new ComponentEventRefHandler<CryostorageComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<CryostorageComponent, ComponentHandleState>(new ComponentEventRefHandler<CryostorageComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      CryostorageComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new CryostorageComponent.CryostorageComponent_AutoState()
      {
        NoMindGracePeriod = component.NoMindGracePeriod,
        GracePeriod = component.GracePeriod,
        StoredPlayers = this.GetNetEntityList(component.StoredPlayers)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CryostorageComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is CryostorageComponent.CryostorageComponent_AutoState current))
        return;
      component.NoMindGracePeriod = current.NoMindGracePeriod;
      component.GracePeriod = current.GracePeriod;
      this.EnsureEntityList<CryostorageComponent>(current.StoredPlayers, uid, component.StoredPlayers);
    }
  }
}
