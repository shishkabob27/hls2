// Created by Crowbar 0.72

$modelname "v_gauss.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "gun"
{
	studio "v_gauss_reference"
}
$bodygroup "body"
{
	studio "v_gauss_v_gordon_hands_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -3 -2.5 -6 1.5 3.08 3
$hbox 0 "GaussMesh" -4 -12.5 -3 2 5 4
$hbox 0 "BoxGaussEnd" -1.8 0 -1.8 1.8 4 1.8
$hbox 0 "Spinner" -3.48 -1.6 -2.2 0 2.4 2.17
$hbox 0 "BoxStock" -0.5 -5 -4 0.5 5 0.6
$hbox 0 "BoxGaussRod" -0.3 -1.8 -0.3 0.3 1.8 0.3

$sequence "idle" {
	"v_gauss_anims\idle"
	fps 15
	loop
}
$sequence "idle2" {
	"v_gauss_anims\idle2"
	fps 15
	loop
}
$sequence "fidget" {
	"v_gauss_anims\fidget"
	fps 30
}
$sequence "spinup" {
	"v_gauss_anims\spinup"
	fps 30
	loop
}
$sequence "spin" {
	"v_gauss_anims\spin"
	fps 30
	loop
}
$sequence "fire" {
	"v_gauss_anims\fire"
	fps 30
}
$sequence "fire2" {
	"v_gauss_anims\fire2"
	fps 30
}
$sequence "holster" {
	"v_gauss_anims\holster"
	fps 45
}
$sequence "draw" {
	"v_gauss_anims\draw"
	fps 60
}
