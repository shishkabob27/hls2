// Created by Crowbar 0.72

$modelname "v_hgun.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_Hgun_Template_Biped1"
}
$bodygroup "body"
{
	studio "v_hgun_v_gordon_hands_ref"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos.001" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 Spine1" -10.5 -5.07 -7.43 12 4.74 8.66
$hbox 0 "Bone10" -3.07 -7.18 -1.59 2 5 2.66
$hbox 0 "Bone06" 0 -7.39 -1.58 4.35 5 2.67
$hbox 0 "Bone02" -8.48 -1.6 -1.68 2 1.3 2.25

$sequence "idle1" {
	"v_hgun_anims\idle1"
	fps 16
	loop
}
$sequence "fidgetSway" {
	"v_hgun_anims\fidgetSway"
	fps 16
}
$sequence "fidgetShake" {
	"v_hgun_anims\fidgetShake"
	fps 16
}
$sequence "down" {
	"v_hgun_anims\down"
	fps 16
}
$sequence "up" {
	"v_hgun_anims\up"
	fps 30
}
$sequence "Shoot" {
	"v_hgun_anims\Shoot"
	fps 24
}
