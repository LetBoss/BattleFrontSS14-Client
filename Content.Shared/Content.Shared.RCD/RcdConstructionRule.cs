namespace Content.Shared.RCD;

public enum RcdConstructionRule : byte
{
	MustBuildOnEmptyTile,
	CanBuildOnEmptyTile,
	MustBuildOnSubfloor,
	IsWindow,
	IsCatwalk
}
