
using static Sandbox.Event;

namespace Sandbox
{
	[Library]
	public class HLDuck : BaseNetworkable
	{
		public BasePlayerController Controller;

		public bool IsActive; // replicate

		public HLDuck( BasePlayerController controller )
		{
			Controller = controller;
		}

        public virtual void PreTick() 
		{

            bool wants = Input.Down( InputButton.Duck );

			if ( wants != IsActive ) 
			{
				if ( wants ) TryDuck();
				else TryUnDuck();
			}

            if ( IsActive )
			{
				Controller.SetTag( "ducked" );
                Controller.EyeLocalPosition *= 0.5f;
            }
			

        }

		protected virtual void TryDuck()
		{
			IsActive = true;
            if (Controller.GroundEntity == null)
			{

                Controller.Position -= originalMins.WithX(0).WithY(0) - newMaxs.WithX(0).WithY(0);
            } 
		}

		protected virtual void TryUnDuck()
		{
			
			var pm = Controller.TraceBBox( Controller.Position, Controller.Position, originalMins, originalMaxs );

			if (Controller.GroundEntity == null)
			{
                pm = Controller.TraceBBox(Controller.Position + originalMins.WithX(0).WithY(0) - newMaxs.WithX(0).WithY(0), Controller.Position + originalMins.WithX(0).WithY(0) - newMaxs.WithX(0).WithY(0), originalMins, originalMaxs);
            }
			if ( pm.StartedSolid ) return;
            IsActive = false;
            if (Controller.GroundEntity == null)
            {
                Controller.Position += originalMins.WithX(0).WithY(0) - newMaxs.WithX(0).WithY(0);
            }
        }

		// Uck, saving off the bbox kind of sucks
		// and we should probably be changing the bbox size in PreTick
		Vector3 originalMins;
		Vector3 originalMaxs;
        Vector3 newMaxs;

        public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
		{
			originalMins = mins;
			originalMaxs = maxs;

			if ( IsActive )
			{

                maxs = maxs.WithZ(36 * scale);

                newMaxs = maxs;
            }

        }

		//
		// Coudl we do this in a generic callback too?
		//
		public virtual float GetWishSpeed()
		{
			if ( !IsActive ) return -1;
			return 64.0f;
		}
	}
}
