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

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 Spine1" -9 -3.6 -6 5 3.4 6
$hbox 0 "Bip01 Spine1" 7 -0.5 -9 13 3.4 9
$hbox 0 "Bone10" -3.62 -7.3 -1.25 0.76 0 1.86
$hbox 0 "Bone06" -0.43 -7.53 -1.09 3.97 0 2
$hbox 0 "Bone02" -8.64 -1.47 -1 5 0 1

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
