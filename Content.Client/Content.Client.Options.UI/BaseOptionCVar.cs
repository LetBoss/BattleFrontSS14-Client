using System;
using System.Collections.Generic;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;

namespace Content.Client.Options.UI;

public abstract class BaseOptionCVar<TValue> : BaseOption where TValue : notnull
{
	private readonly IConfigurationManager _cfg;

	private readonly CVarDef<TValue> _cVar;

	protected abstract TValue Value { get; set; }

	public event Action<TValue>? ImmediateValueChanged;

	protected BaseOptionCVar(OptionsTabControlRow controller, IConfigurationManager cfg, CVarDef<TValue> cVar)
		: base(controller)
	{
		_cfg = cfg;
		_cVar = cVar;
	}

	public override void LoadValue()
	{
		Value = _cfg.GetCVar<TValue>(_cVar);
	}

	public override void SaveValue()
	{
		_cfg.SetCVar<TValue>(_cVar, Value, false);
	}

	public override void ResetToDefault()
	{
		Value = _cVar.DefaultValue;
	}

	public override bool IsModified()
	{
		return !IsValueEqual(Value, _cfg.GetCVar<TValue>(_cVar));
	}

	public override bool IsModifiedFromDefault()
	{
		return !IsValueEqual(Value, _cVar.DefaultValue);
	}

	protected virtual bool IsValueEqual(TValue a, TValue b)
	{
		if (typeof(TValue) == typeof(float))
		{
			return MathHelper.CloseToPercent((float)(object)a, (float)(object)b, 1E-05);
		}
		return EqualityComparer<TValue>.Default.Equals(a, b);
	}

	protected override void ValueChanged()
	{
		base.ValueChanged();
		this.ImmediateValueChanged?.Invoke(Value);
	}
}
