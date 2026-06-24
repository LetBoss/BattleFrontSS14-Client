using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Client.Outline;
using Content.Shared.ActionBlocker;
using Content.Shared.CCVar;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.Interaction;

public sealed class DragDropSystem : SharedDragDropSystem
{
	private static readonly ProtoId<ShaderPrototype> ShaderDropTargetInRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutlineInrange");

	private static readonly ProtoId<ShaderPrototype> ShaderDropTargetOutOfRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutline");

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _cfgMan;

	[Dependency]
	private InteractionOutlineSystem _outline;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private CombatModeSystem _combatMode;

	[Dependency]
	private InputSystem _inputSystem;

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private SpriteSystem _sprite;

	private const float TargetRecheckInterval = 0.25f;

	private const float MaxMouseDownTimeForReplayingClick = 0.85f;

	private EntityUid? _draggedEntity;

	private EntityUid? _dragShadow;

	private float _mouseDownTime;

	private float _targetRecheckTime;

	private PointerInputCmdArgs? _savedMouseDown;

	private bool _isReplaying;

	public float Deadzone;

	private DragState _state;

	private ScreenCoordinates? _mouseDownScreenPos;

	private ShaderInstance? _dropTargetInRangeShader;

	private ShaderInstance? _dropTargetOutOfRangeShader;

	private readonly List<SpriteComponent> _highlightedSprites = new List<SpriteComponent>();

	public override void Initialize()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		base.Initialize();
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedEyeSystem));
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _cfgMan, CCVars.DragDropDeadZone, (Action<float>)SetDeadZone, true);
		_dropTargetInRangeShader = _prototypeManager.Index<ShaderPrototype>(ShaderDropTargetInRange).Instance();
		_dropTargetOutOfRangeShader = _prototypeManager.Index<ShaderPrototype>(ShaderDropTargetOutOfRange).Instance();
		CommandBinds.Builder.BindBefore(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(OnUse), false, true), new Type[1] { typeof(SharedInteractionSystem) }).Register<DragDropSystem>();
	}

	private void SetDeadZone(float deadZone)
	{
		Deadzone = deadZone;
	}

	public override void Shutdown()
	{
		CommandBinds.Unregister<DragDropSystem>();
		((EntitySystem)this).Shutdown();
	}

	private bool OnUse(in PointerInputCmdArgs args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (_inputSystem.Predicted)
		{
			return false;
		}
		if (_isReplaying)
		{
			return false;
		}
		if ((int)args.State == 1)
		{
			return OnUseMouseDown(in args);
		}
		if ((int)args.State == 0)
		{
			return OnUseMouseUp(in args);
		}
		return false;
	}

	private void EndDrag()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_state != DragState.NotDragging)
		{
			if (_dragShadow.HasValue)
			{
				((EntitySystem)this).Del((EntityUid?)_dragShadow.Value);
				_dragShadow = null;
			}
			_draggedEntity = null;
			_state = DragState.NotDragging;
			_mouseDownScreenPos = null;
			RemoveHighlights();
			_outline.SetEnabled(enabled: true);
			_mouseDownTime = 0f;
			_savedMouseDown = null;
		}
	}

	private bool OnUseMouseDown(in PointerInputCmdArgs args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession session = args.Session;
		EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid valueOrDefault = val.GetValueOrDefault();
			if (((EntityUid)(ref valueOrDefault)).Valid && !_combatMode.IsInCombatMode())
			{
				EndDrag();
				EntityUid entityUid = args.EntityUid;
				if (!((EntitySystem)this).Exists(entityUid))
				{
					return false;
				}
				if (!_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(valueOrDefault), Entity<TransformComponent>.op_Implicit(entityUid)))
				{
					return false;
				}
				CanDragEvent canDragEvent = default(CanDragEvent);
				((EntitySystem)this).RaiseLocalEvent<CanDragEvent>(entityUid, ref canDragEvent, false);
				if (!canDragEvent.Handled)
				{
					return false;
				}
				_draggedEntity = entityUid;
				_state = DragState.MouseDown;
				_mouseDownScreenPos = args.ScreenCoordinates;
				_mouseDownTime = 0f;
				_savedMouseDown = args;
				return true;
			}
		}
		return false;
	}

	private void StartDrag()
	{
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Exists(_draggedEntity))
		{
			return;
		}
		_state = DragState.Dragging;
		_outline.SetEnabled(enabled: false);
		HighlightTargets();
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(_draggedEntity, ref item))
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
				_dragShadow = ((EntitySystem)this).EntityManager.SpawnEntity("dragshadow", val, (ComponentRegistry)null);
				SpriteComponent val2 = ((EntitySystem)this).Comp<SpriteComponent>(_dragShadow.Value);
				_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((_draggedEntity.Value, item)), Entity<SpriteComponent>.op_Implicit((_dragShadow.Value, val2)));
				val2.RenderOrder = ((EntitySystem)this).EntityManager.CurrentTick.Value;
				SpriteSystem sprite = _sprite;
				Entity<SpriteComponent> val3 = Entity<SpriteComponent>.op_Implicit((_dragShadow.Value, val2));
				Color color = val2.Color;
				sprite.SetColor(val3, ((Color)(ref color)).WithAlpha(0.7f));
				_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((_dragShadow.Value, val2)), 13);
				if (!val2.NoRotation)
				{
					_transformSystem.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit(_dragShadow.Value), _transformSystem.GetWorldRotation(_draggedEntity.Value));
				}
			}
		}
		else
		{
			((EntitySystem)this).Log.Warning($"Unable to display drag shadow for {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(_draggedEntity.Value))} because it has no sprite component.");
		}
	}

	private bool UpdateDrag(float frameTime)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Exists(_draggedEntity) || _combatMode.IsInCombatMode())
		{
			EndDrag();
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue || !_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(localEntity.Value), Entity<TransformComponent>.op_Implicit(_draggedEntity.Value)))
		{
			return false;
		}
		if (!_dragShadow.HasValue)
		{
			return false;
		}
		_targetRecheckTime += frameTime;
		if (_targetRecheckTime > 0.25f)
		{
			HighlightTargets();
			_targetRecheckTime -= 0.25f;
		}
		return true;
	}

	private bool OnUseMouseUp(in PointerInputCmdArgs args)
	{
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		if (_state == DragState.MouseDown)
		{
			try
			{
				if (_savedMouseDown.HasValue && _mouseDownTime < 0.85f)
				{
					PointerInputCmdArgs value = _savedMouseDown.Value;
					_isReplaying = true;
					IFullInputCmdMessage originalMessage = value.OriginalMessage;
					ClientFullInputCmdMessage val = (ClientFullInputCmdMessage)(object)((originalMessage is ClientFullInputCmdMessage) ? originalMessage : null);
					if (val == null)
					{
						FullInputCmdMessage val2 = (FullInputCmdMessage)(object)((originalMessage is FullInputCmdMessage) ? originalMessage : null);
						if (val2 == null)
						{
							throw new ArgumentOutOfRangeException();
						}
						originalMessage = (IFullInputCmdMessage)new FullInputCmdMessage(args.OriginalMessage.Tick, args.OriginalMessage.SubTick, originalMessage.InputFunctionId, originalMessage.State, val2.Coordinates, val2.ScreenCoordinates, val2.Uid);
					}
					else
					{
						ClientFullInputCmdMessage val3 = new ClientFullInputCmdMessage(args.OriginalMessage.Tick, args.OriginalMessage.SubTick, originalMessage.InputFunctionId);
						val3.set_State(originalMessage.State);
						val3.set_Coordinates(val.Coordinates);
						val3.set_ScreenCoordinates(val.ScreenCoordinates);
						val3.set_Uid(val.Uid);
						originalMessage = (IFullInputCmdMessage)val3;
					}
					if (value.Session != null)
					{
						_inputSystem.HandleInputCommand(value.Session, EngineKeyFunctions.Use, originalMessage, true);
					}
					_isReplaying = false;
				}
			}
			finally
			{
				EndDrag();
			}
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue || !((EntitySystem)this).Exists(_draggedEntity))
		{
			EndDrag();
			return false;
		}
		EntityCoordinates coordinates = args.Coordinates;
		IEnumerable<EntityUid> enumerable = ((!(_stateManager.CurrentState is GameplayState gameplayState)) ? Array.Empty<EntityUid>() : gameplayState.GetClickableEntities(coordinates));
		bool flag = false;
		EntityUid value2 = localEntity.Value;
		foreach (EntityUid item in enumerable)
		{
			EntityUid val4 = item;
			EntityUid? draggedEntity = _draggedEntity;
			if ((!draggedEntity.HasValue || !(val4 == draggedEntity.GetValueOrDefault())) && ValidDragDrop(value2, _draggedEntity.Value, item) == true)
			{
				if (_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(value2), Entity<TransformComponent>.op_Implicit(item)) && _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(value2), Entity<TransformComponent>.op_Implicit(_draggedEntity.Value)))
				{
					((EntitySystem)this).RaisePredictiveEvent<DragDropRequestEvent>(new DragDropRequestEvent(((EntitySystem)this).GetNetEntity(_draggedEntity.Value, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(item, (MetaDataComponent)null)));
					EndDrag();
					return true;
				}
				flag = true;
			}
		}
		if (flag)
		{
			_popup.PopupEntity(((EntitySystem)this).Loc.GetString("drag-drop-system-out-of-range-text"), _draggedEntity.Value, Filter.Local(), recordReplay: true);
		}
		EndDrag();
		return false;
	}

	private void HighlightTargets()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Exists(_draggedEntity) || !((EntitySystem)this).Exists(_dragShadow))
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		RemoveHighlights();
		MapCoordinates val = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);
		Vector2 vector = new Vector2(1.5f, 1.5f);
		Box2 val2 = default(Box2);
		((Box2)(ref val2))._002Ector(val.Position - vector, val.Position + vector);
		HashSet<EntityUid> entitiesIntersecting = _lookup.GetEntitiesIntersecting(val.MapId, val2, (LookupFlags)110);
		EntityQuery<SpriteComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		SpriteComponent val3 = default(SpriteComponent);
		foreach (EntityUid item in entitiesIntersecting)
		{
			if (!entityQuery.TryGetComponent(item, ref val3) || !val3.Visible)
			{
				continue;
			}
			EntityUid val4 = item;
			EntityUid? draggedEntity = _draggedEntity;
			if (draggedEntity.HasValue && val4 == draggedEntity.GetValueOrDefault())
			{
				continue;
			}
			bool? flag = ValidDragDrop(localEntity.Value, _draggedEntity.Value, item);
			if (flag.HasValue)
			{
				if (flag.Value)
				{
					flag = _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(localEntity.Value), Entity<TransformComponent>.op_Implicit(_draggedEntity.Value)) && _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(localEntity.Value), Entity<TransformComponent>.op_Implicit(item));
				}
				if (val3.PostShader == null || val3.PostShader == _dropTargetInRangeShader || val3.PostShader == _dropTargetOutOfRangeShader)
				{
					val3.PostShader = (flag.Value ? _dropTargetInRangeShader : _dropTargetOutOfRangeShader);
					val3.RenderOrder = ((EntitySystem)this).EntityManager.CurrentTick.Value;
					_highlightedSprites.Add(val3);
				}
			}
		}
	}

	private void RemoveHighlights()
	{
		foreach (SpriteComponent highlightedSprite in _highlightedSprites)
		{
			if (highlightedSprite.PostShader == _dropTargetInRangeShader || highlightedSprite.PostShader == _dropTargetOutOfRangeShader)
			{
				highlightedSprite.PostShader = null;
				highlightedSprite.RenderOrder = 0u;
			}
		}
		_highlightedSprites.Clear();
	}

	private bool? ValidDragDrop(EntityUid user, EntityUid dragged, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!_actionBlockerSystem.CanInteract(user, target))
		{
			return null;
		}
		GettingInteractedWithAttemptEvent gettingInteractedWithAttemptEvent = new GettingInteractedWithAttemptEvent(user, dragged);
		((EntitySystem)this).RaiseLocalEvent<GettingInteractedWithAttemptEvent>(dragged, ref gettingInteractedWithAttemptEvent, false);
		if (gettingInteractedWithAttemptEvent.Cancelled)
		{
			return false;
		}
		CanDropDraggedEvent canDropDraggedEvent = new CanDropDraggedEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<CanDropDraggedEvent>(dragged, ref canDropDraggedEvent, false);
		if (canDropDraggedEvent.Handled && !canDropDraggedEvent.CanDrop)
		{
			return false;
		}
		CanDropTargetEvent canDropTargetEvent = new CanDropTargetEvent(user, dragged);
		((EntitySystem)this).RaiseLocalEvent<CanDropTargetEvent>(target, ref canDropTargetEvent, false);
		if (canDropTargetEvent.Handled)
		{
			return canDropTargetEvent.CanDrop;
		}
		if (canDropDraggedEvent.Handled && canDropDraggedEvent.CanDrop)
		{
			return true;
		}
		return null;
	}

	public override void Update(float frameTime)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		switch (_state)
		{
		case DragState.MouseDown:
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			if ((_mouseDownScreenPos.Value.Position - mouseScreenPosition.Position).Length() > Deadzone)
			{
				StartDrag();
			}
			break;
		}
		case DragState.Dragging:
			UpdateDrag(frameTime);
			break;
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		if (((EntitySystem)this).Exists(_dragShadow))
		{
			MapCoordinates val = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);
			_transformSystem.SetWorldPosition(_dragShadow.Value, val.Position);
		}
	}
}
