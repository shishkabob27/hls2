// Created by Crowbar 0.72

$modelname "v_357.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "gun"
{
	studio "v_357_python_gun"
}
$bodygroup "bullets"
{
	studio "v_357_python_bullets"
}
$bodygroup "speedload"
{
	studio "v_357_python_speedload"
}
$bodygroup "hands"
{
	studio "v_357_v_gordon_hands_reference"
}
$bodygroup "scope"
{
	blank
	studio "v_357_python_lasersight"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -3 -1.7 -2.4 4.5 2.3 1.8
$hbox 0 "python_master" -0.5 -13.3 4.5 0.5 -5.5 6.2
$hbox 0 "python_master" -1 -5.5 3 1 -1 6.2

$sequence "idle1" {
	"v_357_anims\idle1"
	fps 30
	loop
}
$sequence "fidget1" {
	"v_357_anims\fidget1"
	fps 30
}
$sequence "fire1" {
	"v_357_anims\fire1"
	{ event 5001 0 "31" }
	fps 30
}
$sequence "reload" {
	"v_357_anims\reload"
	fps 37
}
$sequence "holster" {
	"v_357_anims\holster"
	fps 30
}
$sequence "draw" {
	"v_357_anims\draw"
	fps 30
}
$sequence "idle2" {
	"v_357_anims\idle2"
	fps 30
	loop
}
$sequence "idle3" {
	"v_357_anims\idle3"
	fps 30
	loop
}
