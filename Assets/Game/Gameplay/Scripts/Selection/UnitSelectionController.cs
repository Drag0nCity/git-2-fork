using System.Collections.Generic;
using Game.GameEngine.Ecs;
using SampleProject.ResourceObject;
using UnityEngine;
using Entities;

namespace SampleProject
{
    public sealed class UnitSelectionController : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private float groundY = 0f;

        private readonly List<Entity> selectedUnits =
            new List<Entity>();

        public IReadOnlyList<Entity> SelectedUnits =>
            this.selectedUnits;

        private void Awake()
        {
            if (this.mainCamera == null)
            {
                this.mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.SelectUnit();
            }

            if (Input.GetMouseButtonDown(1))
            {
                this.ExecuteCommand();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                this.PatrolSelectedUnits();
            }
        }

        private void SelectUnit()
        {
            Ray ray =
                this.mainCamera.ScreenPointToRay(
                    Input.mousePosition
                );

            if (!Physics.Raycast(
                ray,
                out RaycastHit hit))
            {
                this.ClearSelection();
                return;
            }

            CharacterEntity character =
                hit.collider.GetComponentInParent<CharacterEntity>();

            if (character == null)
            {
                this.ClearSelection();
                return;
            }

            Entity entity =
                character.GetComponent<Entity>();

            if (entity == null)
            {
                return;
            }

            if (this.selectedUnits.Contains(entity))
            {
                return;
            }

            this.selectedUnits.Add(entity);

            UnitSelectionVisual.Create(entity);

            Debug.Log(
                $"Selected units: {this.selectedUnits.Count}"
            );
        }

        private void ExecuteCommand()
        {
            if (this.selectedUnits.Count == 0)
            {
                return;
            }

            Ray ray =
                this.mainCamera.ScreenPointToRay(
                    Input.mousePosition
                );

            if (Physics.Raycast(
                ray,
                out RaycastHit hit))
            {
                ResourceEntity resource =
                    hit.collider.GetComponentInParent<ResourceEntity>();

                if (resource != null)
                {
                    this.GatherResource(resource);
                    return;
                }

                UnitGroupCommandController.Move(
                    this.selectedUnits,
                    hit.point
                );

                return;
            }

          
            Plane groundPlane =
                new Plane(
                    Vector3.up,
                    new Vector3(
                        0f,
                        this.groundY,
                        0f
                    )
                );

            if (groundPlane.Raycast(
                ray,
                out float distance))
            {
                Vector3 destination =
                    ray.GetPoint(distance);

                UnitGroupCommandController.Move(
                    this.selectedUnits,
                    destination
                );

                Debug.Log(
                    $"Move command to empty ground: {destination}"
                );
            }
        }

        private void GatherResource(
            ResourceEntity resource)
        {
            if (resource == null)
            {
                return;
            }

            Entity resourceEntity =
                resource.GetComponent<Entity>();

            if (resourceEntity == null)
            {
                Debug.LogWarning(
                    "Resource does not have Entity component."
                );

                return;
            }

            for (int i = 0;
                     i < this.selectedUnits.Count;
                     i++)
            {
                Entity unit =
                    this.selectedUnits[i];

                if (unit == null ||
                    !unit.gameObject.activeInHierarchy)
                {
                    continue;
                }

                unit.SetData(new CommandRequest
                {
                    type = CommandType.GATHER_RESOURCE,
                    args = resourceEntity,
                    status = CommandStatus.IDLE
                });
            }

            UnitGroupCommandController.ClearEnemy();

            Debug.Log(
                $"Gather command sent to " +
                $"{this.selectedUnits.Count} units."
            );
        }

        private void PatrolSelectedUnits()
        {
            if (this.selectedUnits.Count == 0)
            {
                Debug.Log("No units selected.");
                return;
            }

            UnitGroupCommandController.Patrol(
                this.selectedUnits
            );
        }

        private void ClearSelection()
        {
            UnitSelectionVisual.Clear(
                this.selectedUnits
            );

            this.selectedUnits.Clear();

            UnitGroupCommandController.ClearEnemy();
        }

        private void OnDestroy()
        {
            UnitSelectionVisual.Clear(
                this.selectedUnits
            );

            this.selectedUnits.Clear();

            UnitGroupCommandController.ClearEnemy();
        }
    }
}