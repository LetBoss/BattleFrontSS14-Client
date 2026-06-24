namespace Robust.Shared.Map;

public struct GridCreateOptions
{
	public static readonly GridCreateOptions Default = new GridCreateOptions
	{
		ChunkSize = 16
	};

	public ushort ChunkSize;
}
