using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;

namespace Robust.Shared.Console.Commands;

internal sealed class ListGridsCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly SharedTransformSystem _transformSystem;

	public override string Command => "lsgrid";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		EntityQuery<TransformComponent> entityQuery = EntityManager.GetEntityQuery<TransformComponent>();
		List<(EntityUid Uid, MapGridComponent Component)> list = EntityManager.AllComponentsList<MapGridComponent>();
		list.Sort(((EntityUid Uid, MapGridComponent Component) x, (EntityUid Uid, MapGridComponent Component) y) => x.Uid.CompareTo(y.Uid));
		foreach (var item2 in list)
		{
			EntityUid item = item2.Uid;
			TransformComponent component = entityQuery.GetComponent(item);
			Vector2 worldPosition = _transformSystem.GetWorldPosition(component);
			_003C_003Ey__InlineArray5<object> buffer = default(_003C_003Ey__InlineArray5<object>);
			buffer[0] = item;
			buffer[1] = component.MapID;
			buffer[2] = item;
			buffer[3] = worldPosition.X;
			buffer[4] = worldPosition.Y;
			stringBuilder.AppendFormat("{0}: map: {1}, ent: {2}, pos: {3:0.0},{4:0.0} \n", (ReadOnlySpan<object?>)buffer);
		}
		string text = stringBuilder.ToString();
		shell.WriteLine(text.Substring(0, text.Length - 1));
	}
}
