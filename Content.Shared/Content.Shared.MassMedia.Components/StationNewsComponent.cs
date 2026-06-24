using System;
using System.Collections.Generic;
using Content.Shared.MassMedia.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.MassMedia.Components;

[RegisterComponent]
public sealed class StationNewsComponent : Component, ISerializationGenerated<StationNewsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<NewsArticle> Articles = new List<NewsArticle>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StationNewsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StationNewsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StationNewsComponent>(this, ref target, hookCtx, false, context))
		{
			List<NewsArticle> ArticlesTemp = null;
			if (Articles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<NewsArticle>>(Articles, ref ArticlesTemp, hookCtx, true, context))
			{
				ArticlesTemp = serialization.CreateCopy<List<NewsArticle>>(Articles, hookCtx, context, false);
			}
			target.Articles = ArticlesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StationNewsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationNewsComponent cast = (StationNewsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationNewsComponent cast = (StationNewsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StationNewsComponent def = (StationNewsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StationNewsComponent Instantiate()
	{
		return new StationNewsComponent();
	}
}
