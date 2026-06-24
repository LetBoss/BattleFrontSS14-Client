// Decompiled with JetBrains decompiler
// Type: Content.Shared.Audio.SharedAmbientSoundSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Audio;

public abstract class SharedAmbientSoundSystem : EntitySystem
{
  private EntityQuery<AmbientSoundComponent> _query;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AmbientSoundComponent, ComponentGetState>(new ComponentEventRefHandler<AmbientSoundComponent, ComponentGetState>((object) this, __methodptr(GetCompState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AmbientSoundComponent, ComponentHandleState>(new ComponentEventRefHandler<AmbientSoundComponent, ComponentHandleState>((object) this, __methodptr(HandleCompState)), (Type[]) null, (Type[]) null);
    this._query = this.GetEntityQuery<AmbientSoundComponent>();
  }

  public virtual void SetAmbience(EntityUid uid, bool value, AmbientSoundComponent? ambience = null)
  {
    if (!this._query.Resolve(uid, ref ambience, false) || ambience.Enabled == value)
      return;
    ambience.Enabled = value;
    this.QueueUpdate(uid, ambience);
    this.Dirty(uid, (IComponent) ambience, (MetaDataComponent) null);
  }

  public virtual void SetRange(EntityUid uid, float value, AmbientSoundComponent? ambience = null)
  {
    if (!this._query.Resolve(uid, ref ambience, false) || MathHelper.CloseToPercent(ambience.Range, value, 1E-05))
      return;
    ambience.Range = value;
    this.QueueUpdate(uid, ambience);
    this.Dirty(uid, (IComponent) ambience, (MetaDataComponent) null);
  }

  protected virtual void QueueUpdate(EntityUid uid, AmbientSoundComponent ambience)
  {
  }

  public virtual void SetVolume(EntityUid uid, float value, AmbientSoundComponent? ambience = null)
  {
    if (!this._query.Resolve(uid, ref ambience, false) || MathHelper.CloseToPercent(ambience.Volume, value, 1E-05))
      return;
    ambience.Volume = value;
    this.Dirty(uid, (IComponent) ambience, (MetaDataComponent) null);
  }

  public virtual void SetSound(EntityUid uid, SoundSpecifier sound, AmbientSoundComponent? ambience = null)
  {
    if (!this._query.Resolve(uid, ref ambience, false) || ambience.Sound == sound)
      return;
    ambience.Sound = sound;
    this.QueueUpdate(uid, ambience);
    this.Dirty(uid, (IComponent) ambience, (MetaDataComponent) null);
  }

  private void HandleCompState(
    EntityUid uid,
    AmbientSoundComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is AmbientSoundComponentState current))
      return;
    this.SetAmbience(uid, current.Enabled, component);
    this.SetRange(uid, current.Range, component);
    this.SetVolume(uid, current.Volume, component);
    this.SetSound(uid, current.Sound, component);
  }

  private void GetCompState(
    EntityUid uid,
    AmbientSoundComponent component,
    ref ComponentGetState args)
  {
    ((ComponentGetState) ref args).State = (IComponentState) new AmbientSoundComponentState()
    {
      Enabled = component.Enabled,
      Range = component.Range,
      Volume = component.Volume,
      Sound = component.Sound
    };
  }
}
