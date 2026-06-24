using System;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.Parallax.Managers;
using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Client.Parallax.Data;

[DataDefinition]
public sealed class GeneratedParallaxTextureSource : IParallaxTextureSource, ISerializationGenerated<IParallaxTextureSource>, ISerializationGenerated, ISerializationGenerated<GeneratedParallaxTextureSource>
{
	[DataField("configPath", false, 1, false, false, null)]
	public ResPath ParallaxConfigPath { get; private set; } = new ResPath("/parallax_config.toml");

	[DataField("id", false, 1, false, false, null)]
	public string Identifier { get; private set; } = "other";

	async Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
	{
		return await IoCManager.Resolve<GeneratedParallaxCache>().Load(Identifier, ParallaxConfigPath, cancel);
	}

	void IParallaxTextureSource.Unload(IDependencyCollection dependencies)
	{
		dependencies.Resolve<GeneratedParallaxCache>().Unload(Identifier);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GeneratedParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<GeneratedParallaxTextureSource>(this, ref target, hookCtx, false, context))
		{
			ResPath parallaxConfigPath = default(ResPath);
			if (!serialization.TryCustomCopy<ResPath>(ParallaxConfigPath, ref parallaxConfigPath, hookCtx, false, context))
			{
				parallaxConfigPath = serialization.CreateCopy<ResPath>(ParallaxConfigPath, hookCtx, context, false);
			}
			target.ParallaxConfigPath = parallaxConfigPath;
			string identifier = null;
			if (Identifier == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Identifier, ref identifier, hookCtx, false, context))
			{
				identifier = Identifier;
			}
			target.Identifier = identifier;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GeneratedParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratedParallaxTextureSource target2 = (GeneratedParallaxTextureSource)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratedParallaxTextureSource target2 = (GeneratedParallaxTextureSource)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public GeneratedParallaxTextureSource Instantiate()
	{
		return new GeneratedParallaxTextureSource();
	}

	IParallaxTextureSource IParallaxTextureSource.Instantiate()
	{
		return Instantiate();
	}

	IParallaxTextureSource ISerializationGenerated<IParallaxTextureSource>.Instantiate()
	{
		return Instantiate();
	}
}
