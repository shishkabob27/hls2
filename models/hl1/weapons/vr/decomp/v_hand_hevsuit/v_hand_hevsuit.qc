// Created by Crowbar 0.72

$modelname "v_hand_hevsuit.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "hands"
{
	studio "v_hand_hevsuit_v_gordon_hands_ref"
}
$bodygroup "light_case"
{
	blank
	studio "v_hand_hevsuit_v_gordon_hand_v_gordon_light_ref"
}
$bodygroup "light_on"
{
	blank
	studio "v_hand_hevsuit_v_gordon_hand_v_gordon_light_on_ref"
}

$flags 0

$texrendermode "gordon_glove_Additive.bmp" additive

$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "hud_pos" 0 0 0
$attachment 3 "hud_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Bip01 R Hand" -2.55 -2.2 -1.73 -0.4 0.84 1.5
$hbox 0 "Bip01 R Hand" -0.4 -1.6 -1.3 2 0.6 1.3
$hbox 0 "Bip01 R Hand" 2 -0.6 -1.5 4.7 0.8 1.7
$hbox 0 "Bip01 R Finger0" 0 -0.5 -0.5 1.8 0.45 0.7
$hbox 0 "Bip01 R Finger01" -0.9 -0.63 -0.3 0.6 0.45 0.7
$hbox 0 "Bip01 R Finger02" -0.7 -0.5 -0.2 0.25 0.3 0.65
$hbox 0 "Bip01 R Finger1" 0 -0.5 -0.1 1.55 0.7 0.6
$hbox 0 "Bip01 R Finger11" 0 -0.4 -0.1 1.55 0.7 0.6
$hbox 0 "Bip01 R Finger12" 0 -0.1 -0.1 1.4 0.6 0.6
$hbox 0 "Bip01 R Finger2" 0.2 -0.6 -0.1 1.55 0.7 0.6
$hbox 0 "Bip01 R Finger21" 0 -0.4 -0.1 1.55 0.7 0.6
$hbox 0 "Bip01 R Finger22" 0 -0.1 -0.1 1.5 0.6 0.6
$hbox 0 "Bip01 R Finger3" 0.2 -0.6 -0.2 1.55 0.7 0.5
$hbox 0 "Bip01 R Finger31" 0 -0.4 -0.2 1.55 0.7 0.5
$hbox 0 "Bip01 R Finger32" 0.3 -0.3 -0.2 1.7 0.4 0.5
$hbox 0 "Bip01 R Finger4" 0 -0.8 -0.2 1.55 0.5 0.5
$hbox 0 "Bip01 R Finger41" 0 -0.7 -0.2 1.55 0.3 0.5
$hbox 0 "Bip01 R Finger42" 0.25 -0.55 0 1.5 0.3 0.6

$sequence "idle" {
	"v_hand_hevsuit_anims\idle"
	fps 0
}
$sequence "point_start" {
	"v_hand_hevsuit_anims\point_start"
	fps 30
}
$sequence "point_stop" {
	"v_hand_hevsuit_anims\point_stop"
	fps 30
}
$sequence "wait_start" {
	"v_hand_hevsuit_anims\wait_start"
	fps 30
}
$sequence "wait_stop" {
	"v_hand_hevsuit_anims\wait_stop"
	fps 30
}
$sequence "halfgrab_start" {
	"v_hand_hevsuit_anims\halfgrab_start"
	fps 30
}
$sequence "halfgrab_stop" {
	"v_hand_hevsuit_anims\halfgrab_stop"
	fps 30
}
$sequence "fullgrab_start" {
	"v_hand_hevsuit_anims\fullgrab_start"
	fps 30
}
$sequence "fullgrab_stop" {
	"v_hand_hevsuit_anims\fullgrab_stop"
	fps 30
}
