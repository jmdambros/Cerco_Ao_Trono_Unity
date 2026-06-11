using UnityEngine;

//tropas herdam dessa, classe basica de trajetoria etc
public abstract class Troop : MonoBehaviour
{
    [Header("Troop Info")]
    public string troopName;
    public int goldCost;
    public Sprite icon;             

    [Header("Launch Settings")]
    public float launchForce = 15f;
    public float maxDragDistance = 2.5f;

    [Header("Trajectory Preview")]
    public int trajectoryDots = 30;
    public float trajectoryTimeStep = 0.1f;
    public GameObject dotPrefab;

    protected Rigidbody2D rb;
    protected Vector2 startPosition;
    protected bool launched = false;
    protected bool isDragging = false;
    private GameObject[] dots;

    public bool IsReadyToLaunch { get; private set; } = false;
    public bool HasLaunched => launched;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        startPosition = transform.position;
        CreateDots();
    }

    protected virtual void Update()
    {
        if (launched) return;
        if (!IsReadyToLaunch) return;   // wait until GameManager activates this troop

        Vector2 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
            if (Vector2.Distance(inputPos, startPosition) < 0.8f)
                isDragging = true;

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 drag = inputPos - startPosition;
            drag = Vector2.ClampMagnitude(drag, maxDragDistance);
            transform.position = startPosition + drag;
            ShowTrajectory(-drag * launchForce);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector2 drag = (Vector2)transform.position - startPosition;
            Launch(-drag * launchForce);
        }
    }

    public void Activate()
    {
        IsReadyToLaunch = true;
    }

    protected virtual void Launch(Vector2 velocity)
    {
        launched = true;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 1f;
        rb.linearVelocity = velocity;
        HideDots();
        OnLaunched(velocity);
    }

    protected virtual void OnLaunched(Vector2 velocity) { }

    protected virtual void OnHit(Collision2D col) { }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (launched) OnHit(col);
    }

    // --- Trajectory ---

    void CreateDots()
    {
        if (dotPrefab == null) return;
        dots = new GameObject[trajectoryDots];
        for (int i = 0; i < trajectoryDots; i++)
        {
            dots[i] = Instantiate(dotPrefab);
            dots[i].SetActive(false);
            float scale = Mathf.Lerp(0.25f, 0.05f, (float)i / trajectoryDots);
            dots[i].transform.localScale = Vector3.one * scale;
        }
    }

    void ShowTrajectory(Vector2 initialVelocity)
    {
        if (dots == null) return;
        Vector2 gravity = Physics2D.gravity;
        for (int i = 0; i < trajectoryDots; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector2 point = startPosition + initialVelocity * t + 0.5f * gravity * t * t;
            dots[i].SetActive(true);
            dots[i].transform.position = point;
        }
    }

    void HideDots()
    {
        if (dots == null) return;
        foreach (var dot in dots)
            if (dot != null) dot.SetActive(false);
    }
}