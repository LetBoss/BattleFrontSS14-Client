using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.HealthExaminable;

[RegisterComponent]
[Access(new Type[] { typeof(HealthExaminableSystem) })]
public sealed class HealthExaminableComponent : Component, ISerializationGenerated<HealthExaminableComponent>, ISerializationGenerated
{
	public List<FixedPoint2> Thresholds = new List<FixedPoint2>
	{
		FixedPoint2.New(8),
		FixedPoint2.New(15),
		FixedPoint2.New(30),
		FixedPoint2.New(50),
		FixedPoint2.New(75),
		FixedPoint2.New(100),
		FixedPoint2.New(200)
	};

	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<DamageTypePrototype>> ExaminableTypes;

	[DataField(null, false, 1, false, false, null)]
	public string LocPrefix = "carbon";

	[DataField(null, false, 1, false, false, null)]
	public bool ExamineShowEmpty = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HealthExaminableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HealthExaminableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HealthExaminableComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<DamageTypePrototype>> ExaminableTypesTemp = null;
			if (ExaminableTypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<DamageTypePrototype>>>(ExaminableTypes, ref ExaminableTypesTemp, hookCtx, true, context))
			{
				ExaminableTypesTemp = serialization.CreateCopy<HashSet<ProtoId<DamageTypePrototype>>>(ExaminableTypes, hookCtx, context, false);
			}
			target.ExaminableTypes = ExaminableTypesTemp;
			string LocPrefixTemp = null;
			if (LocPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LocPrefix, ref LocPrefixTemp, hookCtx, false, context))
			{
				LocPrefixTemp = LocPrefix;
			}
			target.LocPrefix = LocPrefixTemp;
			bool ExamineShowEmptyTemp = false;
			if (!serialization.TryCustomCopy<bool>(ExamineShowEmpty, ref ExamineShowEmptyTemp, hookCtx, false, context))
			{
				ExamineShowEmptyTemp = ExamineShowEmpty;
			}
			target.ExamineShowEmpty = ExamineShowEmptyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HealthExaminableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HealthExaminableComponent cast = (HealthExaminableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HealthExaminableComponent cast = (HealthExaminableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HealthExaminableComponent def = (HealthExaminableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HealthExaminableComponent Instantiate()
	{
		return new HealthExaminableComponent();
	}
}
