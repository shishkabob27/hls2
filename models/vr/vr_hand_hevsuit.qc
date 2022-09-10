// Created by Crowbar 0.72

// Stored modelname (without quotes): "v_hand_hevsuit.mdl"
$modelname "vr_hand_hevsuit.mdl"
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

$hbox 0 "Right wrist" -1.59 -1.81 -2.72 1.88 2.649179 4.213242
$hbox 0 "finger_thumb_0_r" -1.555437 -0.77 -0.7 0.07 0.63 0.54
$hbox 0 "finger_index_meta_r" -2.840858 -0.83 -0.57 0 0.86 0.58
$hbox 0 "root" -3.27 0 -0.47 0 6.34 2.57
