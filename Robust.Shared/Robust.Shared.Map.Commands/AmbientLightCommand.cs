using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Robust.Shared.Map.Commands;

public sealed class AmbientLightCommand : IConsoleCommand
{
	[Dependency]
	private readonly IEntitySystemManager _systems;

	public string Command => "setambientlight";

	public string Description => Loc.GetString("cmd-set-ambient-light-desc");

	public string Help => Loc.GetString("cmd-set-ambient-light-help");

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (args.Length != 5)
		{
			shell.WriteError(Loc.GetString("cmd-invalid-arg-number-error"));
			return;
		}
		if (!int.TryParse(args[0], out var result))
		{
			shell.WriteError(Loc.GetString("cmd-parse-failure-integer"));
			return;
		}
		MapId mapId = new MapId(result);
		SharedMapSystem entitySystem = _systems.GetEntitySystem<SharedMapSystem>();
		byte result2;
		byte result3;
		byte result4;
		byte result5;
		if (!entitySystem.MapExists(mapId))
		{
			shell.WriteError(Loc.GetString("cmd-parse-failure-mapid", ("arg", mapId.Value)));
		}
		else if (!byte.TryParse(args[1], out result2) || !byte.TryParse(args[2], out result3) || !byte.TryParse(args[3], out result4) || !byte.TryParse(args[4], out result5))
		{
			shell.WriteError(Loc.GetString("cmd-set-ambient-light-parse"));
		}
		else
		{
			Color color = Color.FromSrgb(new Color(result2, result3, result4, result5));
			entitySystem.SetAmbientLight(mapId, color);
		}
	}
}
