using System;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Explosion.Components.OnTrigger;

[RegisterComponent]
public sealed class SharedRepulseAttractOnTriggerComponent : Component, ISerializationGenerated<SharedRepulseAttractOnTriggerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Speed;

	[DataField(null, false, 1, false, false, null)]
	public float Range;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public CollisionGroup CollisionMask = CollisionGroup.GhostImpassable;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SharedRepulseAttractOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedRepulseAttractOnTriggerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SharedRepulseAttractOnTriggerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		float SpeedTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Speed, ref SpeedTemp, hookCtx, false, context))
		{
			SpeedTemp = Speed;
		}
		target.Speed = SpeedTemp;
		float RangeTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
		{
			RangeTemp = Range;
		}
		target.Range = RangeTemp;
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		CollisionGroup CollisionMaskTemp = CollisionGroup.None;
		if (!serialization.TryCustomCopy<CollisionGroup>(CollisionMask, ref CollisionMaskTemp, hookCtx, false, context))
		{
			CollisionMaskTemp = CollisionMask;
		}
		target.CollisionMask = CollisionMaskTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SharedRepulseAttractOnTriggerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedRepulseAttractOnTriggerComponent cast = (SharedRepulseAttractOnTriggerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedRepulseAttractOnTriggerComponent cast = (SharedRepulseAttractOnTriggerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedRepulseAttractOnTriggerComponent def = (SharedRepulseAttractOnTriggerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedRepulseAttractOnTriggerComponent Instantiate()
	{
		return new SharedRepulseAttractOnTriggerComponent();
	}
}
