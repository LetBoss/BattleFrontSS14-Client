// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.SharedRingerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.PDA.Ringer;
using Content.Shared.Popups;
using Content.Shared.Store;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.PDA;

public abstract class SharedRingerSystem : EntitySystem
{
  public const int RingtoneLength = 6;
  public const int NoteTempo = 300;
  public const float NoteDelay = 0.2f;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPdaSystem _pda;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  protected SharedUserInterfaceSystem UI;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RingerComponent, RingerSetRingtoneMessage>(new EntityEventRefHandler<RingerComponent, RingerSetRingtoneMessage>(this.OnSetRingtone));
    this.SubscribeLocalEvent<RingerComponent, RingerPlayRingtoneMessage>(new EntityEventRefHandler<RingerComponent, RingerPlayRingtoneMessage>(this.OnPlayRingtone));
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RingerComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RingerComponent, TransformComponent>();
    EntityUid uid;
    RingerComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.Active && comp1.NextNoteTime.HasValue)
      {
        TimeSpan curTime = this._timing.CurTime;
        if (!(curTime < comp1.NextNoteTime.Value))
        {
          if (this._net.IsServer)
            this._audio.PlayEntity((SoundSpecifier) SharedRingerSystem.GetSound(comp1.Ringtone[comp1.NoteCount]), Filter.Empty().AddInRange(this._xform.GetMapCoordinates(uid, comp2), comp1.Range), uid, true, new AudioParams?(AudioParams.Default.WithMaxDistance(comp1.Range).WithVolume(comp1.Volume)));
          comp1.NextNoteTime = new TimeSpan?(curTime + TimeSpan.FromSeconds(0.20000000298023224));
          ++comp1.NoteCount;
          this.DirtyFields<RingerComponent>(uid, comp1, (MetaDataComponent) null, "NextNoteTime", "NoteCount");
          if (comp1.NoteCount >= 6)
          {
            comp1.Active = false;
            comp1.NextNoteTime = new TimeSpan?();
            comp1.NoteCount = 0;
            this.DirtyFields<RingerComponent>(uid, comp1, (MetaDataComponent) null, "Active", "NextNoteTime", "NoteCount");
            this.UpdateRingerUi((Entity<RingerComponent>) (uid, comp1));
          }
        }
      }
    }
  }

  public void RingerPlayRingtone(Entity<RingerComponent?> ent)
  {
    if (!this.Resolve<RingerComponent>((EntityUid) ent, ref ent.Comp))
      return;
    this.StartRingtone((Entity<RingerComponent>) ((EntityUid) ent, ent.Comp));
  }

  public bool TryToggleRingerUi(EntityUid uid, EntityUid actor)
  {
    this.UI.TryToggleUi((Entity<UserInterfaceComponent>) uid, (Enum) RingerUiKey.Key, actor);
    return true;
  }

  public void LockUplink(Entity<RingerUplinkComponent?> ent)
  {
    if (!this.Resolve<RingerUplinkComponent>((EntityUid) ent, ref ent.Comp))
      return;
    ent.Comp.Unlocked = false;
    this.UI.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) StoreUiKey.Key);
  }

  public virtual bool TryToggleUplink(EntityUid uid, Note[] ringtone, EntityUid? user = null)
  {
    return false;
  }

  private void OnSetRingtone(Entity<RingerComponent> ent, ref RingerSetRingtoneMessage args)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (ent.Comp.NextRingtoneSetTime > curTime)
      return;
    ent.Comp.NextRingtoneSetTime = curTime + ent.Comp.Cooldown;
    this.DirtyField<RingerComponent>(ent.AsNullable(), "NextRingtoneSetTime");
    if (args.Ringtone.Length != 6 || this.TryToggleUplink((EntityUid) ent, args.Ringtone))
      return;
    this.UpdateRingerRingtone(ent, args.Ringtone);
  }

  private void OnPlayRingtone(Entity<RingerComponent> ent, ref RingerPlayRingtoneMessage args)
  {
    this.StartRingtone(ent);
  }

  private void StartRingtone(Entity<RingerComponent> ent)
  {
    if (ent.Comp.Active)
      return;
    ent.Comp.Active = true;
    ent.Comp.NoteCount = 0;
    ent.Comp.NextNoteTime = new TimeSpan?(this._timing.CurTime);
    this.UpdateRingerUi(ent);
    this._popup.PopupPredicted(this.Loc.GetString("comp-ringer-vibration-popup"), (EntityUid) ent, new EntityUid?(ent.Owner), Filter.Pvs((EntityUid) ent, 0.05f), false, PopupType.Medium);
    this.DirtyFields<RingerComponent>(ent.AsNullable(), (MetaDataComponent) null, "NextNoteTime", "Active", "NoteCount");
  }

  protected void UpdateRingerRingtone(Entity<RingerComponent> ent, Note[] ringtone)
  {
    ent.Comp.Ringtone = ringtone;
    this.DirtyField<RingerComponent>(ent.AsNullable(), "Ringtone");
    this.UpdateRingerUi(ent);
  }

  protected bool ToggleUplinkInternal(Entity<RingerUplinkComponent> ent)
  {
    ent.Comp.Unlocked = !ent.Comp.Unlocked;
    PdaComponent comp;
    if (this.TryComp<PdaComponent>((EntityUid) ent, out comp))
      this._pda.UpdatePdaUi((EntityUid) ent, comp);
    if (!ent.Comp.Unlocked)
      this.UI.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) StoreUiKey.Key);
    return true;
  }

  private static SoundPathSpecifier GetSound(Note note)
  {
    return new SoundPathSpecifier($"/Audio/Effects/RingtoneNotes/{note.ToString().ToLower()}.ogg");
  }

  protected virtual void UpdateRingerUi(Entity<RingerComponent> ent)
  {
  }
}
