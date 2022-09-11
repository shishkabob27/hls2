// Created by Crowbar 0.72

$modelname "v_9mmar.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "studio"
{
	studio "v_9mmar_m4_view_reference"
}
$bodygroup "studio"
{
	studio "v_9mmar_v_gordon_hands_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "hud_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "R_Arm_bone" -2 -2 -9 2 2 0.1
$hbox 0 "carbine" 7 -14 1 10 21 7
$hbox 0 "carbine" 8 -16 4.5 9.2 -14 5.7

$sequence "longidle" {
	"v_9mmar_anims\longidle"
	fps 0
}
$sequence "idle1" {
	"v_9mmar_anims\idle1"
	fps 0
}
$sequence "grenade" {
	"v_9mmar_anims\grenade"
	fps 8
}
$sequence "reload" {
	"v_9mmar_anims\reload"
	{ event 5004 5 "items/cliprelease1.wav" }
	{ event 5004 24 "items/clipinsert1.wav" }
	fps 30
}
$sequence "deploy" {
	"v_9mmar_anims\deploy"
	fps 8
}
$sequence "shoot_1" {
	"v_9mmar_anims\shoot_1"
	{ event 5001 0 "40" }
	fps 10
}
$sequence "shoot_2" {
	"v_9mmar_anims\shoot_2"
	{ event 5001 0 "40" }
	fps 10
}
$sequence "shoot_3" {
	"v_9mmar_anims\shoot_3"
	{ event 5001 0 "40" }
	fps 10
}
