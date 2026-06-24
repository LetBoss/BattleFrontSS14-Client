using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Commendations;

public abstract class SharedCommendationSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	protected readonly List<Commendation> RoundCommendations = new List<Commendation>();

	public int CharacterLimit { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CommendationReceiverComponent, PlayerAttachedEvent>((EntityEventRefHandler<CommendationReceiverComponent, PlayerAttachedEvent>)OnCommendationReceiverPlayerAttached, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCCommendationMaxLength, (Action<int>)delegate(int v)
		{
			CharacterLimit = v;
		}, true);
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		RoundCommendations.Clear();
	}

	private void OnCommendationReceiverPlayerAttached(Entity<CommendationReceiverComponent> ent, ref PlayerAttachedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.LastPlayerId = args.Player.UserId.UserId.ToString();
	}

	public bool ValidCommendation(Entity<CommendationGiverComponent?, ActorComponent?> giver, Entity<CommendationReceiverComponent?> receiver, string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CommendationGiverComponent, ActorComponent>(Entity<CommendationGiverComponent, ActorComponent>.op_Implicit(giver), ref giver.Comp1, ref giver.Comp2, false) || !((EntitySystem)this).Resolve<CommendationReceiverComponent>(Entity<CommendationReceiverComponent>.op_Implicit(receiver), ref receiver.Comp, false) || receiver.Comp.LastPlayerId == null)
		{
			return false;
		}
		text = text.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		return true;
	}

	public virtual void GiveCommendation(Entity<CommendationGiverComponent?, ActorComponent?> giver, Entity<CommendationReceiverComponent?> receiver, string name, string text, CommendationType type)
	{
	}

	public virtual void GiveCommendationByLastPlayerId(Entity<CommendationGiverComponent?, ActorComponent?> giver, string lastPlayerId, string receiverName, string name, string text, CommendationType type)
	{
	}

	public IReadOnlyList<Commendation> GetCommendations()
	{
		return RoundCommendations;
	}
}
