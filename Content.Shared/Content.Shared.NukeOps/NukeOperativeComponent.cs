using System;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.NukeOps;

[RegisterComponent]
[NetworkedComponent]
public sealed class NukeOperativeComponent : Component, ISerializationGenerated<NukeOperativeComponent>, ISerializationGenerated
{
	[DataField("syndStatusIcon", false, 1, false, false, typeof(PrototypeIdSerializer<FactionIconPrototype>))]
	public string SyndStatusIcon = "SyndicateFaction";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NukeOperativeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NukeOperativeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NukeOperativeComponent>(this, ref target, hookCtx, false, context))
		{
			string SyndStatusIconTemp = null;
			if (SyndStatusIcon == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SyndStatusIcon, ref SyndStatusIconTemp, hookCtx, false, context))
			{
				SyndStatusIconTemp = SyndStatusIcon;
			}
			target.SyndStatusIcon = SyndStatusIconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NukeOperativeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NukeOperativeComponent cast = (NukeOperativeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NukeOperativeComponent cast = (NukeOperativeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NukeOperativeComponent def = (NukeOperativeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NukeOperativeComponent Instantiate()
	{
		return new NukeOperativeComponent();
	}
}
