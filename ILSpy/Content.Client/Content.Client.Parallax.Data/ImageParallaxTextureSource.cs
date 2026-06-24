using System;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.IoC;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Parallax.Data;

[DataDefinition]
public sealed class ImageParallaxTextureSource : IParallaxTextureSource, ISerializationGenerated<IParallaxTextureSource>, ISerializationGenerated, ISerializationGenerated<ImageParallaxTextureSource>
{
	[DataField("path", false, 1, true, false, null)]
	public ResPath Path { get; private set; }

	Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Task.FromResult<Texture>(StaticIoC.ResC.GetTexture(Path));
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ImageParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ImageParallaxTextureSource>(this, ref target, hookCtx, false, context))
		{
			ResPath path = default(ResPath);
			if (!serialization.TryCustomCopy<ResPath>(Path, ref path, hookCtx, false, context))
			{
				path = serialization.CreateCopy<ResPath>(Path, hookCtx, context, false);
			}
			target.Path = path;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ImageParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ImageParallaxTextureSource target2 = (ImageParallaxTextureSource)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ImageParallaxTextureSource target2 = (ImageParallaxTextureSource)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ImageParallaxTextureSource Instantiate()
	{
		return new ImageParallaxTextureSource();
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
