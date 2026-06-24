using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Name;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoRankNamesComponent : Component, ISerializationGenerated<XenoRankNamesComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<int, LocId> RankNames = new Dictionary<int, LocId>
	{
		{
			0,
			LocId.op_Implicit("rmc-xeno-young")
		},
		{
			2,
			LocId.op_Implicit("rmc-xeno-mature")
		},
		{
			3,
			LocId.op_Implicit("rmc-xeno-elder")
		},
		{
			4,
			LocId.op_Implicit("rmc-xeno-ancient")
		},
		{
			5,
			LocId.op_Implicit("rmc-xeno-prime")
		},
		{
			6,
			LocId.op_Implicit("rmc-xeno-prime")
		}
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoRankNamesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoRankNamesComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoRankNamesComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<int, LocId> RankNamesTemp = null;
			if (RankNames == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<int, LocId>>(RankNames, ref RankNamesTemp, hookCtx, true, context))
			{
				RankNamesTemp = serialization.CreateCopy<Dictionary<int, LocId>>(RankNames, hookCtx, context, false);
			}
			target.RankNames = RankNamesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoRankNamesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRankNamesComponent cast = (XenoRankNamesComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRankNamesComponent cast = (XenoRankNamesComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoRankNamesComponent def = (XenoRankNamesComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoRankNamesComponent Instantiate()
	{
		return new XenoRankNamesComponent();
	}
}
