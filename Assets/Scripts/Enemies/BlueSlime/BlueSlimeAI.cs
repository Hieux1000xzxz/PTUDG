using System.Collections;
using UnityEngine;

public class BlueSlimeAi : MonoBehaviour
{
    private enum State
    {
        Roaming
    }

    private State state;
    private BlueSlimePathFinding pathfinding;
    public int damege = 10; // Sát thương của kẻ địch
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRate = 1f; // thời gian giữa 2 lần gây sát thương
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int damage = 10;

    private void Awake()
    {
        pathfinding = GetComponent<BlueSlimePathFinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());

    }

    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            pathfinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
