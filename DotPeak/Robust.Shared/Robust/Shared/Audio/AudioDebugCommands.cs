// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.AudioDebugCommands
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Audio.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Robust.Shared.Audio;

internal sealed class AudioDebugCommands : LocalizedCommands
{
  [Dependency]
  private readonly IEntitySystemManager _entitySystem;

  public override string Command => "audio_length";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 1)
    {
      shell.WriteError(this.LocalizationManager.GetString("cmd-invalid-arg-number-error"));
    }
    else
    {
      TimeSpan audioLength = this._entitySystem.GetEntitySystem<SharedAudioSystem>().GetAudioLength((ResolvedSoundSpecifier) new ResolvedPathSpecifier(args[0]));
      shell.WriteLine(audioLength.ToString());
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHint(this.LocalizationManager.GetString("cmd-audio_length-arg-file-name")) : CompletionResult.Empty;
  }
}
