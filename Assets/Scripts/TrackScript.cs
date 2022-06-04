using UnityEngine;

public class TrackScript : MonoBehaviour
{

    [SerializeField] GameObject track;
    MeshRenderer mesh;
    Vector3 endPos;
    [SerializeField] bool SendSpawnMessage = true;
    [SerializeField] float offsetToDeactivate = 4;
    //Vector3 startPos;
    float minmaxY = 0;
    bool deactivated;
    bool GameStart = false;
    Transform player;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        endPos = GetEndPos(mesh);
        minmaxY = ExtractMinMax(mesh);
        GameMaster.instance.OnGameStart += () => player = GameMaster.instance.m_pc.transform;
        GameMaster.instance.OnGameStart += () => GameStart = true;
        //Extensions.Debug(mesh.bounds.center.ToV3String());
        //Extensions.Debug(mesh.bounds.size.ToV3String());
        //startPos = GetStartPos(mesh);

        //t.transform.position = endPos.WithX(transform.position.x).Add(Vector3.zero.WithZ(GetEndPos(t_rend).z));
        //t.transform.SetPositionY((mesh.bounds.extents == t_rend.bounds.extents) ? transform.position.y : Random.Range(-minmaxY, minmaxY));
    }
    public void GameStartTrue()
    {
        GameStart = true;
        player = GameMaster.instance.m_pc.transform;
    }
    public Vector3 GetNextPos(GameObject t, bool ChangeY = false)
    {
        mesh = GetComponent<MeshRenderer>();

        endPos = GetEndPos(mesh);
        minmaxY = ExtractMinMax(mesh);
        if (!ChangeY)
            t.transform.position = Vector3.zero;
        Renderer t_rend = t.GetComponent<Renderer>();
        return new Vector3(
            transform.position.x,
            (mesh.bounds.extents == t_rend.bounds.extents || ChangeY) ? transform.position.y : Random.Range(-minmaxY, minmaxY / 2),
            endPos.z + GetEndPos(t_rend).z
             );
        //return endPos.WithX(transform.position.x).Add(Vector3.zero.WithZ(GetEndPos(t_rend).z)).WithY((mesh.bounds.extents == t_rend.bounds.extents) ? transform.position.y : Random.Range(-minmaxY, minmaxY));
    }
    private void Update()
    {
        if (GameStart)
            if (player.position.z > (endPos.z + offsetToDeactivate) && !deactivated)
            {
                deactivated = true;
                if (SendSpawnMessage)
                    SpawnManager.instance.NextObject();
                gameObject.SetActive(false);
            }
    }
    private float ExtractMinMax(Renderer mesh)
    {
        return mesh.bounds.center.y + mesh.bounds.size.y;
    }

    private static Vector3 GetStartPos(Renderer mesh)
    {
        return mesh.bounds.extents - mesh.bounds.center;
    }

    private static Vector3 GetEndPos(Renderer mesh)
    {
        return mesh.bounds.extents + mesh.bounds.center;
    }

}
