// Created by Crowbar 0.72

$modelname "v_crowbar.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "studio"
{
	studio "v_crowbar_reference"
}
$bodygroup "studio"
{
	studio "v_crowbar_v_gordon_hands_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" 2.2 0.5 -6.86 4 2.5 22.969999
$hbox 0 "R_Arm_bone" -2.04 -3 -7 3 2.37 0.49
$hbox 0 "BoxEnd" -0.8 -2 -0.8 0.8 3.5 0.8

$sequence "idle1" {
	"v_crowbar_anims\idle1"
	fps 0
}
$sequence "draw" {
	"v_crowbar_anims\draw"
	fps 0
}
$sequence "holster" {
	"v_crowbar_anims\holster"
	fps 0
}
$sequence "attack1" {
	"v_crowbar_anims\attack1"
	fps 22
}
$sequence "attack1miss" {
	"v_crowbar_anims\attack1miss"
	fps 22
}
$sequence "attack2" {
	"v_crowbar_anims\attack2"
	fps 22
}
$sequence "attack2hit" {
	"v_crowbar_anims\attack2hit"
	fps 22
}
$sequence "attack3" {
	"v_crowbar_anims\attack3"
	fps 24
}
$sequence "attack3hit" {
	"v_crowbar_anims\attack3hit"
	fps 24
}
$sequence "idle2" {
	"v_crowbar_anims\idle2"
	fps 0
	loop
}
$sequence "idle3" {
	"v_crowbar_anims\idle3"
	fps 0
	loop
}
