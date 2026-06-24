using System;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Warps;

[RegisterComponent]
[NetworkedComponent]
public sealed class WarpPointComponent : Component, ISerializationGenerated<WarpPointComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, false, null)]
	public string? Location;

	[DataField(null, false, 1, false, false, null)]
	public bool Follow;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WarpPointComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WarpPointComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<WarpPointComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string LocationTemp = null;
		if (!serialization.TryCustomCopy<string>(Location, ref LocationTemp, hookCtx, false, context))
		{
			LocationTemp = Location;
		}
		target.Location = LocationTemp;
		bool FollowTemp = false;
		if (!serialization.TryCustomCopy<bool>(Follow, ref FollowTemp, hookCtx, false, context))
		{
			FollowTemp = Follow;
		}
		target.Follow = FollowTemp;
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		target.Blacklist = BlacklistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WarpPointComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WarpPointComponent cast = (WarpPointComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WarpPointComponent cast = (WarpPointComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WarpPointComponent def = (WarpPointComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WarpPointComponent Instantiate()
	{
		return new WarpPointComponent();
	}
}
