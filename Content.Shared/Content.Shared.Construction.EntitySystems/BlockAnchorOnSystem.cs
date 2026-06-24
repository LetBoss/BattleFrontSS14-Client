using System;
using Content.Shared.Construction.Components;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;

namespace Content.Shared.Construction.EntitySystems;

public sealed class BlockAnchorOnSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _xform;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BlockAnchorOnComponent, AnchorStateChangedEvent>((EntityEventRefHandler<BlockAnchorOnComponent, AnchorStateChangedEvent>)OnAnchorStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockAnchorOnComponent, AnchorAttemptEvent>((EntityEventRefHandler<BlockAnchorOnComponent, AnchorAttemptEvent>)OnAnchorAttempt, (Type[])null, (Type[])null);
	}

	private void OnAnchorStateChanged(Entity<BlockAnchorOnComponent> ent, ref AnchorStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (((AnchorStateChangedEvent)(ref args)).Anchored && HasOverlap(Entity<BlockAnchorOnComponent, TransformComponent>.op_Implicit((Entity<BlockAnchorOnComponent>.op_Implicit(ent), ent.Comp, ((EntitySystem)this).Transform(Entity<BlockAnchorOnComponent>.op_Implicit(ent))))))
		{
			_popup.PopupPredicted(base.Loc.GetString("anchored-already-present"), Entity<BlockAnchorOnComponent>.op_Implicit(ent), null);
			_xform.Unanchor(Entity<BlockAnchorOnComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<BlockAnchorOnComponent>.op_Implicit(ent)), true);
		}
	}

	private void OnAnchorAttempt(Entity<BlockAnchorOnComponent> ent, ref AnchorAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && HasOverlap(Entity<BlockAnchorOnComponent, TransformComponent>.op_Implicit((Entity<BlockAnchorOnComponent>.op_Implicit(ent), ent.Comp, ((EntitySystem)this).Transform(Entity<BlockAnchorOnComponent>.op_Implicit(ent))))))
		{
			_popup.PopupPredicted(base.Loc.GetString("anchored-already-present"), Entity<BlockAnchorOnComponent>.op_Implicit(ent), args.User);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private bool HasOverlap(Entity<BlockAnchorOnComponent, TransformComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gridUid = ent.Comp2.GridUid;
		if (gridUid.HasValue)
		{
			EntityUid grid = gridUid.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp))
			{
				Vector2i indices = _map.TileIndicesFor(grid, gridComp, ent.Comp2.Coordinates);
				AnchoredEntitiesEnumerator enumerator = _map.GetAnchoredEntitiesEnumerator(grid, gridComp, indices);
				EntityUid? otherEnt = default(EntityUid?);
				while (((AnchoredEntitiesEnumerator)(ref enumerator)).MoveNext(ref otherEnt))
				{
					gridUid = otherEnt;
					EntityUid val = Entity<BlockAnchorOnComponent, TransformComponent>.op_Implicit(ent);
					if ((!gridUid.HasValue || !(gridUid.GetValueOrDefault() == val)) && !_whitelist.CheckBoth(otherEnt, ent.Comp1.Blacklist, ent.Comp1.Whitelist))
					{
						return true;
					}
				}
				return false;
			}
		}
		return false;
	}
}
