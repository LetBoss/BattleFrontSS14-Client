using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Medical.Defibrillator;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Access.Components;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Dogtags;

public sealed class DogtagsSystem : EntitySystem
{
	[Dependency]
	private SharedRottingSystem _rotting;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private MetaDataSystem _meta;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private RMCUnrevivableSystem _unrevivableSystem;

	private readonly EntProtoId<SkillDefinitionComponent> Skill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillPolice");

	private readonly int SkillRequired = 2;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TakeableTagsComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>((EntityEventRefHandler<TakeableTagsComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>)GetRelayedTags, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TakeableTagsComponent, GetVerbsEvent<EquipmentVerb>>((EntityEventRefHandler<TakeableTagsComponent, GetVerbsEvent<EquipmentVerb>>)OnGetVerbTags, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TakeableTagsComponent, ExaminedEvent>((EntityEventRefHandler<TakeableTagsComponent, ExaminedEvent>)OnTagsExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InformationTagsComponent, ExaminedEvent>((EntityEventRefHandler<InformationTagsComponent, ExaminedEvent>)OnInfoTagsExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InformationTagsComponent, AfterInteractEvent>((EntityEventRefHandler<InformationTagsComponent, AfterInteractEvent>)OnInfoTagsUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCMemorialComponent, ExaminedEvent>((EntityEventRefHandler<RMCMemorialComponent, ExaminedEvent>)OnMemorialExamined, (Type[])null, (Type[])null);
	}

	private void OnTagsExamine(Entity<TakeableTagsComponent> tags, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			GetTagInformation(Entity<TakeableTagsComponent>.op_Implicit(tags), out string name, out string job, out string blood);
			args.PushMarkup(base.Loc.GetString("rmc-dogtags-read", new(string, object)[3]
			{
				("name", name),
				("assignment", job),
				("bloodtype", blood)
			}));
		}
	}

	private void OnInfoTagsExamine(Entity<InformationTagsComponent> tags, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		args.PushMarkup(base.Loc.GetString("rmc-dogtags-info-read-start", (ValueTuple<string, object>)("tags", tags.Comp.Tags.Count)), -19);
		int count = 1;
		foreach (InfoTagInfo tag in tags.Comp.Tags)
		{
			args.PushMarkup(base.Loc.GetString("rmc-dogtags-info-read", new(string, object)[4]
			{
				("number", count++),
				("name", tag.Name),
				("assignment", tag.Assignment),
				("bloodtype", tag.BloodType)
			}), -19 - count);
		}
	}

	private void OnMemorialExamined(Entity<RMCMemorialComponent> memorial, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner) && memorial.Comp.Names.Count != 0)
		{
			string text = base.Loc.GetString("rmc-memorial-start") + " " + MemorialNamesFormat(memorial.Comp.Names);
			args.PushMarkup(text, -5);
		}
	}

	public string MemorialNamesFormat(List<string> memorialnames)
	{
		string list = "";
		int count = 1;
		foreach (string name in memorialnames)
		{
			list = ((count != memorialnames.Count) ? (list + name + ", ") : (list + name + "."));
			count++;
		}
		return list;
	}

	private void GetRelayedTags(Entity<TakeableTagsComponent> tags, ref InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnGetVerbTags(tags, ref args.Args);
	}

	private bool CanTakeTags(Entity<TakeableTagsComponent> tags, EntityUid wearer, EntityUid taker, out bool equipped, out string reason)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		equipped = true;
		reason = "";
		if (wearer == taker)
		{
			if (_hands.IsHolding(Entity<HandsComponent>.op_Implicit(taker), Entity<TakeableTagsComponent>.op_Implicit(tags)))
			{
				IdCardOwnerComponent cardOwner = default(IdCardOwnerComponent);
				if (((EntitySystem)this).TryComp<IdCardOwnerComponent>(Entity<TakeableTagsComponent>.op_Implicit(tags), ref cardOwner) && ((EntitySystem)this).Exists(cardOwner.Id))
				{
					if (cardOwner.Id == taker)
					{
						reason = base.Loc.GetString("rmc-dogtags-still-exists-self");
					}
					else
					{
						reason = base.Loc.GetString("rmc-dogtags-still-exists");
					}
					return false;
				}
				equipped = false;
				return true;
			}
			return false;
		}
		if (!_mob.IsDead(wearer))
		{
			reason = base.Loc.GetString("rmc-dogtags-still-alive");
			return false;
		}
		if (_rotting.IsRotten(wearer) || _unrevivableSystem.IsUnrevivable(wearer) || ((EntitySystem)this).HasComp<RMCDefibrillatorBlockedComponent>(wearer) || _skills.HasSkill(Entity<SkillsComponent>.op_Implicit(taker), Skill, SkillRequired))
		{
			return true;
		}
		reason = base.Loc.GetString("rmc-dogtags-can-be-saved");
		return false;
	}

	private void OnGetVerbTags(Entity<TakeableTagsComponent> tags, ref GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		if (!args.CanInteract || !args.CanAccess || args.Hands == null || tags.Comp.TagsTaken || ((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			return;
		}
		EntityUid wearer = ((EntitySystem)this).Transform(Entity<TakeableTagsComponent>.op_Implicit(tags)).ParentUid;
		EntityUid user = args.User;
		if (CanTakeTags(tags, wearer, user, out bool _, out string _))
		{
			EquipmentVerb verb = new EquipmentVerb
			{
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/_RMC14/Interface/VerbIcons/dogtag.png")),
				Text = base.Loc.GetString("rmc-dogtags-take")
			};
			verb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				TakeTags(tags, user, wearer);
			};
			args.Verbs.Add(verb);
		}
	}

	private void TakeTags(Entity<TakeableTagsComponent> tags, EntityUid user, EntityUid wearer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		bool equipped;
		string reason;
		if (tags.Comp.TagsTaken)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-dogtags-already-taken", (ValueTuple<string, object>)("target", wearer)), user);
		}
		else if (!CanTakeTags(tags, wearer, user, out equipped, out reason))
		{
			_popup.PopupClient(reason, user);
		}
		else
		{
			if (!_interaction.InRangeAndAccessible(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(wearer)))
			{
				return;
			}
			tags.Comp.TagsTaken = true;
			_appearance.SetData(Entity<TakeableTagsComponent>.op_Implicit(tags), (Enum)DogtagVisuals.Taken, (object)true, (AppearanceComponent)null);
			if (_net.IsClient)
			{
				return;
			}
			if (!equipped)
			{
				EntityUid prop = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(tags.Comp.FallenTag), user.ToCoordinates(), (ComponentRegistry)null);
				IdCardComponent id = default(IdCardComponent);
				if (((EntitySystem)this).TryComp<IdCardComponent>(Entity<TakeableTagsComponent>.op_Implicit(tags), ref id))
				{
					((EntitySystem)this).CopyComp<TakeableTagsComponent>(tags.Owner, prop, tags.Comp, (MetaDataComponent)null);
					((EntitySystem)this).CopyComp<IdCardComponent>(tags.Owner, prop, id, (MetaDataComponent)null);
				}
				((EntitySystem)this).QueueDel((EntityUid?)Entity<TakeableTagsComponent>.op_Implicit(tags));
			}
			EntityUid tag = ((EntitySystem)this).SpawnNextToOrDrop(EntProtoId.op_Implicit(tags.Comp.InfoTag), wearer, (TransformComponent)null, (ComponentRegistry)null);
			((EntitySystem)this).Dirty<TakeableTagsComponent>(tags, (MetaDataComponent)null);
			InformationTagsComponent informationTagsComponent = ((EntitySystem)this).EnsureComp<InformationTagsComponent>(tag);
			GetTagInformation(Entity<TakeableTagsComponent>.op_Implicit(tags), out string name, out string job, out string blood);
			InfoTagInfo tagInfo = new InfoTagInfo
			{
				Name = name,
				Assignment = job,
				BloodType = blood
			};
			informationTagsComponent.Tags.Add(tagInfo);
			_hands.TryPickupAnyHand(user, tag);
		}
	}

	private void OnInfoTagsUse(Entity<InformationTagsComponent> tags, ref AfterInteractEvent args)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Target.HasValue)
		{
			return;
		}
		InformationTagsComponent targTags = default(InformationTagsComponent);
		if (((EntitySystem)this).TryComp<InformationTagsComponent>(args.Target, ref targTags))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_meta.SetEntityName(args.Target.Value, base.Loc.GetString("rmc-dogtags-info-joined-name"), (MetaDataComponent)null, true);
			_meta.SetEntityDescription(args.Target.Value, base.Loc.GetString("rmc-dogtags-info-joined-desc"), (MetaDataComponent)null);
			if (!_net.IsClient)
			{
				string tagsJoinedString = ((tags.Comp.Tags.Count == 1 && targTags.Tags.Count == 1) ? "rmc-dogtags-single-join" : "rmc-dogtags-join");
				_popup.PopupEntity(base.Loc.GetString(tagsJoinedString), args.User, args.User);
				targTags.Tags.AddRange(tags.Comp.Tags);
				_appearance.SetData(args.Target.Value, (Enum)InfoTagVisuals.Number, (object)Math.Min(targTags.Tags.Count, targTags.MaxDisplayTags), (AppearanceComponent)null);
				((EntitySystem)this).QueueDel((EntityUid?)Entity<InformationTagsComponent>.op_Implicit(tags));
			}
		}
		else
		{
			RMCMemorialComponent memorial = default(RMCMemorialComponent);
			if (!((EntitySystem)this).TryComp<RMCMemorialComponent>(args.Target, ref memorial))
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			if (_net.IsClient)
			{
				return;
			}
			_popup.PopupEntity(base.Loc.GetString("rmc-memorial-add", (ValueTuple<string, object>)("tags", tags), (ValueTuple<string, object>)("slab", args.Target.Value)), args.User, args.User);
			foreach (InfoTagInfo tag in tags.Comp.Tags)
			{
				memorial.Names.Add(tag.Name);
			}
			((EntitySystem)this).QueueDel((EntityUid?)Entity<InformationTagsComponent>.op_Implicit(tags));
		}
	}

	private void GetTagInformation(EntityUid dogtag, out string name, out string job, out string bloodtype)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		name = base.Loc.GetString("rmc-dogtags-unknown");
		job = base.Loc.GetString("rmc-dogtags-unknown");
		bloodtype = "O-";
		IdCardComponent id = default(IdCardComponent);
		if (((EntitySystem)this).TryComp<IdCardComponent>(dogtag, ref id))
		{
			if (id.FullName != null)
			{
				name = id.FullName;
			}
			if (id.LocalizedJobTitle != null)
			{
				job = id.LocalizedJobTitle;
			}
		}
	}
}
