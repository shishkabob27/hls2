// Created by Crowbar 0.72

$modelname "v_crossbow.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_crossbow_reference"
}
$bodygroup "body"
{
	studio "v_crossbow_v_gordon_hands_Reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -4 -1.6 -3 4.5 2.2 4.704454
$hbox 0 "crossbow parent" -1.5 -21 -4 2 6 1.2
$hbox 0 "RA3" -7 -1 -1 5 1 1
$hbox 0 "LA3" -7 -1 -1 5 1 1
$hbox 0 "BoxStock" -1 -7 -6 1 5.5 1

$sequence "idle1" {
	"v_crossbow_anims\idle1"
	fps 30
	loop
}
$sequence "idle2" {
	"v_crossbow_anims\idle2"
	fps 30
	loop
}
$sequence "fidget1" {
	"v_crossbow_anims\fidget1"
	fps 30
}
$sequence "fidget2" {
	"v_crossbow_anims\fidget2"
	fps 30
}
$sequence "fire1" {
	"v_crossbow_anims\fire1"
	fps 60
}
$sequence "fire2" {
	"v_crossbow_anims\fire2"
	fps 30
}
$sequence "fire3" {
	"v_crossbow_anims\fire3"
	fps 30
}
$sequence "reload" {
	"v_crossbow_anims\reload"
	fps 30
}
$sequence "draw1" {
	"v_crossbow_anims\draw1"
	fps 30
}
$sequence "draw2" {
	"v_crossbow_anims\draw2"
	fps 30
}
$sequence "holster1" {
	"v_crossbow_anims\holster1"
	fps 30
}
$sequence "holster2" {
	"v_crossbow_anims\holster2"
	fps 30
}
