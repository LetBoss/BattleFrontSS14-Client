using System;
using Linguini.Shared.Types.Bundle;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Robust.Shared.Localization;

internal static class LinguiniAdapter
{
	internal static ILocValue ToLocValue(this IFluentType arg)
	{
		if (!(arg is FluentNone))
		{
			FluentNumber val = (FluentNumber)(object)((arg is FluentNumber) ? arg : null);
			if (val == null)
			{
				FluentString val2 = (FluentString)(object)((arg is FluentString) ? arg : null);
				if (val2 == null)
				{
					if (arg is FluentLocWrapperType fluentLocWrapperType)
					{
						return fluentLocWrapperType.WrappedValue;
					}
					throw new ArgumentOutOfRangeException("arg");
				}
				return new LocValueString(FluentString.op_Implicit(val2));
			}
			return new LocValueNumber(FluentNumber.op_Implicit(val));
		}
		return new LocValueNone("");
	}

	public static IFluentType FluentFromObject(this object obj, LocContext context)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!(obj is ILocValue wrappedValue))
		{
			if (!(obj is EntityUid value))
			{
				if (!(obj is IFluentEntityUid fluentEntityUid))
				{
					if (!(obj is DateTime value2))
					{
						if (!(obj is TimeSpan value3))
						{
							if (!(obj is Color val))
							{
								if (!(obj is bool) && !(obj is Enum))
								{
									if (!(obj is string text))
									{
										if (!(obj is byte b))
										{
											if (!(obj is sbyte b2))
											{
												if (!(obj is short num))
												{
													if (!(obj is ushort num2))
													{
														if (!(obj is int num3))
														{
															if (!(obj is uint num4))
															{
																if (!(obj is long num5))
																{
																	if (!(obj is ulong num6))
																	{
																		if (!(obj is double num7))
																		{
																			if (obj is float num8)
																			{
																				return (IFluentType)(object)FluentNumber.op_Implicit(num8);
																			}
																			return (IFluentType)(object)FluentString.op_Implicit(obj.ToString());
																		}
																		return (IFluentType)(object)FluentNumber.op_Implicit(num7);
																	}
																	return (IFluentType)(object)FluentNumber.op_Implicit((float)num6);
																}
																return (IFluentType)(object)FluentNumber.op_Implicit((float)num5);
															}
															return (IFluentType)(object)FluentNumber.op_Implicit((float)num4);
														}
														return (IFluentType)(object)FluentNumber.op_Implicit((float)num3);
													}
													return (IFluentType)(object)FluentNumber.op_Implicit((float)(int)num2);
												}
												return (IFluentType)(object)FluentNumber.op_Implicit((float)num);
											}
											return (IFluentType)(object)FluentNumber.op_Implicit((float)b2);
										}
										return (IFluentType)(object)FluentNumber.op_Implicit((float)(int)b);
									}
									return (IFluentType)(object)FluentString.op_Implicit(text);
								}
								return (IFluentType)(object)FluentString.op_Implicit(obj.ToString().ToLowerInvariant());
							}
							return (IFluentType)(object)FluentString.op_Implicit(((Color)(ref val)).ToHex());
						}
						return (IFluentType)(object)new FluentLocWrapperType(new LocValueTimeSpan(value3), context);
					}
					return (IFluentType)(object)new FluentLocWrapperType(new LocValueDateTime(value2), context);
				}
				return (IFluentType)(object)new FluentLocWrapperType(new LocValueEntity(fluentEntityUid.FluentOwner), context);
			}
			return (IFluentType)(object)new FluentLocWrapperType(new LocValueEntity(value), context);
		}
		return (IFluentType)(object)new FluentLocWrapperType(wrappedValue, context);
	}

	public static IFluentType FluentFromVal(this ILocValue locValue, LocContext context)
	{
		if (!(locValue is LocValueNone))
		{
			if (!(locValue is LocValueNumber locValueNumber))
			{
				if (locValue is LocValueString locValueString)
				{
					return (IFluentType)(object)FluentString.op_Implicit(locValueString.Value);
				}
				return (IFluentType)(object)new FluentLocWrapperType(locValue, context);
			}
			return (IFluentType)(object)FluentNumber.op_Implicit(locValueNumber.Value);
		}
		return (IFluentType)(object)FluentNone.None;
	}
}
