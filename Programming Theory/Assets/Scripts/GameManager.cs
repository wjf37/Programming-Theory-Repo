using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void ActiveUnitListEventHandler(List<Transform> updatedList);
    public event ActiveUnitListEventHandler OnListUpdated;
    public GameObject selected;
    public List<GameObject> selectedUnitType;
    private List<Transform> activeUnits = new();
    private RectTransform selectedRT;
    private int selectedUnitTypeIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        selectedRT = selected.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){//chimp
            selectedUnitTypeIndex = 0;
            selectedRT.anchoredPosition3D = new Vector3(-100,selectedRT.anchoredPosition3D.y,selectedRT.anchoredPosition3D.z);

        }

        if (Input.GetKeyDown(KeyCode.Alpha2)){//gorilla
            selectedUnitTypeIndex = 1;
            selectedRT.anchoredPosition3D = new Vector3(0,selectedRT.anchoredPosition3D.y,selectedRT.anchoredPosition3D.z);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)){//orangutan
            selectedUnitTypeIndex = 2;
            selectedRT.anchoredPosition3D = new Vector3(100,selectedRT.anchoredPosition3D.y,selectedRT.anchoredPosition3D.z);
        }

        if(Input.GetMouseButtonDown(0)){
            PlaceDown();
        }
    }

    void PlaceDown(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Instantiate object at the hit point
            GameObject spawnedUnit = Instantiate(selectedUnitType[selectedUnitTypeIndex], hit.point, Quaternion.identity);
            OnUnitSpawned(spawnedUnit.transform);
        }
    }

    public void OnUnitSpawned(Transform unitTransform){
        activeUnits.Add(unitTransform);
        OnListUpdated?.Invoke(activeUnits);
    }

    public void OnUnitDestroyed(Transform unitTransform){
        activeUnits.Remove(unitTransform);
        OnListUpdated?.Invoke(activeUnits);
    }
}
