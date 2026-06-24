using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Materials;

[RegisterComponent]
public sealed class RecyclerVisualsComponent : Component, ISerializationGenerated<RecyclerVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string BloodyKey = "bld";

	[DataField(null, false, 1, false, false, null)]
	public string BaseKey = "grinder-o";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RecyclerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (RecyclerVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<RecyclerVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string bloodyKey = null;
			if (BloodyKey == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BloodyKey, ref bloodyKey, hookCtx, false, context))
			{
				bloodyKey = BloodyKey;
			}
			target.BloodyKey = bloodyKey;
			string baseKey = null;
			if (BaseKey == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BaseKey, ref baseKey, hookCtx, false, context))
			{
				baseKey = BaseKey;
			}
			target.BaseKey = baseKey;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RecyclerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RecyclerVisualsComponent target2 = (RecyclerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RecyclerVisualsComponent target2 = (RecyclerVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RecyclerVisualsComponent target2 = (RecyclerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RecyclerVisualsComponent Instantiate()
	{
		return new RecyclerVisualsComponent();
	}
}
