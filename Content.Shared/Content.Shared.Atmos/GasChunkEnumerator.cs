using Content.Shared.Atmos.EntitySystems;

namespace Content.Shared.Atmos;

public struct GasChunkEnumerator(GasOverlayChunk chunk)
{
	private readonly SharedGasTileOverlaySystem.GasOverlayData[] _tileData = chunk.TileData;

	private int _index = -1;

	public int X = 7;

	public int Y = -1;

	public bool MoveNext(out SharedGasTileOverlaySystem.GasOverlayData gas)
	{
		while (++_index < _tileData.Length)
		{
			X++;
			if (X >= 8)
			{
				X = 0;
				Y++;
			}
			gas = _tileData[_index];
			if (!gas.Equals(default(SharedGasTileOverlaySystem.GasOverlayData)))
			{
				return true;
			}
		}
		gas = default(SharedGasTileOverlaySystem.GasOverlayData);
		return false;
	}
}
