using System;
using System.Collections.Generic;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Explosion;

[RegisterComponent]
public sealed class ExplosionVisualsTexturesComponent : Component, ISerializationGenerated<ExplosionVisualsTexturesComponent>, ISerializationGenerated
{
	public EntityUid LightEntity;

	public float IntensityPerState;

	public List<Texture[]> FireFrames = new List<Texture[]>();

	public Color? FireColor;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExplosionVisualsTexturesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ExplosionVisualsTexturesComponent)(object)val;
		serialization.TryCustomCopy<ExplosionVisualsTexturesComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExplosionVisualsTexturesComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionVisualsTexturesComponent target2 = (ExplosionVisualsTexturesComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionVisualsTexturesComponent target2 = (ExplosionVisualsTexturesComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionVisualsTexturesComponent target2 = (ExplosionVisualsTexturesComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExplosionVisualsTexturesComponent Instantiate()
	{
		return new ExplosionVisualsTexturesComponent();
	}
}
