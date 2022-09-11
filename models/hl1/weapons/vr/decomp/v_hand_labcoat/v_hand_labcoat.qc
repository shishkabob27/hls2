// Created by Crowbar 0.72

// Stored modelname (without quotes): "v_scientist_hand.mdl"
$modelname "v_hand_labcoat.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "hands"
{
	studio "scientist_fem_scientist_body_female_ref"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "hud_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -1.8 -2.1 -1.65 0.2 0.75 1.8
$hbox 0 "Bip01 R Hand" 0.2 -1.2 -1.2 2 0.6 1.6
$hbox 0 "Bip01 R Hand" 1.4 -0.8 -1.5 4.1 0.2 1.7
$hbox 0 "Bip01 R Finger0" 0 -0.7 -0.8 1.8 0.3 0.5
$hbox 0 "Bip01 R Finger01" -0.9 -0.63 -0.3 0.6 0.35 0.7
$hbox 0 "Bip01 R Finger02" -0.7 -0.5 0.05 0.25 0.3 0.85
$hbox 0 "Bip01 R Finger1" 0 -0.3 0 1.55 0.7 0.5
$hbox 0 "Bip01 R Finger11" 0 -0.2 -0.1 0.9 0.5 0.6
$hbox 0 "Bip01 R Finger12" -0.2 0.1 -0.1 1.05 0.6 0.55
$hbox 0 "Bip01 R Finger2" 0.2 -0.5 -0.1 1.5 0.2 0.6
$hbox 0 "Bip01 R Finger21" 0 -0.4 -0.2 1.1 0.5 0.6
$hbox 0 "Bip01 R Finger22" -0.4 -0.1 -0.2 1 0.5 0.5
$hbox 0 "Bip01 R Finger3" 0.2 -0.5 -0.2 1.35 0.2 0.5
$hbox 0 "Bip01 R Finger31" 0 -0.5 -0.2 1.1 0.25 0.5
$hbox 0 "Bip01 R Finger32" -0.2 -0.2 -0.2 1.1 0.25 0.5
$hbox 0 "Bip01 R Finger4" 0 -0.5 -0.2 1.2 0.15 0.5
$hbox 0 "Bip01 R Finger41" 0 -0.45 -0.2 0.9 0.15 0.5
$hbox 0 "Bip01 R Finger42" -0.2 -0.3 0 0.8 0.2 0.5

$sequence "idle" {
	"v_hand_labcoat_anims\idle"
	fps 0
}
$sequence "point_start" {
	"v_hand_labcoat_anims\point_start"
	fps 30
}
$sequence "point_stop" {
	"v_hand_labcoat_anims\point_stop"
	fps 30
}
$sequence "wait_start" {
	"v_hand_labcoat_anims\wait_start"
	fps 30
}
$sequence "wait_stop" {
	"v_hand_labcoat_anims\wait_stop"
	fps 30
}
$sequence "halfgrab_start" {
	"v_hand_labcoat_anims\halfgrab_start"
	fps 30
}
$sequence "halfgrab_stop" {
	"v_hand_labcoat_anims\halfgrab_stop"
	fps 30
}
$sequence "fullgrab_start" {
	"v_hand_labcoat_anims\fullgrab_start"
	fps 30
}
$sequence "fullgrab_stop" {
	"v_hand_labcoat_anims\fullgrab_stop"
	fps 30
}
