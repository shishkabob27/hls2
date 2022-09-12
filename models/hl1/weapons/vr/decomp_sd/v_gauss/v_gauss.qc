// Created by Crowbar 0.72

$modelname "v_gauss.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_gauss_gauss_gun"
}
$bodygroup "body"
{
	studio "v_gauss_gauss_hand"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Gauss Master" -14.8 -5.8 -0.1 -6.5 -2.8 3.3
$hbox 0 "Gauss Master" -8 -3.4 -1.5 8.2 2.2 1.5
$hbox 0 "Gauss Master" 8.2 -2.7 -1.7 12.3 0.5 1.7
$hbox 0 "Gauss Master" 12 -1.4 -0.41 17 -0.5 0.41
$hbox 0 "Gauss Master" -18.6 -8 -0.8 -9.6 -4 0.8
$hbox 0 "Spinner" -3.02 -1.75 -2 0 2.55 2

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
