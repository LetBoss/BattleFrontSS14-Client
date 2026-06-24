using System;
using System.Collections.Generic;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics;

namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarSetAlphaOverlay : Overlay
{
	private struct QueryState
	{
		public HashSet<EntityUid> Seen;

		public EntityQuery<SpriteComponent> SpriteQuery;

		public SpriteSystem SpriteSystem;

		public PubgFogOfWarHideSystem HideSystem;
	}

	[Dependency]
	private IEntityManager _ent;

	private readonly PubgFogOfWarHideSystem _hide;

	private readonly PubgFogOfWarOccludableTreeSystem _tree;

	private readonly SpriteSystem _sprite;

	private readonly EntityQuery<SpriteComponent> _spriteQuery;

	private readonly HashSet<EntityUid> _seen = new HashSet<EntityUid>();

	private bool _ready;

	public override OverlaySpace Space => (OverlaySpace)64;

	private static bool QueryCallback(ref QueryState state, in ComponentTreeEntry<PubgFogOfWarOccludableComponent> entry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = entry.Uid;
		if (!state.Seen.Add(uid))
		{
			return true;
		}
		SpriteComponent val = default(SpriteComponent);
		if (!state.SpriteQuery.TryComp(uid, ref val))
		{
			return true;
		}
		float targetAlpha = state.HideSystem.GetTargetAlpha(uid, val, entry.Transform);
		if (MathF.Abs(val.Color.A - targetAlpha) <= 0.001f)
		{
			return true;
		}
		Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((uid, val));
		state.HideSystem.CachedBaseAlphas.Add((val2, val.Color.A));
		SpriteSystem spriteSystem = state.SpriteSystem;
		Color color = val.Color;
		spriteSystem.SetColor(val2, ((Color)(ref color)).WithAlpha(targetAlpha));
		return true;
	}

	public PubgFogOfWarSetAlphaOverlay()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<PubgFogOfWarSetAlphaOverlay>(this);
		_hide = _ent.System<PubgFogOfWarHideSystem>();
		_tree = _ent.System<PubgFogOfWarOccludableTreeSystem>();
		_sprite = _ent.System<SpriteSystem>();
		_spriteQuery = _ent.GetEntityQuery<SpriteComponent>();
	}

	protected override bool BeforeDraw(in OverlayDrawArgs args)
	{
		_ready = _hide.TryPrepare();
		if (!_ready)
		{
			return false;
		}
		_hide.CachedBaseAlphas.Clear();
		_seen.Clear();
		return true;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (_ready)
		{
			QueryState queryState = new QueryState
			{
				Seen = _seen,
				SpriteQuery = _spriteQuery,
				SpriteSystem = _sprite,
				HideSystem = _hide
			};
			((ComponentTreeSystem<PubgFogOfWarOccludableTreeComponent, PubgFogOfWarOccludableComponent>)(object)_tree).QueryAabb<QueryState>(ref queryState, (QueryCallbackDelegate<ComponentTreeEntry<PubgFogOfWarOccludableComponent>, QueryState>)QueryCallback, args.MapId, args.WorldBounds, true);
		}
	}
}
