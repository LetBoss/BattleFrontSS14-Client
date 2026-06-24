using System;
using System.Collections.Generic;
using Content.Shared.Mobs;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Silicons.Bots;

[RegisterComponent]
[Access(new Type[] { typeof(MedibotSystem) })]
public sealed class MedibotComponent : Component, ISerializationGenerated<MedibotComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<MobState, MedibotTreatment> Treatments = new Dictionary<MobState, MedibotTreatment>();

	[DataField("injectSound", false, 1, false, false, null)]
	public SoundSpecifier InjectSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/hypospray.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MedibotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MedibotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MedibotComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<MobState, MedibotTreatment> TreatmentsTemp = null;
			if (Treatments == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<MobState, MedibotTreatment>>(Treatments, ref TreatmentsTemp, hookCtx, true, context))
			{
				TreatmentsTemp = serialization.CreateCopy<Dictionary<MobState, MedibotTreatment>>(Treatments, hookCtx, context, false);
			}
			target.Treatments = TreatmentsTemp;
			SoundSpecifier InjectSoundTemp = null;
			if (InjectSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(InjectSound, ref InjectSoundTemp, hookCtx, true, context))
			{
				InjectSoundTemp = serialization.CreateCopy<SoundSpecifier>(InjectSound, hookCtx, context, false);
			}
			target.InjectSound = InjectSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MedibotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MedibotComponent cast = (MedibotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MedibotComponent cast = (MedibotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MedibotComponent def = (MedibotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MedibotComponent Instantiate()
	{
		return new MedibotComponent();
	}
}
