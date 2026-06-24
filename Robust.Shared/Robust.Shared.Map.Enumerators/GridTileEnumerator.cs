using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Robust.Shared.Map.Enumerators;

public struct GridTileEnumerator
{
	private readonly EntityUid _gridUid;

	private Dictionary<Vector2i, MapChunk>.Enumerator _chunkEnumerator;

	private readonly ushort _chunkSize;

	private int _index;

	private readonly bool _ignoreEmpty;

	internal GridTileEnumerator(EntityUid gridUid, Dictionary<Vector2i, MapChunk>.Enumerator chunkEnumerator, ushort chunkSize, bool ignoreEmpty)
	{
		_gridUid = gridUid;
		_chunkEnumerator = chunkEnumerator;
		_chunkSize = chunkSize;
		_index = _chunkSize * _chunkSize;
		_ignoreEmpty = ignoreEmpty;
	}

	public bool MoveNext([NotNullWhen(true)] out TileRef? tileRef)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val;
		ushort num;
		ushort num2;
		Tile tile;
		do
		{
			if (_index == _chunkSize * _chunkSize)
			{
				if (!_chunkEnumerator.MoveNext())
				{
					tileRef = null;
					return false;
				}
				_index = 0;
			}
			_chunkEnumerator.Current.Deconstruct(out Vector2i key, out MapChunk value);
			val = key;
			MapChunk mapChunk = value;
			num = (ushort)(_index / _chunkSize);
			num2 = (ushort)(_index % _chunkSize);
			tile = mapChunk.GetTile(num, num2);
			_index++;
		}
		while (_ignoreEmpty && tile.IsEmpty);
		int xIndex = num + val.X * _chunkSize;
		int yIndex = num2 + val.Y * _chunkSize;
		tileRef = new TileRef(_gridUid, xIndex, yIndex, tile);
		return true;
	}
}
