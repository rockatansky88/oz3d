using UnityEngine;

//상태 머신
public enum EnemyStateType
{
    Idle,
    Patrol,
    Walk
}

public abstract class EnemyState
{
    protected EnemyController enemy;

    public EnemyState(EnemyController controller)
    {
        enemy = controller;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

// Idle 상태
public class IdleState : EnemyState
{
    private float idleTimer;
    private float idleDuration;

    public IdleState(EnemyController controller) : base(controller) { }

    public override void Enter()
    {
        idleDuration = Random.Range(1f, 3f);
        idleTimer = 0f;
        enemy.SetAnimationSpeed(0f);
    }

    public override void Update()
    {
        idleTimer += Time.deltaTime;  // 대기 시간 증가
        if (idleTimer >= idleDuration)  // 대기 시간이 끝나면 Patrol 상태로 전환
        {
            enemy.ChangeState(EnemyStateType.Patrol);
        }
    }

    public override void Exit()  // 상태 종료 시 호출
    {

    }
}

// Patrol 상태 (정찰)
public class PatrolState : EnemyState
{
    private Vector3 patrolTarget;
    private float patrolTimer;
    private float patrolDuration;

    public PatrolState(EnemyController controller) : base(controller) { }

    public override void Enter()
    {
        patrolDuration = Random.Range(3f, 6f);
        patrolTimer = 0f;
        SetRandomPatrolTarget();
    }

    public override void Update()
    {
        patrolTimer += Time.deltaTime;

        // 목표 지점으로 이동
        Vector3 direction = (patrolTarget - enemy.transform.position).normalized;
        direction.y = 0; // 수평 이동만

        if (direction.magnitude > 0.1f)
        {
            enemy.Move(direction);
        }

        // 목표에 도달하거나 시간이 지나면 새로운 목표 설정
        if (Vector3.Distance(enemy.transform.position, patrolTarget) < 1f || patrolTimer >= patrolDuration)
        {
            if (Random.value > 0.5f)
            {
                enemy.ChangeState(EnemyStateType.Idle);
            }
            else
            {
                SetRandomPatrolTarget();
                patrolTimer = 0f;
            }
        }
    }

    public override void Exit()
    {
    }

    private void SetRandomPatrolTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        float distance = Random.Range(3f, 8f);
        patrolTarget = enemy.transform.position + randomDirection * distance;
    }
}