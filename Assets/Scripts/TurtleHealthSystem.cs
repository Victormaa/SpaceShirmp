using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleHealthSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.transform.tag == "ShrimpShell")
        {
            Destroy(collision.gameObject);
            this.GetComponent<MMF_Player>()?.PlayFeedbacks();
        }
    }
}
