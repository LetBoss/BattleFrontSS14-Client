using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[Access(new Type[] { typeof(RehydratableSystem) })]
public sealed class RehydratableComponent : Component, ISerializationGenerated<RehydratableComponent>, ISerializationGenerated
{
	[DataField("catalyst", false, 1, false, false, null)]
	public ProtoId<ReagentPrototype> CatalystPrototype = ProtoId<ReagentPrototype>.op_Implicit("Water");

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 CatalystMinimum = FixedPoint2.Zero;

	[DataField(null, false, 1, true, false, null)]
	public List<EntProtoId> PossibleSpawns = new List<EntProtoId>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RehydratableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RehydratableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RehydratableComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ReagentPrototype> CatalystPrototypeTemp = default(ProtoId<ReagentPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(CatalystPrototype, ref CatalystPrototypeTemp, hookCtx, false, context))
			{
				CatalystPrototypeTemp = serialization.CreateCopy<ProtoId<ReagentPrototype>>(CatalystPrototype, hookCtx, context, false);
			}
			target.CatalystPrototype = CatalystPrototypeTemp;
			FixedPoint2 CatalystMinimumTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(CatalystMinimum, ref CatalystMinimumTemp, hookCtx, false, context))
			{
				CatalystMinimumTemp = serialization.CreateCopy<FixedPoint2>(CatalystMinimum, hookCtx, context, false);
			}
			target.CatalystMinimum = CatalystMinimumTemp;
			List<EntProtoId> PossibleSpawnsTemp = null;
			if (PossibleSpawns == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntProtoId>>(PossibleSpawns, ref PossibleSpawnsTemp, hookCtx, true, context))
			{
				PossibleSpawnsTemp = serialization.CreateCopy<List<EntProtoId>>(PossibleSpawns, hookCtx, context, false);
			}
			target.PossibleSpawns = PossibleSpawnsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RehydratableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RehydratableComponent cast = (RehydratableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RehydratableComponent cast = (RehydratableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RehydratableComponent def = (RehydratableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RehydratableComponent Instantiate()
	{
		return new RehydratableComponent();
	}
}
