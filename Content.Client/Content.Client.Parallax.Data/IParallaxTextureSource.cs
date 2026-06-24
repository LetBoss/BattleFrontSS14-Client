using System;
using System.Threading;
using System.Threading.Tasks;
using Robust.Client.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Parallax.Data;

[ImplicitDataDefinitionForInheritors]
public interface IParallaxTextureSource : ISerializationGenerated<IParallaxTextureSource>, ISerializationGenerated
{
	Task<Texture> GenerateTexture(CancellationToken cancel = default(CancellationToken));

	void Unload(IDependencyCollection dependencies)
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IParallaxTextureSource>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IParallaxTextureSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IParallaxTextureSource target2 = (IParallaxTextureSource)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IParallaxTextureSource Instantiate()
	{
		throw new NotImplementedException();
	}
}
