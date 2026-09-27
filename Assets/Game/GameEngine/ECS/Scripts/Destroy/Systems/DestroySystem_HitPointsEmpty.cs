using GameECS;

namespace Game.GameEngine.Ecs
{
    public sealed class DestroySystem_HitPointsEmpty : IEcsFixedUpdate
    {
        private readonly EcsPool<HitPointsComponent> hitPointsPool;
        private readonly EcsPool<AnimatorComponent> animatorPool;
        private readonly EcsEmitter<DestroyEvent> destroyEmitter;

        void IEcsFixedUpdate.FixedUpdate(int entity)
        {
            if (!this.hitPointsPool.HasComponent(entity))
            {
                return;
            }

            ref var hitPoints =
                ref this.hitPointsPool.GetComponent(entity);

            if (hitPoints.current > 0)
            {
                return;
            }

            if (!this.animatorPool.HasComponent(entity))
            {
                this.destroyEmitter.SendEvent(
                    entity,
                    new DestroyEvent()
                );

                return;
            }

            ref var animator =
                ref this.animatorPool.GetComponent(entity);

            if (animator.value == null)
            {
                this.destroyEmitter.SendEvent(
                    entity,
                    new DestroyEvent()
                );

                return;
            }

            if (!animator.value.IsAnimationFinished(
                AnimatorStateId.DEATH))
            {
                return;
            }

            this.destroyEmitter.SendEvent(
                entity,
                new DestroyEvent()
            );
        }
    }
}