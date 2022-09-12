// Created by Crowbar 0.72

$modelname "v_9mmar.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "studio"
{
	studio "v_9mmar_mp5sd"
}
$bodygroup "studio"
{
	studio "v_9mmar_v_gordon_hands_reference"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bone09" -16.5 -1.5 -0.6 4 0.5 1
$hbox 0 "Bone09" -14 -3.8 -0.6 -6 -1.5 1
$hbox 0 "Bone09" -5.5 -7.5 -0.6 -4 -1.5 1
$hbox 0 "Bip01 R Hand" -3 -1.7 -1.7 4.6 3 2.1

$sequence "longidle" {
	"v_9mmar_anims\longidle"
	fps 8
}
$sequence "idle1" {
	"v_9mmar_anims\idle1"
	fps 35
}
$sequence "grenade" {
	"v_9mmar_anims\grenade"
	fps 30
}
$sequence "reload" {
	"v_9mmar_anims\reload"
	{ event 5004 5 "items/cliprelease1.wav" }
	{ event 5004 24 "items/clipinsert1.wav" }
	fps 30
}
$sequence "deploy" {
	"v_9mmar_anims\deploy"
	fps 12
}
$sequence "shoot_1" {
	"v_9mmar_anims\shoot_1"
	{ event 5001 0 "20" }
	fps 10
}
$sequence "shoot_2" {
	"v_9mmar_anims\shoot_2"
	{ event 5001 0 "20" }
	fps 10
}
$sequence "shoot_3" {
	"v_9mmar_anims\shoot_3"
	{ event 5001 0 "20" }
	fps 10
}
