
partial class HL1GameMovement
{
	[ConVar.Replicated] public static float sv_gravity { get; set; } = 800;
	[ConVar.Replicated] public static float sv_stopspeed { get; set; } = 100;
	[ConVar.Replicated] public static float sv_noclip_accelerate { get; set; } = 5;
	[ConVar.Replicated] public static float sv_noclip_speed { get; set; } = 5;
	[ConVar.Replicated] public static float sv_spectator_accelerate { get; set; } = 5;
	[ConVar.Replicated] public static float sv_spectator_speed { get; set; } = 3;
	[ConVar.Replicated] public static bool sv_spectator_noclip { get; set; } = true;

	[ConVar.Replicated] public static float sv_maxspeed { get; set; } = 320;
	[ConVar.Replicated] public static float sv_accelerate { get; set; } = 10;

	[ConVar.Replicated] public static float sv_airaccelerate { get; set; } = 10;
	[ConVar.Replicated] public static float sv_aircontrol { get; set; } = 100;
	[ConVar.Replicated] public static float sv_airspeedcap { get; set; } = 30;
	[ConVar.Replicated] public static float sv_wateraccelerate { get; set; } = 10;
	[ConVar.Replicated] public static float sv_waterfriction { get; set; } = 1;
	[ConVar.Replicated] public static float sv_footsteps { get; set; } = 1;
	[ConVar.Replicated] public static float sv_rollspeed { get; set; } = 200;
	[ConVar.Replicated] public static float sv_rollangle { get; set; } = 0;
	[ConVar.Replicated] public static float sv_maxnonjumpvelocity { get; set; } = 250;
	[ConVar.Replicated] public static float sv_maxstandableangle { get; set; } = 45;

	[ConVar.Replicated] public static float sv_friction { get; set; } = 4;

	[ConVar.Replicated] public static float sv_bounce { get; set; } = 0;
	[ConVar.Replicated] public static float sv_maxvelocity { get; set; } = 3500;
	[ConVar.Replicated] public static float sv_stepsize { get; set; } = 18;
	[ConVar.Replicated] public static float sv_backspeed { get; set; } = 0.6f;
	[ConVar.Replicated] public static float sv_waterdist { get; set; } = 12;


	[ConVar.Replicated] public static float sv_sprintspeed { get; set; } = 320.0f;
	[ConVar.Replicated] public static float sv_walkspeed { get; set; } = 150.0f;
	[ConVar.Replicated] public static float sv_defaultspeed { get; set; } = 320.0f;
	[ConVar.Replicated] public static bool sv_autojump { get; set; } = false;
	[ConVar.Replicated] public static bool sv_enablebunnyhopping { get; set; } = true;

}
