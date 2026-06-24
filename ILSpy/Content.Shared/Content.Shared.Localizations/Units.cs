using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Robust.Shared.Localization;

namespace Content.Shared.Localizations;

public static class Units
{
	public sealed class TypeTable
	{
		public sealed class Entry
		{
			public readonly (double? Min, double? Max) Range;

			public readonly double Factor;

			public readonly string Unit;

			public Entry((double?, double?) range, double factor, string unit)
			{
				Range = range;
				Factor = factor;
				Unit = unit;
			}
		}

		public readonly Entry[] E;

		public TypeTable(params Entry[] e)
		{
			E = e;
		}

		public bool TryGetUnit(double val, [NotNullWhen(true)] out Entry? winner)
		{
			Entry w = null;
			Entry[] e = E;
			foreach (Entry e2 in e)
			{
				if ((!e2.Range.Min.HasValue || e2.Range.Min <= val) && (!e2.Range.Max.HasValue || val < e2.Range.Max))
				{
					w = e2;
				}
			}
			winner = w;
			return w != null;
		}

		public string Format(double val)
		{
			if (TryGetUnit(val, out Entry w))
			{
				return val * w.Factor + " " + Loc.GetString("units-" + w.Unit);
			}
			return val.ToString(CultureInfo.InvariantCulture);
		}

		public string Format(double val, string fmt)
		{
			if (TryGetUnit(val, out Entry w))
			{
				return (val * w.Factor).ToString(fmt) + " " + Loc.GetString("units-" + w.Unit);
			}
			return val.ToString(fmt);
		}
	}

	public static readonly TypeTable Generic = new TypeTable(new TypeTable.Entry((null, 1E-24), 1E+24, "si--y"), new TypeTable.Entry((1E-24, 1E-21), 1E+21, "si--z"), new TypeTable.Entry((1E-21, 1E-18), 1E+18, "si--a"), new TypeTable.Entry((1E-18, 1E-15), 1000000000000000.0, "si--f"), new TypeTable.Entry((1E-15, 1E-12), 1000000000000.0, "si--p"), new TypeTable.Entry((1E-12, 1E-09), 1000000000.0, "si--n"), new TypeTable.Entry((1E-09, 0.001), 1000000.0, "si--u"), new TypeTable.Entry((0.001, 1.0), 1000.0, "si--m"), new TypeTable.Entry((1.0, 1000.0), 1.0, "si"), new TypeTable.Entry((1000.0, 1000000.0), 0.0001, "si-k"), new TypeTable.Entry((1000000.0, 1000000000.0), 1E-06, "si-m"), new TypeTable.Entry((1000000000.0, 1000000000000.0), 1E-09, "si-g"), new TypeTable.Entry((1000000000000.0, 1000000000000000.0), 1E-12, "si-t"), new TypeTable.Entry((1000000000000000.0, 1E+18), 1E-15, "si-p"), new TypeTable.Entry((1E+18, 1E+21), 1E-18, "si-e"), new TypeTable.Entry((1E+21, 1E+24), 1E-21, "si-z"), new TypeTable.Entry((1E+24, null), 1E-24, "si-y"));

	public static readonly TypeTable Pressure = new TypeTable(new TypeTable.Entry((null, 1E-06), 1000000000.0, "u--pascal"), new TypeTable.Entry((1E-06, 0.001), 1000000.0, "m--pascal"), new TypeTable.Entry((0.001, 1.0), 1000.0, "pascal"), new TypeTable.Entry((1.0, 1000.0), 1.0, "k-pascal"), new TypeTable.Entry((1000.0, 1000000.0), 0.0001, "m-pascal"), new TypeTable.Entry((1000000.0, null), 1E-06, "g-pascal"));

	public static readonly TypeTable Power = new TypeTable(new TypeTable.Entry((null, 0.001), 1000000.0, "u--watt"), new TypeTable.Entry((0.001, 1.0), 1000.0, "m--watt"), new TypeTable.Entry((1.0, 1000.0), 1.0, "watt"), new TypeTable.Entry((1000.0, 1000000.0), 0.0001, "k-watt"), new TypeTable.Entry((1000000.0, 1000000000.0), 1E-06, "m-watt"), new TypeTable.Entry((1000000000.0, null), 1E-09, "g-watt"));

	public static readonly TypeTable Energy = new TypeTable(new TypeTable.Entry((null, 0.001), 1000000.0, "u--joule"), new TypeTable.Entry((0.001, 1.0), 1000.0, "m--joule"), new TypeTable.Entry((1.0, 1000.0), 1.0, "joule"), new TypeTable.Entry((1000.0, 1000000.0), 0.0001, "k-joule"), new TypeTable.Entry((1000000.0, 1000000000.0), 1E-06, "m-joule"), new TypeTable.Entry((1000000000.0, null), 1E-09, "g-joule"));

	public static readonly TypeTable Temperature = new TypeTable(new TypeTable.Entry((null, 0.001), 1000000.0, "u--kelvin"), new TypeTable.Entry((0.001, 1.0), 1000.0, "m--kelvin"), new TypeTable.Entry((1.0, 1000.0), 1.0, "kelvin"), new TypeTable.Entry((1000.0, 1000000.0), 0.001, "k-kelvin"), new TypeTable.Entry((1000000.0, 1000000000.0), 1E-06, "m-kelvin"), new TypeTable.Entry((1000000000.0, null), 1E-09, "g-kelvin"));

	public static readonly Dictionary<string, TypeTable> Types = new Dictionary<string, TypeTable>
	{
		["generic"] = Generic,
		["pressure"] = Pressure,
		["power"] = Power,
		["energy"] = Energy,
		["temperature"] = Temperature
	};
}
