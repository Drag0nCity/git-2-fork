using System.Collections.Generic;
using Game.GameEngine.Ecs;
using UnityEngine;

namespace SampleProject
{
    public static class UnitGroupCommandController
    {
        private static Entity currentEnemy;

        public static void Move(
            IReadOnlyList<Entity> units,
            Vector3 destination)
        {
            currentEnemy = null;

            for (int i = 0; i < units.Count; i++)
            {
                Entity unit = units[i];

                if (unit == null ||
                    !unit.gameObject.activeInHierarchy)
                {
                    continue;
                }

                unit.SetData(new CommandRequest
                {
                    type = CommandType.MOVE_TO_POSITION,
                    args = destination,
                    status = CommandStatus.IDLE
                });
            }

            Debug.Log(
                $"Move command sent to {units.Count} units."
            );
        }

        public static void Patrol(
            IReadOnlyList<Entity> units)
        {
            currentEnemy = null;

            GameObject[] points =
                GameObject.FindGameObjectsWithTag(
                    "PatrolPoint"
                );

            if (points.Length < 5)
            {
                Debug.LogWarning(
                    $"Need at least 5 patrol points. " +
                    $"Found: {points.Length}"
                );

                return;
            }

            List<Vector3> patrolPoints =
                new List<Vector3>();

            for (int i = 0; i < points.Length; i++)
            {
                patrolPoints.Add(
                    points[i].transform.position
                );
            }

            for (int i = 0; i < units.Count; i++)
            {
                Entity unit = units[i];

                if (unit == null ||
                    !unit.gameObject.activeInHierarchy)
                {
                    continue;
                }

                unit.SetData(new CommandRequest
                {
                    type = CommandType.PATROL_BY_POINTS,
                    args = patrolPoints,
                    status = CommandStatus.IDLE
                });
            }

            Debug.Log(
                $"Patrol command sent to {units.Count} units."
            );
        }

        public static void Attack(
            IReadOnlyList<Entity> units,
            Entity enemy)
        {
            if (enemy == null ||
                !enemy.gameObject.activeInHierarchy)
            {
                return;
            }

            currentEnemy = enemy;

            for (int i = 0; i < units.Count; i++)
            {
                Entity unit = units[i];

                if (unit == null ||
                    !unit.gameObject.activeInHierarchy)
                {
                    continue;
                }

                unit.SetData(new CommandRequest
                {
                    type = CommandType.ATTACK_TARGET,
                    args = enemy,
                    status = CommandStatus.IDLE
                });
            }

            Debug.Log(
                $"Enemy detected. Attack command sent to " +
                $"{units.Count} units."
            );
        }

        public static bool HasCurrentEnemy()
        {
            if (currentEnemy == null)
            {
                return false;
            }

            if (!currentEnemy.gameObject.activeInHierarchy)
            {
                currentEnemy = null;
                return false;
            }

            return true;
        }

        public static void ClearEnemy()
        {
            currentEnemy = null;
        }
    }
}