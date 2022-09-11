// Created by Crowbar 0.72

$modelname "v_satchel.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_satchel_satchel_charge"
}
$bodygroup "body"
{
	studio "v_satchel_gordon_hands"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -8 -2 -2 2 4 1
$hbox 0 "psx_v_satchel" -4 -4 -4 4 4 4

$sequence "idle1" {
	"v_satchel_anims\idle1"
	fps 20
	loop
}
$sequence "fidget1" {
	"v_satchel_anims\fidget1"
	fps 30
}
$sequence "draw" {
	"v_satchel_anims\draw"
	fps 30
}
$sequence "drop" {
	"v_satchel_anims\drop"
	fps 30
}
