using System;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Examine.Pose;

public abstract class SharedRMCSetPoseSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCSetPoseComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<RMCSetPoseComponent, GetVerbsEvent<Verb>>)OnSetPoseGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSetPoseComponent, ExaminedEvent>((EntityEventRefHandler<RMCSetPoseComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSetPoseComponent, MobStateChangedEvent>((EntityEventRefHandler<RMCSetPoseComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnSetPoseGetVerbs(Entity<RMCSetPoseComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		if (args.CanInteract && !(args.User != args.Target))
		{
			Verb verb = new Verb
			{
				Text = base.Loc.GetString("rmc-set-pose-title"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/character.svg.192dpi.png")),
				Priority = -5,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					SetPose(ent);
				}
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnExamine(Entity<RMCSetPoseComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		RMCSetPoseComponent comp = ent.Comp;
		if (comp.Pose.Trim() == string.Empty)
		{
			return;
		}
		using (args.PushGroup("RMCSetPoseComponent"))
		{
			string pose = base.Loc.GetString("rmc-set-pose-examined", (ValueTuple<string, object>)("ent", ent), (ValueTuple<string, object>)("pose", FormattedMessage.EscapeText(comp.Pose)));
			args.PushMarkup(pose, -5);
		}
	}

	private void OnMobStateChanged(Entity<RMCSetPoseComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Alive)
		{
			ent.Comp.Pose = string.Empty;
			((EntitySystem)this).Dirty<RMCSetPoseComponent>(ent, (MetaDataComponent)null);
		}
	}

	protected virtual void SetPose(Entity<RMCSetPoseComponent> ent)
	{
	}
}
