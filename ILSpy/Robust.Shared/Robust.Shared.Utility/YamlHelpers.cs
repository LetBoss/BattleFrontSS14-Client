using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.Maths;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Utility;

public static class YamlHelpers
{
	private static readonly ThreadLocal<YamlScalarNode> FetchNode = new ThreadLocal<YamlScalarNode>(() => new YamlScalarNode());

	public static int AsInt(this YamlNode node)
	{
		return int.Parse(node.AsString(), CultureInfo.InvariantCulture);
	}

	public static string AsString(this YamlNode node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((YamlScalarNode)node).Value ?? "";
	}

	public static float AsFloat(this YamlNode node)
	{
		return float.Parse(node.AsString(), CultureInfo.InvariantCulture);
	}

	public static bool AsBool(this YamlNode node)
	{
		return bool.Parse(node.AsString());
	}

	public static Vector2 AsVector2(this YamlNode node)
	{
		string text = node.AsString();
		string[] array = text.Split(',');
		if (array.Length != 2)
		{
			throw new ArgumentException(string.Format("Could not parse {0}: '{1}'", "Vector2", text));
		}
		return new Vector2(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture));
	}

	public static Vector2i AsVector2i(this YamlNode node)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		string text = node.AsString();
		string[] array = text.Split(',');
		if (array.Length != 2)
		{
			throw new ArgumentException(string.Format("Could not parse {0}: '{1}'", "Vector2", text));
		}
		return new Vector2i(int.Parse(array[0], CultureInfo.InvariantCulture), int.Parse(array[1], CultureInfo.InvariantCulture));
	}

	public static Vector3 AsVector3(this YamlNode node)
	{
		string text = node.AsString();
		string[] array = text.Split(',');
		if (array.Length != 3)
		{
			throw new ArgumentException(string.Format("Could not parse {0}: '{1}'", "Vector3", text));
		}
		return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture));
	}

	public static Vector4 AsVector4(this YamlNode node)
	{
		string text = node.AsString();
		string[] array = text.Split(',');
		if (array.Length != 4)
		{
			throw new ArgumentException(string.Format("Could not parse {0}: '{1}'", "Vector4", text));
		}
		return new Vector4(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture), float.Parse(array[3], CultureInfo.InvariantCulture));
	}

	public static Matrix3x2 AsMatrix3x2(this YamlNode node)
	{
		string text = node.AsString();
		string[] array = text.Split(',');
		if (array.Length != 6)
		{
			throw new ArgumentException(string.Format("Could not parse {0}: '{1}'", "Matrix3x2", text));
		}
		float[] array2 = new float[6];
		for (int i = 0; i < 6; i++)
		{
			array2[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
		}
		return new Matrix3x2(array2[0], array2[1], array2[2], array2[3], array2[4], array2[5]);
	}

	public static Matrix4x4 AsMatrix4(this YamlNode node)
	{
		string text = node.AsString();
		string[] array = text.Split(',');
		if (array.Length != 16)
		{
			throw new ArgumentException(string.Format("Could not parse {0}: '{1}'", "Matrix4x4", text));
		}
		float m = Parse.Float(array[0].AsSpan());
		float m2 = Parse.Float(array[1].AsSpan());
		float m3 = Parse.Float(array[2].AsSpan());
		float m4 = Parse.Float(array[3].AsSpan());
		float m5 = Parse.Float(array[4].AsSpan());
		float m6 = Parse.Float(array[5].AsSpan());
		float m7 = Parse.Float(array[6].AsSpan());
		float m8 = Parse.Float(array[7].AsSpan());
		float m9 = Parse.Float(array[8].AsSpan());
		float m10 = Parse.Float(array[9].AsSpan());
		float m11 = Parse.Float(array[10].AsSpan());
		float m12 = Parse.Float(array[11].AsSpan());
		float m13 = Parse.Float(array[12].AsSpan());
		float m14 = Parse.Float(array[13].AsSpan());
		float m15 = Parse.Float(array[14].AsSpan());
		float m16 = Parse.Float(array[15].AsSpan());
		return new Matrix4x4(m, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11, m12, m13, m14, m15, m16);
	}

	public static T AsEnum<T>(this YamlNode node)
	{
		return (T)Enum.Parse(typeof(T), node.AsString(), ignoreCase: true);
	}

	public static Color AsHexColor(this YamlNode node, Color? fallback = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Color.FromHex(node.AsString().AsSpan(), fallback);
	}

	public static Color AsColor(this YamlNode node, Color? fallback = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Color result);
		if (Color.TryFromName(node.AsString(), ref result))
		{
			return result;
		}
		return node.AsHexColor(fallback);
	}

	public static ResPath AsResourcePath(this YamlNode node)
	{
		return new ResPath(((object)node).ToString());
	}

	public static T GetNode<T>(this YamlMappingNode mapping, string key) where T : YamlNode
	{
		return (T)(object)((YamlNode)mapping)[(YamlNode)(object)_getFetchNode(key)];
	}

	public static YamlNode GetNode(this YamlMappingNode mapping, string key)
	{
		return mapping.GetNode<YamlNode>(key);
	}

	public static bool TryGetNode<T>(this YamlMappingNode mapping, string key, [NotNullWhen(true)] out T? returnNode) where T : YamlNode
	{
		if (((IDictionary<YamlNode, YamlNode>)mapping.Children).TryGetValue((YamlNode)(object)_getFetchNode(key), out YamlNode value))
		{
			returnNode = (T)(object)value;
			return true;
		}
		returnNode = default(T);
		return false;
	}

	public static bool TryGetNode(this YamlMappingNode mapping, string key, [NotNullWhen(true)] out YamlNode? returnNode)
	{
		return ((IDictionary<YamlNode, YamlNode>)mapping.Children).TryGetValue((YamlNode)(object)_getFetchNode(key), out returnNode);
	}

	public static bool HasNode(this YamlMappingNode mapping, string key)
	{
		YamlNode returnNode;
		return mapping.TryGetNode(key, out returnNode);
	}

	public static Dictionary<string, YamlNode> YamlMappingToDict(YamlMappingNode mapping)
	{
		return ((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)mapping).ToDictionary((KeyValuePair<YamlNode, YamlNode> p) => p.Key.AsString(), (KeyValuePair<YamlNode, YamlNode> p) => p.Value);
	}

	private static YamlScalarNode _getFetchNode(string key)
	{
		YamlScalarNode value = FetchNode.Value;
		value.Value = key;
		return value;
	}
}
