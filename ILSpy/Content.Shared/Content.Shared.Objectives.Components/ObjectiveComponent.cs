using System;
using Content.Shared.Objectives.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Objectives.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedObjectivesSystem) })]
[EntityCategory(new string[] { "Objectives" })]
public sealed class ObjectiveComponent : Component, ISerializationGenerated<ObjectiveComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public float Difficulty;

	[DataField(null, false, 1, false, false, null)]
	public bool Unique = true;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Icon;

	[DataField("issuer", false, 1, true, false, null)]
	private LocId Issuer { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocIssuer => Loc.GetString(LocId.op_Implicit(Issuer));

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ObjectiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ObjectiveComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ObjectiveComponent>(this, ref target, hookCtx, false, context))
		{
			float DifficultyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Difficulty, ref DifficultyTemp, hookCtx, false, context))
			{
				DifficultyTemp = Difficulty;
			}
			target.Difficulty = DifficultyTemp;
			LocId IssuerTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Issuer, ref IssuerTemp, hookCtx, false, context))
			{
				IssuerTemp = serialization.CreateCopy<LocId>(Issuer, hookCtx, context, false);
			}
			target.Issuer = IssuerTemp;
			bool UniqueTemp = false;
			if (!serialization.TryCustomCopy<bool>(Unique, ref UniqueTemp, hookCtx, false, context))
			{
				UniqueTemp = Unique;
			}
			target.Unique = UniqueTemp;
			SpriteSpecifier IconTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Icon, ref IconTemp, hookCtx, true, context))
			{
				IconTemp = serialization.CreateCopy<SpriteSpecifier>(Icon, hookCtx, context, false);
			}
			target.Icon = IconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ObjectiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ObjectiveComponent cast = (ObjectiveComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ObjectiveComponent cast = (ObjectiveComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ObjectiveComponent def = (ObjectiveComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ObjectiveComponent Instantiate()
	{
		return new ObjectiveComponent();
	}
}
