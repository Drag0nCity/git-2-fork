using System.Collections.Generic;
using Game.GameEngine.Ecs;
using UnityEngine;

namespace SampleProject
{
    public sealed class UnitEnemyDetection : MonoBehaviour
    {
        [SerializeField]
        private float detectionRadius = 8f;

        [SerializeField]
        private LayerMask enemyLayer;

        [SerializeField]
        private UnitSelectionController selectionController;

        private void Awake()
        {
            if (this.selectionController == null)
            {
                this.selectionController =
                    this.GetComponent<UnitSelectionController>();
            }
        }

        private void Update()
        {
            this.CheckForEnemies();
        }

        private void CheckForEnemies()
        {
            IReadOnlyList<Entity> selectedUnits =
                this.selectionController.SelectedUnits;

            if (selectedUnits.Count == 0)
            {
                return;
            }

            if (UnitGroupCommandController.HasCurrentEnemy())
            {
                return;
            }

            for (int i = 0;
                     i < selectedUnits.Count;
                     i++)
            {
                Entity unit = selectedUnits[i];

                if (unit == null ||
                    !unit.gameObject.activeInHierarchy)
                {
                    continue;
                }

                Collider[] enemies =
                    Physics.OverlapSphere(
                        unit.transform.position,
                        this.detectionRadius,
                        this.enemyLayer
                    );

                for (int j = 0;
                         j < enemies.Length;
                         j++)
                {
                    Entity enemy =
                        enemies[j]
                            .GetComponentInParent<Entity>();

                    if (enemy == null)
                    {
                        continue;
                    }

                    if (!enemy.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    UnitGroupCommandController.Attack(
                        selectedUnits,
                        enemy
                    );

                    return;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position,
                this.detectionRadius
            );
        }
    }
}