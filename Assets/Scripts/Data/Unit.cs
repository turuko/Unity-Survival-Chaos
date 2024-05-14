using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Utility;

namespace Data
{
    public enum UnitType
    {
        T1_Melee,
        T1_Ranged,
        T1_Mage,
        T2,
        T3,
        T4
    }

    public enum AttackType
    {
        Melee,
        Ranged
    }
    public class Unit : MonoBehaviour
    {
        public string Name;
        public UnitType Type;
        public AttackType AttackType;

                
        [SerializeField]
        private float maxHealth;
        
                
        [SerializeField]
        private float currentHealth;

        public float CurrentHealth
        {
            get => currentHealth;
        }

        public float MaxHealth
        {
            get => maxHealth;
        }
        
        [SerializeField]
        private float damage;
                
        [SerializeField]
        private float attackSpeed;
                
        [SerializeField]
        private float timeSinceLastAttack;

        private Player player;

        public Player Player
        {
            get => player;
        }
        
        [SerializeField]
        private Vector3 target;

        private Vector3 lastTarget;
        
                
        [SerializeField]
        private Unit attackTarget = null;

        private HashSet<Unit> attackedBy;
        
        [SerializeField]
        private float attackRange;
                
        [SerializeField]
        private float aggroRange;

        public Vector3 Target
        {
            get => target;
            set => target = value;
        }
        
        public Vector3 Position
        {
            get => transform.position;
        }
        
        public float AttackRange
        {
            get => attackRange;
        }
        
        public float AggroRange
        {
            get => aggroRange;
        }
        
        [SerializeField]
        private float moveSpeed = 5f;
        private bool hasBaseTarget = false;
        private List<Vector2> passedMidpoints = new List<Vector2>();

        public ObstacleAgent navAgent;
        private Queue<Vector3> path;
        private Image healthBar;

        private Action<Unit> onUnitMoved;
        private Action<Unit> onUnitDestroyed;
        
        public void InitFromData(UnitGameData data, Vector3 position, Player player, Queue<Vector2> path)
        {
            Name = data.Name;
            Type = data.Type;
            AttackType = data.AttackType;
            attackRange = data.attackRange;
            aggroRange = data.aggroRange;
            currentHealth = maxHealth = data.maxHealth;
            damage = data.damage;
            timeSinceLastAttack = attackSpeed = 1/data.attacksPerSecond;

            this.path = new Queue<Vector3>(path.Select(x => new Vector3(x.x, 0f, x.y)));
            target = this.path.Peek();
            lastTarget = Vector3.negativeInfinity;
            
            this.player = player;
            attackedBy = new HashSet<Unit>();
            navAgent = GetComponent<ObstacleAgent>();

            for (int i = 0; i < transform.childCount; i++)
            {
                for (int j = 0; j < transform.GetChild(i).childCount; j++)
                {
                    if (transform.GetChild(i).GetChild(j).CompareTag("Healthbar"))
                    {
                        healthBar = transform.GetChild(i).GetChild(j).GetComponent<Image>();
                    }
                }
            }

            //SetTarget();
        }

        public void Update()
        {
            SetTarget();
        }

        void Attack()
        {
            //Take animation into account, projectile travel time?

            if (attackTarget == null)
            {
                Debug.LogError("Cant attack without target!");
                return;
            }

            attackTarget.ChangeHealth(-damage);
            timeSinceLastAttack = attackSpeed;
        }

        public void ChangeHealth(float healthChange)
        {
            currentHealth += healthChange;
            if (currentHealth >= maxHealth)
                currentHealth = maxHealth;
            if(currentHealth <= 0f)
            {
                onUnitDestroyed(this);
                Destroy(gameObject);
            }

            healthBar.fillAmount = currentHealth / maxHealth;
        }

        private void SetTarget()
        {
            //Check if there is an enemy unit within aggro range
            //if there is one then set the position of that unit to be the target
            FindAttackTarget();
            
            //If we have an attackTarget, check if we are close enough to attack them
            //if we are close enough then set the target to be our current position
            //otherwise set the target to be the position of the attackTarget unit.
            if (attackTarget != null)
            {
                if (Vector3.Distance(this.Position, attackTarget.Position) <= attackRange)
                {
                    target = this.Position;
                }
                else
                {
                    target = attackTarget.Position;
                }
            }
            else
            {
                GetPathTarget();
            }
            
            //Set the destination of the navAgent to be the target of the unit.
            if (target != lastTarget)
            {
                navAgent.SetDestination(target);
            }
            
            lastTarget = target;
            onUnitMoved?.Invoke(this);
        }

        private void FindAttackTarget()
        {
            foreach (var unit in MapPartitioner.Instance.Query(this))
            {
                if(unit.Player == this.Player) continue;
                if (!(Vector3.Distance(unit.Position, this.Position) <= aggroRange)) continue;
                attackTarget = unit;
                break;
            }
        }

        private void GetPathTarget()
        {
            if (path.Count <= 0)
                return;
            target = path.Peek();

            if (Vector3.Distance(Position, path.Peek()) < 3f && path.Count > 0)
            {
                path.Dequeue();
            }
        }

        public void RegisterOnMovedCallback(Action<Unit> cb)
        {
            onUnitMoved += cb;
        }

        public void RegisterOnDestroyedCallback(Action<Unit> cb)
        {
            onUnitDestroyed += cb;
        }
        
        public void UnregisterOnMovedCallback(Action<Unit> cb)
        {
            onUnitMoved -= cb;
        }

        public void UnregisterOnDestroyedCallback(Action<Unit> cb)
        {
            onUnitDestroyed -= cb;
        }

    }
}