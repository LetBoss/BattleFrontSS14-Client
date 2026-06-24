using System;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;

namespace Content.Shared._CIV14merka.Chat;

public abstract class CivRankSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, TransformSpeakerNameEvent>((EntityEventRefHandler<CivTeamMemberComponent, TransformSpeakerNameEvent>)OnTransformSpeakerName, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, ExaminedEvent>((EntityEventRefHandler<CivTeamMemberComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnTransformSpeakerName(Entity<CivTeamMemberComponent> ent, ref TransformSpeakerNameEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string rank = GetCiv14Rank(ent.Owner, ent.Comp);
		if (!string.IsNullOrEmpty(rank))
		{
			args.VoiceName = rank + " " + args.VoiceName;
		}
	}

	private void OnExamined(Entity<CivTeamMemberComponent> ent, ref ExaminedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			string rank = GetCiv14Rank(ent.Owner, ent.Comp);
			if (!string.IsNullOrEmpty(rank))
			{
				string pronoun = GetGenderPronoun(ent.Owner);
				args.PushMarkup(pronoun + " is " + rank);
			}
		}
	}

	private string GetGenderPronoun(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		HumanoidAppearanceComponent humanoid = default(HumanoidAppearanceComponent);
		if (!((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(uid, ref humanoid))
		{
			return "They";
		}
		Gender gender = humanoid.Gender;
		if ((int)gender != 2)
		{
			if ((int)gender == 3)
			{
				return "He";
			}
			return "They";
		}
		return "She";
	}

	protected abstract string? GetCiv14Rank(EntityUid source, CivTeamMemberComponent speakerTeam);
}
