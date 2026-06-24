// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radio.EntitySystems.SharedJammerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Radio.EntitySystems;

public abstract class SharedJammerSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedDeviceNetworkJammerSystem _jammer;
  [Dependency]
  protected SharedPopupSystem Popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RadioJammerComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<RadioJammerComponent, GetVerbsEvent<Verb>>(this.OnGetVerb));
    this.SubscribeLocalEvent<RadioJammerComponent, ExaminedEvent>(new EntityEventRefHandler<RadioJammerComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnGetVerb(Entity<RadioJammerComponent> entity, ref GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    byte num = 0;
    foreach (RadioJammerComponent.RadioJamSetting setting1 in entity.Comp.Settings)
    {
      RadioJammerComponent.RadioJamSetting setting = setting1;
      byte currIndex = num;
      Verb verb = new Verb()
      {
        Priority = (int) currIndex,
        Category = VerbCategory.PowerLevel,
        Disabled = entity.Comp.SelectedPowerLevel == (int) currIndex,
        Act = (Action) (() =>
        {
          entity.Comp.SelectedPowerLevel = (int) currIndex;
          this.Dirty<RadioJammerComponent>(entity);
          this._jammer.TrySetRange((Entity<DeviceNetworkJammerComponent>) entity.Owner, this.GetCurrentRange(entity));
          this.Popup.PopupClient(this.Loc.GetString((string) setting.Message), user, new EntityUid?(user));
        }),
        Text = this.Loc.GetString((string) setting.Name)
      };
      args.Verbs.Add(verb);
      ++num;
    }
  }

  private void OnExamine(Entity<RadioJammerComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    string markup1 = this.HasComp<ActiveRadioJammerComponent>((EntityUid) ent) ? this.Loc.GetString("radio-jammer-component-examine-on-state") : this.Loc.GetString("radio-jammer-component-examine-off-state");
    args.PushMarkup(markup1);
    string markup2 = this.Loc.GetString("radio-jammer-component-switch-setting", ("powerLevel", (object) this.Loc.GetString((string) ent.Comp.Settings[ent.Comp.SelectedPowerLevel].Name)));
    args.PushMarkup(markup2);
  }

  public float GetCurrentWattage(Entity<RadioJammerComponent> jammer)
  {
    return jammer.Comp.Settings[jammer.Comp.SelectedPowerLevel].Wattage;
  }

  public float GetCurrentRange(Entity<RadioJammerComponent> jammer)
  {
    return jammer.Comp.Settings[jammer.Comp.SelectedPowerLevel].Range;
  }

  protected void ChangeLEDState(Entity<AppearanceComponent?> ent, bool isLEDOn)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) RadioJammerVisuals.LEDOn, (object) isLEDOn, ent.Comp);
  }

  protected void ChangeChargeLevel(
    Entity<AppearanceComponent?> ent,
    RadioJammerChargeLevel chargeLevel)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) RadioJammerVisuals.ChargeLevel, (object) chargeLevel, ent.Comp);
  }
}
