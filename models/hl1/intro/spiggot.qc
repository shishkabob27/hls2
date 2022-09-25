
// Stored modelname (without quotes): "valve/models/spiggot.mdl"
$modelname "spiggot.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "spiggot"
}

$flags 0


$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Torus01" -3.87 -3.93 -1.98 3.87 3.93 1.98

$sequence "idle" {
	"spiggot_anims\idle"
	fps 30
}
$sequence "finalspiggot" {
	"spiggot_anims\finalspiggot"
	fps 20
}
$sequence "spiggotidle" {
	"spiggot_anims\spiggotidle"
	fps 20
	loop
}
