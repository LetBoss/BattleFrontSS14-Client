using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.DoAfter;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedDoAfterSystem) })]
public sealed class DoAfterComponent : Component, ISerializationGenerated<DoAfterComponent>, ISerializationGenerated
{
	[DataField("nextId", false, 1, false, false, null)]
	public ushort NextId;

	[DataField("doAfters", false, 1, false, false, null)]
	public Dictionary<ushort, DoAfter> DoAfters = new Dictionary<ushort, DoAfter>();

	public readonly Dictionary<ushort, TaskCompletionSource<DoAfterStatus>> AwaitedDoAfters = new Dictionary<ushort, TaskCompletionSource<DoAfterStatus>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DoAfterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DoAfterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DoAfterComponent>(this, ref target, hookCtx, false, context))
		{
			ushort NextIdTemp = 0;
			if (!serialization.TryCustomCopy<ushort>(NextId, ref NextIdTemp, hookCtx, false, context))
			{
				NextIdTemp = NextId;
			}
			target.NextId = NextIdTemp;
			Dictionary<ushort, DoAfter> DoAftersTemp = null;
			if (DoAfters == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ushort, DoAfter>>(DoAfters, ref DoAftersTemp, hookCtx, true, context))
			{
				DoAftersTemp = serialization.CreateCopy<Dictionary<ushort, DoAfter>>(DoAfters, hookCtx, context, false);
			}
			target.DoAfters = DoAftersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DoAfterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterComponent cast = (DoAfterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterComponent cast = (DoAfterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterComponent def = (DoAfterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DoAfterComponent Instantiate()
	{
		return new DoAfterComponent();
	}
}
