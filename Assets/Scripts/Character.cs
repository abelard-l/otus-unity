using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
        BeginFistAttack,
        FistAttack,
        Dead
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Fist
    }

    Animator animator;
    State state;

    public Weapon weapon;
    public GameObject target;
    Transform targetTransform;
    Character targetCharacter;

    public float runSpeed;
    public float distanceFromEnemy;

    Vector3 originalPosition;
    Quaternion originalRotation;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        targetTransform = target.GetComponent<Transform>();
        targetCharacter = target.GetComponent<Character>();

        state = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    public State GetState()
    {
        return state;
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        if (targetCharacter.GetState() == State.Dead) return;
        if (state == State.Dead) return;

        switch (weapon)
        {
            case Weapon.Pistol:
                state = State.BeginShoot;
                break;
            case Weapon.Bat:
                state = State.RunningToEnemy;
                break;
            case Weapon.Fist:
                state = State.RunningToEnemy;
                break;
        }
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f)
        {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude)
        {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                transform.rotation = originalRotation;
                animator.SetFloat("Speed", 0.0f);
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(targetTransform.position, distanceFromEnemy))
                    state = State.BeginAttack;
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    state = State.Idle;
                break;

            case State.BeginAttack:
                animator.SetTrigger("MeleeAttack");
                state = State.Attack;
                break;

            case State.Attack:
                targetCharacter.SetState(State.Dead);
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                state = State.Shoot;
                break;

            case State.Shoot:
                targetCharacter.SetState(State.Dead);
                break;

            case State.BeginFistAttack:
                animator.SetTrigger("FistAttack");
                state = State.FistAttack;
                break;

            case State.FistAttack:
                targetCharacter.SetState(State.Dead);
                break;

            case State.Dead:
                animator.SetTrigger("Death");
                break;
        }
    }
}
