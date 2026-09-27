using Game.GameEngine.Ecs;
using GameECS;

namespace SampleProject
{
    public sealed class CharacterAnimatorSystem : IEcsUpdate
    {
        private EcsPool<AnimatorComponent> animatorPool;

        private EcsPool<MoveStepData> moveStep;
        private EcsPool<HitDuration> attackPool;
        private EcsPool<GatherDuration> gatherPool;
        private EcsPool<HitPointsComponent> hitPointsPool;
        private EcsPool<AttackTarget> attackTargetPool;

        void IEcsUpdate.Update(int entity)
        {
            ref var animator =
                ref this.animatorPool.GetComponent(entity).value;

            var animatorState =
                this.ResolveState(entity);

            animator.ChangeState(animatorState);
        }

        private int ResolveState(int entity)
        {
            if (this.hitPointsPool.HasComponent(entity))
            {
                ref var hitPoints =
                    ref this.hitPointsPool.GetComponent(entity);

                if (hitPoints.current <= 0)
                {
                    return AnimatorStateId.DEATH;
                }
            }

            if (this.attackPool.HasComponent(entity))
            {
                if (!this.IsAttackTargetAlive(entity))
                {
                    return AnimatorStateId.IDLE;
                }

                return AnimatorStateId.ATTACK;
            }

            if (this.gatherPool.HasComponent(entity))
            {
                return AnimatorStateId.GATHERING;
            }

            if (this.moveStep.HasComponent(entity))
            {
                return AnimatorStateId.MOVE;
            }

            return AnimatorStateId.IDLE;
        }

        private bool IsAttackTargetAlive(int entity)
        {
            
            if (!this.attackTargetPool.HasComponent(entity))
            {
                return false;
            }

            ref var attackTarget =
                ref this.attackTargetPool.GetComponent(entity);

            int targetId =
                attackTarget.targetId;

            if (!this.hitPointsPool.HasComponent(targetId))
            {
                return false;
            }

            ref var targetHitPoints =
                ref this.hitPointsPool.GetComponent(targetId);

            return targetHitPoints.current > 0;
        }
    }
}