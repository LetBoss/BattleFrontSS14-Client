using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagAdminInterferenceComponent : Component, ISerializationGenerated<GulagAdminInterferenceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Accepted;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GulagAdminInterferenceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GulagAdminInterferenceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GulagAdminInterferenceComponent>(this, ref target, hookCtx, false, context))
		{
			bool AcceptedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Accepted, ref AcceptedTemp, hookCtx, false, context))
			{
				AcceptedTemp = Accepted;
			}
			target.Accepted = AcceptedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GulagAdminInterferenceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagAdminInterferenceComponent cast = (GulagAdminInterferenceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagAdminInterferenceComponent cast = (GulagAdminInterferenceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagAdminInterferenceComponent def = (GulagAdminInterferenceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GulagAdminInterferenceComponent Instantiate()
	{
		return new GulagAdminInterferenceComponent();
	}
}
