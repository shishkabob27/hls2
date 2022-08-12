using System;
using System.Collections.Generic;

namespace Sandbox
{
	public partial class GlassShard
	{
		private struct ShatterSpoke
		{
			public Vector2 OuterPos;
			public Vector2 IntersectionPos;
			public int IntersectsEdgeIndex;
			public float Length;
		};

		private struct ShatterEdgeSegment
		{
			public ShatterEdgeSegment( Vector2 start, Vector2 end )
			{
				Start = start;
				End = end;
			}

			public Vector2 Start;
			public Vector2 End;
		};

		private bool GenerateShatterShards()
		{
			var shatterType = ShatterTypes[ShatterTypeIndex];
			var shouldFreeze = IsFrozen() && ParentPanel.Constraint == ShatterGlass.ShatterGlassConstraint.StaticEdges;
			var shatterPos = TransformPosPanelToWorld( StressPosition );
			var shatterVel = StressVelocity;
			var didShatter = false;

			float spokeLength = ParentPanel.PanelSize.x + ParentPanel.PanelSize.y;
			int numSpokes = Math.Max( 3, Rand.Int( shatterType.SpokesMin, shatterType.SpokesMax ) );
			var spokes = new List<ShatterSpoke>();

			float segmentRange = (MathF.PI * 2.0f) / numSpokes;
			float limitedRangeDeviation = Math.Min( segmentRange, (MathF.PI * 2.0f) * (1.0f / 3.0f) );

			for ( int i = 0; i < numSpokes; i++ )
			{
				float spokeRadians = (i * segmentRange) + (Rand.Float( limitedRangeDeviation * -0.5f, limitedRangeDeviation * 0.5f ) * 0.9f);

				var spoke = new ShatterSpoke
				{
					OuterPos = new Vector2( StressPosition.x + spokeLength * MathF.Cos( spokeRadians ), StressPosition.y + spokeLength * MathF.Sin( spokeRadians ) ),
					IntersectionPos = Vector2.Zero,
					IntersectsEdgeIndex = -1,
					Length = -1
				};

				spokes.Insert( 0, spoke );
			}

			var edgeSegments = new List<ShatterEdgeSegment>();

			for ( int i = 0; i < PanelVertices.Count; i++ )
			{
				var v1 = PanelVertices[i];
				var v2 = PanelVertices[i < PanelVertices.Count - 1 ? i + 1 : 0];

				edgeSegments.Add( new ShatterEdgeSegment( v1, v2 ) );
			}

			for ( int spokeIndex = 0; spokeIndex < spokes.Count; spokeIndex++ )
			{
				for ( int edgeIndex = 0; edgeIndex < edgeSegments.Count; edgeIndex++ )
				{
					if ( LineIntersect( edgeSegments[edgeIndex].Start, edgeSegments[edgeIndex].End, spokes[spokeIndex].OuterPos, StressPosition, out var point ) )
					{
						var spoke = spokes[spokeIndex];
						spoke.IntersectionPos = point;
						spoke.IntersectsEdgeIndex = edgeIndex;
						spoke.Length = Vector2.DistanceBetween( StressPosition, spoke.IntersectionPos );

						spokes[spokeIndex] = spoke;

						break;
					}
				}
			}

			var centerHoleVertices = new List<Vector2>();

			for ( int spokeIndex = 0; spokeIndex < spokes.Count; spokeIndex++ )
			{
				int nextSpokeIndex = spokeIndex < spokes.Count - 1 ? spokeIndex + 1 : 0;
				int currentEdgeIndex = spokes[spokeIndex].IntersectsEdgeIndex;
				int nextEdgeIndex = spokes[nextSpokeIndex].IntersectsEdgeIndex;

				if ( nextSpokeIndex < 0 || currentEdgeIndex < 0 || nextEdgeIndex < 0 )
					continue;

				if ( spokes[spokeIndex].Length < 0.5f && spokes[nextSpokeIndex].Length < 0.5f )
					continue;

				var subShard = ParentPanel.CreateNewShard( this );
				subShard.AddVertex( StressPosition );
				subShard.AddVertex( spokes[spokeIndex].IntersectionPos );

				if ( currentEdgeIndex == nextEdgeIndex )
				{
					subShard.AddVertex( spokes[nextSpokeIndex].IntersectionPos );
				}
				else
				{
					for ( int i = 0; i < 32 && currentEdgeIndex != nextEdgeIndex; i++ )
					{
						subShard.AddVertex( edgeSegments[currentEdgeIndex].End );

						currentEdgeIndex = currentEdgeIndex < edgeSegments.Count - 1 ? currentEdgeIndex + 1 : 0;
					}

					subShard.AddVertex( spokes[nextSpokeIndex].IntersectionPos );
				}

				Assert.True( subShard.PanelVertices.Count >= 3 );

				var tipPoint1 = Vector2.Lerp( subShard.PanelVertices[0], subShard.PanelVertices[1], Rand.Float( shatterType.TipScaleMin, shatterType.TipScaleMax ) );
				var tipPoint2 = Vector2.Lerp( subShard.PanelVertices[0], subShard.PanelVertices[^1], Rand.Float( shatterType.TipScaleMin, shatterType.TipScaleMax ) );

				centerHoleVertices.Add( Vector2.Lerp( tipPoint1, tipPoint2, 0.5f ) );

				if ( shatterType.TipSpawnChance > 0 && Rand.Float( 0, shatterType.TipSpawnChance ) < 1.0f )
				{
					var tipShard = ParentPanel.CreateNewShard( this );
					tipShard.AddVertex( subShard.PanelVertices[0] );
					tipShard.AddVertex( tipPoint1 );
					tipShard.AddVertex( tipPoint2 );
					tipShard.ScaleVerts( shatterType.TipScale );
					tipShard.TryGenerateShardModel( shatterPos, shatterVel, true );

					didShatter = true;
				}

				if ( shatterType.SecondTipSpawnChance > 0 && Rand.Float( 0, shatterType.SecondTipSpawnChance ) < 1.0f )
				{
					var secondTipPoint1 = Vector2.Lerp( tipPoint1, subShard.PanelVertices[1], Rand.Float( 0.2f, 0.5f ) );
					var secondTopPoint2 = Vector2.Lerp( tipPoint2, subShard.PanelVertices[^1], Rand.Float( 0.2f, 0.5f ) );

					var tipShard = ParentPanel.CreateNewShard( this );
					tipShard.AddVertex( tipPoint1 );
					tipShard.AddVertex( secondTipPoint1 );
					tipShard.AddVertex( secondTopPoint2 );
					tipShard.AddVertex( tipPoint2 );
					tipShard.ScaleVerts( shatterType.SecondShardScale );
					tipShard.TryGenerateShardModel( shatterPos, shatterVel, false );

					tipPoint1 = secondTipPoint1;
					tipPoint2 = secondTopPoint2;

					didShatter = true;
				}

				subShard.PanelVertices.RemoveAt( 0 );
				subShard.PanelVertices.Insert( 0, tipPoint1 );
				subShard.PanelVertices.Add( tipPoint2 );

				if ( (tipPoint1 - tipPoint2).LengthSquared > 9.0f )
				{
					var vecBetweenCorners = Vector2.Lerp( Vector2.Lerp( tipPoint1, tipPoint2, Rand.Float( 0.4f, 0.6f ) ), StressPosition, Rand.Float( 0.1f, 0.3f ) );
					subShard.PanelVertices.Add( vecBetweenCorners );
				}

				subShard.ScaleVerts( shatterType.ShardScale );
				subShard.TryGenerateShardModel( shatterPos, shatterVel, !shouldFreeze, shouldFreeze );

				didShatter = true;
			}

			if ( shatterType.HasCenterChunk && centerHoleVertices.Count > 2 )
			{
				var pShardCenter = ParentPanel.CreateNewShard( this );

				foreach ( var vertex in centerHoleVertices )
				{
					pShardCenter.AddVertex( vertex );
				}

				pShardCenter.ScaleVerts( shatterType.CenterChunkScale );
				pShardCenter.TryGenerateShardModel( shatterPos, shatterVel, true );

				didShatter = true;
			}

			return didShatter;
		}

		public Model GenerateShardModel( out Vector2 localPanelSpaceOrigin )
		{
			localPanelSpaceOrigin = Vector2.Zero;

			if ( Desc == null )
				return null;

			if ( Desc.PanelVertices == null )
				return null;

			var renderData = new RenderData
			{
				Average = Desc.GetPanelVertexAverage()
			};

			var panelVertices = Desc.PanelVertices;
			localPanelSpaceOrigin = renderData.Average;
			renderData.LocalPanelSpaceOrigin = new Vector3( renderData.Average.x, renderData.Average.y, 0.0f );

			renderData.Init( panelVertices.Length );
			float halfThickness = Desc.HalfThickness;

			renderData.VertexPositions.Add( new Vector3( renderData.Average.x, renderData.Average.y, halfThickness ) );
			for ( int i = 0; i < renderData.FaceVertexCount - 1; i++ )
			{
				renderData.VertexPositions.Add( new Vector3( panelVertices[i].x, panelVertices[i].y, halfThickness ) );
			}

			renderData.VertexPositions.Add( new Vector3( renderData.Average.x, renderData.Average.y, -halfThickness ) );
			for ( int i = 0; i < renderData.FaceVertexCount - 1; i++ )
			{
				renderData.VertexPositions.Add( new Vector3( panelVertices[i].x, panelVertices[i].y, -halfThickness ) );
			}

			var modelBuilder = new ModelBuilder();
			{
				var hullPos = new Vector3[renderData.VertexPositions.Count];
				for ( int i = 0; i < hullPos.Length; i++ )
				{
					hullPos[i] = renderData.VertexPositions[i] - renderData.LocalPanelSpaceOrigin;
				}

				modelBuilder.AddCollisionHull( hullPos );
			}

			// We don't need a render mesh on the server
			if ( !IsServer )
			{
				for ( int i = 0; i < renderData.EdgeQuadCount; i++ )
				{
					int v0 = i;
					int v1 = (i < renderData.EdgeQuadCount - 1) ? i + 1 : 0;

					renderData.VertexPositions.Add( new Vector3( panelVertices[v0].x, panelVertices[v0].y, -halfThickness ) );
					renderData.VertexPositions.Add( new Vector3( panelVertices[v1].x, panelVertices[v1].y, -halfThickness ) );
					renderData.VertexPositions.Add( new Vector3( panelVertices[v1].x, panelVertices[v1].y, halfThickness ) );
					renderData.VertexPositions.Add( new Vector3( panelVertices[v0].x, panelVertices[v0].y, halfThickness ) );
				}

				renderData.EdgeVerticesStart = renderData.TotalShardVertices - renderData.EdgeVertexCount;

				Assert.AreEqual( renderData.VertexPositions.Count, renderData.TotalShardVertices );

				modelBuilder.AddMesh( CreateMeshForShard( renderData ) );
			}

			var surf = "glass";
			if ( IsABigPiece() ) surf = "glass.pane";
			if ( CalculateArea() < 20 ) surf = "glass.shard";
			modelBuilder.WithSurface( surf );

			// Cubic Inches * Glass's volume to weight ratio
			modelBuilder.WithMass( CalculateArea() * ParentPanel.Thickness * 0.04226f );

			return modelBuilder.Create();
		}
	}
}
