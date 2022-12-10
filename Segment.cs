using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class Segment {
    private float length;
    private float theta;
    private float phi;
    private float posX;
    private float posY;
    private float posZ;
    private Segment Parent;
    private Vector3 start;
    private Vector3 end;

    public Segment(float length, float theta, float phi, float posX, float posY, float posZ, Segment Parent){
        this.length = length;
        this.theta = theta;
        this.phi = phi;
        if(Parent == null){
            this.start = new Vector3(this.posX, this.posY, this.posZ);
        }
        else{
            this.Parent = Parent;
            this.start = Parent.getEnd();
        }
        
        setEnd();
    }

    public void setEnd()
    {
        this.end = new Vector3(
            this.start.x + this.length * (float)Math.Cos(this.theta) * (float)Math.Sin(this.phi),
            this.start.y + this.length * (float)Math.Sin(this.theta) * (float)Math.Sin(this.phi),
            this.start.z + this.length * (float)Math.Cos(this.phi)
        );
    }
    
    public Vector3 getStart()
    {
        return this.start;
    }

    public Vector3 getEnd()
    {
        return this.end;
    }

    public void changeAngle(float theta, float phi)
    {
        this.theta = theta;
        this.phi = phi;
        setEnd();
    }

    public void follow(Vector3 target)
    {
        Vector3 v = (target - this.start).normalized;  
        phi = (float)Math.Acos(v.z);
        theta = (float)Math.Atan2(v.y, v.x);
        Vector3 update = v * this.length;
        update = -update;
        this.start = target + update;
        changeAngle(theta, phi);
    }

    public void setStart(Vector3 start)
    {
        this.start = start;
        setEnd();
    }
}
