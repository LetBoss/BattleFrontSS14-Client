using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Clickable;

[RegisterComponent]
public sealed class ClickableComponent : Component, ISerializationGenerated<ClickableComponent>, ISerializationGenerated
{
	[DataDefinition]
	public sealed class DirBoundData : ISerializationGenerated<DirBoundData>, ISerializationGenerated
	{
		[DataField(null, false, 1, false, false, null)]
		public Box2 All;

		[DataField(null, false, 1, false, false, null)]
		public Box2 North;

		[DataField(null, false, 1, false, false, null)]
		public Box2 South;

		[DataField(null, false, 1, false, false, null)]
		public Box2 East;

		[DataField(null, false, 1, false, false, null)]
		public Box2 West;

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref DirBoundData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<DirBoundData>(this, ref target, hookCtx, false, context))
			{
				Box2 all = default(Box2);
				if (!serialization.TryCustomCopy<Box2>(All, ref all, hookCtx, false, context))
				{
					all = serialization.CreateCopy<Box2>(All, hookCtx, context, false);
				}
				target.All = all;
				Box2 north = default(Box2);
				if (!serialization.TryCustomCopy<Box2>(North, ref north, hookCtx, false, context))
				{
					north = serialization.CreateCopy<Box2>(North, hookCtx, context, false);
				}
				target.North = north;
				Box2 south = default(Box2);
				if (!serialization.TryCustomCopy<Box2>(South, ref south, hookCtx, false, context))
				{
					south = serialization.CreateCopy<Box2>(South, hookCtx, context, false);
				}
				target.South = south;
				Box2 east = default(Box2);
				if (!serialization.TryCustomCopy<Box2>(East, ref east, hookCtx, false, context))
				{
					east = serialization.CreateCopy<Box2>(East, hookCtx, context, false);
				}
				target.East = east;
				Box2 west = default(Box2);
				if (!serialization.TryCustomCopy<Box2>(West, ref west, hookCtx, false, context))
				{
					west = serialization.CreateCopy<Box2>(West, hookCtx, context, false);
				}
				target.West = west;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref DirBoundData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DirBoundData target2 = (DirBoundData)target;
			Copy(ref target2, serialization, hookCtx, context);
			target = target2;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public DirBoundData Instantiate()
		{
			return new DirBoundData();
		}
	}

	[DataField(null, false, 1, false, false, null)]
	public DirBoundData? Bounds;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClickableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ClickableComponent)(object)val;
		if (serialization.TryCustomCopy<ClickableComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DirBoundData bounds = null;
		if (!serialization.TryCustomCopy<DirBoundData>(Bounds, ref bounds, hookCtx, false, context))
		{
			if (Bounds == null)
			{
				bounds = null;
			}
			else
			{
				serialization.CopyTo<DirBoundData>(Bounds, ref bounds, hookCtx, context, false);
			}
		}
		target.Bounds = bounds;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClickableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClickableComponent target2 = (ClickableComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClickableComponent target2 = (ClickableComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClickableComponent target2 = (ClickableComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClickableComponent Instantiate()
	{
		return new ClickableComponent();
	}
}
