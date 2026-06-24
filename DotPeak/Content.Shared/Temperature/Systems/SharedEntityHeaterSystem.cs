// Decompiled with JetBrains decompiler
// Type: Content.Shared.Temperature.Systems.SharedEntityHeaterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Temperature.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Temperature.Systems;

public abstract class SharedEntityHeaterSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPowerReceiverSystem _receiver;
  [Dependency]
  private SharedAudioSystem _audio;
  private readonly int _settingCount = Enum.GetValues<EntityHeaterSetting>().Length;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EntityHeaterComponent, ExaminedEvent>(new EntityEventRefHandler<EntityHeaterComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<EntityHeaterComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<EntityHeaterComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<EntityHeaterComponent, PowerChangedEvent>(new EntityEventRefHandler<EntityHeaterComponent, PowerChangedEvent>(this.OnPowerChanged));
  }

  private void OnExamined(Entity<EntityHeaterComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("entity-heater-examined", ("setting", (object) ent.Comp.Setting)));
  }

  private void OnGetVerbs(
    Entity<EntityHeaterComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityHeaterSetting nextSetting = (EntityHeaterSetting) ((int) (ent.Comp.Setting + 1) % this._settingCount);
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("entity-heater-switch-setting", ("setting", (object) nextSetting));
    alternativeVerb.Act = (Action) (() => this.ChangeSetting(ent, nextSetting, new EntityUid?(user)));
    verbs.Add(alternativeVerb);
  }

  private void OnPowerChanged(Entity<EntityHeaterComponent> ent, ref PowerChangedEvent args)
  {
    EntityHeaterSetting entityHeaterSetting = args.Powered ? ent.Comp.Setting : EntityHeaterSetting.Off;
    this._appearance.SetData((EntityUid) ent, (Enum) EntityHeaterVisuals.Setting, (object) entityHeaterSetting);
  }

  protected virtual void ChangeSetting(
    Entity<EntityHeaterComponent> ent,
    EntityHeaterSetting setting,
    EntityUid? user = null)
  {
    ent.Comp.Setting = setting;
    this._audio.PlayPredicted((SoundSpecifier) ent.Comp.SettingSound, (EntityUid) ent, user);
    this._popup.PopupClient(this.Loc.GetString("entity-heater-switched-setting", (nameof (setting), (object) setting)), (EntityUid) ent, user);
    this.Dirty<EntityHeaterComponent>(ent);
    if (!this._receiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent.Owner))
      return;
    this._appearance.SetData((EntityUid) ent, (Enum) EntityHeaterVisuals.Setting, (object) setting);
  }

  protected float SettingPower(EntityHeaterSetting setting, float max)
  {
    float num;
    switch (setting)
    {
      case EntityHeaterSetting.Low:
        num = max / 3f;
        break;
      case EntityHeaterSetting.Medium:
        num = (float) ((double) max * 2.0 / 3.0);
        break;
      case EntityHeaterSetting.High:
        num = max;
        break;
      default:
        num = 0.01f;
        break;
    }
    return num;
  }
}
