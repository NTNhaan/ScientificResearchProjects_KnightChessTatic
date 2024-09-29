using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PieceReward : MonoBehaviour
{
    public Transform target;
    public Transform targetDemon;
    public Camera cam;
    private TimeBar timeBar;
    private Vector3 targetPos = Vector3.zero;
    public static PieceReward Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        timeBar = FindObjectOfType<TimeBar>();
    }
    public void Update()
    {
        if (timeBar.role == TimeBar.Role.Player)
        {
            targetPos = target.position;
        }
        else if (timeBar.role == TimeBar.Role.Demon)
        {
            targetPos = targetDemon.position;
        }
        Debug.Log("Role Pos: " + timeBar.role);
    }
    public void StartCoinMove(Vector3 _intialPos, GameObject Coiprefab)
    {
        GameObject _coin = Instantiate(Coiprefab, _intialPos, Quaternion.identity);
        _coin.transform.localScale = new Vector3(1, 1, 1);
        StartCoroutine(MoveCoin(_coin.transform, _intialPos, targetPos));

    }
    IEnumerator MoveCoin(Transform obj, Vector3 StartPos, Vector3 EndPos)
    {
        float time = 0;
        EndPos = new Vector3(EndPos.x, EndPos.y + 3, EndPos.z);
        while (time < 1)
        {
            time += 1 * Time.deltaTime;

            obj.position = Vector3.Lerp(StartPos, EndPos, time);
            yield return null;
        }
        Destroy(obj.gameObject);
    }
}
