// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Deafness.DeafnessSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Deafness;
using Content.Shared.StatusEffect;
using Robust.Client.Audio;
using Robust.Client.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Deafness;

public sealed class DeafnessSystem : SharedDeafnessSystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IAudioManager _audio;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private IGameTiming _timing;
  private float _originalVolume = 0.5f;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeafComponent, ComponentShutdown>(new ComponentEventHandler<DeafComponent, ComponentShutdown>((object) this, __methodptr(OnDeafShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeafComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<DeafComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._cfg, CVars.AudioMasterVolume, (Action<float>) (value => this._originalVolume = value), true);
  }

  private void OnDeafShutdown(EntityUid uid, DeafComponent component, ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this._audio.SetMasterGain(this._originalVolume);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    DeafComponent component,
    LocalPlayerDetachedEvent args)
  {
    this._audio.SetMasterGain(this._originalVolume);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    TimeSpan curTime = this._timing.CurTime;
    EntityQueryEnumerator<DeafComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DeafComponent>();
    EntityUid entityUid;
    DeafComponent deafComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref deafComponent))
    {
      if (!EntityUid.op_Inequality(valueOrDefault, entityUid))
      {
        (TimeSpan, TimeSpan)? time1 = new (TimeSpan, TimeSpan)?();
        (TimeSpan, TimeSpan)? time2 = new (TimeSpan, TimeSpan)?();
        if (this._statusEffects.TryGetTime(valueOrDefault, ProtoId<StatusEffectPrototype>.op_Implicit(this.DeafKey), out time1) || this._statusEffects.TryGetTime(valueOrDefault, "Unconscious", out time2))
        {
          if (time2.HasValue && (!time1.HasValue || time1.Value.Item2 < time2.Value.Item2))
            time1 = new (TimeSpan, TimeSpan)?(time2.Value);
          if (time1.HasValue)
          {
            (TimeSpan, TimeSpan) valueTuple = time1.Value;
            TimeSpan timeSpan = valueTuple.Item2 - valueTuple.Item1;
            double totalSeconds1 = timeSpan.TotalSeconds;
            timeSpan = valueTuple.Item2 - curTime;
            float totalSeconds2 = (float) timeSpan.TotalSeconds;
            timeSpan = curTime - valueTuple.Item1;
            float totalSeconds3 = (float) timeSpan.TotalSeconds;
            float val2 = 0.0f;
            float num1 = Math.Clamp((float) (totalSeconds1 * 0.34999999403953552), 0.2f, 2f);
            float num2 = Math.Clamp((float) (totalSeconds1 * 0.15000000596046448), 0.1f, 1f);
            if ((double) totalSeconds3 <= 2.0 && !deafComponent.DidFadeOut)
            {
              val2 = (float) (1.0 - (double) totalSeconds3 / (double) num1) * this._originalVolume;
              if ((double) val2 <= 0.10000000149011612)
              {
                val2 = 0.0f;
                deafComponent.DidFadeOut = true;
              }
            }
            else if ((double) totalSeconds2 <= 1.0)
              val2 = (float) (1.0 - (double) totalSeconds2 / (double) num2) * this._originalVolume;
            this._audio.SetMasterGain(Math.Max(0.0f, val2));
          }
        }
      }
    }
  }
}
