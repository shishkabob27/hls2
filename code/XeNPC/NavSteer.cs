﻿
namespace XeNPC;

using Sandbox;
using System.Collections.Generic;
using XeNPC.Debug;
public class NavSteer
{
	protected NavPath Path { get; private set; }

	public NavSteer()
	{
		Path = new NavPath();
	}

	public virtual void Tick(Vector3 currentPosition)
	{
		using (Profile.Scope("Update Path"))
		{
			Path.Update(currentPosition, Target);
		}

		Output.Finished = Path.IsEmpty;

		if (Output.Finished)
		{
			Output.Direction = Vector3.Zero;
			return;
		}

		using (Profile.Scope("Update Direction"))
		{
			Output.Direction = Path.GetDirection(currentPosition);
		}

		var avoid = GetAvoidance(currentPosition, 512);
		if (!avoid.IsNearlyZero())
		{
			Output.Direction = (Output.Direction + avoid).Normal;
		}
	}

	Vector3 GetAvoidance(Vector3 position, float radius)
	{
		var center = position + Output.Direction * radius * 0.5f;
		 
		Vector3 avoidance = default;

		foreach (var ent in Entity.FindInSphere( position, radius)) //WHY NO BRUSH ENTITIES!!!!!!!!!!!!!???????????????
		{
			var objectRadius = 200.0f;
			//var draw = XeNPC.Debug.Draw.ForSeconds( 1 );
			//draw.WithColor( Color.Red.WithAlpha( 1 ) ).Circle( ent.Position, Vector3.Up, objectRadius * 0.25f );
			if (!(ent is ModelEntity) ) continue;
			//draw.WithColor( Color.Green.WithAlpha( 1 ) ).Circle( ent.Position, Vector3.Up, objectRadius * 0.25f );
			if ((ent is Weapon || ent is HLViewModel || ent is HLGib)) continue;  
			if (ent is ModelEntity) objectRadius = ((ent as ModelEntity).CollisionBounds.Size.Length * 0.5f) + 64;
			if (ent is BrushEntity ) objectRadius = ((ent as BrushEntity).CollisionBounds.Size.Length * 0.5f) + 64;
			objectRadius = objectRadius.Clamp( 128, 2048 );

			//draw.WithColor( Color.White.WithAlpha( 1 ) ).Circle( ent.Position, Vector3.Up, objectRadius * 0.25f );

			var delta = (position - ent.Position).WithZ(0);
			var closeness = delta.Length;
			if (closeness < 0.001f) continue;
			var a = (objectRadius );
			var thrust = ((a - closeness) / a).Clamp(0, 1);
			if (thrust <= 0) continue;

			//avoidance += delta.Cross( Output.Direction ).Normal * thrust * 2.5f;
			avoidance += delta.Normal * thrust * thrust;
		}

		return avoidance;
	}

	public virtual void DebugDrawPath()
	{
		using (Profile.Scope("Path Debug Draw"))
		{
			Path.DebugDraw(0.1f, 0.1f);
		}
	}

	public Vector3 Target { get; set; }

	public NavSteerOutput Output;


	public struct NavSteerOutput
	{
		public bool Finished;
		public Vector3 Direction;
	}
}
