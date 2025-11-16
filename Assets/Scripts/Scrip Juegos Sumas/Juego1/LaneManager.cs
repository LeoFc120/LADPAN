using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager instance;
    public Transform[] carriles;            // Arrastrar Carril1..Carril3 (3)
    public GameObject[] ocupantes;          // interno: cuál GameObject ocupa el carril (size = carriles.Length)

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        ocupantes = new GameObject[carriles.Length];
    }

    // Devuelve índice de carril más cercano a una posición X
    public int GetClosestLaneIndex(Vector3 worldPos)
    {
        int idx = 0;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < carriles.Length; i++)
        {
            float d = Mathf.Abs(worldPos.x - carriles[i].position.x);
            if (d < minDist)
            {
                minDist = d;
                idx = i;
            }
        }
        return idx;
    }

    // Registrar que un auto ocupa un carril
    public void RegisterOccupant(int laneIndex, GameObject go)
    {
        if (laneIndex < 0 || laneIndex >= ocupantes.Length) return;
        ocupantes[laneIndex] = go;
    }

    // Remover ocupante en un carril (si es ese objeto)
    public void UnregisterOccupant(int laneIndex, GameObject go)
    {
        if (laneIndex < 0 || laneIndex >= ocupantes.Length) return;
        if (ocupantes[laneIndex] == go) ocupantes[laneIndex] = null;
    }

    // Obtener ocupante
    public GameObject GetOccupant(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= ocupantes.Length) return null;
        return ocupantes[laneIndex];
    }

    // Intercambia ocupantes entre dos carriles de forma segura (inicia coroutine en cada auto si existe)
    public void SwapOccupants(int fromLane, int toLane)
    {
        if (fromLane == toLane) return;
        if (fromLane < 0 || toLane < 0 || fromLane >= ocupantes.Length || toLane >= ocupantes.Length) return;

        GameObject a = ocupantes[fromLane];
        GameObject b = ocupantes[toLane];

        // actualiza registros inmediatamente para evitar condiciones de carrera
        ocupantes[toLane] = a;
        ocupantes[fromLane] = b;

        // Si hay objeto 'a', pídele que se mueva al carril 'toLane'
        if (a != null)
        {
            var mover = a.GetComponent<ICarriable>();
            if (mover != null) mover.MoveToLane(toLane);
            else StartCoroutine(DirectMoveToLane(a.transform, carriles[toLane].position));
        }

        // Si hay objeto 'b', muévelo al 'fromLane'
        if (b != null)
        {
            var mover = b.GetComponent<ICarriable>();
            if (mover != null) mover.MoveToLane(fromLane);
            else StartCoroutine(DirectMoveToLane(b.transform, carriles[fromLane].position));
        }
    }

    // helper para mover directamente (si el objeto no implementa ICarriable)
    IEnumerator DirectMoveToLane(Transform t, Vector3 target)
    {
        float duration = 0.25f;
        Vector3 start = t.position;
        float time = 0f;
        while (time < duration)
        {
            t.position = Vector3.Lerp(start, new Vector3(target.x, t.position.y, t.position.z), time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        t.position = new Vector3(target.x, t.position.y, t.position.z);
    }
}
