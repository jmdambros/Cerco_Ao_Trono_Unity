using UnityEngine;

// Goblin Mineiro - clica no destino, cava pelo chão, explode sob as fundações
public class GoblinMineiro : Troop
{
    [Header("Goblin Settings")]
    public float moveSpeed = 8f;
    public float explosionRadius = 3f;
    public float explosionForce = 25f;
    public float explosionUpwardModifier = 2f;
    public GameObject explosionEffectPrefab;

    private enum MineState { Idle, DiggingDown, Underground, SurfacingUp, Exploding }
    private MineState _state = MineState.Idle;

    private Vector2 _targetX;
    private float _groundY;
    private float _undergroundY;
    private Vector2 _moveTarget;

    [Header("Dig Settings")]
    public float undergroundDepth = 2f;
    public float surfaceOffset = 0.3f;

    protected override void Start()
    {
        base.Start();
        _groundY = transform.position.y;
        _undergroundY = _groundY - undergroundDepth;
    }

    protected override void Update()
    {
        if (!IsReadyToLaunch) return;

        switch (_state)
        {
            case MineState.Idle:
                WaitForTargetClick();
                break;

            case MineState.DiggingDown:
                MoveTowards(new Vector2(transform.position.x, _undergroundY));
                if (Mathf.Abs(transform.position.y - _undergroundY) < 0.05f)
                {
                    _state = MineState.Underground;
                    _moveTarget = new Vector2(_targetX.x, _undergroundY);
                }
                break;

            case MineState.Underground:
                MoveTowards(_moveTarget);
                if (Vector2.Distance(transform.position, _moveTarget) < 0.05f)
                    _state = MineState.SurfacingUp;
                break;

            case MineState.SurfacingUp:
                MoveTowards(new Vector2(transform.position.x, _groundY + surfaceOffset));
                if (Mathf.Abs(transform.position.y - (_groundY + surfaceOffset)) < 0.05f)
                    Explode();
                break;
        }
    }

    private void WaitForTargetClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (clickPos.x <= transform.position.x) return;

            _targetX = clickPos;
            launched = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            _state = MineState.DiggingDown;
        }
    }

    private void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(
            transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void Explode()
    {
        _state = MineState.Exploding;

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
            if (hitRb == null) continue;

            Vector2 dir = (hit.transform.position - transform.position).normalized;
            dir = new Vector2(dir.x * 0.3f, Mathf.Max(dir.y, 0f) + explosionUpwardModifier).normalized;
            hitRb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }

    protected override void Launch(Vector2 velocity) { }
}