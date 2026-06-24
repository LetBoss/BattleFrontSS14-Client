using System;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components;

[Access(new Type[] { typeof(SharedCreamPieSystem) })]
[RegisterComponent]
public sealed class CreamPieComponent : Component, ISerializationGenerated<CreamPieComponent>, ISerializationGenerated
{
	public const string PayloadSlotName = "payloadSlot";

	[DataField("paralyzeTime", false, 1, false, false, null)]
	public float ParalyzeTime { get; private set; } = 1f;

	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier Sound { get; private set; } = (SoundSpecifier)new SoundCollectionSpecifier("desecration", (AudioParams?)null);

	[ViewVariables]
	public bool Splatted { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CreamPieComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CreamPieComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CreamPieComponent>(this, ref target, hookCtx, false, context))
		{
			float ParalyzeTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ParalyzeTime, ref ParalyzeTimeTemp, hookCtx, false, context))
			{
				ParalyzeTimeTemp = ParalyzeTime;
			}
			target.ParalyzeTime = ParalyzeTimeTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CreamPieComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreamPieComponent cast = (CreamPieComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreamPieComponent cast = (CreamPieComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreamPieComponent def = (CreamPieComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CreamPieComponent Instantiate()
	{
		return new CreamPieComponent();
	}
}
