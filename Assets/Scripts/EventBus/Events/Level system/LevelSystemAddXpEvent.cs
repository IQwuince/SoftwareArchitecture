using UnityEngine;
using System.Collections;


public class LevelSystemAddXpEvent : Event
{
	public LevelSystem levelSystem { get; }
	public int XpAdd;
    public LevelSystemAddXpEvent(int XpValueE)
	{
        XpAdd = XpValueE;
    }

}
