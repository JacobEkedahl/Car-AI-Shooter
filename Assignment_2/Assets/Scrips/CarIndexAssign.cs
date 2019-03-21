using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CarIndexAssign : MonoBehaviour {
    int index;

    public CarIndexAssign(){
        index = 0;
    }

    public int get_my_index(){
        return index++;
    }
}