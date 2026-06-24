using System;
using Robust.Shared.Analyzers;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(OccluderSystem) })]
public sealed class OccluderComponent : Component, IComponentTreeEntry<OccluderComponent>, ISerializationGenerated<OccluderComponent>, ISerializationGenerated
{
	[Flags]
	public enum OccluderDir : byte
	{
		None = 0,
		North = 1,
		East = 2,
		South = 4,
		West = 8
	}

	[Serializable]
	[NetSerializable]
	public sealed class OccluderComponentState : ComponentState
	{
		public bool Enabled { get; }

		public Box2 BoundingBox { get; }

		public OccluderComponentState(bool enabled, Box2 boundingBox)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Enabled = enabled;
			BoundingBox = boundingBox;
		}
	}

	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField("boundingBox", false, 1, false, false, null)]
	public Box2 BoundingBox = new Box2(-0.5f, -0.5f, 0.5f, 0.5f);

	[ViewVariables]
	public (EntityUid Grid, Vector2i Tile)? LastPosition;

	[ViewVariables]
	public OccluderDir Occluding;

	public EntityUid? TreeUid { get; set; }

	public DynamicTree<ComponentTreeEntry<OccluderComponent>>? Tree { get; set; }

	public bool AddToTree => Enabled;

	public bool TreeUpdateQueued { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OccluderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Component target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (OccluderComponent)target2;
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target3 = false;
			if (!serialization.TryCustomCopy(Enabled, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Enabled;
			}
			target.Enabled = target3;
			Box2 target4 = default(Box2);
			if (!serialization.TryCustomCopy(BoundingBox, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = serialization.CreateCopy<Box2>(BoundingBox, hookCtx, context);
			}
			target.BoundingBox = target4;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OccluderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OccluderComponent target2 = (OccluderComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OccluderComponent target2 = (OccluderComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OccluderComponent target2 = (OccluderComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OccluderComponent Instantiate()
	{
		return new OccluderComponent();
	}
}
