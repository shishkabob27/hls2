// Created by Crowbar 0.72

$modelname "v_tripmine.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "hands"
{
	studio "v_tripmine_v_Gordon_hands_reference"
	blank
}
$bodygroup "mine"
{
	studio "V_tripmine_reference"
	studio "w_tripmine_Reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "hud_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "tripmine" -4 -1 -5 4.5 3 0.5
$hbox 0 "tripmine" -7.5 -2 -7.5 -5 2 0.5

$sequence "idle1" {
	"v_tripmine_anims\idle1"
	fps 15
	loop
}
$sequence "idle2" {
	"v_tripmine_anims\idle2"
	fps 15
	loop
}
$sequence "arm1" {
	"v_tripmine_anims\arm1"
	fps 15
	loop
}
$sequence "place" {
	"v_tripmine_anims\place"
	fps 30
}
$sequence "fidget" {
	"v_tripmine_anims\fidget"
	fps 15
	loop
}
$sequence "holster" {
	"v_tripmine_anims\holster"
	fps 30
}
$sequence "draw" {
	"v_tripmine_anims\draw"
	fps 30
}
$sequence "world" {
	"v_tripmine_anims\world"
	fps 30
	loop
}
$sequence "ground" {
	"v_tripmine_anims\ground"
	fps 30
	loop
}
