// Created by Crowbar 0.72

$modelname "v_crossbow.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_crossbow_compound_scope"
}
$bodygroup "body"
{
	studio "v_crossbow_crossbow_hand"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "crossbow parent" -3.8 5 -4 -1 9 1
$hbox 0 "crossbow parent" -1.5 -20.5 -1 2 4 1
$hbox 0 "crossbow parent" -0.4 -12 -12 0.45 -8.5 -4
$hbox 0 "crossbow parent" -1 -5 1.5 1.3 2 3.8
$hbox 0 "crossbow parent" -0.8 2 -6.5 0.8 15.8 -1
$hbox 0 "LA3" -5 -0.5 -1.2 6 1.6 1.4
$hbox 0 "RA3" -6 -0.5 -1.2 5 1.6 1.4

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
