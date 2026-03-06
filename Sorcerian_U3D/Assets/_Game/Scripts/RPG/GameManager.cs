using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private GameObject _character0;
    [SerializeField] private GameObject _character1;
    [SerializeField] private GameObject _character2;
    private Vector3 _charPosition0;
    private Vector3 _charPosition1;
    private Vector3 _charPosition2;

    [Header("Enemies")]
    [SerializeField] private GameObject _enemy0;
    [SerializeField] private GameObject _enemy1;
    [SerializeField] private GameObject _enemy2;

    [Header("JumpPositions")]
    [SerializeField] private Transform _jumpPosition0;
    [SerializeField] private Transform _jumpPosition1;
    [SerializeField] private Transform _jumpPosition2;

    [Header("Inputs")]
    [SerializeField] private KeyCode _charAttack0;
    [SerializeField] private KeyCode _charAttack1;
    [SerializeField] private KeyCode _charAttack2;

    [Header("Animation Settings")]
    [SerializeField] private float _jumpDuration = 0.5f;
    [SerializeField] private float _attackDuration = 0.3f;
    [SerializeField] private float _returnDuration = 0.4f;
    [SerializeField] private float _jumpHeight = 2f;

    private bool[] _isAttacking = new bool[3];
    private int[] _enemyHP = new int[3];

    private void Start()
    {
        _charPosition0 = _character0.transform.position;
        _charPosition1 = _character1.transform.position;
        _charPosition2 = _character2.transform.position;
        _enemyHP[0] = 2;
        _enemyHP[1] = 2;
        _enemyHP[2] = 2;
    }

    private void Update()
    {
        //if (_isAttacking.Any(ia => ia == true))
        //{
        //    return;
        //}
        if (Input.GetKeyDown(_charAttack0) && !_isAttacking[0])
        {
            Attack(_character0, _charPosition0, 0);
        }
        else if (Input.GetKeyDown(_charAttack1) && !_isAttacking[1])
        {
            Attack(_character1, _charPosition1, 1);
        }
        else if (Input.GetKeyDown(_charAttack2) && !_isAttacking[2])
        {
            Attack(_character2, _charPosition2, 2);
        }
    }

    private void Attack(GameObject character, Vector3 originalPosition, int characterIndex)
    {
        (GameObject, Transform) enemyTargetJumpPosition = GetEnemyTarget();
        if (enemyTargetJumpPosition.Item1 != null && enemyTargetJumpPosition.Item2 != null)
        {
            StartCoroutine(AttackRoutine(character, originalPosition, enemyTargetJumpPosition, characterIndex));
        }
    }

    private (GameObject, Transform) GetEnemyTarget()
    {
        if (_enemy0 != null)
        {
            return (_enemy0, _jumpPosition0);
        }
        else if (_enemy1 != null)
        {
            return (_enemy1, _jumpPosition1);
        }
        else if (_enemy2 != null)
        {
            return (_enemy2, _jumpPosition2);
        }
        return (null, null);
    }

    private IEnumerator AttackRoutine(GameObject character, Vector3 originalPosition, (GameObject, Transform) enemyTargetJumpPosition, int characterIndex)
    {
        _isAttacking[characterIndex] = true;

        // Salto hacia el enemigo (con arco)
        yield return StartCoroutine(MoveInArc(character, enemyTargetJumpPosition.Item2.position, _jumpDuration, true));

        int enemyIndex = GetEnemyIndex(enemyTargetJumpPosition.Item1);
        _enemyHP[enemyIndex]--;
        if (_enemyHP[enemyIndex] == 0)
        {
            Destroy(enemyTargetJumpPosition.Item1);
        }
        // Realizar ataque (breve pausa)
        yield return new WaitForSeconds(_attackDuration);

        // Volver a posición original (más recto)
        yield return StartCoroutine(MoveDirectly(character, originalPosition, _returnDuration));

        _isAttacking[characterIndex] = false;
    }

    private int GetEnemyIndex(GameObject target)
    {
        if (target == _enemy0)
        {
            return 0;
        }
        else if (target == _enemy1)
        {
            return 1;
        }
        else if (target == _enemy2)
        {
            return 2;
        }
        return -1;
    }

    private IEnumerator MoveInArc(GameObject character, Vector3 targetPosition, float duration, bool isJump)
    {
        Vector3 startPosition = character.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolación lineal en XZ
            Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, t);

            // Curva de salto en Y (arco parabólico)
            float verticalOffset = 0f;
            if (isJump)
            {
                verticalOffset = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            }

            character.transform.position = new Vector3(
                horizontalPosition.x,
                startPosition.y + verticalOffset,
                horizontalPosition.z
            );

            yield return null;
        }

        // Asegurar posición final exacta
        character.transform.position = targetPosition;
    }

    private IEnumerator MoveDirectly(GameObject character, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = character.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Movimiento directo con Lerp
            character.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // Asegurar posición final exacta
        character.transform.position = targetPosition;
    }
}
