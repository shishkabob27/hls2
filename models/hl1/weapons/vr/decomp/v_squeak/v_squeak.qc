// Created by Crowbar 0.72

$modelname "v_squeak.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_squeak_v_gordon_hands_reference"
}
$bodygroup "body"
{
	studio "v_squeak_v_snark_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "hud_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "R_Arm_bone" -2 -2 -10 1 2 0
$hbox 0 "Bip02 Spine" -1.87 -4.46 -2.51 6 1.71 2.68
$hbox 0 "Bip02 Head" -1 -0.58 -0.45 1.39 0.6 0.62

$sequence "idle1" {
	"v_squeak_anims\idle1"
	fps 16
	loop
}
$sequence "fidgetfit" {
	"v_squeak_anims\fidgetfit"
	fps 16
}
$sequence "fidgetnip" {
	"v_squeak_anims\fidgetnip"
	fps 16
}
$sequence "down" {
	"v_squeak_anims\down"
	fps 16
}
$sequence "up" {
	"v_squeak_anims\up"
	fps 30
}
$sequence "throw" {
	"v_squeak_anims\throw"
	fps 24
}
