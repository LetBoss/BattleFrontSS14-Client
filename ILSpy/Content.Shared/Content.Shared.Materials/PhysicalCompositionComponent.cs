using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Materials;

[RegisterComponent]
public sealed class PhysicalCompositionComponent : Component, ISerializationGenerated<PhysicalCompositionComponent>, ISerializationGenerated
{
	[DataField("materialComposition", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MaterialPrototype>))]
	public Dictionary<string, int> MaterialComposition = new Dictionary<string, int>();

	[DataField("chemicalComposition", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
	public Dictionary<string, FixedPoint2> ChemicalComposition = new Dictionary<string, FixedPoint2>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PhysicalCompositionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PhysicalCompositionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PhysicalCompositionComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, int> MaterialCompositionTemp = null;
			if (MaterialComposition == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, int>>(MaterialComposition, ref MaterialCompositionTemp, hookCtx, true, context))
			{
				MaterialCompositionTemp = serialization.CreateCopy<Dictionary<string, int>>(MaterialComposition, hookCtx, context, false);
			}
			target.MaterialComposition = MaterialCompositionTemp;
			Dictionary<string, FixedPoint2> ChemicalCompositionTemp = null;
			if (ChemicalComposition == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, FixedPoint2>>(ChemicalComposition, ref ChemicalCompositionTemp, hookCtx, true, context))
			{
				ChemicalCompositionTemp = serialization.CreateCopy<Dictionary<string, FixedPoint2>>(ChemicalComposition, hookCtx, context, false);
			}
			target.ChemicalComposition = ChemicalCompositionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PhysicalCompositionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicalCompositionComponent cast = (PhysicalCompositionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicalCompositionComponent cast = (PhysicalCompositionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PhysicalCompositionComponent def = (PhysicalCompositionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PhysicalCompositionComponent Instantiate()
	{
		return new PhysicalCompositionComponent();
	}
}
