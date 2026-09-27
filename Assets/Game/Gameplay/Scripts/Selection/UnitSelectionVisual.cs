using System.Collections.Generic;
using Game.GameEngine.Ecs;
using UnityEngine;

namespace SampleProject
{
    public static class UnitSelectionVisual
    {
        private const float RADIUS = 0.6f;
        private const float HEIGHT = 0.03f;

        private static readonly Dictionary<Entity, GameObject>
            visuals =
                new Dictionary<Entity, GameObject>();

        public static void Create(Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            if (visuals.ContainsKey(entity))
            {
                return;
            }

            GameObject visual =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cylinder
                );

            visual.name = "Selection";

            visual.transform.SetParent(
                entity.transform
            );

            visual.transform.localPosition =
                new Vector3(
                    0f,
                    HEIGHT,
                    0f
                );

            visual.transform.localRotation =
                Quaternion.identity;

            visual.transform.localScale =
                new Vector3(
                    RADIUS,
                    0.01f,
                    RADIUS
                );

            Collider collider =
                visual.GetComponent<Collider>();

            if (collider != null)
            {
                Object.Destroy(collider);
            }

            Renderer renderer =
                visual.GetComponent<Renderer>();

            if (renderer != null)
            {
                Material material =
                    new Material(
                        Shader.Find("Standard")
                    );

                material.color = Color.green;

                renderer.material = material;
            }

            visuals.Add(
                entity,
                visual
            );
        }

        public static void Clear(
            IReadOnlyList<Entity> units)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Entity entity = units[i];

                if (entity == null)
                {
                    continue;
                }

                if (visuals.TryGetValue(
                    entity,
                    out GameObject visual))
                {
                    if (visual != null)
                    {
                        Object.Destroy(visual);
                    }
                }
            }

            visuals.Clear();
        }
    }
}