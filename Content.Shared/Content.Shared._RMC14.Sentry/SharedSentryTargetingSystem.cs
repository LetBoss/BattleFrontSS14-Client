using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Inventory;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Sentry;

public abstract class SharedSentryTargetingSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private GunIFFSystem _iff;

	[Dependency]
	private SharedTransformSystem _xform;

	private const string SentryExcludedFaction = "RMCDumb";

	public static readonly Dictionary<string, EntProtoId<IFFFactionComponent>> SentryFactionToIff = new Dictionary<string, EntProtoId<IFFFactionComponent>>
	{
		{
			"UNMC",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionMarine")
		},
		{
			"CLF",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionCLF")
		},
		{
			"SPP",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionSPP")
		},
		{
			"Halcyon",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionHalcyon")
		},
		{
			"WeYa",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionWeYa")
		},
		{
			"Civilian",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionSurvivor")
		},
		{
			"RoyalMarines",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionRoyalMarines")
		},
		{
			"Bureau",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionBureau")
		},
		{
			"TSE",
			EntProtoId<IFFFactionComponent>.op_Implicit("FactionTSE")
		}
	};

	public static readonly HashSet<string> SentryAllowedFactions = SentryFactionToIff.Keys.ToHashSet();

	private readonly HashSet<EntProtoId<IFFFactionComponent>> _friendlyIffBuffer = new HashSet<EntProtoId<IFFFactionComponent>>();

	private readonly HashSet<EntProtoId<IFFFactionComponent>> _targetIffBuffer = new HashSet<EntProtoId<IFFFactionComponent>>();

	private readonly HashSet<Entity<NpcFactionMemberComponent>> _factionLookupBuffer = new HashSet<Entity<NpcFactionMemberComponent>>();

	private readonly HashSet<Entity<UserIFFComponent>> _userIffLookupBuffer = new HashSet<Entity<UserIFFComponent>>();

	private readonly HashSet<EntityUid> _candidateLookupBuffer = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SentryTargetingComponent, MapInitEvent>((EntityEventRefHandler<SentryTargetingComponent, MapInitEvent>)OnTargetingMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SentryTargetingComponent, ComponentStartup>((EntityEventRefHandler<SentryTargetingComponent, ComponentStartup>)OnTargetingStartup, (Type[])null, (Type[])null);
	}

	private void OnTargetingMapInit(Entity<SentryTargetingComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		NpcFactionMemberComponent factionMember = default(NpcFactionMemberComponent);
		if (((EntitySystem)this).TryComp<NpcFactionMemberComponent>(ent.Owner, ref factionMember) && factionMember.Factions.Count > 0)
		{
			ent.Comp.OriginalFaction = ProtoId<NpcFactionPrototype>.op_Implicit(factionMember.Factions.First());
		}
		if (!((EntitySystem)this).HasComp<GunIFFComponent>(ent.Owner) && ((EntitySystem)this).HasComp<GunComponent>(ent.Owner))
		{
			_iff.EnableIntrinsicIFF(Entity<SentryTargetingComponent>.op_Implicit(ent));
		}
	}

	private void OnTargetingStartup(Entity<SentryTargetingComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.FriendlyFactions.Count == 0 && !string.IsNullOrEmpty(ent.Comp.OriginalFaction))
		{
			ent.Comp.FriendlyFactions.Add(ent.Comp.OriginalFaction);
		}
		if (_net.IsServer)
		{
			ApplyTargeting(ent);
		}
	}

	public void ApplyDeployerFactions(EntityUid sentry, EntityUid deployer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		SentryTargetingComponent targeting = ((EntitySystem)this).EnsureComp<SentryTargetingComponent>(sentry);
		targeting.FriendlyFactions.Clear();
		targeting.HumanoidAdded.Clear();
		HashSet<EntProtoId<IFFFactionComponent>> iffFactions = new HashSet<EntProtoId<IFFFactionComponent>>();
		GetIFFFactionEvent ev = new GetIFFFactionEvent(SlotFlags.BELT | SlotFlags.IDCARD | SlotFlags.POCKET, iffFactions);
		((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(deployer, ref ev, false);
		NpcFactionMemberComponent npcFaction = default(NpcFactionMemberComponent);
		if (iffFactions.Count > 0)
		{
			foreach (var (sentryFaction, iffFaction) in SentryFactionToIff)
			{
				if (iffFactions.Contains(iffFaction))
				{
					targeting.FriendlyFactions.Add(sentryFaction);
				}
			}
		}
		else if (((EntitySystem)this).TryComp<NpcFactionMemberComponent>(deployer, ref npcFaction))
		{
			foreach (ProtoId<NpcFactionPrototype> faction in npcFaction.Factions)
			{
				if (faction != ProtoId<NpcFactionPrototype>.op_Implicit("RMCDumb") && SentryAllowedFactions.Contains(ProtoId<NpcFactionPrototype>.op_Implicit(faction)))
				{
					targeting.FriendlyFactions.Add(ProtoId<NpcFactionPrototype>.op_Implicit(faction));
				}
			}
			if (npcFaction.Factions.Count > 0)
			{
				targeting.OriginalFaction = ProtoId<NpcFactionPrototype>.op_Implicit(npcFaction.Factions.First());
			}
		}
		targeting.DeployedFriendlyFactions.Clear();
		targeting.DeployedFriendlyFactions.UnionWith(targeting.FriendlyFactions);
		if (_net.IsServer)
		{
			ApplyTargeting(Entity<SentryTargetingComponent>.op_Implicit((sentry, targeting)));
		}
		((EntitySystem)this).Dirty(sentry, (IComponent)(object)targeting, (MetaDataComponent)null);
	}

	public void SetFriendlyFactions(Entity<SentryTargetingComponent> ent, HashSet<string> factions)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.FriendlyFactions.Clear();
		ent.Comp.HumanoidAdded.Clear();
		HashSet<string> friendly = factions.Where((string f) => f != "RMCDumb" && f != "Humanoid" && SentryAllowedFactions.Contains(f)).ToHashSet();
		if (factions.Contains("Humanoid"))
		{
			foreach (string faction in GetHumanoidFactions())
			{
				if (friendly.Add(faction))
				{
					ent.Comp.HumanoidAdded.Add(faction);
				}
			}
		}
		ent.Comp.FriendlyFactions.UnionWith(friendly);
		if (_net.IsServer)
		{
			ApplyTargeting(ent);
		}
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	public void ResetToDefault(Entity<SentryTargetingComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.FriendlyFactions.Clear();
		ent.Comp.HumanoidAdded.Clear();
		if (ent.Comp.DeployedFriendlyFactions.Count > 0)
		{
			ent.Comp.FriendlyFactions.UnionWith(ent.Comp.DeployedFriendlyFactions);
		}
		if (_net.IsServer)
		{
			ApplyTargeting(ent);
		}
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	public void ToggleFaction(Entity<SentryTargetingComponent> ent, string faction, bool friendly)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (faction == "RMCDumb")
		{
			return;
		}
		if (faction == "Humanoid")
		{
			ToggleHumanoid(ent, friendly);
			if (_net.IsServer)
			{
				ApplyTargeting(ent);
			}
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			return;
		}
		if (friendly)
		{
			ent.Comp.FriendlyFactions.Add(faction);
		}
		else
		{
			ent.Comp.FriendlyFactions.Remove(faction);
		}
		if (_net.IsServer)
		{
			ApplyTargeting(ent);
		}
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
	}

	public void ToggleHumanoid(Entity<SentryTargetingComponent> ent, bool friendly)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (friendly)
		{
			foreach (string faction in GetHumanoidFactions())
			{
				if (ent.Comp.FriendlyFactions.Add(faction))
				{
					ent.Comp.HumanoidAdded.Add(faction);
				}
			}
			return;
		}
		foreach (string faction2 in ent.Comp.HumanoidAdded)
		{
			ent.Comp.FriendlyFactions.Remove(faction2);
		}
		ent.Comp.HumanoidAdded.Clear();
	}

	private void BuildFriendlyIff(SentryTargetingComponent comp)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		_friendlyIffBuffer.Clear();
		foreach (string faction in comp.FriendlyFactions)
		{
			if (SentryFactionToIff.TryGetValue(faction, out EntProtoId<IFFFactionComponent> iff))
			{
				_friendlyIffBuffer.Add(iff);
			}
		}
	}

	private bool IsFriendlyByIff(EntityUid target)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		_targetIffBuffer.Clear();
		GetIFFFactionEvent ev = new GetIFFFactionEvent(SlotFlags.IDCARD, _targetIffBuffer);
		((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(target, ref ev, false);
		foreach (EntProtoId<IFFFactionComponent> faction in _targetIffBuffer)
		{
			if (_friendlyIffBuffer.Contains(faction))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsValidTarget(Entity<SentryTargetingComponent> sentry, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<UserIFFComponent>(target) && !((EntitySystem)this).HasComp<NpcFactionMemberComponent>(target))
		{
			return false;
		}
		BuildFriendlyIff(sentry.Comp);
		bool num = IsFriendlyByIff(target);
		_friendlyIffBuffer.Clear();
		_targetIffBuffer.Clear();
		return !num;
	}

	public IEnumerable<EntityUid> GetNearbyIffHostiles(Entity<SentryTargetingComponent> ent, float range)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		BuildFriendlyIff(ent.Comp);
		MapCoordinates coords = _xform.GetMapCoordinates(Entity<SentryTargetingComponent>.op_Implicit(ent), (TransformComponent)null);
		_candidateLookupBuffer.Clear();
		_lookup.GetEntitiesInRange<UserIFFComponent>(coords, range, _userIffLookupBuffer, (LookupFlags)110);
		foreach (Entity<UserIFFComponent> target in _userIffLookupBuffer)
		{
			_candidateLookupBuffer.Add(target.Owner);
		}
		_lookup.GetEntitiesInRange<NpcFactionMemberComponent>(coords, range, _factionLookupBuffer, (LookupFlags)110);
		foreach (Entity<NpcFactionMemberComponent> target2 in _factionLookupBuffer)
		{
			_candidateLookupBuffer.Add(target2.Owner);
		}
		foreach (EntityUid target3 in _candidateLookupBuffer)
		{
			if (!(target3 == ent.Owner) && !IsFriendlyByIff(target3))
			{
				yield return target3;
			}
		}
		_candidateLookupBuffer.Clear();
		_userIffLookupBuffer.Clear();
		_factionLookupBuffer.Clear();
		_friendlyIffBuffer.Clear();
		_targetIffBuffer.Clear();
	}

	private void ApplyTargeting(Entity<SentryTargetingComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateSentryIFF(ent);
	}

	private void UpdateSentryIFF(Entity<SentryTargetingComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		UserIFFComponent userIff = default(UserIFFComponent);
		if (!((EntitySystem)this).TryComp<UserIFFComponent>(ent.Owner, ref userIff))
		{
			return;
		}
		_iff.ClearUserFactions(Entity<UserIFFComponent>.op_Implicit((ent.Owner, userIff)));
		foreach (string faction in ent.Comp.FriendlyFactions)
		{
			if (SentryFactionToIff.TryGetValue(faction, out EntProtoId<IFFFactionComponent> iff))
			{
				_iff.AddUserFaction(Entity<UserIFFComponent>.op_Implicit((ent.Owner, userIff)), iff);
			}
		}
	}

	public IEnumerable<string> GetHumanoidFactions()
	{
		return SentryAllowedFactions;
	}

	public bool ContainsAllNonXeno(HashSet<string> friendlyFactions)
	{
		return GetHumanoidFactions().All(friendlyFactions.Contains);
	}
}
