[Library("monster_test"), HammerEntity] // THIS WILL NOT BE AN NPC BUT A BASE THAT EVERY NPC SHOULD DERIVE FROM!!! THIS IS HERE FOR TESTING PURPOSES ONLY!
public partial class NPC : AnimatedEntity, IUse
{
	public bool InScriptedSequence = false;
	public bool InPriorityScriptedSequence = false;
	public bool DontSleep = false;
	public bool NoNav = false;
    
	[ConVar.Replicated]
	public static bool nav_drawpath { get; set; }

	[ConCmd.Server("npc_clear")]
	public static void NpcClear()
	{
		foreach (var npc in Entity.All.OfType<NPC>().ToArray())
			npc.Delete();
	}

	public float Speed;
	public float WalkSpeed = 80;
	public float RunSpeed = 200;

	public string NPCAnimGraph = "";
	NavPath Path;
	public NavSteer Steer;

	public override void Spawn()
    {
		Tags.Add("npc", "playerclip");
		base.Spawn();
		animHelper = new HLAnimationHelper(this);
		if (!NoNav)
		{

			Path = new NavPath();

            Steer = new NavSteer();

        }
		SetModel("models/citizen/citizen.vmdl");
		EyePosition = Position + Vector3.Up * 64;
		if (PhysicsBody == null) SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius(72, 8));
		
		EnableHitboxes = true;
		PhysicsBody.SetSurface("surface/hlflesh.surface");
		Speed = 50;
	}

	public Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;

	Vector3 InputVelocity;

	Vector3 LookDir;
	public Rotation targetRotation;

	public Nullable<Rotation> targetRotationOVERRIDE;
	public Sound CurrentSound;
	public HLAnimationHelper animHelper;
	[Event.Tick.Server]
	public void Tick()
	{
        if (NoNav)
		{
			Think();
			SoundProcess();
			return;
		}

        if (HLUtils.PlayerInRangeOf(Position, 2048) == false && DontSleep == false)
			return;
		using var _a = Sandbox.Debug.Profile.Scope("NpcTest::Tick");

        


		InputVelocity = 0;

		if (Steer != null && !NoNav)
		{
			using var _b = Sandbox.Debug.Profile.Scope("Steer");

			Steer.Tick(Position);

			if (!Steer.Output.Finished)
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped(InputVelocity * Time.Delta * 500, Speed);
			}

			if (nav_drawpath)
			{
				Steer.DebugDrawPath();
			}
		}

		using (Sandbox.Debug.Profile.Scope("Move"))
		{
			Move(Time.Delta);
		}

		var walkVelocity = Velocity.WithZ(0);
		var turnSpeed = 0.32f;
		if (walkVelocity.Length > 0.5f)
		{
			turnSpeed = walkVelocity.Length.LerpInverse(0, 100, true);
			targetRotation = Rotation.LookAt(walkVelocity.Normal, Vector3.Up);
		}
		if (targetRotationOVERRIDE != null)
		{
            targetRotation = (Rotation)targetRotationOVERRIDE;
            Rotation = Rotation.Lerp(Rotation, (Rotation)targetRotationOVERRIDE, turnSpeed * Time.Delta * 20.0f);
            if (Angles.AngleVector(Rotation.Angles()).AlmostEqual(Angles.AngleVector(targetRotation.Angles())))
            {
                targetRotationOVERRIDE = null;
            }
        }
		else
		{
			Rotation = Rotation.Lerp(Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f);
		}
		
		//var animHelper = new HLAnimationHelper(this);

		LookDir = Vector3.Lerp(LookDir, InputVelocity.WithZ(0) * 1000, Time.Delta * 100.0f);
		animHelper.WithLookAt(EyePosition + LookDir);
		animHelper.WithVelocity(Velocity);
		animHelper.WithWishVelocity(InputVelocity);
		animHelper.HealthLevel = Health;

		//animHelper.VoiceLevel = CurrentSound.Index / 100;


		//Log.Info();//SoundFile.Load("sounds/hl1/scientist/alright.wav"));
		if (CurrentSound.Finished != true)
        {
			animHelper.VoiceLevel = Rand.Float();
			// It's not possible to get the sound volume for animating the mouths so i'll just randomise a float for now,.
		}
		else
        {
            animHelper.VoiceLevel = 0;
        }
		if (LifeState == LifeState.Dead)
			return;
		Think();
		SoundProcess();
		//SoundStream test;
		//test.


	}

    public virtual void Think()
    {
		
	}

	public virtual void SoundProcess()
	{

	}

	public virtual bool OnUse(Entity user)
	{
		if (LifeState == LifeState.Dead)
			return false;
		return true;
	}
    
	protected virtual void Move(float timeDelta)
	{
		if (LifeState == LifeState.Dead)
			return;
		var bbox = BBox.FromHeightAndRadius(64, 4);
		//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

		MoveHelper move = new(Position, Velocity);
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore(this).Size(bbox);

		if (!Velocity.IsNearlyZero(0.001f))
		{
			//	Sandbox.Debug.Draw.Once
			//						.WithColor( Color.Red )
			//						.IgnoreDepth()
			//						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

			using (Sandbox.Debug.Profile.Scope("TryUnstuck"))
				move.TryUnstuck();

			using (Sandbox.Debug.Profile.Scope("TryMoveWithStep"))
				move.TryMoveWithStep(timeDelta, 30);
		}

		using (Sandbox.Debug.Profile.Scope("Ground Checks"))
		{
			var tr = move.TraceDirection(Vector3.Down * 10.0f);

			if (move.IsFloor(tr))
			{
				GroundEntity = tr.Entity;

				if (!tr.StartedSolid)
				{
					move.Position = tr.EndPosition;
				}

				if (InputVelocity.Length > 0)
				{
					var movement = move.Velocity.Dot(InputVelocity.Normal);
					move.Velocity = move.Velocity - movement * InputVelocity.Normal;
					move.ApplyFriction(tr.Surface.Friction * 10.0f, timeDelta);
					move.Velocity += movement * InputVelocity.Normal;

				}
				else
				{
					move.ApplyFriction(tr.Surface.Friction * 10.0f, timeDelta);
				}
			}
			else
			{
				GroundEntity = null;
				move.Velocity += Vector3.Down * 900 * timeDelta;
				Sandbox.Debug.Draw.Once.WithColor(Color.Red).Circle(Position, Vector3.Up, 10.0f);
			}
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}


	DamageInfo LastDamage;
    
	public override void TakeDamage(DamageInfo info)
	{
		

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;
		if (IsServer)
		{
			Health -= info.Damage;
			if (Health <= 0f)
			{
				if (LifeState == LifeState.Alive)
				{
                    OnKilled();
					LifeState = LifeState.Dead;
					//Delete();
				}
			}
		}
		LastDamage = info;

		if (Health <= 0f)
		{
			if (LifeState == LifeState.Alive)
			{
				LifeState = LifeState.Dead;
				//Delete();
			}
		}
        if (Health < -20)
        {
			HLCombat.CreateGibs(this.CollisionWorldSpaceCenter, info.Position, Health, this.CollisionBounds);
			Delete();
		}
		this.ProceduralHitReaction(info);
		//
		// Add a score to the killer
		//
		if (LifeState == LifeState.Dead && info.Attacker != null)
		{
			if (info.Attacker.Client != null && info.Attacker != this)
			{
				info.Attacker.Client.AddInt("kills");
			}
		}
        
	}

	public void SpeakSound(string sound, float pitch = 100)
	{
        if (IsServer)
        {
			using (Prediction.Off())
			{
				CurrentSound.Stop();
				CurrentSound = PlaySound(sound).SetPitch(HLUtils.CorrectPitch(pitch));
			}
			//SpeakSoundcl(sound, pitch);
		}
        
	}
    
	[ClientRpc]
	public void SpeakSoundcl(string sound, float pitch = 100)
	{
		//CurrentSound.Stop();
		CurrentSound = PlaySound(sound).SetPitch(HLUtils.CorrectPitch(pitch));
		Log.Info($"IsClient: {IsClient} IsServer: {IsServer}");
	}
    
	public override void OnKilled()
	{
		Tags.Add("debris");

		SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius(1, 8));
		if (LifeState == LifeState.Alive)
		{
			LifeState = LifeState.Dead;
			//Delete();
		}

		if (LastDamage.Flags.HasFlag(DamageFlags.Blast))
		{
			
		}
		else
		{
			//BecomeRagdollOnClient(Velocity, LastDamage.Flags, LastDamage.Position, LastDamage.Force, GetHitboxBone(LastDamage.HitboxIndex));
		}
	}

    bool IUse.OnUse(Entity user)
    {
        return OnUse(user);
    }

    bool IUse.IsUsable(Entity user)
    {
		return true;
    }
}
