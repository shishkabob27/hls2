// Created by Crowbar 0.72

$modelname "v_satchel_radio.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_satchel_radio_satchel_radio_hand"
}
$bodygroup "body"
{
	studio "v_satchel_radio_satchel_radio_detonator"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_posB" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Dummy22" -1.91 -0.76 -2.02 1.71 0.9 4
$hbox 0 "Dummy22" 1 -0.15 4 1.3 0.15 9.9
$hbox 0 "Dummy22" -4 -1 -2 0.2 6 3

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
