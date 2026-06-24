using System;
using Content.Shared.Labels.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Labels.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(LabelSystem) })]
public sealed class PaperLabelTypeComponent : Component, ISerializationGenerated<PaperLabelTypeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string PaperType = "Paper";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PaperLabelTypeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PaperLabelTypeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PaperLabelTypeComponent>(this, ref target, hookCtx, false, context))
		{
			string PaperTypeTemp = null;
			if (PaperType == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PaperType, ref PaperTypeTemp, hookCtx, false, context))
			{
				PaperTypeTemp = PaperType;
			}
			target.PaperType = PaperTypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PaperLabelTypeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperLabelTypeComponent cast = (PaperLabelTypeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperLabelTypeComponent cast = (PaperLabelTypeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PaperLabelTypeComponent def = (PaperLabelTypeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PaperLabelTypeComponent Instantiate()
	{
		return new PaperLabelTypeComponent();
	}
}
