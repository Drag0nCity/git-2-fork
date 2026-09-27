using System.Collections.Generic;
using Game.GameEngine.Ecs;
using UnityEngine;

namespace SampleProject
{
    public sealed class UnitPool : MonoBehaviour
    {
        [SerializeField]
        private Transform unitsRoot;

        private readonly List<Entity> units =
            new List<Entity>();

        public IReadOnlyList<Entity> Units =>
            this.units;

        public int Count =>
            this.units.Count;

        private void Awake()
        {
            if (this.unitsRoot == null)
            {
                this.unitsRoot = this.transform;
            }

            this.CollectUnits();

            Debug.Log(
                $"Unit Pool initialized. Units: {this.units.Count}"
            );
        }

        private void CollectUnits()
        {
            this.units.Clear();

            Entity[] foundUnits =
                this.unitsRoot.GetComponentsInChildren<Entity>(
                    true
                );

            for (int i = 0; i < foundUnits.Length; i++)
            {
                Entity unit = foundUnits[i];

                if (unit == null)
                {
                    continue;
                }

                this.units.Add(unit);
            }
        }

        public Entity GetUnit(int index)
        {
            if (index < 0 || index >= this.units.Count)
            {
                Debug.LogWarning(
                    $"UnitPool: invalid index {index}."
                );

                return null;
            }

            Entity unit = this.units[index];

            if (unit == null)
            {
                return null;
            }

            unit.gameObject.SetActive(true);

            return unit;
        }

        public void ReleaseUnit(Entity unit)
        {
            if (unit == null)
            {
                return;
            }

            if (!this.units.Contains(unit))
            {
                return;
            }

            unit.gameObject.SetActive(false);
        }
    }
}