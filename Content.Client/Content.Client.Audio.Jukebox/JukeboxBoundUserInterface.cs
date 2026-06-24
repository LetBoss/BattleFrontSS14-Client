using System;
using Content.Shared.Audio.Jukebox;
using Robust.Client.Audio;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Audio.Jukebox;

public sealed class JukeboxBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _protoManager;

	[ViewVariables]
	private JukeboxMenu? _menu;

	public JukeboxBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<JukeboxBoundUserInterface>(this);
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<JukeboxMenu>((BoundUserInterface)(object)this);
		_menu.OnPlayPressed += delegate(bool args)
		{
			if (args)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new JukeboxPlayingMessage());
			}
			else
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new JukeboxPauseMessage());
			}
		};
		_menu.OnStopPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new JukeboxStopMessage());
		};
		_menu.OnSongSelected += SelectSong;
		_menu.SetTime += SetTime;
		PopulateMusic();
		Reload();
	}

	public void Reload()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		JukeboxComponent jukeboxComponent = default(JukeboxComponent);
		if (_menu != null && base.EntMan.TryGetComponent<JukeboxComponent>(((BoundUserInterface)this).Owner, ref jukeboxComponent))
		{
			_menu.SetAudioStream(jukeboxComponent.AudioStream);
			JukeboxPrototype jukeboxPrototype = default(JukeboxPrototype);
			if (_protoManager.TryIndex<JukeboxPrototype>(jukeboxComponent.SelectedSongId, ref jukeboxPrototype))
			{
				TimeSpan audioLength = ((SharedAudioSystem)base.EntMan.System<AudioSystem>()).GetAudioLength((ResolvedSoundSpecifier)new ResolvedPathSpecifier(jukeboxPrototype.Path.Path));
				_menu.SetSelectedSong(jukeboxPrototype.Name, (float)audioLength.TotalSeconds);
			}
			else
			{
				_menu.SetSelectedSong(string.Empty, 0f);
			}
		}
	}

	public void PopulateMusic()
	{
		_menu?.Populate(_protoManager.EnumeratePrototypes<JukeboxPrototype>());
	}

	public void SelectSong(ProtoId<JukeboxPrototype> songid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new JukeboxSelectedMessage(songid));
	}

	public void SetTime(float time)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		JukeboxComponent jukeboxComponent = default(JukeboxComponent);
		AudioComponent val = default(AudioComponent);
		if (base.EntMan.TryGetComponent<JukeboxComponent>(((BoundUserInterface)this).Owner, ref jukeboxComponent) && base.EntMan.TryGetComponent<AudioComponent>(jukeboxComponent.AudioStream, ref val))
		{
			val.PlaybackPosition = time;
		}
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new JukeboxSetTimeMessage(time));
	}
}
