using System;
using Content.Shared.Actions;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Decals;

public sealed class PlaceDecalActionEvent : WorldTargetActionEvent, ISerializationGenerated<PlaceDecalActionEvent>, ISerializationGenerated
{
	[DataField("decalId", false, 1, true, false, typeof(PrototypeIdSerializer<DecalPrototype>))]
	public string DecalId = string.Empty;

	[DataField("color", false, 1, false, false, null)]
	public Color Color;

	[DataField("rotation", false, 1, false, false, null)]
	public double Rotation;

	[DataField("snap", false, 1, false, false, null)]
	public bool Snap;

	[DataField("zIndex", false, 1, false, false, null)]
	public int ZIndex;

	[DataField("cleanable", false, 1, false, false, null)]
	public bool Cleanable;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlaceDecalActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlaceDecalActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<PlaceDecalActionEvent>(this, ref target, hookCtx, false, context))
		{
			string DecalIdTemp = null;
			if (DecalId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DecalId, ref DecalIdTemp, hookCtx, false, context))
			{
				DecalIdTemp = DecalId;
			}
			target.DecalId = DecalIdTemp;
			Color ColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
			{
				ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
			}
			target.Color = ColorTemp;
			double RotationTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(Rotation, ref RotationTemp, hookCtx, false, context))
			{
				RotationTemp = Rotation;
			}
			target.Rotation = RotationTemp;
			bool SnapTemp = false;
			if (!serialization.TryCustomCopy<bool>(Snap, ref SnapTemp, hookCtx, false, context))
			{
				SnapTemp = Snap;
			}
			target.Snap = SnapTemp;
			int ZIndexTemp = 0;
			if (!serialization.TryCustomCopy<int>(ZIndex, ref ZIndexTemp, hookCtx, false, context))
			{
				ZIndexTemp = ZIndex;
			}
			target.ZIndex = ZIndexTemp;
			bool CleanableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Cleanable, ref CleanableTemp, hookCtx, false, context))
			{
				CleanableTemp = Cleanable;
			}
			target.Cleanable = CleanableTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlaceDecalActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlaceDecalActionEvent cast = (PlaceDecalActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlaceDecalActionEvent cast = (PlaceDecalActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlaceDecalActionEvent Instantiate()
	{
		return new PlaceDecalActionEvent();
	}
}
