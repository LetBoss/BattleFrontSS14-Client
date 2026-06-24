using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Content.Shared._RMC14.Map;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Water;

public sealed class RMCWaterSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private NameModifierSystem _nameModifier;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private IGameTiming _timing;

	private readonly List<(EntityUid Id, TimeSpan SpreadAt)> _makeActive = new List<(EntityUid, TimeSpan)>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PurifiableWaterComponent, MapInitEvent>((EntityEventRefHandler<PurifiableWaterComponent, MapInitEvent>)OnPurifiableWaterMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PurifiableWaterComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<PurifiableWaterComponent, RefreshNameModifiersEvent>)OnPurifiableWaterRefreshNameModifiers, (Type[])null, (Type[])null);
	}

	private void OnPurifiableWaterMapInit(Entity<PurifiableWaterComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnPurifiableWaterRefreshNameModifiers(Entity<PurifiableWaterComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		string loc = (ent.Comp.Toxic ? "rmc-water-toxic-name" : "rmc-water-purified-name");
		args.AddModifier(LocId.op_Implicit(loc), 0);
	}

	private void UpdateAppearance(Entity<PurifiableWaterComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		PurifiableWaterVisuals visual = ((!ent.Comp.Toxic) ? PurifiableWaterVisuals.Purified : PurifiableWaterVisuals.Toxic);
		_appearance.SetData(ent.Owner, (Enum)PurifiableWaterLayers.Layer, (object)visual, (AppearanceComponent)null);
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
	}

	public bool CanCollide(Entity<RMCWaterComponent?> water, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RMCWaterComponent>(Entity<RMCWaterComponent>.op_Implicit(water), ref water.Comp, false))
		{
			return true;
		}
		EntityWhitelist cover = water.Comp.Cover;
		if (cover == null)
		{
			return true;
		}
		RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(Entity<RMCWaterComponent>.op_Implicit(water), null, (DirectionFlag)0);
		EntityUid anchoredId;
		while (anchored.MoveNext(out anchoredId))
		{
			if (_entityWhitelist.IsWhitelistPass(cover, anchoredId))
			{
				return false;
			}
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		_makeActive.Clear();
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveWaterComponent, PurifiableWaterComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveWaterComponent, PurifiableWaterComponent>();
		EntityUid uid = default(EntityUid);
		ActiveWaterComponent active = default(ActiveWaterComponent);
		PurifiableWaterComponent purifiable = default(PurifiableWaterComponent);
		PurifiableWaterComponent adjacentPurifiable = default(PurifiableWaterComponent);
		while (query.MoveNext(ref uid, ref active, ref purifiable))
		{
			if (time < active.SpreadAt)
			{
				continue;
			}
			ImmutableArray<Direction>.Enumerator enumerator = _rmcMap.CardinalDirections.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Direction cardinal = enumerator.Current;
				RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(uid, cardinal, (DirectionFlag)0);
				EntityUid adjacent;
				while (anchored.MoveNext(out adjacent))
				{
					if (((EntitySystem)this).TryComp<PurifiableWaterComponent>(adjacent, ref adjacentPurifiable) && adjacentPurifiable.Toxic != purifiable.Toxic)
					{
						adjacentPurifiable.Toxic = purifiable.Toxic;
						((EntitySystem)this).Dirty(adjacent, (IComponent)(object)adjacentPurifiable, (MetaDataComponent)null);
						UpdateAppearance(Entity<PurifiableWaterComponent>.op_Implicit((adjacent, adjacentPurifiable)));
						_makeActive.Add((adjacent, time + adjacentPurifiable.Delay));
					}
				}
			}
		}
		foreach (var item in _makeActive)
		{
			EntityUid id = item.Id;
			TimeSpan spreadAt = item.SpreadAt;
			ActiveWaterComponent adjacentActive = ((EntitySystem)this).EnsureComp<ActiveWaterComponent>(id);
			adjacentActive.SpreadAt = spreadAt;
			((EntitySystem)this).Dirty(id, (IComponent)(object)adjacentActive, (MetaDataComponent)null);
		}
	}
}
