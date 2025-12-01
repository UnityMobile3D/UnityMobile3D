using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int hp;
    [SerializeField] private int maxHp;
   
    public void AddHP(int _value)
    {
        hp += _value;
        if (hp >= maxHp)
            hp = maxHp;
        else if(hp <=0)
            hp = 0;
    }

    public int HP => hp;
    public int MaxHp => maxHp;

    [Header("MP")]
    [SerializeField] private int mp;
    [SerializeField] private int maxMp;

    public int MP => mp;
    public int MaxMp => maxMp;

    [Header("Speed")]
    [SerializeField] private int speed;
    [SerializeField] private int maxSpeed;
    
    public int Speed => speed;
    public int MaxSpeed => maxSpeed;

    [Header("Defense")]
    [SerializeField] private int defense;
    [SerializeField] private int maxDefense;

    public int Defense => defense;
    public int MaxDefense => maxDefense;

    [Header("Attack")]
    [SerializeField] private int attack;
    [SerializeField] private int maxAttack;

    public int Attack => attack;
    public int MaxAttack => maxAttack;

    public void UpAttack(int _value)
    {
        attack += _value;
        if(attack >= maxAttack)
            attack = maxAttack; 
    }
}
