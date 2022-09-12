// Created by Crowbar 0.72

$modelname "v_squeak.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_squeak_SQUEAK_Template_Biped(Weapon)1"
}
$bodygroup "body"
{
	studio "v_squeak_v_gordon_hands_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip02 Spine" -1.9 -4.38 -2.04 6.01 1.85 2.23
$hbox 0 "Bip02 Head" 0 -0.72 -0.46 1.43 0.63 0.68
$hbox 0 "Bip01 R Hand" -3 -2 0 4 1 3

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
