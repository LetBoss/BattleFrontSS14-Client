using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Medical.Cryogenics;

[RegisterComponent]
[NetworkedComponent]
public sealed class InsideCryoPodComponent : Component, ISerializationGenerated<InsideCryoPodComponent>, ISerializationGenerated
{
	[ViewVariables]
	[DataField("previousOffset", false, 1, false, false, null)]
	public Vector2 PreviousOffset { get; set; } = new Vector2(0f, 0f);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InsideCryoPodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InsideCryoPodComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<InsideCryoPodComponent>(this, ref target, hookCtx, false, context))
		{
			Vector2 PreviousOffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(PreviousOffset, ref PreviousOffsetTemp, hookCtx, false, context))
			{
				PreviousOffsetTemp = serialization.CreateCopy<Vector2>(PreviousOffset, hookCtx, context, false);
			}
			target.PreviousOffset = PreviousOffsetTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InsideCryoPodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InsideCryoPodComponent cast = (InsideCryoPodComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InsideCryoPodComponent cast = (InsideCryoPodComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InsideCryoPodComponent def = (InsideCryoPodComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InsideCryoPodComponent Instantiate()
	{
		return new InsideCryoPodComponent();
	}
}
