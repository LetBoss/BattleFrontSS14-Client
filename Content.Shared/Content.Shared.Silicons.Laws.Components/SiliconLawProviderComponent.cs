using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedSiliconLawSystem) })]
public sealed class SiliconLawProviderComponent : Component, ISerializationGenerated<SiliconLawProviderComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<SiliconLawsetPrototype> Laws = ProtoId<SiliconLawsetPrototype>.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SiliconLawset? Lawset;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? LawUploadSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/Misc/cryo_warning.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public bool Subverted;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SiliconLawProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SiliconLawProviderComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<SiliconLawProviderComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		ProtoId<SiliconLawsetPrototype> LawsTemp = default(ProtoId<SiliconLawsetPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<SiliconLawsetPrototype>>(Laws, ref LawsTemp, hookCtx, false, context))
		{
			LawsTemp = serialization.CreateCopy<ProtoId<SiliconLawsetPrototype>>(Laws, hookCtx, context, false);
		}
		target.Laws = LawsTemp;
		SiliconLawset LawsetTemp = null;
		if (!serialization.TryCustomCopy<SiliconLawset>(Lawset, ref LawsetTemp, hookCtx, false, context))
		{
			if (Lawset == null)
			{
				LawsetTemp = null;
			}
			else
			{
				serialization.CopyTo<SiliconLawset>(Lawset, ref LawsetTemp, hookCtx, context, false);
			}
		}
		target.Lawset = LawsetTemp;
		SoundSpecifier LawUploadSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(LawUploadSound, ref LawUploadSoundTemp, hookCtx, true, context))
		{
			LawUploadSoundTemp = serialization.CreateCopy<SoundSpecifier>(LawUploadSound, hookCtx, context, false);
		}
		target.LawUploadSound = LawUploadSoundTemp;
		bool SubvertedTemp = false;
		if (!serialization.TryCustomCopy<bool>(Subverted, ref SubvertedTemp, hookCtx, false, context))
		{
			SubvertedTemp = Subverted;
		}
		target.Subverted = SubvertedTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SiliconLawProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawProviderComponent cast = (SiliconLawProviderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawProviderComponent cast = (SiliconLawProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawProviderComponent def = (SiliconLawProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SiliconLawProviderComponent Instantiate()
	{
		return new SiliconLawProviderComponent();
	}
}
