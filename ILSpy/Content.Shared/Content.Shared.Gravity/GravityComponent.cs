using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Gravity;

[RegisterComponent]
[NetworkedComponent]
public sealed class GravityComponent : Component, ISerializationGenerated<GravityComponent>, ISerializationGenerated
{
	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("inherent", false, 1, false, false, null)]
	public bool Inherent;

	[DataField("gravityShakeSound", false, 1, false, false, null)]
	public SoundSpecifier GravityShakeSound { get; set; } = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/alert.ogg", (AudioParams?)null);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool EnabledVV
	{
		get
		{
			return Enabled;
		}
		set
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if (Enabled != value)
			{
				Enabled = value;
				IEntityManager obj = IoCManager.Resolve<IEntityManager>();
				GravityChangedEvent ev = new GravityChangedEvent(((Component)this).Owner, value);
				((IDirectedEventBus)obj.EventBus).RaiseLocalEvent<GravityChangedEvent>(((Component)this).Owner, ref ev, false);
				obj.Dirty(((Component)this).Owner, (IComponent)(object)this, (MetaDataComponent)null);
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GravityComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GravityComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GravityComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier GravityShakeSoundTemp = null;
			if (GravityShakeSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(GravityShakeSound, ref GravityShakeSoundTemp, hookCtx, true, context))
			{
				GravityShakeSoundTemp = serialization.CreateCopy<SoundSpecifier>(GravityShakeSound, hookCtx, context, false);
			}
			target.GravityShakeSound = GravityShakeSoundTemp;
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			bool InherentTemp = false;
			if (!serialization.TryCustomCopy<bool>(Inherent, ref InherentTemp, hookCtx, false, context))
			{
				InherentTemp = Inherent;
			}
			target.Inherent = InherentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GravityComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GravityComponent cast = (GravityComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GravityComponent cast = (GravityComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GravityComponent def = (GravityComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GravityComponent Instantiate()
	{
		return new GravityComponent();
	}
}
