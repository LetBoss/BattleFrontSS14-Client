using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Pinpointer.UI;
using Content.Shared.Atmos.Components;
using Robust.Client.Graphics;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Consoles;

public sealed class AtmosMonitoringConsoleNavMapControl : NavMapControl
{
	[Dependency]
	private IEntityManager _entManager;

	public bool ShowPipeNetwork = true;

	public int? FocusNetId;

	private const int ChunkSize = 4;

	private const float ScaleModifier = 4f;

	private readonly float[] _layerFraction = new float[3] { 0.5f, 0.75f, 0.25f };

	private const float LineThickness = 0.05f;

	private readonly Color _basePipeNetColor = Color.LightGray;

	private readonly Color _unfocusedPipeNetColor = Color.DimGray;

	private List<AtmosMonitoringConsoleLine> _atmosPipeNetwork = new List<AtmosMonitoringConsoleLine>();

	private Dictionary<Color, Color> _sRGBLookUp = new Dictionary<Color, Color>();

	private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _horizLines = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();

	private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _horizLinesReversed = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();

	private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _vertLines = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();

	private Dictionary<Color, Dictionary<Vector2i, Vector2i>> _vertLinesReversed = new Dictionary<Color, Dictionary<Vector2i, Vector2i>>();

	public AtmosMonitoringConsoleNavMapControl()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.PostWallDrawingAction += DrawAllPipeNetworks;
	}

	protected override void UpdateNavMap()
	{
		base.UpdateNavMap();
		AtmosMonitoringConsoleComponent atmosMonitoringConsoleComponent = default(AtmosMonitoringConsoleComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (_entManager.TryGetComponent<AtmosMonitoringConsoleComponent>(Owner, ref atmosMonitoringConsoleComponent) && _entManager.TryGetComponent<MapGridComponent>(MapUid, ref grid))
		{
			_atmosPipeNetwork = GetDecodedAtmosPipeChunks(atmosMonitoringConsoleComponent.AtmosPipeChunks, grid);
		}
	}

	private void DrawAllPipeNetworks(DrawingHandleScreen handle)
	{
		if (ShowPipeNetwork && _atmosPipeNetwork != null && _atmosPipeNetwork.Any())
		{
			DrawPipeNetwork(handle, _atmosPipeNetwork);
		}
	}

	private void DrawPipeNetwork(DrawingHandleScreen handle, List<AtmosMonitoringConsoleLine> atmosPipeNetwork)
	{
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		Vector2 offset = GetOffset();
		Vector2 vector = offset;
		vector.Y = 0f - offset.Y;
		offset = vector;
		Color key;
		ValueList<Vector2> value2;
		if (WorldRange / WorldMaxRange > 0.5f)
		{
			Dictionary<Color, ValueList<Vector2>> dictionary = new Dictionary<Color, ValueList<Vector2>>();
			foreach (AtmosMonitoringConsoleLine item in atmosPipeNetwork)
			{
				Vector2 vector2 = ScalePosition(item.Origin - offset);
				Vector2 vector3 = ScalePosition(item.Terminus - offset);
				if (!dictionary.TryGetValue(item.Color, out var value))
				{
					value = default(ValueList<Vector2>);
				}
				value.Add(vector2);
				value.Add(vector3);
				dictionary[item.Color] = value;
			}
			{
				foreach (KeyValuePair<Color, ValueList<Vector2>> item2 in dictionary)
				{
					item2.Deconstruct(out key, out value2);
					Color val = key;
					ValueList<Vector2> val2 = value2;
					if (val2.Count > 0)
					{
						((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)4, (ReadOnlySpan<Vector2>)val2.Span, val);
					}
				}
				return;
			}
		}
		Dictionary<Color, ValueList<Vector2>> dictionary2 = new Dictionary<Color, ValueList<Vector2>>();
		foreach (AtmosMonitoringConsoleLine item3 in atmosPipeNetwork)
		{
			Vector2 vector4 = ScalePosition(new Vector2(Math.Min(item3.Origin.X, item3.Terminus.X) - 0.05f, Math.Min(item3.Origin.Y, item3.Terminus.Y) - 0.05f) - offset);
			Vector2 vector5 = ScalePosition(new Vector2(Math.Max(item3.Origin.X, item3.Terminus.X) + 0.05f, Math.Min(item3.Origin.Y, item3.Terminus.Y) - 0.05f) - offset);
			Vector2 vector6 = ScalePosition(new Vector2(Math.Min(item3.Origin.X, item3.Terminus.X) - 0.05f, Math.Max(item3.Origin.Y, item3.Terminus.Y) + 0.05f) - offset);
			Vector2 vector7 = ScalePosition(new Vector2(Math.Max(item3.Origin.X, item3.Terminus.X) + 0.05f, Math.Max(item3.Origin.Y, item3.Terminus.Y) + 0.05f) - offset);
			if (!dictionary2.TryGetValue(item3.Color, out var value3))
			{
				value3 = default(ValueList<Vector2>);
			}
			value3.Add(vector6);
			value3.Add(vector4);
			value3.Add(vector7);
			value3.Add(vector4);
			value3.Add(vector7);
			value3.Add(vector5);
			dictionary2[item3.Color] = value3;
		}
		foreach (KeyValuePair<Color, ValueList<Vector2>> item4 in dictionary2)
		{
			item4.Deconstruct(out key, out value2);
			Color val3 = key;
			ValueList<Vector2> val4 = value2;
			if (val4.Count > 0)
			{
				((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)val4.Span, val3);
			}
		}
	}

	private List<AtmosMonitoringConsoleLine> GetDecodedAtmosPipeChunks(Dictionary<Vector2i, AtmosPipeChunk>? chunks, MapGridComponent? grid)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		List<AtmosMonitoringConsoleLine> list = new List<AtmosMonitoringConsoleLine>();
		if (chunks == null || grid == null)
		{
			return list;
		}
		_horizLines.Clear();
		_horizLinesReversed.Clear();
		_vertLines.Clear();
		_vertLinesReversed.Clear();
		ulong num = 1uL;
		ulong num2 = 2uL;
		ulong num3 = 4uL;
		ulong num4 = 8uL;
		Vector2i key;
		Color key3;
		foreach (KeyValuePair<Vector2i, AtmosPipeChunk> chunk in chunks)
		{
			chunk.Deconstruct(out key, out var value);
			AtmosPipeChunk atmosPipeChunk = value;
			new List<AtmosMonitoringConsoleLine>();
			foreach (KeyValuePair<AtmosMonitoringConsoleSubnet, ulong> atmosPipeDatum in atmosPipeChunk.AtmosPipeData)
			{
				atmosPipeDatum.Deconstruct(out var key2, out var value2);
				key2.Deconstruct(out int NetId, out AtmosPipeLayer PipeLayer, out string HexCode);
				int num5 = NetId;
				AtmosPipeLayer atmosPipeLayer = PipeLayer;
				string text = HexCode;
				ulong num6 = value2;
				key3 = Color.FromHex((ReadOnlySpan<char>)text, (Color?)null);
				Color key4 = (ref key3) * (ref _basePipeNetColor);
				if (FocusNetId.HasValue)
				{
					int? focusNetId = FocusNetId;
					NetId = num5;
					if (focusNetId != NetId)
					{
						key4 = (ref key4) * (ref _unfocusedPipeNetColor);
					}
				}
				if (!_horizLines.TryGetValue(key4, out Dictionary<Vector2i, Vector2i> value3))
				{
					value3 = new Dictionary<Vector2i, Vector2i>();
					_horizLines[key4] = value3;
				}
				if (!_horizLinesReversed.TryGetValue(key4, out Dictionary<Vector2i, Vector2i> value4))
				{
					value4 = new Dictionary<Vector2i, Vector2i>();
					_horizLinesReversed[key4] = value4;
				}
				if (!_vertLines.TryGetValue(key4, out Dictionary<Vector2i, Vector2i> value5))
				{
					value5 = new Dictionary<Vector2i, Vector2i>();
					_vertLines[key4] = value5;
				}
				if (!_vertLinesReversed.TryGetValue(key4, out Dictionary<Vector2i, Vector2i> value6))
				{
					value6 = new Dictionary<Vector2i, Vector2i>();
					_vertLinesReversed[key4] = value6;
				}
				float num7 = _layerFraction[(int)atmosPipeLayer];
				Vector2 vector = new Vector2((float)(int)grid.TileSize * num7, (float)(-grid.TileSize) * num7);
				for (int i = 0; i < 16; i++)
				{
					if (num6 != 0L)
					{
						ulong num8 = (ulong)(15L << i * 4);
						if ((num6 & num8) != 0L)
						{
							Vector2i tileFromIndex = GetTileFromIndex(i);
							Vector2i val = (atmosPipeChunk.Origin * 4 + tileFromIndex) * (int)grid.TileSize;
							key = val;
							key.Y = -val.Y;
							val = key;
							Vector2 vector2 = (((num6 & (num << i * 4)) != 0) ? new Vector2((float)(int)grid.TileSize * num7, (float)(-grid.TileSize) * 1f) : vector);
							Vector2 vector3 = (((num6 & (num2 << i * 4)) != 0) ? new Vector2((float)(int)grid.TileSize * num7, (float)(-grid.TileSize) * 0f) : vector);
							Vector2 vector4 = (((num6 & (num4 << i * 4)) != 0) ? new Vector2((float)(int)grid.TileSize * 1f, (float)(-grid.TileSize) * num7) : vector);
							Vector2 vector5 = (((num6 & (num3 << i * 4)) != 0) ? new Vector2((float)(int)grid.TileSize * 0f, (float)(-grid.TileSize) * num7) : vector);
							AddOrUpdateNavMapLine(ConvertVector2ToVector2i(Vector2i.op_Implicit(val) + vector4, 4f), ConvertVector2ToVector2i(Vector2i.op_Implicit(val) + vector5, 4f), value3, value4);
							AddOrUpdateNavMapLine(ConvertVector2ToVector2i(Vector2i.op_Implicit(val) + vector2, 4f), ConvertVector2ToVector2i(Vector2i.op_Implicit(val) + vector3, 4f), value5, value6);
						}
					}
				}
			}
		}
		Dictionary<Vector2i, Vector2i> value7;
		Vector2i key5;
		foreach (KeyValuePair<Color, Dictionary<Vector2i, Vector2i>> horizLine in _horizLines)
		{
			horizLine.Deconstruct(out key3, out value7);
			Color color = key3;
			Dictionary<Vector2i, Vector2i> dictionary = value7;
			Color color2 = GetsRGBColor(color);
			foreach (KeyValuePair<Vector2i, Vector2i> item in dictionary)
			{
				item.Deconstruct(out key, out key5);
				Vector2i vector6 = key;
				Vector2i vector7 = key5;
				list.Add(new AtmosMonitoringConsoleLine(ConvertVector2iToVector2(vector6, 0.25f), ConvertVector2iToVector2(vector7, 0.25f), color2));
			}
		}
		foreach (KeyValuePair<Color, Dictionary<Vector2i, Vector2i>> vertLine in _vertLines)
		{
			vertLine.Deconstruct(out key3, out value7);
			Color color3 = key3;
			Dictionary<Vector2i, Vector2i> dictionary2 = value7;
			Color color4 = GetsRGBColor(color3);
			foreach (KeyValuePair<Vector2i, Vector2i> item2 in dictionary2)
			{
				item2.Deconstruct(out key5, out key);
				Vector2i vector8 = key5;
				Vector2i vector9 = key;
				list.Add(new AtmosMonitoringConsoleLine(ConvertVector2iToVector2(vector8, 0.25f), ConvertVector2iToVector2(vector9, 0.25f), color4));
			}
		}
		return list;
	}

	private Vector2 ConvertVector2iToVector2(Vector2i vector, float scale = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)vector.X * scale, (float)vector.Y * scale);
	}

	private Vector2i ConvertVector2ToVector2i(Vector2 vector, float scale = 1f)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)MathF.Round(vector.X * scale), (int)MathF.Round(vector.Y * scale));
	}

	private Vector2i GetTileFromIndex(int index)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int num = index / 4;
		int num2 = index % 4;
		return new Vector2i(num, num2);
	}

	private Color GetsRGBColor(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!_sRGBLookUp.TryGetValue(color, out var value))
		{
			value = Color.ToSrgb(color);
			_sRGBLookUp[color] = value;
		}
		return value;
	}
}
