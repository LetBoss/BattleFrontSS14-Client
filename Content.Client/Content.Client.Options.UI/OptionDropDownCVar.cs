using System;
using System.Collections.Generic;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;

namespace Content.Client.Options.UI;

public sealed class OptionDropDownCVar<T> : BaseOptionCVar<T> where T : notnull
{
	public sealed class ValueOption(T key, string label)
	{
		public readonly T Key = key;

		public readonly string Label = label;
	}

	private struct ItemEntry
	{
		public T Key;
	}

	private readonly OptionDropDown _dropDown;

	private readonly ItemEntry[] _entries;

	protected override T Value
	{
		get
		{
			return (T)_dropDown.Button.SelectedMetadata;
		}
		set
		{
			_dropDown.Button.SelectId(FindValueId(value));
		}
	}

	public OptionDropDownCVar(OptionsTabControlRow controller, IConfigurationManager cfg, CVarDef<T> cVar, OptionDropDown dropDown, IReadOnlyCollection<ValueOption> options)
		: base(controller, cfg, cVar)
	{
		OptionDropDownCVar<T> optionDropDownCVar = this;
		if (options.Count == 0)
		{
			throw new ArgumentException("Need at least one option!");
		}
		_dropDown = dropDown;
		_entries = new ItemEntry[options.Count];
		OptionButton button = dropDown.Button;
		int num = 0;
		foreach (ValueOption option in options)
		{
			_entries[num] = new ItemEntry
			{
				Key = option.Key
			};
			button.AddItem(option.Label, (int?)num);
			button.SetItemMetadata(button.GetIdx(num), (object)option.Key);
			num++;
		}
		dropDown.Button.OnItemSelected += delegate(ItemSelectedEventArgs args)
		{
			dropDown.Button.SelectId(args.Id);
			optionDropDownCVar.ValueChanged();
		};
	}

	private int FindValueId(T value)
	{
		for (int i = 0; i < _entries.Length; i++)
		{
			if (IsValueEqual(_entries[i].Key, value))
			{
				return i;
			}
		}
		return 0;
	}
}
