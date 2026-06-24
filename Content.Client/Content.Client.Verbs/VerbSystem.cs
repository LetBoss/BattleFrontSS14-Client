using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Popups;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.LandMines;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Client.ComponentTrees;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Verbs;

public sealed class VerbSystem : SharedVerbSystem
{
	[Dependency]
	private PopupSystem _popupSystem;

	[Dependency]
	private ExamineSystem _examine;

	[Dependency]
	private SpriteTreeSystem _tree;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private EntityLookupSystem _lookup;

	private float _lookupSize;

	private static readonly ProtoId<TagPrototype> HideContextMenuTag = ProtoId<TagPrototype>.op_Implicit("HideContextMenu");

	public MenuVisibility Visibility;

	public Action<VerbsResponseEvent>? OnVerbsResponse;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<VerbsResponseEvent>((EntityEventHandler<VerbsResponseEvent>)HandleVerbResponse, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _cfg, CCVars.GameEntityMenuLookup, (Action<float>)OnLookupChanged, true);
	}

	private void OnLookupChanged(float val)
	{
		_lookupSize = val;
	}

	public bool TryGetEntityMenuEntities(MapCoordinates targetPos, [NotNullWhen(true)] out List<EntityUid>? entities)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		entities = null;
		if (!(_stateManager.CurrentState is GameplayStateBase))
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid player = localEntity.GetValueOrDefault();
			MenuVisibility visibility = (_eyeManager.CurrentEye.DrawFov ? Visibility : (Visibility | MenuVisibility.NoFov));
			MenuVisibilityEvent menuVisibilityEvent = new MenuVisibilityEvent
			{
				TargetPos = targetPos,
				Visibility = visibility
			};
			((EntitySystem)this).RaiseLocalEvent<MenuVisibilityEvent>(player, ref menuVisibilityEvent, false);
			visibility = menuVisibilityEvent.Visibility;
			Box2 val = Box2.CenteredAround(targetPos.Position, new Vector2(_lookupSize, _lookupSize));
			HashSet<ComponentTreeEntry<SpriteComponent>> hashSet = ((ComponentTreeSystem<SpriteTreeComponent, SpriteComponent>)(object)_tree).QueryAabb(targetPos.MapId, val, true);
			entities = new List<EntityUid>(hashSet.Count);
			foreach (ComponentTreeEntry<SpriteComponent> item in hashSet)
			{
				entities.Add(item.Uid);
			}
			BaseContainer val2 = default(BaseContainer);
			BaseContainer val3 = default(BaseContainer);
			if (_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(player, null)), ref val2) && (entities.Contains(val2.Owner) || (_containers.TryGetOuterContainer(val2.Owner, ((EntitySystem)this).Transform(val2.Owner), ref val3) && entities.Contains(val3.Owner))))
			{
				if (!entities.Contains(val2.Owner))
				{
					entities.Add(val2.Owner);
				}
				foreach (EntityUid containedEntity in val2.ContainedEntities)
				{
					if (!entities.Contains(containedEntity))
					{
						entities.Add(containedEntity);
					}
				}
			}
			if ((visibility & MenuVisibility.InContainer) != MenuVisibility.Default)
			{
				LookupFlags val4 = (LookupFlags)46;
				foreach (EntityUid item2 in _lookup.GetEntitiesInRange(targetPos, _lookupSize, val4))
				{
					if (!entities.Contains(item2))
					{
						entities.Add(item2);
					}
				}
			}
			if ((visibility & MenuVisibility.NoFov) == 0)
			{
				ExaminerComponent examinerComp = default(ExaminerComponent);
				((EntitySystem)this).TryComp<ExaminerComponent>(player, ref examinerComp);
				for (int num = entities.Count - 1; num >= 0; num--)
				{
					if (!_examine.CanExamine(player, targetPos, (EntityUid e) => e == player, entities[num], examinerComp))
					{
						Extensions.RemoveSwap<EntityUid>((IList<EntityUid>)entities, num);
					}
				}
			}
			if ((visibility & MenuVisibility.Invisible) != MenuVisibility.Default)
			{
				return entities.Count != 0;
			}
			for (int num2 = entities.Count - 1; num2 >= 0; num2--)
			{
				if (_tagSystem.HasTag(entities[num2], HideContextMenuTag))
				{
					Extensions.RemoveSwap<EntityUid>((IList<EntityUid>)entities, num2);
				}
			}
			if (!((EntitySystem)this).HasComp<GhostComponent>(player))
			{
				for (int num3 = entities.Count - 1; num3 >= 0; num3--)
				{
					if (((EntitySystem)this).HasComp<LandMineComponent>(entities[num3]))
					{
						Extensions.RemoveSwap<EntityUid>((IList<EntityUid>)entities, num3);
					}
				}
			}
			if (val2 == null && (visibility & MenuVisibility.InContainer) == 0)
			{
				return entities.Count != 0;
			}
			EntityQuery<SpriteComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
			SpriteComponent val5 = default(SpriteComponent);
			for (int num4 = entities.Count - 1; num4 >= 0; num4--)
			{
				if (!entityQuery.TryGetComponent(entities[num4], ref val5) || !val5.Visible)
				{
					Extensions.RemoveSwap<EntityUid>((IList<EntityUid>)entities, num4);
				}
			}
			return entities.Count != 0;
		}
		return false;
	}

	public SortedSet<Verb> GetVerbs(NetEntity target, EntityUid user, List<Type> verbTypes, out List<VerbCategory> extraCategories, bool force = false)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!((NetEntity)(ref target)).IsClientSide())
		{
			NetEntity entityUid = target;
			bool adminRequest = force;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestServerVerbsEvent(entityUid, verbTypes, null, adminRequest));
		}
		EntityUid? val = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(target, ref val))
		{
			extraCategories = new List<VerbCategory>();
			return new SortedSet<Verb>();
		}
		return GetLocalVerbs(val.Value, user, verbTypes, out extraCategories, force);
	}

	public void ExecuteVerb(EntityUid target, Verb verb)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		ExecuteVerb(((EntitySystem)this).GetNetEntity(target, (MetaDataComponent)null), verb);
	}

	public void ExecuteVerb(NetEntity target, Verb verb)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (verb.Disabled)
		{
			if (!string.IsNullOrWhiteSpace(verb.Message))
			{
				_popupSystem.PopupEntity(FormattedMessage.RemoveMarkupOrThrow(verb.Message), valueOrDefault);
			}
		}
		else if (verb.ClientExclusive || ((NetEntity)(ref target)).IsClientSide())
		{
			ExecuteVerb(verb, valueOrDefault, ((EntitySystem)this).GetEntity(target));
		}
		else
		{
			((EntitySystem)this).RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(target, verb));
		}
	}

	private void HandleVerbResponse(VerbsResponseEvent msg)
	{
		OnVerbsResponse?.Invoke(msg);
	}
}
