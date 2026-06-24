using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Weapons.Melee.Components;

[RegisterComponent]
public sealed class WeaponArcVisualsComponent : Component, ISerializationGenerated<WeaponArcVisualsComponent>, ISerializationGenerated
{
	public EntityUid? User;

	[DataField("animation", false, 1, false, false, null)]
	public WeaponArcAnimation Animation;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("fadeOut", false, 1, false, false, null)]
	public bool Fadeout = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WeaponArcVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (WeaponArcVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<WeaponArcVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			WeaponArcAnimation animation = WeaponArcAnimation.None;
			if (!serialization.TryCustomCopy<WeaponArcAnimation>(Animation, ref animation, hookCtx, false, context))
			{
				animation = Animation;
			}
			target.Animation = animation;
			bool fadeout = false;
			if (!serialization.TryCustomCopy<bool>(Fadeout, ref fadeout, hookCtx, false, context))
			{
				fadeout = Fadeout;
			}
			target.Fadeout = fadeout;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WeaponArcVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeaponArcVisualsComponent target2 = (WeaponArcVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeaponArcVisualsComponent target2 = (WeaponArcVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WeaponArcVisualsComponent target2 = (WeaponArcVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WeaponArcVisualsComponent Instantiate()
	{
		return new WeaponArcVisualsComponent();
	}
}
