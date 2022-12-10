using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using PythonUnitySharedMemory;
using static Segment;


public class PositionTracker : MonoBehaviour {
    public GameObject hmd;
    public GameObject controllerR;

    public GameObject t;



    public List<Segment> segList = new List<Segment>();

    public List<GameObject> cylinders = new List<GameObject>();
    public List<GameObject> joints = new List<GameObject>();




    string filename = "Assets/Logs/log.txt";

    // Start is called before the first frame update
    void Start() {

        // // the length of each individual robot arm segment
        // float[] segLengths = new float[] {0.15f, 0.277f, 0.225f, 0.134f, 0.230f, 0.230f};
        float[] segLengths = new float[] {0.225f, 0.4155f, 0.3375f, 0.201f, 0.345f, 0.345f};

        // creating first segment, this is the base of the robot, and is therefore treated as special
        Segment first = new Segment(segLengths[0], 0, 0, 0, 0, 0, null);
        segList.Add(first);
        Segment current = first;

        // creating cylinders for each segment. These are purely there to visualize the math of the performed by segments
        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // cyl.transform.localScale = new Vector3(0.05f, 0.1f, 0.05f);
        cyl.transform.localScale = new Vector3(0.075f, 0.15f, 0.075f);
        cylinders.Add(cyl);
        
        GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // sph.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sph.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        joints.Add(sph);

        // adding the rest of the segments/cylinders to the robot arm
        for(int i = 1; i < segLengths.Length; i++) 
        {
            cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            // cyl.transform.localScale = new Vector3(0.05f, 0.1f, 0.05f);
            cyl.transform.localScale = new Vector3(0.075f, 0.15f, 0.075f);
            cylinders.Add(cyl);

            sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sph.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sph.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            joints.Add(sph);

            Segment seg = new Segment(segLengths[i], 0, 0, 0, 0, 0, current);
            segList.Add(seg);
            current = seg;
        }

        cylinders[cylinders.Count-1].GetComponent<Renderer>().material.color = Color.gray;
        cylinders[cylinders.Count-1].name = "Tip";

        /*
        var sm = new SharedMemory(
            prefix:"my-file",    // The ID of the shared memory 
            capacity:100,       // The number of bytes of the file
            createNew:true,     // Wether to create the file or open it
            timeout:-1);        // How low to timeout (-1 means no timeout)

        sm.WriteInt32(0, 69);
        */



        // sm = new SharedMemory(
        //     prefix:"my-file",    // The ID of the shared memory 
        //     capacity:100,       // The number of bytes of the file
        //     createNew:true,     // Wether to create the file or open it
        //     timeout:-1);        // How low to timeout (-1 means no timeout)
        // sm.WriteInt32(0, 69);


        File.WriteAllText(filename, "");
        // File.AppendAllText(filename, "huga");

    }

    // Update is called once per frame
    
    void Update() {
        Vector3 posControllerR = controllerR.transform.position;

        Vector3 target = new Vector3(posControllerR.x * 2, posControllerR.y * 2 - 1, posControllerR.z * 2);

        // inverse kinemtatics to follow some given point in 3d space
        segList[segList.Count-1].follow(target);
        for(int i = segList.Count-2; i >= 0; i--){
            segList[i].follow(segList[i+1].getStart());
        }

        // set first segment back to correct position, forward kinematics to update all other segments
        segList[0].setStart(new Vector3(0, 0, 0));
        for(int i = 1; i < segList.Count; i++){
            segList[i].setStart(segList[i-1].getEnd());
        }
        
        // "Render" the robot arm
        for(int i = 0; i < cylinders.Count; i++){
            //starts[i].transform.localPosition = segList[i].getStart();
            //ends[i].transform.localPosition = segList[i].getEnd();
            joints[i].transform.localPosition = segList[i].getStart();
            cylinders[i].transform.position = Vector3.Lerp(segList[i].getStart(), segList[i].getEnd(), 0.5f);
            cylinders[i].transform.rotation = Quaternion.LookRotation((segList[i].getEnd() - segList[i].getStart()).normalized) * Quaternion.Euler(Vector3.right * 90f);
        }
        
        //arm.transform.localScale = new Vector3(transform.localScale.x, Vector3.Distance(segList[0].getStart(), segList[0].getEnd()) * 0.5f, transform.localScale.z);

        t.transform.localPosition = target;
        float x = t.transform.localPosition.x;
        float y = t.transform.localPosition.y;
        float z = t.transform.localPosition.z;
        String txt = x.ToString() + " " + y.ToString() + " " + z.ToString() + "\n";
        File.WriteAllText(filename, txt);
        
        /*
        var sm = new SharedMemory(
            prefix:"my-file",    // The ID of the shared memory 
            capacity:100,       // The number of bytes of the file
            createNew:false,     // Wether to create the file or open it
            timeout:30);        // How low to timeout (-1 means no timeout)
        */
        // sm.WriteInt32(0, 69);

        
        // Debug.Log(controllerR.transform.localPosition.x);
        // this.transform.position = 10 * controllerR.transform.position;
        // this.transform.localPosition = new Vector3(posControllerR.x * 10, posControllerR.y * 10, posControllerR.z * 10);
    }
}