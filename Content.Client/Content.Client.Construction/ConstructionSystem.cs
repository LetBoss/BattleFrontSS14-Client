using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Client.Popups;
using Content.Shared._RMC14.Construction;
using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Construction.Steps;
using Content.Shared.Examine;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Wall;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Construction;

public sealed class ConstructionSystem : SharedConstructionSystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private ExamineSystemShared _examineSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private PopupSystem _popupSystem;

	[Dependency]
	private RMCConstructionSystem _rmcConstruction;

	private readonly Dictionary<int, EntityUid> _ghosts = new Dictionary<int, EntityUid>();

	private readonly Dictionary<string, ConstructionGuide> _guideCache = new Dictionary<string, ConstructionGuide>();

	private readonly Dictionary<string, string> _recipesMetadataCache = new Dictionary<string, string>();

	public bool CraftingEnabled { get; private set; }

	public event EventHandler<CraftingAvailabilityChangedArgs>? CraftingAvailabilityChanged;

	public event EventHandler<string>? ConstructionGuideAvailable;

	public event EventHandler? ToggleCraftingWindow;

	public event EventHandler? FlipConstructionPrototype;

	public override void Initialize()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		WarmupRecipesCache();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerAttachedEvent>((EntityEventHandler<LocalPlayerAttachedEvent>)HandlePlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<AckStructureConstructionMessage>((EntityEventHandler<AckStructureConstructionMessage>)HandleAckStructure, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<ResponseConstructionGuide>((EntityEventHandler<ResponseConstructionGuide>)OnConstructionGuideReceived, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCraftingMenu, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleOpenCraftingMenu), true, true)).Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleUse), true, true)).Bind(ContentKeyFunctions.EditorFlipObject, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleFlip), true, true))
			.Register<ConstructionSystem>();
		((EntitySystem)this).SubscribeLocalEvent<ConstructionGhostComponent, ExaminedEvent>((ComponentEventHandler<ConstructionGhostComponent, ExaminedEvent>)HandleConstructionGhostExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConstructionGhostComponent, ComponentShutdown>((ComponentEventHandler<ConstructionGhostComponent, ComponentShutdown>)HandleGhostComponentShutdown, (Type[])null, (Type[])null);
	}

	private void HandleGhostComponentShutdown(EntityUid uid, ConstructionGhostComponent component, ComponentShutdown args)
	{
		ClearGhost(component.GhostId);
	}

	public bool TryGetRecipePrototype(string constructionProtoId, [NotNullWhen(true)] out string? targetProtoId)
	{
		if (_recipesMetadataCache.TryGetValue(constructionProtoId, out targetProtoId))
		{
			return true;
		}
		targetProtoId = null;
		return false;
	}

	private void WarmupRecipesCache()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		ConstructionGraphPrototype constructionGraphPrototype = default(ConstructionGraphPrototype);
		ConstructionPrototype constructionPrototype = default(ConstructionPrototype);
		EntityPrototype val = default(EntityPrototype);
		string text = default(string);
		string text2 = default(string);
		foreach (ConstructionPrototype item in PrototypeManager.EnumeratePrototypes<ConstructionPrototype>())
		{
			if (!PrototypeManager.TryIndex<ConstructionGraphPrototype>(item.Graph, ref constructionGraphPrototype))
			{
				continue;
			}
			string targetNode = item.TargetNode;
			if (targetNode == null || !constructionGraphPrototype.Nodes.TryGetValue(targetNode, out ConstructionGraphNode value))
			{
				continue;
			}
			Stack<ConstructionGraphNode> stack = new Stack<ConstructionGraphNode>();
			stack.Push(value);
			do
			{
				ConstructionGraphNode constructionGraphNode = stack.Pop();
				string id = constructionGraphNode.Entity.GetId(null, null, new GraphNodeEntityArgs((IEntityManager)(object)((EntitySystem)this).EntityManager));
				if (id == null)
				{
					if (stack.Count != 0)
					{
						continue;
					}
					foreach (ConstructionGraphEdge edge in constructionGraphNode.Edges)
					{
						if (constructionGraphPrototype.Nodes.TryGetValue(edge.Target, out ConstructionGraphNode value2))
						{
							stack.Push(value2);
						}
					}
				}
				else
				{
					stack.Clear();
					if (PrototypeManager.TryIndex<ConstructionPrototype>(item.ID, ref constructionPrototype) && PrototypeManager.TryIndex(EntProtoId.op_Implicit(id), ref val))
					{
						string name = ((constructionPrototype.SetName == null) ? val.Name : (((EntitySystem)this).Loc.TryGetString(constructionPrototype.SetName, ref text) ? text : constructionPrototype.SetName));
						string description = ((constructionPrototype.SetDescription == null) ? val.Description : (((EntitySystem)this).Loc.TryGetString(constructionPrototype.SetDescription, ref text2) ? text2 : constructionPrototype.SetDescription));
						constructionPrototype.Name = name;
						constructionPrototype.Description = description;
						_recipesMetadataCache.Add(item.ID, id);
					}
				}
			}
			while (stack.Count > 0);
		}
	}

	private void OnConstructionGuideReceived(ResponseConstructionGuide ev)
	{
		_guideCache[ev.ConstructionId] = ev.Guide;
		this.ConstructionGuideAvailable?.Invoke(this, ev.ConstructionId);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<ConstructionSystem>();
	}

	public ConstructionGuide? GetGuide(ConstructionPrototype prototype)
	{
		if (_guideCache.TryGetValue(prototype.ID, out ConstructionGuide value))
		{
			return value;
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestConstructionGuide(prototype.ID));
		return null;
	}

	private void HandleConstructionGhostExamined(EntityUid uid, ConstructionGhostComponent component, ExaminedEvent args)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (component.Prototype?.Name == null)
		{
			return;
		}
		using (args.PushGroup("ConstructionGhostComponent"))
		{
			args.PushMarkup(((EntitySystem)this).Loc.GetString("construction-ghost-examine-message", (ValueTuple<string, object>)("name", component.Prototype.Name)));
			ConstructionGraphPrototype constructionGraphPrototype = default(ConstructionGraphPrototype);
			if (!PrototypeManager.TryIndex<ConstructionGraphPrototype>(component.Prototype.Graph, ref constructionGraphPrototype))
			{
				return;
			}
			ConstructionGraphNode constructionGraphNode = constructionGraphPrototype.Nodes[component.Prototype.StartNode];
			if (!constructionGraphPrototype.TryPath(component.Prototype.StartNode, component.Prototype.TargetNode, out ConstructionGraphNode[] path) || !constructionGraphNode.TryGetEdge(path[0].Name, out ConstructionGraphEdge edge))
			{
				return;
			}
			foreach (ConstructionGraphStep step in edge.Steps)
			{
				step.DoExamine(args);
			}
		}
	}

	private void HandleAckStructure(AckStructureConstructionMessage msg)
	{
		ClearGhost(msg.GhostId);
	}

	private void HandlePlayerAttached(LocalPlayerAttachedEvent msg)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		bool available = IsCraftingAvailable(msg.Entity);
		if (!_rmcConstruction.CanConstruct(msg.Entity))
		{
			available = false;
		}
		UpdateCraftingAvailability(available);
	}

	private bool HandleOpenCraftingMenu(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)args.State == 1)
		{
			this.ToggleCraftingWindow?.Invoke(this, EventArgs.Empty);
		}
		return true;
	}

	private bool HandleFlip(in PointerInputCmdArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)args.State == 1)
		{
			this.FlipConstructionPrototype?.Invoke(this, EventArgs.Empty);
		}
		return true;
	}

	private void UpdateCraftingAvailability(bool available)
	{
		if (CraftingEnabled != available)
		{
			this.CraftingAvailabilityChanged?.Invoke(this, new CraftingAvailabilityChangedArgs(available));
			CraftingEnabled = available;
		}
	}

	private static bool IsCraftingAvailable(EntityUid? entity)
	{
		if (!entity.HasValue)
		{
			return false;
		}
		return true;
	}

	private bool HandleUse(in PointerInputCmdArgs args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityUid)(ref args.EntityUid)).IsValid() || !((EntitySystem)this).IsClientSide(args.EntityUid, (MetaDataComponent)null))
		{
			return false;
		}
		if (!((EntitySystem)this).HasComp<ConstructionGhostComponent>(args.EntityUid))
		{
			return false;
		}
		TryStartConstruction(args.EntityUid);
		return true;
	}

	public void SpawnGhost(ConstructionPrototype prototype, EntityCoordinates loc, Direction dir)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		TrySpawnGhost(prototype, loc, dir, out var _);
	}

	public bool TrySpawnGhost(ConstructionPrototype prototype, EntityCoordinates loc, Direction dir, [NotNullWhen(true)] out EntityUid? ghost)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Expected O, but got Unknown
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		ghost = null;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault)).IsValid())
			{
				EntityPrototype val = default(EntityPrototype);
				if (!TryGetRecipePrototype(prototype.ID, out string targetProtoId) || !PrototypeManager.TryIndex<EntityPrototype>(targetProtoId, ref val))
				{
					return false;
				}
				if (GhostPresent(loc))
				{
					return false;
				}
				SharedInteractionSystem.Ignored predicate = GetPredicate(prototype.CanBuildInImpassable, _transformSystem.ToMapCoordinates(loc, true));
				if (!_examineSystem.InRangeUnOccluded(valueOrDefault, loc, 20f, predicate))
				{
					return false;
				}
				if (!CheckConstructionConditions(prototype, loc, dir, valueOrDefault, showPopup: true))
				{
					return false;
				}
				ghost = ((EntitySystem)this).Spawn("constructionghost", loc);
				ConstructionGhostComponent constructionGhostComponent = ((EntitySystem)this).Comp<ConstructionGhostComponent>(ghost.Value);
				constructionGhostComponent.Prototype = prototype;
				constructionGhostComponent.GhostId = ghost.GetHashCode();
				((EntitySystem)this).Comp<TransformComponent>(ghost.Value).LocalRotation = DirectionExtensions.ToAngle(dir);
				_ghosts.Add(constructionGhostComponent.GhostId, ghost.Value);
				SpriteComponent val2 = ((EntitySystem)this).Comp<SpriteComponent>(ghost.Value);
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), new Color((byte)48, byte.MaxValue, (byte)48, (byte)128));
				IconComponent val3 = default(IconComponent);
				if (val.TryGetComponent<IconComponent>(ref val3, ((EntitySystem)this).EntityManager.ComponentFactory))
				{
					_sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), (int?)0);
					_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), 0, (SpriteSpecifier)(object)val3.Icon);
					val2.LayerSetShader(0, "unshaded");
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), 0, true);
				}
				else
				{
					if (!((Dictionary<string, ComponentRegistryEntry>)(object)val.Components).TryGetValue("Sprite", out ComponentRegistryEntry _))
					{
						return false;
					}
					EntityUid val4 = ((EntitySystem)this).EntityManager.SpawnEntity(targetProtoId, MapCoordinates.Nullspace, (ComponentRegistry)null);
					SpriteComponent val5 = ((EntitySystem)this).EnsureComp<SpriteComponent>(val4);
					((EntitySystem)this).EntityManager.System<AppearanceSystem>().OnChangeData(val4, val5, (AppearanceComponent)null);
					State val7 = default(State);
					for (int i = 0; i < val5.AllLayers.Count(); i++)
					{
						if (!val5[i].Visible)
						{
							continue;
						}
						StateId rsiState = val5[i].RsiState;
						if (((StateId)(ref rsiState)).IsValid)
						{
							RSI val6 = val5[i].Rsi ?? val5.BaseRSI;
							if (val6 != null && val6.TryGetState(val5[i].RsiState, ref val7) && val7.StateId.Name != null)
							{
								_sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), (int?)i);
								_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), i, (SpriteSpecifier)new Rsi(val6.Path, val7.StateId.Name));
								val2.LayerSetShader(i, "unshaded");
								_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost.Value, val2)), i, true);
							}
						}
					}
					((EntitySystem)this).Del((EntityUid?)val4);
				}
				if (prototype.CanBuildInImpassable)
				{
					((EntitySystem)this).EnsureComp<WallMountComponent>(ghost.Value).Arc = new Angle(Math.PI * 2.0);
				}
				return true;
			}
		}
		return false;
	}

	private bool CheckConstructionConditions(ConstructionPrototype prototype, EntityCoordinates loc, Direction dir, EntityUid user, bool showPopup = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		RMCConstructionAttemptEvent rMCConstructionAttemptEvent = new RMCConstructionAttemptEvent(loc, prototype.Name);
		((EntitySystem)this).RaiseLocalEvent<RMCConstructionAttemptEvent>(ref rMCConstructionAttemptEvent);
		if (rMCConstructionAttemptEvent.Cancelled)
		{
			string popup = rMCConstructionAttemptEvent.Popup;
			if (popup != null)
			{
				_popupSystem.PopupCoordinates(popup, loc);
			}
			return false;
		}
		foreach (IConstructionCondition condition in prototype.Conditions)
		{
			if (condition.Condition(user, loc, dir))
			{
				continue;
			}
			if (showPopup)
			{
				string text = condition.GenerateGuideEntry()?.Localization;
				if (text != null)
				{
					_popupSystem.PopupCoordinates(((EntitySystem)this).Loc.GetString(text), loc);
				}
			}
			return false;
		}
		return true;
	}

	private bool GhostPresent(EntityCoordinates loc)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<int, EntityUid> ghost in _ghosts)
		{
			EntityCoordinates coordinates = ((EntitySystem)this).Comp<TransformComponent>(ghost.Value).Coordinates;
			if (((EntityCoordinates)(ref coordinates)).Equals(loc))
			{
				return true;
			}
		}
		return false;
	}

	public unsafe void TryStartConstruction(EntityUid ghostId, ConstructionGhostComponent? ghostComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ConstructionGhostComponent>(ghostId, ref ghostComp, true))
		{
			if (ghostComp.Prototype == null)
			{
				throw new ArgumentException($"Can't start construction for a ghost with no prototype. Ghost id: {ghostId}");
			}
			TransformComponent val = ((EntitySystem)this).Comp<TransformComponent>(ghostId);
			TryStartStructureConstructionMessage tryStartStructureConstructionMessage = new TryStartStructureConstructionMessage(((EntitySystem)this).GetNetCoordinates(val.Coordinates, (MetaDataComponent)null), ghostComp.Prototype.ID, val.LocalRotation, ((object)(*(EntityUid*)(&ghostId))/*cast due to constrained. prefix*/).GetHashCode());
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)tryStartStructureConstructionMessage);
		}
	}

	public void TryStartItemConstruction(string prototypeName)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new TryStartItemConstructionMessage(prototypeName));
	}

	public void ClearGhost(int ghostId)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (_ghosts.TryGetValue(ghostId, out var value))
		{
			((EntitySystem)this).QueueDel((EntityUid?)value);
			_ghosts.Remove(ghostId);
		}
	}

	public void ClearAllGhosts()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid value in _ghosts.Values)
		{
			((EntitySystem)this).QueueDel((EntityUid?)value);
		}
		_ghosts.Clear();
	}
}
