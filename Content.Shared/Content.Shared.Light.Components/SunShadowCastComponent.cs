using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SunShadowCastComponent : Component, ISerializationGenerated<SunShadowCastComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Vector2[] Points = new Vector2[4]
	{
		new Vector2(-0.5f, -0.5f),
		new Vector2(0.5f, -0.5f),
		new Vector2(0.5f, 0.5f),
		new Vector2(-0.5f, 0.5f)
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SunShadowCastComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SunShadowCastComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SunShadowCastComponent>(this, ref target, hookCtx, false, context))
		{
			Vector2[] PointsTemp = null;
			if (Points == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Vector2[]>(Points, ref PointsTemp, hookCtx, true, context))
			{
				PointsTemp = serialization.CreateCopy<Vector2[]>(Points, hookCtx, context, false);
			}
			target.Points = PointsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SunShadowCastComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SunShadowCastComponent cast = (SunShadowCastComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SunShadowCastComponent cast = (SunShadowCastComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SunShadowCastComponent def = (SunShadowCastComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SunShadowCastComponent Instantiate()
	{
		return new SunShadowCastComponent();
	}
}
