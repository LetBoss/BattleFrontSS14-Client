// Decompiled with JetBrains decompiler
// Type: Content.Client.PDA.Ringer.RingerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using System;
using System.Threading;

#nullable enable
namespace Content.Client.PDA.Ringer;

public sealed class RingerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private RingtoneMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<RingtoneMenu>((BoundUserInterface) this);
    this._menu.OpenToLeft();
    this._menu.TestRingtoneButtonPressed += new Action(this.OnTestRingtoneButtonPressed);
    this._menu.SetRingtoneButtonPressed += new Action(this.OnSetRingtoneButtonPressed);
    base.Update();
  }

  private bool TryGetRingtone(out Note[] ringtone)
  {
    if (this._menu == null)
    {
      ringtone = Array.Empty<Note>();
      return false;
    }
    ringtone = new Note[this._menu.RingerNoteInputs.Length];
    for (int index = 0; index < this._menu.RingerNoteInputs.Length; ++index)
    {
      Note result;
      if (!Enum.TryParse<Note>(this._menu.RingerNoteInputs[index].Text.Replace("#", "sharp"), false, out result))
        return false;
      ringtone[index] = result;
    }
    return true;
  }

  public virtual void Update()
  {
    base.Update();
    RingerComponent ringerComponent;
    if (this._menu == null || !this.EntMan.TryGetComponent<RingerComponent>(this.Owner, ref ringerComponent))
      return;
    for (int index = 0; index < this._menu.RingerNoteInputs.Length; ++index)
    {
      string input = ringerComponent.Ringtone[index].ToString();
      if (RingtoneMenu.IsNote(input))
      {
        this._menu.PreviousNoteInputs[index] = input.Replace("sharp", "#");
        this._menu.RingerNoteInputs[index].Text = this._menu.PreviousNoteInputs[index];
      }
    }
    ((BaseButton) this._menu.TestRingerButton).Disabled = ringerComponent.Active;
  }

  private void OnTestRingtoneButtonPressed()
  {
    if (this._menu == null)
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new RingerPlayRingtoneMessage());
    ((BaseButton) this._menu.TestRingerButton).Disabled = true;
  }

  private void OnSetRingtoneButtonPressed()
  {
    Note[] ringtone;
    if (this._menu == null || !this.TryGetRingtone(out ringtone))
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new RingerSetRingtoneMessage(ringtone));
    ((BaseButton) this._menu.SetRingerButton).Disabled = true;
    Timer.Spawn(333, (Action) (() =>
    {
      RingtoneMenu menu = this._menu;
      if (menu == null || ((Control) menu).Disposed)
        return;
      Button setRingerButton = menu.SetRingerButton;
      if (setRingerButton == null || ((Control) setRingerButton).Disposed)
        return;
      ((BaseButton) setRingerButton).Disabled = false;
    }), new CancellationToken());
  }
}
