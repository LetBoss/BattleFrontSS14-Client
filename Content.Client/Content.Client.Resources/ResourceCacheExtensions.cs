using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Utility;

namespace Content.Client.Resources;

public static class ResourceCacheExtensions
{
	public static Texture GetTexture(this IResourceCache cache, ResPath path)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TextureResource.op_Implicit(cache.GetResource<TextureResource>(path, true));
	}

	public static Texture GetTexture(this IResourceCache cache, string path)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return cache.GetTexture(new ResPath(path));
	}

	public static Font GetFont(this IResourceCache cache, ResPath path, int size)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		return (Font)new VectorFont(cache.GetResource<FontResource>(path, true), size);
	}

	public static Font GetFont(this IResourceCache cache, string path, int size)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return cache.GetFont(new ResPath(path), size);
	}

	public static Font GetFont(this IResourceCache cache, ResPath[] path, int size)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		Font[] array = (Font[])(object)new Font[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = (Font)new VectorFont(cache.GetResource<FontResource>(path[i], true), size);
		}
		return (Font)new StackedFont(array);
	}

	public static Font GetFont(this IResourceCache cache, string[] path, int size)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ResPath[] array = (ResPath[])(object)new ResPath[path.Length];
		for (int i = 0; i < path.Length; i++)
		{
			array[i] = new ResPath(path[i]);
		}
		return cache.GetFont(array, size);
	}
}
