using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Blocking;

[RegisterComponent]
public sealed class BlockingUserComponent : Component, ISerializationGenerated<BlockingUserComponent>, ISerializationGenerated
{
	[DataField("blockingItem", false, 1, false, false, null)]
	public EntityUid? BlockingItem;

	[DataField("originalBodyType", false, 1, false, false, null)]
	public BodyType OriginalBodyType;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BlockingUserComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BlockingUserComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BlockingUserComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? BlockingItemTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(BlockingItem, ref BlockingItemTemp, hookCtx, false, context))
			{
				BlockingItemTemp = serialization.CreateCopy<EntityUid?>(BlockingItem, hookCtx, context, false);
			}
			target.BlockingItem = BlockingItemTemp;
			BodyType OriginalBodyTypeTemp = (BodyType)0;
			if (!serialization.TryCustomCopy<BodyType>(OriginalBodyType, ref OriginalBodyTypeTemp, hookCtx, false, context))
			{
				OriginalBodyTypeTemp = OriginalBodyType;
			}
			target.OriginalBodyType = OriginalBodyTypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BlockingUserComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlockingUserComponent cast = (BlockingUserComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlockingUserComponent cast = (BlockingUserComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BlockingUserComponent def = (BlockingUserComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BlockingUserComponent Instantiate()
	{
		return new BlockingUserComponent();
	}
}
