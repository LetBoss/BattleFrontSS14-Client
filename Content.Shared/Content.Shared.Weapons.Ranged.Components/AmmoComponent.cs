using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[Virtual]
public class AmmoComponent : Component, IShootable, ISerializationGenerated<AmmoComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? MuzzleFlash = EntProtoId.op_Implicit("MuzzleFlashEffect");

	[DataField(null, false, 1, false, false, null)]
	public Vector2 MuzzleFlashOffset = new Vector2(0.5f, 0f);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref AmmoComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AmmoComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AmmoComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId? MuzzleFlashTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(MuzzleFlash, ref MuzzleFlashTemp, hookCtx, false, context))
			{
				MuzzleFlashTemp = serialization.CreateCopy<EntProtoId?>(MuzzleFlash, hookCtx, context, false);
			}
			target.MuzzleFlash = MuzzleFlashTemp;
			Vector2 MuzzleFlashOffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(MuzzleFlashOffset, ref MuzzleFlashOffsetTemp, hookCtx, false, context))
			{
				MuzzleFlashOffsetTemp = serialization.CreateCopy<Vector2>(MuzzleFlashOffset, hookCtx, context, false);
			}
			target.MuzzleFlashOffset = MuzzleFlashOffsetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref AmmoComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoComponent cast = (AmmoComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoComponent cast = (AmmoComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoComponent def = (AmmoComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AmmoComponent Instantiate()
	{
		return new AmmoComponent();
	}
}
