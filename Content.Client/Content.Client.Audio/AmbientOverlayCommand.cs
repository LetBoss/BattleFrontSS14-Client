using System;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Audio;

public sealed class AmbientOverlayCommand : LocalizedEntityCommands
{
	[Dependency]
	private AmbientSoundSystem _ambient;

	public override string Command => "showambient";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		AmbientSoundSystem ambient = _ambient;
		ambient.OverlayEnabled = !ambient.OverlayEnabled;
		shell.WriteLine(((LocalizedCommands)this).Loc.GetString("cmd-showambient-status", (ValueTuple<string, object>)("status", _ambient.OverlayEnabled)));
	}
}
