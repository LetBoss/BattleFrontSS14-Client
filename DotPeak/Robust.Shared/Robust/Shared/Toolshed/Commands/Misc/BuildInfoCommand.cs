// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.BuildInfoCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class BuildInfoCommand : ToolshedCommand
{
  [Dependency]
  private readonly IConfigurationManager _cfg;
  private static readonly string Gold;

  [CommandImplementation(null)]
  public void BuildInfo(IInvocationContext ctx)
  {
    string cvar1 = this._cfg.GetCVar<string>(CVars.BuildForkId);
    string cvar2 = this._cfg.GetCVar<string>(CVars.BuildHash);
    string cvar3 = this._cfg.GetCVar<string>(CVars.BuildManifestHash);
    string cvar4 = this._cfg.GetCVar<string>(CVars.BuildEngineVersion);
    ctx.WriteLine(FormattedMessage.FromMarkupOrThrow($"[color={BuildInfoCommand.Gold}]Game:[/color] {cvar1}\n[color={BuildInfoCommand.Gold}]Build commit:[/color] {cvar2}\n[color={BuildInfoCommand.Gold}]Manifest hash:[/color] {cvar3}\n[color={BuildInfoCommand.Gold}]Engine ver:[/color] {cvar4}"));
  }

  static BuildInfoCommand()
  {
    Color gold = Color.Gold;
    BuildInfoCommand.Gold = ((Color) ref gold).ToHex();
  }
}
