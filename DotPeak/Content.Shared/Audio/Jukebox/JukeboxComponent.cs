// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.Jukebox.JukeboxComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Audio.Jukebox;

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedJukeboxSystem)})]
public sealed class JukeboxComponent : 
  Component,
  ISerializationGenerated<JukeboxComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<JukeboxPrototype>? SelectedSongId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? AudioStream;
  [DataField(null, false, 1, false, false, null)]
  public string? OnState;
  [DataField(null, false, 1, false, false, null)]
  public string? OffState;
  [DataField(null, false, 1, false, false, null)]
  public string? SelectState;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Selecting;
  [Robust.Shared.ViewVariables.ViewVariables]
  public float SelectAccumulator;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JukeboxComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (JukeboxComponent) component;
    if (serialization.TryCustomCopy<JukeboxComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<JukeboxPrototype>? nullable1 = new ProtoId<JukeboxPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<JukeboxPrototype>?>(this.SelectedSongId, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<ProtoId<JukeboxPrototype>?>(this.SelectedSongId, hookCtx, context, false);
    target.SelectedSongId = nullable1;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AudioStream, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.AudioStream, hookCtx, context, false);
    target.AudioStream = nullable2;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.OnState, ref str1, hookCtx, false, context))
      str1 = this.OnState;
    target.OnState = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.OffState, ref str2, hookCtx, false, context))
      str2 = this.OffState;
    target.OffState = str2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SelectState, ref str3, hookCtx, false, context))
      str3 = this.SelectState;
    target.SelectState = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JukeboxComponent target,
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
    JukeboxComponent target1 = (JukeboxComponent) target;
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
    JukeboxComponent target1 = (JukeboxComponent) target;
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
    JukeboxComponent target1 = (JukeboxComponent) target;
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
  virtual JukeboxComponent Component.Instantiate() => new JukeboxComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class JukeboxComponent_AutoState : IComponentState
  {
    public ProtoId<JukeboxPrototype>? SelectedSongId;
    public NetEntity? AudioStream;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class JukeboxComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<JukeboxComponent, ComponentGetState>(new ComponentEventRefHandler<JukeboxComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<JukeboxComponent, ComponentHandleState>(new ComponentEventRefHandler<JukeboxComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, JukeboxComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new JukeboxComponent.JukeboxComponent_AutoState()
      {
        SelectedSongId = component.SelectedSongId,
        AudioStream = this.GetNetEntity(component.AudioStream, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      JukeboxComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is JukeboxComponent.JukeboxComponent_AutoState current))
        return;
      component.SelectedSongId = current.SelectedSongId;
      component.AudioStream = this.EnsureEntity<JukeboxComponent>(current.AudioStream, uid);
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, JukeboxComponent>(uid, component, ref handleStateEvent);
    }
  }
}
