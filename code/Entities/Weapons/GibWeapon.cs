[Library("weapon_gibweapon")]
[EditorModel( "models/hl1/weapons/world/glock.vmdl" )]
[Title("GibWeapon"), Category( "Weapons" )]
partial class GibWeapon : HLWeapon
{
	public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/glock.vmdl");
	public override string ViewModelPath => "models/hl1/weapons/view/v_glock.vmdl";
	public override float PrimaryRate => 10;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack();
	}

	public override void AttackPrimary()
	{

        if (IsServer)
        {
			HLCombat.CreateGibs(Owner.Position, Owner.Position, 0, new BBox(new Vector3(-16, -16, 0), new Vector3(16, 16, 72)));

		}

		(Owner as AnimatedEntity).SetAnimParameter("b_attack", true);
	}

	public override void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;

		var shootEase = Easing.EaseIn( lastAttack.LerpInverse( 0.2f, 0.0f ) );
		var color = Color.Lerp( Color.Red, Color.Yellow, lastReload.LerpInverse( 0.0f, 0.4f ) );

		draw.BlendMode = BlendMode.Lighten;
		draw.Color = color.WithAlpha( 0.2f + lastAttack.LerpInverse( 1.2f, 0 ) * 0.5f );

		var length = 8.0f - shootEase * 2.0f;
		var gap = 10.0f + shootEase * 30.0f;
		var thickness = 2.0f;

		draw.Line( thickness, center + Vector2.Left * gap, center + Vector2.Left * (length + gap) );
		draw.Line( thickness, center - Vector2.Left * gap, center - Vector2.Left * (length + gap) );

		draw.Line( thickness, center + Vector2.Up * gap, center + Vector2.Up * (length + gap) );
		draw.Line( thickness, center - Vector2.Up * gap, center - Vector2.Up * (length + gap) );
	}

}
