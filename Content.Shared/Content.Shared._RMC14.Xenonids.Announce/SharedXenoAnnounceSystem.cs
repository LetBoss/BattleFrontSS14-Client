using System;
using System.Runtime.CompilerServices;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Bioscan;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Announce;

public abstract class SharedXenoAnnounceSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _areas;

	[Dependency]
	private XenoEvolutionSystem _xenoEvolution;

	[Dependency]
	protected SharedXenoHiveSystem Hive;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoAnnounceDeathComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoAnnounceDeathComponent, MobStateChangedEvent>)OnAnnounceDeathMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnAnnounceDeathMobStateChanged(Entity<XenoAnnounceDeathComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			string locationName = "Unknown";
			if (_areas.TryGetArea(Entity<XenoAnnounceDeathComponent>.op_Implicit(ent), out Entity<AreaComponent>? _, out EntityPrototype areaProto))
			{
				locationName = areaProto.Name;
			}
			if (((EntitySystem)this).HasComp<ParasiteSpentComponent>(Entity<XenoAnnounceDeathComponent>.op_Implicit(ent)))
			{
				Entity<HiveMemberComponent> xeno = Entity<HiveMemberComponent>.op_Implicit(ent.Owner);
				string message = base.Loc.GetString("rmc-xeno-parasite-announce-infect", (ValueTuple<string, object>)("xeno", ent.Owner), (ValueTuple<string, object>)("location", locationName));
				Color? color = ent.Comp.Color;
				AnnounceSameHive(xeno, message, null, null, color);
			}
			else if (((EntitySystem)this).HasComp<XenoEvolutionGranterComponent>(Entity<XenoAnnounceDeathComponent>.op_Implicit(ent)) || _xenoEvolution.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>?)null))
			{
				Entity<HiveMemberComponent> xeno2 = Entity<HiveMemberComponent>.op_Implicit(ent.Owner);
				string message2 = base.Loc.GetString(LocId.op_Implicit(ent.Comp.Message), (ValueTuple<string, object>)("xeno", ent.Owner), (ValueTuple<string, object>)("location", locationName));
				Color? color = ent.Comp.Color;
				AnnounceSameHive(xeno2, message2, null, null, color);
			}
		}
	}

	public string WrapHive(string message, Color? color = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Color value = color.GetValueOrDefault();
		if (!color.HasValue)
		{
			value = Color.FromHex((ReadOnlySpan<char>)"#921992", (Color?)null);
			color = value;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 2);
		defaultInterpolatedStringHandler.AppendLiteral("[color=");
		value = color.Value;
		defaultInterpolatedStringHandler.AppendFormatted(((Color)(ref value)).ToHex());
		defaultInterpolatedStringHandler.AppendLiteral("][font size=16][bold]");
		defaultInterpolatedStringHandler.AppendFormatted(message);
		defaultInterpolatedStringHandler.AppendLiteral("[/bold][/font][/color]\n\n");
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	public virtual void Announce(EntityUid source, Filter filter, string message, string wrapped, SoundSpecifier? sound = null, PopupType? popup = null, bool needsQueen = false)
	{
	}

	public void AnnounceToHive(EntityUid source, EntityUid hive, string message, SoundSpecifier? sound = null, PopupType? popup = null, Color? color = null, bool needsQueen = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Filter filter = Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => Hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(e), hive)));
		Announce(source, filter, message, WrapHive(message, color), sound, popup, needsQueen);
	}

	public void AnnounceSameHive(Entity<HiveMemberComponent?> xeno, string message, SoundSpecifier? sound = null, PopupType? popup = null, Color? color = null, bool needsQueen = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = Hive.GetHive(xeno);
		if (hive.HasValue)
		{
			Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
			AnnounceToHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveComponent>.op_Implicit(hive2), message, sound, popup, color, needsQueen);
		}
	}

	public void AnnounceSameHiveDefaultSound(Entity<HiveMemberComponent?> xeno, string message, PopupType? popup = null, Color? color = null, bool needsQueen = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		SoundSpecifier sound = new BioscanComponent().XenoSound;
		AnnounceSameHive(xeno, message, sound, popup, color, needsQueen);
	}

	public void AnnounceAll(EntityUid source, string message, SoundSpecifier? sound = null, PopupType? popup = null, bool needsQueen = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Announce(source, Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>)base.HasComp<XenoComponent>), message, message, sound, popup, needsQueen);
	}

	public void AnnounceQueenMother(string message)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		SoundSpecifier sound = new BioscanComponent().XenoSound;
		AnnounceAll(default(EntityUid), FormatQueenMother(message), sound);
	}

	public string FormatQueenMother(string message)
	{
		return "\n[bold][color=#7575F3][font size=24]Queen Mother Psychic Directive[/font][/color][/bold]\n\n[color=red][font size=14]" + message + "[/font][/color]\n\n";
	}
}
