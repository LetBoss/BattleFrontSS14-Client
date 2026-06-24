using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;

namespace Robust.Shared.Console.Commands;

public sealed class TeleportToCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly ISharedPlayerManager _players;

	[Dependency]
	private readonly IEntityManager _entities;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	public override string Command => "tpto";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length == 0)
		{
			return;
		}
		string str = args[0];
		if (!TryGetTransformFromUidOrUsername(str, shell, out EntityUid? victimUid, out TransformComponent _))
		{
			return;
		}
		EntityCoordinates coordinates = new EntityCoordinates(victimUid.Value, Vector2.Zero);
		if (_entities.TryGetComponent<PhysicsComponent>(victimUid, out PhysicsComponent component))
		{
			coordinates = coordinates.Offset(component.LocalCenter);
		}
		List<(EntityUid, TransformComponent)> list = new List<(EntityUid, TransformComponent)>();
		if (args.Length == 1)
		{
			EntityUid? uid = shell.Player?.AttachedEntity;
			if (!_entities.TryGetComponent<TransformComponent>(uid, out TransformComponent component2))
			{
				shell.WriteError(base.Loc.GetString("cmd-failure-no-attached-entity"));
				return;
			}
			list.Add((uid.Value, component2));
		}
		else
		{
			foreach (string str2 in args)
			{
				if (TryGetTransformFromUidOrUsername(str2, shell, out EntityUid? victimUid2, out TransformComponent transform2) && !(victimUid2 == victimUid))
				{
					list.Add((victimUid2.Value, transform2));
				}
			}
		}
		MapCoordinates coordinates2 = _transform.ToMapCoordinates(coordinates);
		foreach (var item in list)
		{
			_transform.SetMapCoordinates(item.Item1, coordinates2);
			_transform.AttachToGridOrMap(item.Item1, item.Item2);
		}
	}

	private bool TryGetTransformFromUidOrUsername(string str, IConsoleShell shell, [NotNullWhen(true)] out EntityUid? victimUid, [NotNullWhen(true)] out TransformComponent? transform)
	{
		if (NetEntity.TryParse(str.AsSpan(), out var entity) && _entities.TryGetEntity(entity, out var entity2) && _entities.TryGetComponent<TransformComponent>(entity2, out transform) && !_entities.HasComponent<MapComponent>(entity2))
		{
			victimUid = entity2;
			return true;
		}
		if (_players.TryGetSessionByUsername(str, out ICommonSession session) && _entities.TryGetComponent<TransformComponent>(session.AttachedEntity, out transform))
		{
			victimUid = session.AttachedEntity;
			return true;
		}
		shell.WriteError(base.Loc.GetString("cmd-tpto-parse-error", ("str", str)));
		transform = null;
		victimUid = null;
		return false;
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 0)
		{
			return CompletionResult.Empty;
		}
		string last = args[^1];
		IEnumerable<string> options = from x in _players.Sessions
			select x.Name ?? string.Empty into x
			where !string.IsNullOrWhiteSpace(x) && x.StartsWith(last, StringComparison.CurrentCultureIgnoreCase)
			select x;
		string messageId = ((args.Length == 1) ? "cmd-tpto-destination-hint" : "cmd-tpto-victim-hint");
		messageId = base.Loc.GetString(messageId);
		return CompletionResult.FromHintOptions(options, messageId);
	}
}
