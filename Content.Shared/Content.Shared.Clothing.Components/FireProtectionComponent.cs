using System;
using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[Access(new Type[] { typeof(FireProtectionSystem) })]
public sealed class FireProtectionComponent : Component, ISerializationGenerated<FireProtectionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public float Reduction;

	[DataField(null, false, 1, false, false, null)]
	public LocId ExamineMessage = LocId.op_Implicit("fire-protection-reduction-value");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FireProtectionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FireProtectionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FireProtectionComponent>(this, ref target, hookCtx, false, context))
		{
			float ReductionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Reduction, ref ReductionTemp, hookCtx, false, context))
			{
				ReductionTemp = Reduction;
			}
			target.Reduction = ReductionTemp;
			LocId ExamineMessageTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ExamineMessage, ref ExamineMessageTemp, hookCtx, false, context))
			{
				ExamineMessageTemp = serialization.CreateCopy<LocId>(ExamineMessage, hookCtx, context, false);
			}
			target.ExamineMessage = ExamineMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FireProtectionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireProtectionComponent cast = (FireProtectionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireProtectionComponent cast = (FireProtectionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireProtectionComponent def = (FireProtectionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FireProtectionComponent Instantiate()
	{
		return new FireProtectionComponent();
	}
}
