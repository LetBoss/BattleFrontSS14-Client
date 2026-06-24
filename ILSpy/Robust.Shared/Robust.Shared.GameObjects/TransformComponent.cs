using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
public sealed class TransformComponent : Component, IComponentDebug, IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<IComponentDebug>, ISerializationGenerated<TransformComponent>
{
	[Dependency]
	private readonly IEntityManager _entMan;

	[DataField("parent", false, 1, false, false, null)]
	internal EntityUid _parent;

	[DataField("pos", false, 1, false, false, null)]
	internal Vector2 _localPosition = Vector2.Zero;

	[DataField("rot", false, 1, false, false, null)]
	internal Angle _localRotation;

	[DataField("noRot", false, 1, false, false, null)]
	internal bool _noLocalRotation;

	[DataField("anchored", false, 1, false, false, null)]
	internal bool _anchored;

	[DataField(null, false, 1, false, false, null)]
	public bool GridTraversal = true;

	[ViewVariables]
	internal BroadphaseData? Broadphase;

	internal bool MatricesDirty = true;

	private Matrix3x2 _localMatrix = Matrix3x2.Identity;

	private Matrix3x2 _invLocalMatrix = Matrix3x2.Identity;

	[ViewVariables]
	public bool ActivelyLerping;

	[ViewVariables]
	public GameTick LastLerp = GameTick.Zero;

	[ViewVariables]
	internal readonly HashSet<EntityUid> _children = new HashSet<EntityUid>();

	[Dependency]
	private readonly IMapManager _mapManager;

	internal bool _mapIdInitialized;

	internal bool _gridInitialized;

	[Access(new Type[] { typeof(SharedTransformSystem) })]
	internal EntityUid? _gridUid;

	[ViewVariables]
	public EntityUid LerpParent;

	public bool PredictedLerp;

	[ViewVariables]
	private NetEntity NetParent => _entMan.GetNetEntity(_parent);

	public Matrix3x2 LocalMatrix
	{
		get
		{
			if (MatricesDirty)
			{
				RebuildMatrices();
			}
			return _localMatrix;
		}
	}

	public Matrix3x2 InvLocalMatrix
	{
		get
		{
			if (MatricesDirty)
			{
				RebuildMatrices();
			}
			return _invLocalMatrix;
		}
	}

	[ViewVariables]
	public Vector2? NextPosition { get; internal set; }

	[ViewVariables]
	public Angle? NextRotation { get; internal set; }

	[ViewVariables]
	public Vector2 PrevPosition { get; internal set; }

	[ViewVariables]
	public Angle PrevRotation { get; internal set; }

	[ViewVariables]
	public MapId MapID { get; internal set; }

	public EntityUid? MapUid { get; internal set; }

	[ViewVariables]
	public EntityUid? GridUid => _gridUid;

	[ViewVariables(VVAccess.ReadWrite)]
	public bool NoLocalRotation
	{
		get
		{
			return _noLocalRotation;
		}
		set
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			if (value)
			{
				LocalRotation = Angle.Zero;
			}
			_noLocalRotation = value;
			_entMan.Dirty(base.Owner, this);
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[Animatable]
	public Angle LocalRotation
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _localRotation;
		}
		set
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (!_noLocalRotation && !((Angle)(ref _localRotation)).EqualsApprox(value))
			{
				Angle localRotation = _localRotation;
				_localRotation = value;
				MetaDataComponent component = _entMan.GetComponent<MetaDataComponent>(base.Owner);
				_entMan.Dirty(base.Owner, this, component);
				MatricesDirty = true;
				if (base.Initialized)
				{
					_entMan.System<SharedTransformSystem>().RaiseMoveEvent((Owner: base.Owner, Comp1: this, Comp2: component), _parent, _localPosition, localRotation, MapUid, checkTraversal: false);
				}
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[Obsolete("Use the system method instead")]
	public Angle WorldRotation
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			EntityUid uid = _parent;
			EntityQuery<TransformComponent> entityQuery = _entMan.GetEntityQuery<TransformComponent>();
			Angle val = _localRotation;
			while (uid.IsValid())
			{
				TransformComponent component = entityQuery.GetComponent(uid);
				val += component._localRotation;
				uid = component.ParentUid;
			}
			return val;
		}
		set
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (!NoLocalRotation)
			{
				Angle worldRotation = WorldRotation;
				Angle val = value - worldRotation;
				LocalRotation += val;
			}
		}
	}

	[ViewVariables]
	private TransformComponent? _parentXform
	{
		get
		{
			if (_parent.IsValid())
			{
				return _entMan.GetComponent<TransformComponent>(_parent);
			}
			return null;
		}
	}

	public EntityUid ParentUid => _parent;

	[Obsolete("Use the system method instead")]
	public Matrix3x2 WorldMatrix
	{
		get
		{
			EntityQuery<TransformComponent> entityQuery = _entMan.GetEntityQuery<TransformComponent>();
			EntityUid uid = _parent;
			Matrix3x2 matrix3x = LocalMatrix;
			while (uid.IsValid())
			{
				TransformComponent component = entityQuery.GetComponent(uid);
				Matrix3x2 localMatrix = component.LocalMatrix;
				uid = component.ParentUid;
				matrix3x = Matrix3x2.Multiply(matrix3x, localMatrix);
			}
			return matrix3x;
		}
	}

	[Obsolete("Use the system method instead")]
	public Matrix3x2 InvWorldMatrix
	{
		get
		{
			EntityQuery<TransformComponent> entityQuery = _entMan.GetEntityQuery<TransformComponent>();
			EntityUid uid = _parent;
			Matrix3x2 matrix3x = InvLocalMatrix;
			while (uid.IsValid())
			{
				TransformComponent component = entityQuery.GetComponent(uid);
				Matrix3x2 invLocalMatrix = component.InvLocalMatrix;
				uid = component.ParentUid;
				matrix3x = Matrix3x2.Multiply(invLocalMatrix, matrix3x);
			}
			return matrix3x;
		}
	}

	[Animatable]
	[ViewVariables(VVAccess.ReadWrite)]
	[Obsolete("Use the system method instead")]
	public Vector2 WorldPosition
	{
		get
		{
			if (_parent.IsValid())
			{
				return Vector2.Transform(_localPosition, _entMan.GetComponent<TransformComponent>(ParentUid).WorldMatrix);
			}
			return Vector2.Zero;
		}
		set
		{
			if (_parent.IsValid())
			{
				Vector2 localPosition = Vector2.Transform(value, _entMan.GetComponent<TransformComponent>(ParentUid).InvWorldMatrix);
				LocalPosition = localPosition;
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public EntityCoordinates Coordinates
	{
		get
		{
			bool flag = _parent.IsValid();
			return new EntityCoordinates(flag ? _parent : base.Owner, flag ? LocalPosition : Vector2.Zero);
		}
		[Obsolete("Use the system's setter method instead.")]
		set
		{
			_entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().SetCoordinates(base.Owner, this, value);
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	[Obsolete("Use TransformSystem.GetMapCoordinates")]
	public MapCoordinates MapPosition => new MapCoordinates(WorldPosition, MapID);

	[Animatable]
	[ViewVariables(VVAccess.ReadWrite)]
	public Vector2 LocalPosition
	{
		get
		{
			return _localPosition;
		}
		[Obsolete("Use the system method instead")]
		set
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			if (!Anchored && !Vector2Helpers.EqualsApprox(_localPosition, value))
			{
				EntityUid parent = _parent;
				Vector2 localPosition = _localPosition;
				_localPosition = value;
				MetaDataComponent component = _entMan.GetComponent<MetaDataComponent>(base.Owner);
				_entMan.Dirty(base.Owner, this, component);
				MatricesDirty = true;
				if (base.Initialized)
				{
					_entMan.System<SharedTransformSystem>().RaiseMoveEvent((Owner: base.Owner, Comp1: this, Comp2: component), parent, localPosition, _localRotation, MapUid);
				}
			}
		}
	}

	[ViewVariables(VVAccess.ReadWrite)]
	public bool Anchored
	{
		get
		{
			return _anchored;
		}
		[Obsolete("Use the SharedTransformSystem.AnchorEntity/Unanchor methods instead.")]
		set
		{
			EntityUid uid;
			MapGridComponent grid;
			if (!base.Initialized)
			{
				_anchored = value;
			}
			else if (value && !_anchored && _mapManager.TryFindGridAt(MapPosition, out uid, out grid))
			{
				_anchored = _entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().AnchorEntity(base.Owner, this, grid);
			}
			else if (!value && _anchored)
			{
				_entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().Unanchor(base.Owner, this);
			}
		}
	}

	public TransformChildrenEnumerator ChildEnumerator => new TransformChildrenEnumerator(_children.GetEnumerator());

	[ViewVariables]
	public int ChildCount => _children.Count;

	[Obsolete("Use the system's method instead.")]
	public void AttachToGridOrMap()
	{
		_entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().AttachToGridOrMap(base.Owner, this);
	}

	[Obsolete("Use TransformSystem.SetParent() instead")]
	public void AttachParent(EntityUid parent)
	{
		_entMan.EntitySysManager.GetEntitySystem<SharedTransformSystem>().SetParent(base.Owner, this, parent, _entMan.GetEntityQuery<TransformComponent>());
	}

	[Obsolete("Use the system method instead")]
	public (Vector2 WorldPosition, Angle WorldRotation) GetWorldPositionRotation()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		var (item, item2, _) = GetWorldPositionRotationMatrix();
		return (WorldPosition: item, WorldRotation: item2);
	}

	[Obsolete("Use the system method instead")]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix(EntityQuery<TransformComponent> xforms)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = _parent;
		Angle val = _localRotation;
		Matrix3x2 matrix3x = LocalMatrix;
		while (uid.IsValid())
		{
			TransformComponent component = xforms.GetComponent(uid);
			val += component.LocalRotation;
			Matrix3x2 localMatrix = component.LocalMatrix;
			matrix3x = Matrix3x2.Multiply(matrix3x, localMatrix);
			uid = component.ParentUid;
		}
		return (WorldPosition: matrix3x.Translation, WorldRotation: val, WorldMatrix: matrix3x);
	}

	[Obsolete("Use the system method instead")]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix) GetWorldPositionRotationMatrix()
	{
		EntityQuery<TransformComponent> entityQuery = _entMan.GetEntityQuery<TransformComponent>();
		return GetWorldPositionRotationMatrix(entityQuery);
	}

	[Obsolete("Use the system method instead")]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationInvMatrix(EntityQuery<TransformComponent> xformQuery)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		var (item, item2, _, item3) = GetWorldPositionRotationMatrixWithInv(xformQuery);
		return (WorldPosition: item, WorldRotation: item2, InvWorldMatrix: item3);
	}

	[Obsolete("Use the system method instead")]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv()
	{
		EntityQuery<TransformComponent> entityQuery = _entMan.GetEntityQuery<TransformComponent>();
		return GetWorldPositionRotationMatrixWithInv(entityQuery);
	}

	[Obsolete("Use the system method instead")]
	public (Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) GetWorldPositionRotationMatrixWithInv(EntityQuery<TransformComponent> xformQuery)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = _parent;
		Angle val = _localRotation;
		Matrix3x2 matrix3x = InvLocalMatrix;
		Matrix3x2 matrix3x2 = LocalMatrix;
		while (uid.IsValid())
		{
			TransformComponent component = xformQuery.GetComponent(uid);
			val += component.LocalRotation;
			Matrix3x2 localMatrix = component.LocalMatrix;
			matrix3x2 = Matrix3x2.Multiply(matrix3x2, localMatrix);
			matrix3x = Matrix3x2.Multiply(component.InvLocalMatrix, matrix3x);
			uid = component.ParentUid;
		}
		return (WorldPosition: matrix3x2.Translation, WorldRotation: val, WorldMatrix: matrix3x2, InvWorldMatrix: matrix3x);
	}

	public void RebuildMatrices()
	{
		MatricesDirty = false;
		if (!_parent.IsValid())
		{
			_localMatrix = Matrix3x2.Identity;
			_invLocalMatrix = Matrix3x2.Identity;
		}
		_localMatrix = Matrix3Helpers.CreateTransform(ref _localPosition, ref _localRotation);
		_invLocalMatrix = Matrix3Helpers.CreateInverseTransform(ref _localPosition, ref _localRotation);
	}

	public string GetDebugString()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		return $"pos/rot/wpos/wrot: {Coordinates}/{LocalRotation}/{WorldPosition}/{WorldRotation}";
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TransformComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (TransformComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			EntityUid target3 = default(EntityUid);
			if (!serialization.TryCustomCopy(_parent, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = serialization.CreateCopy(_parent, hookCtx, context);
			}
			target._parent = target3;
			Vector2 target4 = default(Vector2);
			if (!serialization.TryCustomCopy(_localPosition, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = serialization.CreateCopy(_localPosition, hookCtx, context);
			}
			target._localPosition = target4;
			Angle target5 = default(Angle);
			if (!serialization.TryCustomCopy(_localRotation, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = serialization.CreateCopy<Angle>(_localRotation, hookCtx, context);
			}
			target._localRotation = target5;
			bool target6 = false;
			if (!serialization.TryCustomCopy(_noLocalRotation, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = _noLocalRotation;
			}
			target._noLocalRotation = target6;
			bool target7 = false;
			if (!serialization.TryCustomCopy(_anchored, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = _anchored;
			}
			target._anchored = target7;
			bool target8 = false;
			if (!serialization.TryCustomCopy(GridTraversal, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = GridTraversal;
			}
			target.GridTraversal = target8;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TransformComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransformComponent target2 = (TransformComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransformComponent target2 = (TransformComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IComponentDebug target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransformComponent target2 = (TransformComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IComponentDebug target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TransformComponent target2 = (TransformComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TransformComponent Instantiate()
	{
		return new TransformComponent();
	}

	IComponentDebug IComponentDebug.Instantiate()
	{
		return Instantiate();
	}

	IComponentDebug ISerializationGenerated<IComponentDebug>.Instantiate()
	{
		return Instantiate();
	}
}
