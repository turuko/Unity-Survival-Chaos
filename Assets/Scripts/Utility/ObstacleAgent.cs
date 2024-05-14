using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Utility
{
    [RequireComponent(typeof(NavMeshAgent), typeof(NavMeshObstacle))]
    public class ObstacleAgent : MonoBehaviour
    {
        [SerializeField] private float CarvingTime = 0.1f;
        [SerializeField] private float CarvingMoveThreshold = 0.01f;

        private NavMeshAgent Agent;
        private NavMeshObstacle Obstacle;

        private float LastMoveTime;
        private Vector3 LastPosition;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Obstacle = GetComponent<NavMeshObstacle>();

            Obstacle.enabled = false;
            Obstacle.carveOnlyStationary = false;
            Obstacle.carving = true;

            LastPosition = transform.position;
        }


        private void Update()
        {
            if (Vector3.Distance(LastPosition, transform.position) > CarvingMoveThreshold)
            {
                LastMoveTime = Time.time;
                LastPosition = transform.position;
            }

            if (LastMoveTime + CarvingTime < Time.time)
            {
                Agent.enabled = false;
                Obstacle.enabled = true;
            }
        }

        public void SetDestination(Vector3 position)
        {
            Obstacle.enabled = false;

            LastMoveTime = Time.time;
            LastPosition = transform.position;

            StartCoroutine(MoveAgent(position));
        }

        IEnumerator MoveAgent(Vector3 position)
        {
            yield return null;
            Agent.enabled = true;
            Agent.SetDestination(position);
        }
    }
}