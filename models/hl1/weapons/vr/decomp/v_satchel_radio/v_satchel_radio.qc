// Created by Crowbar 0.72

$modelname "v_satchel_radio.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_satchel_v_gordon_hands_reference"
}
$bodygroup "body"
{
	studio "v_satchel_radio_satchel_radio_detonator"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "hud_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "R_Arm_bone" -2 -2 -8 1 2 0
$hbox 0 "Cylinder02" -0.49 -0.75 -6 3.14 0.98 0
$hbox 0 "BoxAntenna" -0.2 -0.2 -3.2 0.2 0.2 3.2

$sequence "idle1" {
	"v_satchel_radio_anims\idle1"
	fps 30
	loop
}
$sequence "fidget1" {
	"v_satchel_radio_anims\fidget1"
	fps 0
}
$sequence "draw" {
	"v_satchel_radio_anims\draw"
	fps 0
}
$sequence "fire" {
	"v_satchel_radio_anims\fire"
	fps 0
}
$sequence "holster" {
	"v_satchel_radio_anims\holster"
	fps 30
}
