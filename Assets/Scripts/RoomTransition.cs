using UnityEngine;

public class RoomTransition : MonoBehaviour {

    public TransitionDirection transitionDirection;
    
}

public enum TransitionDirection {
    North,
    South,
    East,
    West
}
