using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos, startposY;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        startposY = transform.localPosition.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        if(cam != null)
        {
            float temp = (cam.transform.position.x * (1 - parallaxEffect));
            float dist = (cam.transform.position.x * parallaxEffect);

            transform.position = new Vector3(startpos + dist, cam.transform.position.y + startposY, transform.position.z);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        

            if (temp > startpos + length) startpos += length;
            else if (temp < startpos - length) startpos -= length;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
}
