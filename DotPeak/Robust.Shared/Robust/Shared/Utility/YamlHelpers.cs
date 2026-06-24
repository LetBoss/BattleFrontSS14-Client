// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.YamlHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Utility;

public static class YamlHelpers
{
  private static readonly ThreadLocal<YamlScalarNode> FetchNode = new ThreadLocal<YamlScalarNode>((Func<YamlScalarNode>) (() => new YamlScalarNode()));

  public static int AsInt(this YamlNode node)
  {
    return int.Parse(node.AsString(), (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static string AsString(this YamlNode node) => ((YamlScalarNode) node).Value ?? "";

  public static float AsFloat(this YamlNode node)
  {
    return float.Parse(node.AsString(), (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static bool AsBool(this YamlNode node) => bool.Parse(node.AsString());

  public static Vector2 AsVector2(this YamlNode node)
  {
    string str = node.AsString();
    string[] strArray = str.Split(',');
    return strArray.Length == 2 ? new Vector2(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture)) : throw new ArgumentException($"Could not parse {"Vector2"}: '{str}'");
  }

  public static Vector2i AsVector2i(this YamlNode node)
  {
    string str = node.AsString();
    string[] strArray = str.Split(',');
    return strArray.Length == 2 ? new Vector2i(int.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), int.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture)) : throw new ArgumentException($"Could not parse {"Vector2"}: '{str}'");
  }

  public static Vector3 AsVector3(this YamlNode node)
  {
    string str = node.AsString();
    string[] strArray = str.Split(',');
    return strArray.Length == 3 ? new Vector3(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture)) : throw new ArgumentException($"Could not parse {"Vector3"}: '{str}'");
  }

  public static Vector4 AsVector4(this YamlNode node)
  {
    string str = node.AsString();
    string[] strArray = str.Split(',');
    return strArray.Length == 4 ? new Vector4(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture)) : throw new ArgumentException($"Could not parse {"Vector4"}: '{str}'");
  }

  public static Matrix3x2 AsMatrix3x2(this YamlNode node)
  {
    string str = node.AsString();
    string[] strArray = str.Split(',');
    if (strArray.Length != 6)
      throw new ArgumentException($"Could not parse {"Matrix3x2"}: '{str}'");
    float[] numArray = new float[6];
    for (int index = 0; index < 6; ++index)
      numArray[index] = float.Parse(strArray[index], (IFormatProvider) CultureInfo.InvariantCulture);
    return new Matrix3x2(numArray[0], numArray[1], numArray[2], numArray[3], numArray[4], numArray[5]);
  }

  public static Matrix4x4 AsMatrix4(this YamlNode node)
  {
    string str = node.AsString();
    string[] strArray = str.Split(',');
    return strArray.Length == 16 /*0x10*/ ? new Matrix4x4(Parse.Float(strArray[0].AsSpan()), Parse.Float(strArray[1].AsSpan()), Parse.Float(strArray[2].AsSpan()), Parse.Float(strArray[3].AsSpan()), Parse.Float(strArray[4].AsSpan()), Parse.Float(strArray[5].AsSpan()), Parse.Float(strArray[6].AsSpan()), Parse.Float(strArray[7].AsSpan()), Parse.Float(strArray[8].AsSpan()), Parse.Float(strArray[9].AsSpan()), Parse.Float(strArray[10].AsSpan()), Parse.Float(strArray[11].AsSpan()), Parse.Float(strArray[12].AsSpan()), Parse.Float(strArray[13].AsSpan()), Parse.Float(strArray[14].AsSpan()), Parse.Float(strArray[15].AsSpan())) : throw new ArgumentException($"Could not parse {"Matrix4x4"}: '{str}'");
  }

  public static T AsEnum<T>(this YamlNode node)
  {
    return (T) Enum.Parse(typeof (T), node.AsString(), true);
  }

  public static Color AsHexColor(this YamlNode node, Color? fallback = null)
  {
    return Color.FromHex(node.AsString().AsSpan(), fallback);
  }

  public static Color AsColor(this YamlNode node, Color? fallback = null)
  {
    Color color;
    return Color.TryFromName(node.AsString(), ref color) ? color : node.AsHexColor(fallback);
  }

  public static ResPath AsResourcePath(this YamlNode node) => new ResPath(node.ToString());

  public static T GetNode<T>(this YamlMappingNode mapping, string key) where T : YamlNode
  {
    return (T) ((YamlNode) mapping)[(YamlNode) YamlHelpers._getFetchNode(key)];
  }

  public static YamlNode GetNode(this YamlMappingNode mapping, string key)
  {
    return mapping.GetNode<YamlNode>(key);
  }

  public static bool TryGetNode<T>(this YamlMappingNode mapping, string key, [NotNullWhen(true)] out T? returnNode) where T : YamlNode
  {
    YamlNode yamlNode;
    if (((IDictionary<YamlNode, YamlNode>) mapping.Children).TryGetValue((YamlNode) YamlHelpers._getFetchNode(key), out yamlNode))
    {
      returnNode = (T) yamlNode;
      return true;
    }
    returnNode = default (T);
    return false;
  }

  public static bool TryGetNode(this YamlMappingNode mapping, string key, [NotNullWhen(true)] out YamlNode? returnNode)
  {
    return ((IDictionary<YamlNode, YamlNode>) mapping.Children).TryGetValue((YamlNode) YamlHelpers._getFetchNode(key), out returnNode);
  }

  public static bool HasNode(this YamlMappingNode mapping, string key)
  {
    return mapping.TryGetNode(key, out YamlNode _);
  }

  public static Dictionary<string, YamlNode> YamlMappingToDict(YamlMappingNode mapping)
  {
    return ((IEnumerable<KeyValuePair<YamlNode, YamlNode>>) mapping).ToDictionary<KeyValuePair<YamlNode, YamlNode>, string, YamlNode>((Func<KeyValuePair<YamlNode, YamlNode>, string>) (p => p.Key.AsString()), (Func<KeyValuePair<YamlNode, YamlNode>, YamlNode>) (p => p.Value));
  }

  private static YamlScalarNode _getFetchNode(string key)
  {
    YamlScalarNode fetchNode = YamlHelpers.FetchNode.Value;
    fetchNode.Value = key;
    return fetchNode;
  }
}
