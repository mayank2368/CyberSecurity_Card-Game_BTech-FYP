using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDragDrop
{
   
    bool CanBeDragged { get; set; } 

    
    void OnGrab(Vector3 _pos);

    
    void OnDrag(Vector3 _pos);

    
    void OnDrop(Vector3 _pos);

    
    void OnHover(bool _value);
}
