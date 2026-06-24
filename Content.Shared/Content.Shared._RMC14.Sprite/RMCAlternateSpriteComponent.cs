using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Sprite;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCAlternateSpriteComponent : Component, ISerializationGenerated<RMCAlternateSpriteComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string NormalSprite;

	[DataField(null, false, 1, false, false, null)]
	public string AlternateSprite;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCAlternateSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCAlternateSpriteComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCAlternateSpriteComponent>(this, ref target, hookCtx, false, context))
		{
			string NormalSpriteTemp = null;
			if (NormalSprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(NormalSprite, ref NormalSpriteTemp, hookCtx, false, context))
			{
				NormalSpriteTemp = NormalSprite;
			}
			target.NormalSprite = NormalSpriteTemp;
			string AlternateSpriteTemp = null;
			if (AlternateSprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(AlternateSprite, ref AlternateSpriteTemp, hookCtx, false, context))
			{
				AlternateSpriteTemp = AlternateSprite;
			}
			target.AlternateSprite = AlternateSpriteTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCAlternateSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCAlternateSpriteComponent cast = (RMCAlternateSpriteComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCAlternateSpriteComponent cast = (RMCAlternateSpriteComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCAlternateSpriteComponent def = (RMCAlternateSpriteComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCAlternateSpriteComponent Instantiate()
	{
		return new RMCAlternateSpriteComponent();
	}
}
