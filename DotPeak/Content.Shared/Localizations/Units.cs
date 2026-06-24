// Decompiled with JetBrains decompiler
// Type: Content.Shared.Localizations.Units
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#nullable enable
namespace Content.Shared.Localizations;

public static class Units
{
  public static readonly Units.TypeTable Generic = new Units.TypeTable(new Units.TypeTable.Entry[17]
  {
    new Units.TypeTable.Entry((new double?(), new double?(1E-24)), 1E+24, "si--y"),
    new Units.TypeTable.Entry((new double?(1E-24), new double?(1E-21)), 1E+21, "si--z"),
    new Units.TypeTable.Entry((new double?(1E-21), new double?(1E-18)), 1E+18, "si--a"),
    new Units.TypeTable.Entry((new double?(1E-18), new double?(1E-15)), 1E+15, "si--f"),
    new Units.TypeTable.Entry((new double?(1E-15), new double?(1E-12)), 1000000000000.0, "si--p"),
    new Units.TypeTable.Entry((new double?(1E-12), new double?(1E-09)), 1000000000.0, "si--n"),
    new Units.TypeTable.Entry((new double?(1E-09), new double?(0.001)), 1000000.0, "si--u"),
    new Units.TypeTable.Entry((new double?(0.001), new double?(1.0)), 1000.0, "si--m"),
    new Units.TypeTable.Entry((new double?(1.0), new double?(1000.0)), 1.0, "si"),
    new Units.TypeTable.Entry((new double?(1000.0), new double?(1000000.0)), 0.0001, "si-k"),
    new Units.TypeTable.Entry((new double?(1000000.0), new double?(1000000000.0)), 1E-06, "si-m"),
    new Units.TypeTable.Entry((new double?(1000000000.0), new double?(1000000000000.0)), 1E-09, "si-g"),
    new Units.TypeTable.Entry((new double?(1000000000000.0), new double?(1E+15)), 1E-12, "si-t"),
    new Units.TypeTable.Entry((new double?(1E+15), new double?(1E+18)), 1E-15, "si-p"),
    new Units.TypeTable.Entry((new double?(1E+18), new double?(1E+21)), 1E-18, "si-e"),
    new Units.TypeTable.Entry((new double?(1E+21), new double?(1E+24)), 1E-21, "si-z"),
    new Units.TypeTable.Entry((new double?(1E+24), new double?()), 1E-24, "si-y")
  });
  public static readonly Units.TypeTable Pressure = new Units.TypeTable(new Units.TypeTable.Entry[6]
  {
    new Units.TypeTable.Entry((new double?(), new double?(1E-06)), 1000000000.0, "u--pascal"),
    new Units.TypeTable.Entry((new double?(1E-06), new double?(0.001)), 1000000.0, "m--pascal"),
    new Units.TypeTable.Entry((new double?(0.001), new double?(1.0)), 1000.0, "pascal"),
    new Units.TypeTable.Entry((new double?(1.0), new double?(1000.0)), 1.0, "k-pascal"),
    new Units.TypeTable.Entry((new double?(1000.0), new double?(1000000.0)), 0.0001, "m-pascal"),
    new Units.TypeTable.Entry((new double?(1000000.0), new double?()), 1E-06, "g-pascal")
  });
  public static readonly Units.TypeTable Power = new Units.TypeTable(new Units.TypeTable.Entry[6]
  {
    new Units.TypeTable.Entry((new double?(), new double?(0.001)), 1000000.0, "u--watt"),
    new Units.TypeTable.Entry((new double?(0.001), new double?(1.0)), 1000.0, "m--watt"),
    new Units.TypeTable.Entry((new double?(1.0), new double?(1000.0)), 1.0, "watt"),
    new Units.TypeTable.Entry((new double?(1000.0), new double?(1000000.0)), 0.0001, "k-watt"),
    new Units.TypeTable.Entry((new double?(1000000.0), new double?(1000000000.0)), 1E-06, "m-watt"),
    new Units.TypeTable.Entry((new double?(1000000000.0), new double?()), 1E-09, "g-watt")
  });
  public static readonly Units.TypeTable Energy = new Units.TypeTable(new Units.TypeTable.Entry[6]
  {
    new Units.TypeTable.Entry((new double?(), new double?(0.001)), 1000000.0, "u--joule"),
    new Units.TypeTable.Entry((new double?(0.001), new double?(1.0)), 1000.0, "m--joule"),
    new Units.TypeTable.Entry((new double?(1.0), new double?(1000.0)), 1.0, "joule"),
    new Units.TypeTable.Entry((new double?(1000.0), new double?(1000000.0)), 0.0001, "k-joule"),
    new Units.TypeTable.Entry((new double?(1000000.0), new double?(1000000000.0)), 1E-06, "m-joule"),
    new Units.TypeTable.Entry((new double?(1000000000.0), new double?()), 1E-09, "g-joule")
  });
  public static readonly Units.TypeTable Temperature = new Units.TypeTable(new Units.TypeTable.Entry[6]
  {
    new Units.TypeTable.Entry((new double?(), new double?(0.001)), 1000000.0, "u--kelvin"),
    new Units.TypeTable.Entry((new double?(0.001), new double?(1.0)), 1000.0, "m--kelvin"),
    new Units.TypeTable.Entry((new double?(1.0), new double?(1000.0)), 1.0, "kelvin"),
    new Units.TypeTable.Entry((new double?(1000.0), new double?(1000000.0)), 0.001, "k-kelvin"),
    new Units.TypeTable.Entry((new double?(1000000.0), new double?(1000000000.0)), 1E-06, "m-kelvin"),
    new Units.TypeTable.Entry((new double?(1000000000.0), new double?()), 1E-09, "g-kelvin")
  });
  public static readonly Dictionary<string, Units.TypeTable> Types = new Dictionary<string, Units.TypeTable>()
  {
    ["generic"] = Units.Generic,
    ["pressure"] = Units.Pressure,
    ["power"] = Units.Power,
    ["energy"] = Units.Energy,
    ["temperature"] = Units.Temperature
  };

  public sealed class TypeTable
  {
    public readonly Units.TypeTable.Entry[] E;

    public TypeTable(params Units.TypeTable.Entry[] e) => this.E = e;

    public bool TryGetUnit(double val, [NotNullWhen(true)] out Units.TypeTable.Entry? winner)
    {
      Units.TypeTable.Entry entry1 = (Units.TypeTable.Entry) null;
      foreach (Units.TypeTable.Entry entry2 in this.E)
      {
        double? nullable;
        if (entry2.Range.Min.HasValue)
        {
          nullable = entry2.Range.Min;
          double num = val;
          if (!(nullable.GetValueOrDefault() <= num & nullable.HasValue))
            continue;
        }
        if (entry2.Range.Max.HasValue)
        {
          double num = val;
          nullable = entry2.Range.Max;
          double valueOrDefault = nullable.GetValueOrDefault();
          if (!(num < valueOrDefault & nullable.HasValue))
            continue;
        }
        entry1 = entry2;
      }
      winner = entry1;
      return entry1 != null;
    }

    public string Format(double val)
    {
      Units.TypeTable.Entry winner;
      return this.TryGetUnit(val, out winner) ? $"{(val * winner.Factor).ToString()} {Loc.GetString("units-" + winner.Unit)}" : val.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string Format(double val, string fmt)
    {
      Units.TypeTable.Entry winner;
      return this.TryGetUnit(val, out winner) ? $"{(val * winner.Factor).ToString(fmt)} {Loc.GetString("units-" + winner.Unit)}" : val.ToString(fmt);
    }

    public sealed class Entry
    {
      public readonly (double? Min, double? Max) Range;
      public readonly double Factor;
      public readonly string Unit;

      public Entry((double?, double?) range, double factor, string unit)
      {
        this.Range = range;
        this.Factor = factor;
        this.Unit = unit;
      }
    }
  }
}
