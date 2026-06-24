using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.AcidSlash;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(XenoAcidSlashSystem) })]
public sealed class XenoAcidSlashComponent : Component, ISerializationGenerated<XenoAcidSlashComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry? Acid;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoAcidSlashComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoAcidSlashComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoAcidSlashComponent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry AcidTemp = null;
			if (!serialization.TryCustomCopy<ComponentRegistry>(Acid, ref AcidTemp, hookCtx, false, context))
			{
				AcidTemp = serialization.CreateCopy<ComponentRegistry>(Acid, hookCtx, context, false);
			}
			target.Acid = AcidTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoAcidSlashComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAcidSlashComponent cast = (XenoAcidSlashComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAcidSlashComponent cast = (XenoAcidSlashComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAcidSlashComponent def = (XenoAcidSlashComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoAcidSlashComponent Instantiate()
	{
		return new XenoAcidSlashComponent();
	}
}
