using System;
using System.Collections.Generic;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Storage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedLightReplacerSystem) })]
public sealed class LightReplacerComponent : Component, ISerializationGenerated<LightReplacerComponent>, ISerializationGenerated
{
	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier Sound;

	[ViewVariables]
	public Container InsertedBulbs;

	[DataField("contents", false, 1, false, false, null)]
	public List<EntitySpawnEntry> Contents;

	public LightReplacerComponent()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		SoundPathSpecifier val = new SoundPathSpecifier("/Audio/Weapons/click.ogg", (AudioParams?)null);
		AudioParams val2 = default(AudioParams);
		((AudioParams)(ref val2))._002Ector();
		((AudioParams)(ref val2)).Volume = -4f;
		((SoundSpecifier)val).Params = val2;
		Sound = (SoundSpecifier)val;
		Contents = new List<EntitySpawnEntry>();
		((Component)this)._002Ector();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LightReplacerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LightReplacerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LightReplacerComponent>(this, ref target, hookCtx, false, context))
		{
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
			List<EntitySpawnEntry> ContentsTemp = null;
			if (Contents == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntitySpawnEntry>>(Contents, ref ContentsTemp, hookCtx, true, context))
			{
				ContentsTemp = serialization.CreateCopy<List<EntitySpawnEntry>>(Contents, hookCtx, context, false);
			}
			target.Contents = ContentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LightReplacerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightReplacerComponent cast = (LightReplacerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightReplacerComponent cast = (LightReplacerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightReplacerComponent def = (LightReplacerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LightReplacerComponent Instantiate()
	{
		return new LightReplacerComponent();
	}
}
