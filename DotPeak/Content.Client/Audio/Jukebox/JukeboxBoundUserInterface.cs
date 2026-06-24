// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.Jukebox.JukeboxBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Audio.Jukebox;
using Robust.Client.Audio;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Audio.Jukebox;

public sealed class JukeboxBoundUserInterface : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _protoManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private JukeboxMenu? _menu;

  public JukeboxBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<JukeboxBoundUserInterface>(this);
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<JukeboxMenu>((BoundUserInterface) this);
    this._menu.OnPlayPressed += (Action<bool>) (args =>
    {
      if (args)
        this.SendMessage((BoundUserInterfaceMessage) new JukeboxPlayingMessage());
      else
        this.SendMessage((BoundUserInterfaceMessage) new JukeboxPauseMessage());
    });
    this._menu.OnStopPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new JukeboxStopMessage()));
    this._menu.OnSongSelected += new Action<ProtoId<JukeboxPrototype>>(this.SelectSong);
    this._menu.SetTime += new Action<float>(this.SetTime);
    this.PopulateMusic();
    this.Reload();
  }

  public void Reload()
  {
    JukeboxComponent jukeboxComponent;
    if (this._menu == null || !this.EntMan.TryGetComponent<JukeboxComponent>(this.Owner, ref jukeboxComponent))
      return;
    this._menu.SetAudioStream(jukeboxComponent.AudioStream);
    JukeboxPrototype jukeboxPrototype;
    if (this._protoManager.TryIndex<JukeboxPrototype>(jukeboxComponent.SelectedSongId, ref jukeboxPrototype))
    {
      TimeSpan audioLength = ((SharedAudioSystem) this.EntMan.System<AudioSystem>()).GetAudioLength((ResolvedSoundSpecifier) new ResolvedPathSpecifier(jukeboxPrototype.Path.Path));
      this._menu.SetSelectedSong(jukeboxPrototype.Name, (float) audioLength.TotalSeconds);
    }
    else
      this._menu.SetSelectedSong(string.Empty, 0.0f);
  }

  public void PopulateMusic()
  {
    this._menu?.Populate(this._protoManager.EnumeratePrototypes<JukeboxPrototype>());
  }

  public void SelectSong(ProtoId<JukeboxPrototype> songid)
  {
    this.SendMessage((BoundUserInterfaceMessage) new JukeboxSelectedMessage(songid));
  }

  public void SetTime(float time)
  {
    float songTime = time;
    JukeboxComponent jukeboxComponent;
    AudioComponent audioComponent;
    if (this.EntMan.TryGetComponent<JukeboxComponent>(this.Owner, ref jukeboxComponent) && this.EntMan.TryGetComponent<AudioComponent>(jukeboxComponent.AudioStream, ref audioComponent))
      audioComponent.PlaybackPosition = time;
    this.SendMessage((BoundUserInterfaceMessage) new JukeboxSetTimeMessage(songTime));
  }
}
