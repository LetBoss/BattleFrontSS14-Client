using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Stunnable;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedStunSystem) })]
public sealed class StunOnContactComponent : Component, ISerializationGenerated<StunOnContactComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string FixtureId = "fix";

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Duration = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist Blacklist = new EntityWhitelist();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StunOnContactComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StunOnContactComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<StunOnContactComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string FixtureIdTemp = null;
		if (FixtureId == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(FixtureId, ref FixtureIdTemp, hookCtx, false, context))
		{
			FixtureIdTemp = FixtureId;
		}
		target.FixtureId = FixtureIdTemp;
		TimeSpan DurationTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(Duration, ref DurationTemp, hookCtx, false, context))
		{
			DurationTemp = serialization.CreateCopy<TimeSpan>(Duration, hookCtx, context, false);
		}
		target.Duration = DurationTemp;
		EntityWhitelist BlacklistTemp = null;
		if (Blacklist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, true);
			}
		}
		target.Blacklist = BlacklistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StunOnContactComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StunOnContactComponent cast = (StunOnContactComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StunOnContactComponent cast = (StunOnContactComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StunOnContactComponent def = (StunOnContactComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StunOnContactComponent Instantiate()
	{
		return new StunOnContactComponent();
	}
}
