using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RisingEnemy
{
    protected void Attack(); 
    int Damage { get; set;}
}
