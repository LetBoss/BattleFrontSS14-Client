using Linguini.Shared.Types.Bundle;

namespace Robust.Shared.Localization;

internal sealed class FluentLocWrapperType : IFluentType
{
	public readonly ILocValue WrappedValue;

	private readonly LocContext _context;

	public FluentLocWrapperType(ILocValue wrappedValue, LocContext context)
	{
		WrappedValue = wrappedValue;
		_context = context;
	}

	public string AsString()
	{
		return WrappedValue.Format(_context);
	}

	public bool IsError()
	{
		return false;
	}

	public bool Matches(IFluentType other, IScope scope)
	{
		if (other is FluentLocWrapperType fluentLocWrapperType)
		{
			ILocValue wrappedValue = WrappedValue;
			ILocValue wrappedValue2 = fluentLocWrapperType.WrappedValue;
			if (wrappedValue is LocValueNone)
			{
				if (wrappedValue2 is LocValueNone)
				{
					return true;
				}
			}
			else if (wrappedValue is LocValueDateTime locValueDateTime)
			{
				if (wrappedValue2 is LocValueDateTime locValueDateTime2)
				{
					return locValueDateTime.Value.Equals(locValueDateTime2.Value);
				}
			}
			else if (wrappedValue is LocValueTimeSpan locValueTimeSpan)
			{
				if (wrappedValue2 is LocValueTimeSpan locValueTimeSpan2)
				{
					return locValueTimeSpan.Value.Equals(locValueTimeSpan2.Value);
				}
			}
			else if (wrappedValue is LocValueNumber locValueNumber)
			{
				if (wrappedValue2 is LocValueNumber locValueNumber2)
				{
					return locValueNumber.Value.Equals(locValueNumber2.Value);
				}
			}
			else if (wrappedValue is LocValueString locValueString)
			{
				if (wrappedValue2 is LocValueString locValueString2)
				{
					return locValueString.Value.Equals(locValueString2.Value);
				}
			}
			else if (wrappedValue is LocValueEntity locValueEntity)
			{
				if (wrappedValue2 is LocValueEntity locValueEntity2)
				{
					return locValueEntity.Value.Equals(locValueEntity2.Value);
				}
			}
			else if (wrappedValue == null)
			{
				goto IL_0177;
			}
			if (wrappedValue2 == null)
			{
				goto IL_0177;
			}
			return object.Equals(wrappedValue, wrappedValue2);
		}
		return false;
		IL_0177:
		return false;
	}

	public IFluentType Copy()
	{
		return (IFluentType)(object)this;
	}
}
