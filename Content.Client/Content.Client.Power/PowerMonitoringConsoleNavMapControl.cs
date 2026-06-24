using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Pinpointer.UI;
using Content.Shared.Power;
using Robust.Client.Graphics;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Power;

public sealed class PowerMonitoringConsoleNavMapControl : NavMapControl
{
	[Dependency]
	private IEntityManager _entManager;

	private readonly Color[] _powerCableColors = (Color[])(object)new Color[3]
	{
		Color.OrangeRed,
		Color.Yellow,
		Color.LimeGreen
	};

	private readonly Vector2[] _powerCableOffsets = new Vector2[3]
	{
		new Vector2(-0.2f, -0.2f),
		Vector2.Zero,
		new Vector2(0.2f, 0.2f)
	};

	private Dictionary<Color, Color> _sRGBLookUp = new Dictionary<Color, Color>();

	public PowerMonitoringCableNetworksComponent? PowerMonitoringCableNetworks;

	public List<PowerMonitoringConsoleLineGroup> HiddenLineGroups = new List<PowerMonitoringConsoleLineGroup>();

	public List<PowerMonitoringConsoleLine> PowerCableNetwork = new List<PowerMonitoringConsoleLine>();

	public List<PowerMonitoringConsoleLine> FocusCableNetwork = new List<PowerMonitoringConsoleLine>();

	private Dictionary<Vector2i, Vector2i>[] _horizLines = new Dictionary<Vector2i, Vector2i>[3]
	{
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>()
	};

	private Dictionary<Vector2i, Vector2i>[] _horizLinesReversed = new Dictionary<Vector2i, Vector2i>[3]
	{
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>()
	};

	private Dictionary<Vector2i, Vector2i>[] _vertLines = new Dictionary<Vector2i, Vector2i>[3]
	{
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>()
	};

	private Dictionary<Vector2i, Vector2i>[] _vertLinesReversed = new Dictionary<Vector2i, Vector2i>[3]
	{
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>(),
		new Dictionary<Vector2i, Vector2i>()
	};

	private MapGridComponent? _grid;

	public PowerMonitoringConsoleNavMapControl()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		TileColor = new Color((byte)30, (byte)57, (byte)67, byte.MaxValue);
		WallColor = new Color((byte)102, (byte)164, (byte)217, byte.MaxValue);
		BackgroundColor = Color.FromSrgb(((Color)(ref TileColor)).WithAlpha(BackgroundOpacity));
		base.PostWallDrawingAction += DrawAllCableNetworks;
	}

	protected override void UpdateNavMap()
	{
		base.UpdateNavMap();
		PowerMonitoringCableNetworksComponent powerMonitoringCableNetworksComponent = default(PowerMonitoringCableNetworksComponent);
		if (Owner.HasValue && _entManager.TryGetComponent<PowerMonitoringCableNetworksComponent>(Owner, ref powerMonitoringCableNetworksComponent))
		{
			PowerCableNetwork = GetDecodedPowerCableChunks(powerMonitoringCableNetworksComponent.AllChunks);
			FocusCableNetwork = GetDecodedPowerCableChunks(powerMonitoringCableNetworksComponent.FocusChunks);
		}
	}

	public void DrawAllCableNetworks(DrawingHandleScreen handle)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (_entManager.TryGetComponent<MapGridComponent>(MapUid, ref _grid))
		{
			if (PowerCableNetwork != null && PowerCableNetwork.Count > 0)
			{
				Color modulator = ((FocusCableNetwork != null && FocusCableNetwork.Count > 0) ? Color.DimGray : Color.White);
				DrawCableNetwork(handle, PowerCableNetwork, modulator);
			}
			if (FocusCableNetwork != null && FocusCableNetwork.Count > 0)
			{
				DrawCableNetwork(handle, FocusCableNetwork, Color.White);
			}
		}
	}

	public void DrawCableNetwork(DrawingHandleScreen handle, List<PowerMonitoringConsoleLine> fullCableNetwork, Color modulator)
	{
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (!_entManager.TryGetComponent<MapGridComponent>(MapUid, ref _grid))
		{
			return;
		}
		Vector2 offset = GetOffset();
		Vector2 vector = offset;
		vector.Y = 0f - offset.Y;
		offset = vector;
		if (WorldRange / WorldMaxRange > 0.5f)
		{
			ValueList<Vector2>[] array = new ValueList<Vector2>[3];
			foreach (PowerMonitoringConsoleLine item in fullCableNetwork)
			{
				if (!HiddenLineGroups.Contains(item.Group))
				{
					Vector2 vector2 = _powerCableOffsets[(uint)item.Group];
					Vector2 vector3 = ScalePosition(item.Origin + vector2 - offset);
					Vector2 vector4 = ScalePosition(item.Terminus + vector2 - offset);
					array[(uint)item.Group].Add(vector3);
					array[(uint)item.Group].Add(vector4);
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				ValueList<Vector2> val = array[i];
				if (val.Count > 0)
				{
					Color val2 = (ref _powerCableColors[i]) * (ref modulator);
					if (!_sRGBLookUp.TryGetValue(val2, out var value))
					{
						value = Color.ToSrgb(val2);
						_sRGBLookUp[val2] = value;
					}
					((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)4, (ReadOnlySpan<Vector2>)val.Span, value);
				}
			}
			return;
		}
		ValueList<Vector2>[] array2 = new ValueList<Vector2>[3];
		foreach (PowerMonitoringConsoleLine item2 in fullCableNetwork)
		{
			if (!HiddenLineGroups.Contains(item2.Group))
			{
				Vector2 vector5 = _powerCableOffsets[(uint)item2.Group];
				Vector2 vector6 = ScalePosition(new Vector2(Math.Min(item2.Origin.X, item2.Terminus.X) - 0.1f, Math.Min(item2.Origin.Y, item2.Terminus.Y) - 0.1f) + vector5 - offset);
				Vector2 vector7 = ScalePosition(new Vector2(Math.Max(item2.Origin.X, item2.Terminus.X) + 0.1f, Math.Min(item2.Origin.Y, item2.Terminus.Y) - 0.1f) + vector5 - offset);
				Vector2 vector8 = ScalePosition(new Vector2(Math.Min(item2.Origin.X, item2.Terminus.X) - 0.1f, Math.Max(item2.Origin.Y, item2.Terminus.Y) + 0.1f) + vector5 - offset);
				Vector2 vector9 = ScalePosition(new Vector2(Math.Max(item2.Origin.X, item2.Terminus.X) + 0.1f, Math.Max(item2.Origin.Y, item2.Terminus.Y) + 0.1f) + vector5 - offset);
				array2[(uint)item2.Group].Add(vector8);
				array2[(uint)item2.Group].Add(vector6);
				array2[(uint)item2.Group].Add(vector9);
				array2[(uint)item2.Group].Add(vector6);
				array2[(uint)item2.Group].Add(vector9);
				array2[(uint)item2.Group].Add(vector7);
			}
		}
		for (int j = 0; j < array2.Length; j++)
		{
			ValueList<Vector2> val3 = array2[j];
			if (val3.Count > 0)
			{
				Color val4 = (ref _powerCableColors[j]) * (ref modulator);
				if (!_sRGBLookUp.TryGetValue(val4, out var value2))
				{
					value2 = Color.ToSrgb(val4);
					_sRGBLookUp[val4] = value2;
				}
				((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)val3.Span, value2);
			}
		}
	}

	public List<PowerMonitoringConsoleLine> GetDecodedPowerCableChunks(Dictionary<Vector2i, PowerCableChunk>? chunks)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		List<PowerMonitoringConsoleLine> list = new List<PowerMonitoringConsoleLine>();
		if (!_entManager.TryGetComponent<MapGridComponent>(MapUid, ref _grid))
		{
			return list;
		}
		if (chunks == null)
		{
			return list;
		}
		Array.ForEach(_horizLines, delegate(Dictionary<Vector2i, Vector2i> x)
		{
			x.Clear();
		});
		Array.ForEach(_horizLinesReversed, delegate(Dictionary<Vector2i, Vector2i> x)
		{
			x.Clear();
		});
		Array.ForEach(_vertLines, delegate(Dictionary<Vector2i, Vector2i> x)
		{
			x.Clear();
		});
		Array.ForEach(_vertLinesReversed, delegate(Dictionary<Vector2i, Vector2i> x)
		{
			x.Clear();
		});
		Vector2i value;
		foreach (KeyValuePair<Vector2i, PowerCableChunk> chunk in chunks)
		{
			chunk.Deconstruct(out value, out var value2);
			Vector2i val = value;
			PowerCableChunk powerCableChunk = value2;
			for (int num = 0; num < 3; num++)
			{
				Dictionary<Vector2i, Vector2i> lookup = _horizLines[num];
				Dictionary<Vector2i, Vector2i> lookupReversed = _horizLinesReversed[num];
				Dictionary<Vector2i, Vector2i> lookup2 = _vertLines[num];
				Dictionary<Vector2i, Vector2i> lookupReversed2 = _vertLinesReversed[num];
				int num2 = powerCableChunk.PowerCableData[num];
				for (int num3 = 0; num3 < 25; num3++)
				{
					int num4 = 1 << num3;
					if ((num2 & num4) != 0)
					{
						Vector2i tileFromIndex = SharedPowerMonitoringConsoleSystem.GetTileFromIndex(num3);
						Vector2i val2 = (powerCableChunk.Origin * 5 + tileFromIndex) * (int)_grid.TileSize;
						value = val2;
						value.Y = -val2.Y;
						val2 = value;
						PowerCableChunk value3;
						bool flag;
						if (tileFromIndex.X == 4)
						{
							flag = chunks.TryGetValue(val + new Vector2i(1, 0), out value3) && (value3.PowerCableData[num] & SharedPowerMonitoringConsoleSystem.GetFlag(new Vector2i(0, tileFromIndex.Y))) != 0;
						}
						else
						{
							int flag2 = SharedPowerMonitoringConsoleSystem.GetFlag(tileFromIndex + new Vector2i(1, 0));
							flag = (num2 & flag2) != 0;
						}
						if (flag)
						{
							AddOrUpdateNavMapLine(val2, val2 + new Vector2i((int)_grid.TileSize, 0), lookup, lookupReversed);
						}
						if (tileFromIndex.Y == 4)
						{
							flag = chunks.TryGetValue(val + new Vector2i(0, 1), out value3) && (value3.PowerCableData[num] & SharedPowerMonitoringConsoleSystem.GetFlag(new Vector2i(tileFromIndex.X, 0))) != 0;
						}
						else
						{
							int flag3 = SharedPowerMonitoringConsoleSystem.GetFlag(tileFromIndex + new Vector2i(0, 1));
							flag = (num2 & flag3) != 0;
						}
						if (flag)
						{
							AddOrUpdateNavMapLine(val2 + new Vector2i(0, -_grid.TileSize), val2, lookup2, lookupReversed2);
						}
					}
				}
			}
		}
		Vector2 vector = new Vector2((float)(int)_grid.TileSize * 0.5f, (float)(-_grid.TileSize) * 0.5f);
		Vector2i value4;
		for (int num5 = 0; num5 < _horizLines.Length; num5++)
		{
			foreach (KeyValuePair<Vector2i, Vector2i> item in _horizLines[num5])
			{
				item.Deconstruct(out value, out value4);
				Vector2i val3 = value;
				Vector2i val4 = value4;
				list.Add(new PowerMonitoringConsoleLine(Vector2i.op_Implicit(val3) + vector, Vector2i.op_Implicit(val4) + vector, (PowerMonitoringConsoleLineGroup)num5));
			}
		}
		for (int num6 = 0; num6 < _vertLines.Length; num6++)
		{
			foreach (KeyValuePair<Vector2i, Vector2i> item2 in _vertLines[num6])
			{
				item2.Deconstruct(out value4, out value);
				Vector2i val5 = value4;
				Vector2i val6 = value;
				list.Add(new PowerMonitoringConsoleLine(Vector2i.op_Implicit(val5) + vector, Vector2i.op_Implicit(val6) + vector, (PowerMonitoringConsoleLineGroup)num6));
			}
		}
		return list;
	}
}
