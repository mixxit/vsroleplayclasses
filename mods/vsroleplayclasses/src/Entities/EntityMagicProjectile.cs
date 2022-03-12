using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using vsroleplayclasses.src.Systems;

namespace vsroleplayclasses.src.Entities
{
    public class EntityMagicProjectile : Entity
    {
        bool beforeCollided;
        bool stuck;
        public static SimpleParticleProperties spellParticles;
        Random rand;

        long msLaunch;
        long msCollide;

        Vec3d motionBeforeCollide = new Vec3d();

        CollisionTester collTester = new CollisionTester();

        public Entity FiredBy;
        public float Weight = 0.0f;
        public float DropOnImpactChance = 0f;

        Cuboidf collisionTestBox;

        EntityPartitioning ep;


        public override bool ApplyGravity
        {
            get { return false; }
        }

        public override bool IsInteractable
        {
            get { return false; }
        }

        public long AbilityId { get; internal set; }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            msLaunch = World.ElapsedMilliseconds;

            int randSeed = 0;
            if (this.EntityId > int.MaxValue)
                randSeed = int.MaxValue;
            else
                randSeed = (int)this.EntityId;

            rand = new Random(randSeed);
            collisionTestBox = CollisionBox.Clone().OmniGrowBy(0.05f);

            if (api.Side == EnumAppSide.Server)
            {
                GetBehavior<EntityBehaviorPassivePhysics>().OnPhysicsTickCallback = onPhysicsTickCallback;

                ep = api.ModLoader.GetModSystem<EntityPartitioning>();
            }

            spellParticles = new SimpleParticleProperties(1, 1, 1, new Vec3d(), new Vec3d(), new Vec3f(), new Vec3f()); ;


        }

        private void onPhysicsTickCallback(float dtFac)
        {
            if (ShouldDespawn || !Alive) return;
            if (World is IClientWorldAccessor || World.ElapsedMilliseconds <= msCollide + 500) return;
            if (ServerPos.Motion.X == 0 && ServerPos.Motion.Y == 0 && ServerPos.Motion.Z == 0) return;  //don't do damage if stuck in ground

            Cuboidd projectileBox = CollisionBox.ToDouble().Translate(ServerPos.X, ServerPos.Y, ServerPos.Z);

            if (ServerPos.Motion.X < 0) projectileBox.X1 += ServerPos.Motion.X * dtFac;
            else projectileBox.X2 += ServerPos.Motion.X * dtFac;
            if (ServerPos.Motion.Y < 0) projectileBox.Y1 += ServerPos.Motion.Y * dtFac;
            else projectileBox.Y2 += ServerPos.Motion.Y * dtFac;
            if (ServerPos.Motion.Z < 0) projectileBox.Z1 += ServerPos.Motion.Z * dtFac;
            else projectileBox.Z2 += ServerPos.Motion.Z * dtFac;

            ep.WalkEntities(ServerPos.XYZ, 5f, (e) => {
                if (e.EntityId == this.EntityId || !e.IsInteractable) return false;
                Cuboidd eBox = e.CollisionBox.ToDouble().Translate(e.ServerPos.X, e.ServerPos.Y, e.ServerPos.Z);
                if (eBox.IntersectsOrTouches(projectileBox))
                {
                    if (FiredBy != null && e.EntityId == FiredBy.EntityId && World.ElapsedMilliseconds - msLaunch < 500)
                    {
                        return true;
                    }

                    impactOnEntity(e);

                    return false;
                }
                return true;
            });
        }


        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
            if (ShouldDespawn) return;

            EntityPos pos = SidedPos;

            stuck = Collided || collTester.IsColliding(World.BlockAccessor, collisionTestBox, pos.XYZ);
            double impactSpeed = Math.Max(motionBeforeCollide.Length(), pos.Motion.Length());

            if (stuck)
            {
                IsColliding(pos, impactSpeed);
                return;
            }

            if (TryAttackEntity(impactSpeed))
            {
                return;
            }

            SpawnSpellParticles();

            beforeCollided = false;
            motionBeforeCollide.Set(pos.Motion.X, pos.Motion.Y, pos.Motion.Z);
            SetRotation();
        }


        public override void OnCollided()
        {
            EntityPos pos = SidedPos;

            IsColliding(SidedPos, Math.Max(motionBeforeCollide.Length(), pos.Motion.Length()));
            motionBeforeCollide.Set(pos.Motion.X, pos.Motion.Y, pos.Motion.Z);
        }


        private void IsColliding(EntityPos pos, double impactSpeed)
        {
            pos.Motion.Set(0, 0, 0);

            if (!beforeCollided && World is IServerWorldAccessor && World.ElapsedMilliseconds > msCollide + 500)
            {
                if (impactSpeed >= 0.07)
                {
                    World.PlaySoundAt(new AssetLocation("sounds/arrow-impact"), this, null, false, 32);
                    WatchedAttributes.MarkAllDirty();

                    Die();
                }

                TryAttackEntity(impactSpeed);

                msCollide = World.ElapsedMilliseconds;

                beforeCollided = true;
            }


        }

        private void SpawnSpellParticles()
        {
            var particleColours = this.GetParticleColours();
            if (particleColours == null)
                return;

            spellParticles.MinPos = this.Pos.XYZ + new Vec3d(-this.Properties.Client.Size / 2, -this.Properties.Client.Size / 2, -this.Properties.Client.Size / 2);
            spellParticles.AddPos = new Vec3d(this.Properties.Client.Size, this.Properties.Client.Size, this.Properties.Client.Size);

            spellParticles.MinVelocity = new Vec3f(0.1f, 0, 0.1f);

            spellParticles.GravityEffect = -0.01f;
            spellParticles.WindAffected = false ;

            spellParticles.MinSize = 1.0f;
            spellParticles.MaxSize = 1.0f;
            spellParticles.SizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, -2);

            spellParticles.MinQuantity = 4;
            spellParticles.AddQuantity = 25;

            spellParticles.LifeLength = 1.5f;
            spellParticles.addLifeLength = 0.5f;

            spellParticles.ShouldDieInLiquid = true;

            // have to swap them round for particle system
            spellParticles.Color = ColorUtil.ColorFromRgba(particleColours[2], particleColours[1], particleColours[0], rand.Next(100, 255));
            spellParticles.OpacityEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEARREDUCE, 255);
            spellParticles.BlueEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEARREDUCE, 255);
            spellParticles.GreenEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEARREDUCE, 255);
            spellParticles.RedEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEARREDUCE, 255);

            spellParticles.VertexFlags = rand.Next(150, 255);

            spellParticles.ParticleModel = EnumParticleModel.Quad;

            World.SpawnParticles(spellParticles);
        }

        private int[] GetParticleColours()
        {
            var red = (int)this.WatchedAttributes.GetLong("particle_red");
            var green = (int)this.WatchedAttributes.GetLong("particle_green");
            var blue = (int)this.WatchedAttributes.GetLong("particle_blue");
            return new int[3] { red, green, blue };
        }

        bool TryAttackEntity(double impactSpeed)
        {
            if (World is IClientWorldAccessor || World.ElapsedMilliseconds <= msCollide + 250) return false;
            if (impactSpeed <= 0.01) return false;

            EntityPos pos = SidedPos;

            Cuboidd projectileBox = CollisionBox.ToDouble().Translate(ServerPos.X, ServerPos.Y, ServerPos.Z);

            // We give it a bit of extra leeway of 50% because physics ticks can run twice or 3 times in one game tick 
            if (ServerPos.Motion.X < 0) projectileBox.X1 += 1.5 * ServerPos.Motion.X;
            else projectileBox.X2 += 1.5 * ServerPos.Motion.X;
            if (ServerPos.Motion.Y < 0) projectileBox.Y1 += 1.5 * ServerPos.Motion.Y;
            else projectileBox.Y2 += 1.5 * ServerPos.Motion.Y;
            if (ServerPos.Motion.Z < 0) projectileBox.Z1 += 1.5 * ServerPos.Motion.Z;
            else projectileBox.Z2 += 1.5 * ServerPos.Motion.Z;

            Entity entity = World.GetNearestEntity(ServerPos.XYZ, 5f, 5f, (e) => {
                if (e.EntityId == this.EntityId || !e.IsInteractable) return false;

                if (FiredBy != null && e.EntityId == FiredBy.EntityId && World.ElapsedMilliseconds - msLaunch < 500)
                {
                    return false;
                }

                Cuboidd eBox = e.CollisionBox.ToDouble().Translate(e.ServerPos.X, e.ServerPos.Y, e.ServerPos.Z);

                return eBox.IntersectsOrTouches(projectileBox);
            });

            if (entity != null)
            {
                impactOnEntity(entity);
                return true;
            }


            return false;
        }


        private void impactOnEntity(Entity entity)
        {
            if (!Alive) return;

            EntityPos pos = SidedPos;

            IServerPlayer fromPlayer = null;
            if (FiredBy is EntityPlayer)
            {
                fromPlayer = (FiredBy as EntityPlayer).Player as IServerPlayer;
            }

            msCollide = World.ElapsedMilliseconds;
            var abilitiesMod = World.Api.ModLoader.GetModSystem<SystemAbilities>();
            if (abilitiesMod != null)
            {
                var ability = abilitiesMod.GetAbilityById(AbilityId);
                if (ability != null)
                {
                    ability.OnSpellCollidedEntity(FiredBy, entity);
                }
            }


            pos.Motion.Set(0, 0, 0);
            Die();

        }


        public virtual void SetRotation()
        {
            EntityPos pos = (World is IServerWorldAccessor) ? ServerPos : Pos;

            double speed = pos.Motion.Length();

            if (speed > 0.01)
            {
                pos.Pitch = 0;
                pos.Yaw =
                    GameMath.PI + (float)Math.Atan2(pos.Motion.X / speed, pos.Motion.Z / speed)
                    + GameMath.Cos((World.ElapsedMilliseconds - msLaunch) / 200f) * 0.03f
                ;
                pos.Roll =
                    -(float)Math.Asin(GameMath.Clamp(-pos.Motion.Y / speed, -1, 1))
                    + GameMath.Sin((World.ElapsedMilliseconds - msLaunch) / 200f) * 0.03f
                ;
            }
        }


        public override bool CanCollect(Entity byEntity)
        {
            return false;
        }

        public override void OnCollideWithLiquid()
        {
            base.OnCollideWithLiquid();
        }
    }
}
