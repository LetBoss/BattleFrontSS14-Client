using System;
using Content.Shared._CIV14merka.Medical;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Medical;

public sealed class CivRevivableIconSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _players;

	private static readonly ProtoId<HealthIconPrototype> IconId = ProtoId<HealthIconPrototype>.op_Implicit("HealthIconCivRevivable");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivDeathTimeComponent, GetStatusIconsEvent>((EntityEventRefHandler<CivDeathTimeComponent, GetStatusIconsEvent>)OnGetStatusIcons, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIcons(Entity<CivDeathTimeComponent> ent, ref GetStatusIconsEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_players).LocalEntity;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		CivTeamMemberComponent civTeamMemberComponent2 = default(CivTeamMemberComponent);
		MobStateComponent mobStateComponent = default(MobStateComponent);
		CivRevivedTrackerComponent civRevivedTrackerComponent = default(CivRevivedTrackerComponent);
		HealthIconPrototype item = default(HealthIconPrototype);
		if (localEntity.HasValue && ((EntitySystem)this).TryComp<CivTeamMemberComponent>(localEntity, ref civTeamMemberComponent) && civTeamMemberComponent.TeamId > 0 && ((EntitySystem)this).TryComp<CivTeamMemberComponent>(Entity<CivDeathTimeComponent>.op_Implicit(ent), ref civTeamMemberComponent2) && civTeamMemberComponent.TeamId == civTeamMemberComponent2.TeamId && ((EntitySystem)this).TryComp<MobStateComponent>(Entity<CivDeathTimeComponent>.op_Implicit(ent), ref mobStateComponent) && mobStateComponent.CurrentState == MobState.Dead && !(ent.Comp.DeathTime == TimeSpan.Zero) && !(_timing.CurTime - ent.Comp.DeathTime > TimeSpan.FromMinutes(4.0)) && (!((EntitySystem)this).TryComp<CivRevivedTrackerComponent>(Entity<CivDeathTimeComponent>.op_Implicit(ent), ref civRevivedTrackerComponent) || civRevivedTrackerComponent.RevivedCount < civRevivedTrackerComponent.MaxRevives) && _prototype.TryIndex<HealthIconPrototype>(IconId, ref item))
		{
			args.StatusIcons.Add(item);
		}
	}
}
