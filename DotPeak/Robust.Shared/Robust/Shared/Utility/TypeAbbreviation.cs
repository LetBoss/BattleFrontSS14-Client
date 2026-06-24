// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.TypeAbbreviation
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Utility;

public static class TypeAbbreviation
{
  private static readonly TypeAbbreviation.Abbreviation[] _abbreviations;

  static TypeAbbreviation()
  {
    using (Stream manifestResourceStream = typeof (TypeAbbreviation).Assembly.GetManifestResourceStream("Robust.Shared.Utility.TypeAbbreviations.yaml"))
    {
      using (StreamReader streamReader = new StreamReader(manifestResourceStream, EncodingHelpers.UTF8))
      {
        YamlStream yamlStream = new YamlStream();
        yamlStream.Load((TextReader) streamReader);
        TypeAbbreviation._abbreviations = TypeAbbreviation.ParseAbbreviations((YamlSequenceNode) yamlStream.Documents[0].RootNode);
      }
    }
  }

  public static string Abbreviate(Type type)
  {
    if (type.FullName == null)
      return "<unnamed type>";
    StringBuilder output = new StringBuilder();
    TypeAbbreviation.AbbreviateName(type.FullName.Split('`')[0].AsSpan(), TypeAbbreviation._abbreviations, output);
    Type[] genericArguments = type.GetGenericArguments();
    if (genericArguments.Length != 0)
    {
      output.Append("`").Append(genericArguments.Length).Append("[");
      foreach (Type type1 in genericArguments)
        TypeAbbreviation.AbbreviateName(type1.FullName.AsSpan(), TypeAbbreviation._abbreviations, output);
      output.Append("]");
    }
    return output.ToString();
  }

  public static string Abbreviate(string name)
  {
    StringBuilder output = new StringBuilder();
    TypeAbbreviation.AbbreviateName(name.AsSpan(), TypeAbbreviation._abbreviations, output);
    return output.ToString();
  }

  private static void AbbreviateName(
    ReadOnlySpan<char> name,
    TypeAbbreviation.Abbreviation[] abbreviations,
    StringBuilder output)
  {
    foreach (TypeAbbreviation.Abbreviation abbreviation in abbreviations)
    {
      if (name.StartsWith<char>(abbreviation.Long.AsSpan()))
      {
        output.Append(abbreviation.Short);
        ref ReadOnlySpan<char> local = ref name;
        int length = abbreviation.Long.Length;
        name = local.Slice(length, local.Length - length);
        if (abbreviation.SubAbbreviations.Length != 0)
        {
          TypeAbbreviation.AbbreviateName(name, abbreviation.SubAbbreviations, output);
          return;
        }
        break;
      }
    }
    output.Append(name);
  }

  private static TypeAbbreviation.Abbreviation[] ParseAbbreviations(YamlSequenceNode sequence)
  {
    TypeAbbreviation.Abbreviation[] abbreviations = new TypeAbbreviation.Abbreviation[sequence.Children.Count];
    for (int index = 0; index < abbreviations.Length; ++index)
    {
      YamlMappingNode mapping = (YamlMappingNode) ((YamlNode) sequence)[index];
      string str1 = mapping.GetNode("long").AsString() + ".";
      string str2 = mapping.GetNode("short").AsString() + ".";
      TypeAbbreviation.Abbreviation[] abbreviationArray = Array.Empty<TypeAbbreviation.Abbreviation>();
      YamlSequenceNode returnNode;
      if (mapping.TryGetNode<YamlSequenceNode>("sub", out returnNode))
        abbreviationArray = TypeAbbreviation.ParseAbbreviations(returnNode);
      TypeAbbreviation.Abbreviation abbreviation = new TypeAbbreviation.Abbreviation()
      {
        Long = str1,
        Short = str2,
        SubAbbreviations = abbreviationArray
      };
      abbreviations[index] = abbreviation;
    }
    return abbreviations;
  }

  private struct Abbreviation
  {
    public string Long { get; set; }

    public string Short { get; set; }

    public TypeAbbreviation.Abbreviation[] SubAbbreviations { get; set; }
  }
}
