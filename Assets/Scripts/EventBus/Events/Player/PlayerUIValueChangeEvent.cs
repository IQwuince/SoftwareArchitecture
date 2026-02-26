using UnityEngine;
using System.Collections;

public class PlayerUIValueChangeEvent : Event
{
	public PlayerHealth PlayerHealth { get; }
    public int currentHealthE;
    public int minHealthE;
    public int maxHealthE;

    public PlayerUIValueChangeEvent(int currentH, int minH, int maxH)
    {
        currentHealthE = currentH;
        minHealthE = minH;
        maxHealthE = maxH;

    }



}
