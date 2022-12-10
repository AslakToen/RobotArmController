using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // spawner = .GetCompoent
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Tip") {
            Destroy(this.gameObject);
        }
    }
}
