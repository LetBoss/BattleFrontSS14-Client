using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Weather;

[RegisterComponent]
[NetworkedComponent]
public sealed class WeatherComponent : Component, ISerializationGenerated<WeatherComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<WeatherPrototype>, WeatherData> Weather = new Dictionary<ProtoId<WeatherPrototype>, WeatherData>();

	public static readonly TimeSpan StartupTime = TimeSpan.FromSeconds(15L);

	public static readonly TimeSpan ShutdownTime = TimeSpan.FromSeconds(15L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WeatherComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WeatherComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<WeatherComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<ProtoId<WeatherPrototype>, WeatherData> WeatherTemp = null;
			if (Weather == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<WeatherPrototype>, WeatherData>>(Weather, ref WeatherTemp, hookCtx, true, context))
			{
				WeatherTemp = serialization.CreateCopy<Dictionary<ProtoId<WeatherPrototype>, WeatherData>>(Weather, hookCtx, context, false);
			}
			target.Weather = WeatherTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WeatherComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeatherComponent cast = (WeatherComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeatherComponent cast = (WeatherComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeatherComponent def = (WeatherComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WeatherComponent Instantiate()
	{
		return new WeatherComponent();
	}
}
