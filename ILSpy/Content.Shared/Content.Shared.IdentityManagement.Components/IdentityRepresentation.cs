using Robust.Shared.Enums;
using Robust.Shared.Localization;

namespace Content.Shared.IdentityManagement.Components;

public sealed class IdentityRepresentation
{
	public string TrueName;

	public Gender TrueGender;

	public string AgeString;

	public string? PresumedName;

	public string? PresumedJob;

	public IdentityRepresentation(string trueName, Gender trueGender, string ageString, string? presumedName = null, string? presumedJob = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		TrueName = trueName;
		TrueGender = trueGender;
		AgeString = ageString;
		PresumedJob = presumedJob;
		PresumedName = presumedName;
	}

	public string ToStringKnown(bool trueName)
	{
		string text;
		if (!trueName)
		{
			text = PresumedName;
			if (text == null)
			{
				return ToStringUnknown();
			}
		}
		else
		{
			text = TrueName;
		}
		return text;
	}

	public string ToStringUnknown()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		Gender trueGender = TrueGender;
		string text = (((int)trueGender == 2) ? Loc.GetString("identity-gender-feminine") : (((int)trueGender != 3) ? Loc.GetString("identity-gender-person") : Loc.GetString("identity-gender-masculine")));
		string genderString = text;
		if (PresumedJob != null)
		{
			return $"{AgeString} {PresumedJob} {genderString}";
		}
		return AgeString + " " + genderString;
	}
}
