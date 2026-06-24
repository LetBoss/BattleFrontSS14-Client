using System;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedToolSystem) })]
public sealed class ToolComponent : Component, ISerializationGenerated<ToolComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<SkillDefinitionComponent> Skill = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillEngineer");

	[DataField(null, false, 1, false, false, null)]
	public PrototypeFlags<ToolQualityPrototype> Qualities = new PrototypeFlags<ToolQualityPrototype>();

	[DataField(null, false, 1, false, false, null)]
	public float SpeedModifier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? UseSound;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToolComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ToolComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ToolComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId<SkillDefinitionComponent> SkillTemp = default(EntProtoId<SkillDefinitionComponent>);
			if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(Skill, ref SkillTemp, hookCtx, false, context))
			{
				SkillTemp = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(Skill, hookCtx, context, false);
			}
			target.Skill = SkillTemp;
			PrototypeFlags<ToolQualityPrototype> QualitiesTemp = null;
			if (Qualities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<PrototypeFlags<ToolQualityPrototype>>(Qualities, ref QualitiesTemp, hookCtx, false, context))
			{
				QualitiesTemp = serialization.CreateCopy<PrototypeFlags<ToolQualityPrototype>>(Qualities, hookCtx, context, false);
			}
			target.Qualities = QualitiesTemp;
			float SpeedModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpeedModifier, ref SpeedModifierTemp, hookCtx, false, context))
			{
				SpeedModifierTemp = SpeedModifier;
			}
			target.SpeedModifier = SpeedModifierTemp;
			SoundSpecifier UseSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(UseSound, ref UseSoundTemp, hookCtx, true, context))
			{
				UseSoundTemp = serialization.CreateCopy<SoundSpecifier>(UseSound, hookCtx, context, false);
			}
			target.UseSound = UseSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToolComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolComponent cast = (ToolComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolComponent cast = (ToolComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolComponent def = (ToolComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToolComponent Instantiate()
	{
		return new ToolComponent();
	}
}
