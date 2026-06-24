namespace Robust.Shared.ViewVariables;

public enum ViewVariablesResponseCode : ushort
{
	Ok = 200,
	InvalidRequest = 400,
	NoAccess = 401,
	NoObject = 404
}
