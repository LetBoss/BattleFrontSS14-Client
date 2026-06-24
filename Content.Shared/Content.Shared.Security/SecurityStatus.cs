namespace Content.Shared.Security;

public enum SecurityStatus : byte
{
	None,
	Suspected,
	Wanted,
	Detained,
	Paroled,
	Discharged
}
