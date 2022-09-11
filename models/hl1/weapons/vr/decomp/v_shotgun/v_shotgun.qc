// Created by Crowbar 0.72

$modelname "v_shotgun.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "studio"
{
	studio "v_shotgun_v_spas12_reference"
}
$bodygroup "studio"
{
	studio "v_shotgun_v_gordon_hands_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -3 -2 -1.71 6 2 1.82
$hbox 0 "Reciever" -1 -20 -2.5 1 3 3
$hbox 0 "Reciever" -1 -28.5 -1 1 -20 1.5

$sequence "sm_idle" {
	"v_shotgun_anims\sm_idle"
	fps 9
	loop
}
$sequence "shoot" {
	"v_shotgun_anims\shoot"
	{ event 5001 0 "41" }
	fps 30
}
$sequence "shoot_big" {
	"v_shotgun_anims\shoot_big"
	{ event 5001 0 "41" }
	{ event 5001 1 "41" }
	fps 30
}
$sequence "reload" {
	"v_shotgun_anims\reload"
	fps 16
}
$sequence "pump" {
	"v_shotgun_anims\pump"
	fps 30
}
$sequence "start_reload" {
	"v_shotgun_anims\start_reload"
	fps 12
}
$sequence "idle4" {
	"v_shotgun_anims\idle4"
	fps 9
	loop
}
$sequence "deepidle" {
	"v_shotgun_anims\deepidle"
	fps 12
	loop
}
