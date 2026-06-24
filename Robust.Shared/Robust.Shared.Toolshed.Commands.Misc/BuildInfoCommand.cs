using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

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
		string cVar = _cfg.GetCVar(CVars.BuildForkId);
		string cVar2 = _cfg.GetCVar(CVars.BuildHash);
		string cVar3 = _cfg.GetCVar(CVars.BuildManifestHash);
		string cVar4 = _cfg.GetCVar(CVars.BuildEngineVersion);
		ctx.WriteLine(FormattedMessage.FromMarkupOrThrow($"[color={Gold}]Game:[/color] {cVar}\n[color={Gold}]Build commit:[/color] {cVar2}\n[color={Gold}]Manifest hash:[/color] {cVar3}\n[color={Gold}]Engine ver:[/color] {cVar4}"));
	}

	static BuildInfoCommand()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Color gold = Color.Gold;
		Gold = ((Color)(ref gold)).ToHex();
	}
}
