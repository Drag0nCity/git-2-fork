using GameECS;
using UnityEngine;

namespace Game.GameEngine.Ecs
{
    public sealed class MoveStepSystem : IEcsFixedUpdate
    {
        private readonly EcsPool<MoveStepData> stepDataPool;
        private readonly EcsPool<MoveSpeedComponent> speedPool;
        private readonly EcsPool<RigidbodyComponent> rigidbodyPool;

        private readonly EcsEmitter<SmoothRotateEvent> rotateEmitter;

        private const float OBSTACLE_CHECK_DISTANCE = 0.25f;

        void IEcsFixedUpdate.FixedUpdate(int entity)
        {
            if (!this.stepDataPool.HasComponent(entity))
            {
                return;
            }

            ref var stepData =
                ref this.stepDataPool.GetComponent(entity);

            if (stepData.completed)
            {
                this.stepDataPool.RemoveComponent(entity);
                return;
            }

            Vector3 direction =
                this.GetSafeDirection(
                    entity,
                    stepData.direction
                );

            /*
             * Если ни одно направление не найдено,
             * не двигаем юнита в стену.
             */
            if (direction.sqrMagnitude <= 0.001f)
            {
                stepData.completed = true;
                return;
            }

            this.UpdatePosition(
                entity,
                direction
            );

            this.UpdateRotation(
                entity,
                direction
            );

            stepData.completed = true;
        }

        private Vector3 GetSafeDirection(
            int entity,
            Vector3 desiredDirection)
        {
            desiredDirection.y = 0f;

            if (desiredDirection.sqrMagnitude <= 0.001f)
            {
                return Vector3.zero;
            }

            desiredDirection.Normalize();

            ref var rigidbody =
                ref this.rigidbodyPool
                    .GetComponent(entity)
                    .value;

            /*
             * Сначала проверяем обычное движение.
             */
            if (!this.IsBlocked(
                rigidbody,
                desiredDirection
            ))
            {
                return desiredDirection;
            }

            /*
             * Впереди препятствие.
             *
             * Пробуем обойти его вправо.
             */
            Vector3 right =
                Quaternion.Euler(
                    0f,
                    45f,
                    0f
                ) * desiredDirection;

            if (!this.IsBlocked(
                rigidbody,
                right
            ))
            {
                return right.normalized;
            }

            /*
             * Если справа нельзя —
             * пробуем влево.
             */
            Vector3 left =
                Quaternion.Euler(
                    0f,
                    -45f,
                    0f
                ) * desiredDirection;

            if (!this.IsBlocked(
                rigidbody,
                left
            ))
            {
                return left.normalized;
            }

            /*
             * Если под углом 45 градусов
             * не получилось, пробуем полностью
             * повернуть вправо.
             */
            Vector3 right90 =
                Quaternion.Euler(
                    0f,
                    90f,
                    0f
                ) * desiredDirection;

            if (!this.IsBlocked(
                rigidbody,
                right90
            ))
            {
                return right90.normalized;
            }

            /*
             * И полностью влево.
             */
            Vector3 left90 =
                Quaternion.Euler(
                    0f,
                    -90f,
                    0f
                ) * desiredDirection;

            if (!this.IsBlocked(
                rigidbody,
                left90
            ))
            {
                return left90.normalized;
            }

            /*
             * Вокруг пройти не удалось.
             * Лучше остановиться, чем идти сквозь стену.
             */
            return Vector3.zero;
        }

        private bool IsBlocked(
            Rigidbody rigidbody,
            Vector3 direction)
        {
            return rigidbody.SweepTest(
                direction,
                out _,
                OBSTACLE_CHECK_DISTANCE,
                QueryTriggerInteraction.Ignore
            );
        }

        private void UpdatePosition(
            int entity,
            Vector3 direction)
        {
            ref var rigidbody =
                ref this.rigidbodyPool
                    .GetComponent(entity)
                    .value;

            ref var moveSpeed =
                ref this.speedPool
                    .GetComponent(entity)
                    .value;

            var moveStep =
                direction *
                moveSpeed *
                Time.fixedDeltaTime;

            var newPosition =
                rigidbody.position +
                moveStep;

            rigidbody.MovePosition(
                newPosition
            );
        }

        private void UpdateRotation(
            int entity,
            Vector3 direction)
        {
            this.rotateEmitter.SendEvent(
                entity,
                new SmoothRotateEvent
                {
                    direction = direction
                }
            );
        }
    }
}