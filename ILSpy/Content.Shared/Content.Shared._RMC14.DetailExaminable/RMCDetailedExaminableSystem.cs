using System;
using System.Collections.Generic;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DetailExaminable;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.DetailExaminable;

public sealed class RMCDetailedExaminableSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	private readonly List<Entity<DetailExaminableComponent>> _queue = new List<Entity<DetailExaminableComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DetailExaminableComponent, MapInitEvent>((EntityEventRefHandler<DetailExaminableComponent, MapInitEvent>)OnDetailExaminableMapInit, (Type[])null, (Type[])null);
	}

	private void OnRoundRestart(RoundRestartCleanupEvent ev)
	{
		_queue.Clear();
	}

	private void OnDetailExaminableMapInit(Entity<DetailExaminableComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_queue.Add(ent);
	}

	public override void Update(float frameTime)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (Entity<DetailExaminableComponent> ent in _queue)
			{
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(36, 2);
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DetailExaminableComponent>.op_Implicit(ent), (MetaDataComponent)null), "player", "ToPrettyString(ent)");
				handler.AppendLiteral(" had a character description added:\n");
				handler.AppendFormatted(ent.Comp.Content, 0, "description");
				adminLog.Add(LogType.RMCCharacterDescription, ref handler);
			}
		}
		finally
		{
			_queue.Clear();
		}
	}
}
