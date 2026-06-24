using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Sprite;

[RegisterComponent]
[NetworkedComponent]
public sealed class RandomSpriteComponent : Component, ISerializationGenerated<RandomSpriteComponent>, ISerializationGenerated
{
	[DataField("getAllGroups", false, 1, false, false, null)]
	public bool GetAllGroups;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("available", false, 1, false, false, null)]
	public List<Dictionary<string, Dictionary<string, string?>>> Available = new List<Dictionary<string, Dictionary<string, string>>>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("selected", false, 1, false, false, null)]
	public Dictionary<string, (string State, Color? Color)> Selected = new Dictionary<string, (string, Color?)>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RandomSpriteComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RandomSpriteComponent>(this, ref target, hookCtx, false, context))
		{
			bool GetAllGroupsTemp = false;
			if (!serialization.TryCustomCopy<bool>(GetAllGroups, ref GetAllGroupsTemp, hookCtx, false, context))
			{
				GetAllGroupsTemp = GetAllGroups;
			}
			target.GetAllGroups = GetAllGroupsTemp;
			List<Dictionary<string, Dictionary<string, string>>> AvailableTemp = null;
			if (Available == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<Dictionary<string, Dictionary<string, string>>>>(Available, ref AvailableTemp, hookCtx, true, context))
			{
				AvailableTemp = serialization.CreateCopy<List<Dictionary<string, Dictionary<string, string>>>>(Available, hookCtx, context, false);
			}
			target.Available = AvailableTemp;
			Dictionary<string, (string, Color?)> SelectedTemp = null;
			if (Selected == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, (string, Color?)>>(Selected, ref SelectedTemp, hookCtx, true, context))
			{
				SelectedTemp = serialization.CreateCopy<Dictionary<string, (string, Color?)>>(Selected, hookCtx, context, false);
			}
			target.Selected = SelectedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomSpriteComponent cast = (RandomSpriteComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomSpriteComponent cast = (RandomSpriteComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomSpriteComponent def = (RandomSpriteComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RandomSpriteComponent Instantiate()
	{
		return new RandomSpriteComponent();
	}
}
