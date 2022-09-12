// Created by Crowbar 0.72

$modelname "v_grenade.mdl"
$cd "."
$cdtexture "."
$cliptotextures
$scale 1.0

$bodygroup "body"
{
	studio "v_grenade_grenade"
}
$bodygroup "body"
{
	studio "v_grenade_grenade_hand"
}

$flags 0


$attachment 0 "muzzle_pos" 0 0 0
$attachment 1 "muzzle_pos2" 0 0 0
$attachment 2 "HUD_pos" 0 0 0
$attachment 3 "HUD_pos2" 0 0 0

$cbox 0 0 0 0 0 0

$bbox 0 0 0 0 0 0

$hbox 0 "Dummy22" -1.01 -1.22 -2.13 1.62 1.92 3.11
$hbox 0 "Bip01 R Finger1" -6 -1.5 -3 3 4 1

$sequence "idle" {
	"v_grenade_anims\idle"
	fps 0
}
$sequence "fidget" {
	"v_grenade_anims\fidget"
	fps 30
	loop
}
$sequence "pinpull" {
	"v_grenade_anims\pinpull"
	fps 45
}
$sequence "throw1" {
	"v_grenade_anims\throw1"
	fps 0
}
$sequence "throw2" {
	"v_grenade_anims\throw2"
	fps 0
}
$sequence "throw3" {
	"v_grenade_anims\throw3"
	fps 0
}
$sequence "holster" {
	"v_grenade_anims\holster"
	fps 0
}
$sequence "draw" {
	"v_grenade_anims\draw"
	fps 0
}
