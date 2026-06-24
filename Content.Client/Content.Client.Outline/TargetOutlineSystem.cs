using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Interaction;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Outline;

public sealed class TargetOutlineSystem : EntitySystem
{
	private static readonly ProtoId<ShaderPrototype> ShaderTargetValid = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutlineInrange");

	private static readonly ProtoId<ShaderPrototype> ShaderTargetInvalid = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutline");

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	private bool _enabled;

	public EntityWhitelist? Whitelist;

	public EntityWhitelist? Blacklist;

	public Func<EntityUid, bool>? Predicate;

	public CancellableEntityEventArgs? ValidationEvent;

	public float Range = -1f;

	public bool CheckObstruction = true;

	public float LookupSize = 1f;

	private ShaderInstance? _shaderTargetValid;

	private ShaderInstance? _shaderTargetInvalid;

	private readonly HashSet<SpriteComponent> _highlightedSprites = new HashSet<SpriteComponent>();

	private Vector2 LookupVector => new Vector2(LookupSize, LookupSize);

	public override void Initialize()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_shaderTargetValid = _prototypeManager.Index<ShaderPrototype>(ShaderTargetValid).InstanceUnique();
		_shaderTargetInvalid = _prototypeManager.Index<ShaderPrototype>(ShaderTargetInvalid).InstanceUnique();
	}

	public void Disable()
	{
		if (_enabled)
		{
			_enabled = false;
			RemoveHighlights();
		}
	}

	public void Enable(float range, bool checkObstructions, Func<EntityUid, bool>? predicate, EntityWhitelist? whitelist, EntityWhitelist? blacklist, CancellableEntityEventArgs? validationEvent)
	{
		Range = range;
		CheckObstruction = checkObstructions;
		Predicate = predicate;
		Whitelist = whitelist;
		Blacklist = blacklist;
		ValidationEvent = validationEvent;
		_enabled = Predicate != null || Whitelist != null || Blacklist != null || ValidationEvent != null;
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_enabled && _timing.IsFirstTimePredicted)
		{
			HighlightTargets();
		}
	}

	private void HighlightTargets()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (!((EntityUid)(ref valueOrDefault)).Valid)
		{
			return;
		}
		RemoveHighlights();
		Vector2 position = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition).Position;
		Box2 val = default(Box2);
		((Box2)(ref val))._002Ector(position - LookupVector, position + LookupVector);
		HashSet<EntityUid> entitiesIntersecting = _lookup.GetEntitiesIntersecting(_eyeManager.CurrentEye.Position.MapId, val, (LookupFlags)5);
		EntityQuery<SpriteComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		SpriteComponent val2 = default(SpriteComponent);
		foreach (EntityUid item in entitiesIntersecting)
		{
			if (!entityQuery.TryGetComponent(item, ref val2) || !val2.Visible)
			{
				continue;
			}
			bool flag = Predicate?.Invoke(item) ?? true;
			if (flag && Whitelist != null)
			{
				flag = _whitelistSystem.IsWhitelistPass(Whitelist, item);
			}
			if (flag && ValidationEvent != null)
			{
				ValidationEvent.Uncancel();
				((EntitySystem)this).RaiseLocalEvent(item, (object)ValidationEvent, false);
				flag = !ValidationEvent.Cancelled;
			}
			if (!flag)
			{
				if (_highlightedSprites.Remove(val2) && (val2.PostShader == _shaderTargetValid || val2.PostShader == _shaderTargetInvalid))
				{
					val2.PostShader = null;
					val2.RenderOrder = 0u;
				}
				continue;
			}
			if (CheckObstruction)
			{
				flag = _interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(valueOrDefault), Entity<TransformComponent>.op_Implicit(item), Range);
			}
			else if (Range >= 0f)
			{
				Vector2 worldPosition = _transformSystem.GetWorldPosition(valueOrDefault);
				Vector2 worldPosition2 = _transformSystem.GetWorldPosition(item);
				flag = (worldPosition - worldPosition2).LengthSquared() <= Range;
			}
			if (val2.PostShader != null && val2.PostShader != _shaderTargetValid && val2.PostShader != _shaderTargetInvalid)
			{
				break;
			}
			val2.PostShader = (flag ? _shaderTargetValid : _shaderTargetInvalid);
			val2.RenderOrder = base.EntityManager.CurrentTick.Value;
			_highlightedSprites.Add(val2);
		}
	}

	private void RemoveHighlights()
	{
		foreach (SpriteComponent highlightedSprite in _highlightedSprites)
		{
			if (highlightedSprite.PostShader == _shaderTargetValid || highlightedSprite.PostShader == _shaderTargetInvalid)
			{
				highlightedSprite.PostShader = null;
				highlightedSprite.RenderOrder = 0u;
			}
		}
		_highlightedSprites.Clear();
	}
}
