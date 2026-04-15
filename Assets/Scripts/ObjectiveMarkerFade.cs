using UnityEngine;

public class ObjectiveMarkerFade : MonoBehaviour
{
    public Transform player;
    public float hideDistance = 5f;
    public float fadeSpeed = 3f;

    private Material mat;
    private Color color;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        color = mat.color;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 markerPos = transform.position;
        Vector3 playerPos = player.position;

        markerPos.y = 0f;
        playerPos.y = 0f;

        float distance = Vector3.Distance(playerPos, markerPos);

        float targetAlpha = distance < hideDistance ? 0f : 1f;

        color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
        mat.color = color;

        if (color.a < 0.02f)
        {
            meshRenderer.enabled = false; 
        }
        else
        {
            meshRenderer.enabled = true; 
        }
    }
}