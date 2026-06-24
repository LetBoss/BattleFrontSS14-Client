using System;
using System.Linq;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Robust.Shared.Console.Commands;

internal sealed class ListMapsCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly IEntityManager _entManager;

	[Dependency]
	private readonly IMapManager _map;

	[Dependency]
	private readonly SharedMapSystem _mapSystem;

	public override string Command => "lsmap";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (MapId item in from id in _mapSystem.GetAllMapIds()
			orderby id.Value
			select id)
		{
			if (_mapSystem.TryGetMap(item, out var uid))
			{
				_003C_003Ey__InlineArray6<object> buffer = default(_003C_003Ey__InlineArray6<object>);
				buffer[0] = item;
				buffer[1] = _entManager.GetComponent<MetaDataComponent>(uid.Value).EntityName;
				buffer[2] = _mapSystem.IsInitialized(uid);
				buffer[3] = _mapSystem.IsPaused(item);
				buffer[4] = _entManager.GetNetEntity(uid);
				buffer[5] = string.Join(",", from grid in _map.GetAllGrids(item)
					select grid.Owner);
				stringBuilder.AppendFormat("{0}: {1}, init: {2}, paused: {3}, nent: {4}, grids: {5}\n", (ReadOnlySpan<object?>)buffer);
			}
		}
		string text = stringBuilder.ToString();
		shell.WriteLine(text.Substring(0, text.Length - 1));
	}
}
